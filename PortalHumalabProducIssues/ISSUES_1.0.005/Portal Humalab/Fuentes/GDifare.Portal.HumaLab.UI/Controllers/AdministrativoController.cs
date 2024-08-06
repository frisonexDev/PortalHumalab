﻿using GDifare.Portal.EstadoCartera.Operation;
using GDifare.Portal.Humalab.Seguridad.Modelos;
using GDifare.Portal.Humalab.Seguridad.Modelos.Perfil;
using GDifare.Portal.Humalab.Seguridad.Operation;
using GDifare.Portal.Humalab.Servicio.CarteraCliente;
using GDifare.Portal.Humalab.Servicio.ClienteOperacion;
using GDifare.Portal.Humalab.Servicio.Facturas;
using GDifare.Portal.Humalab.Servicio.GestionCliente;
using GDifare.Portal.Humalab.Servicio.Modelos;
using GDifare.Portal.Humalab.Servicio.Modelos.Facturas;
using GDifare.Portal.Humalab.Servicio.Modelos.GestionCliente;
using GDifare.Portal.HumaLab.UI.Models;
using GDifare.Portal.HumaLab.UI.Utils;
using GDifare.Portales.HumaLab.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Text;
using System.Web.Helpers;


namespace GDifare.Portales.HumaLab.UI.Controllers
{
    [Authorize]
    public class AdministrativoController : Controller
    {
        private readonly UsuarioOperation usuarioOperations;
        private readonly PerfilOperation perfilOperations;
        private readonly RolOperation rolOperations;


        private readonly ClienteOperation clienteOperation;
        private readonly FacturasOperacion facturasOperacion;
        private readonly CarteraOperacion carteraOperacion;

        private readonly ClienteOperationNew clienteOperationNew;


        public AdministrativoController(AppServicioClienteApi settingsApiFactura, AppSettings settings, AppServicioMicros settingsApi, AppServicioMicrosExternos configurationApisExt)
        {

            //Microservicio Difare de seguridad
            usuarioOperations = new UsuarioOperation(settings.ServerSeguridad, settings.PortSeguridad, settings.Token, settings.UserAvalab, settings.PassAvalab);
            perfilOperations = new PerfilOperation(settings.ServerSeguridad, settings.PortSeguridad, settings.Token, settings.UserAvalab, settings.PassAvalab);
            rolOperations = new RolOperation(settings.ServerSeguridad, settings.PortSeguridad, settings.Token, settings.UserAvalab, settings.PassAvalab);

            //Microservicio humalab

            //microservicio para validar cartera            
            clienteOperation = new ClienteOperation(settingsApi.ServerGestionCliente, settingsApi.PortGestionCliente, settings.Token,
                                                  configurationApisExt.ServerFlexLine, configurationApisExt.PortFlexLine, configurationApisExt.TokenFlexline);

            //microservicio para la facturacion            
            facturasOperacion = new FacturasOperacion(settingsApiFactura, configurationApisExt);

            //microservicio nuevo para validar cartera
            carteraOperacion = new CarteraOperacion(settingsApi.ServerGestionCliente, settingsApi.PortGestionCliente, settings.Token,
                                                    configurationApisExt.PathServicesCarteraEstado, configurationApisExt.ServerFlexLine, configurationApisExt.PortFlexLine, 
                                                    configurationApisExt.TokenFlexline, configurationApisExt.PathServicesClientesFlexLine,
                                                    settingsApi.ServerAdministracion, settingsApi.PortAdministracion, settingsApi.RouteAdministracion,
                                                    settingsApi.RouteFlexCartera);

            clienteOperationNew = new ClienteOperationNew(settingsApi.ServerGestionCliente, settingsApi.PortGestionCliente, settings.Token,
                                                    configurationApisExt.ServerFlexLine, configurationApisExt.PortFlexLine, configurationApisExt.TokenFlexline,                                                     
                                                    settingsApi.ServerAdministracion, settingsApi.PortAdministracion, settingsApi.RouteAdministracion,
                                                    settingsApi.RouteFlexCartera);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Facturacion()
        {

            EstadosFactura facturaEsta = new();            
            facturaEsta = facturasOperacion.EstadosFacturacion();

            ViewBag.Validadas = facturaEsta.Validadas;
            ViewBag.Facturadas = facturaEsta.Facturdas;
            return View();
        }

        public IActionResult PruebasAdmin()
        {
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Login");
        }

        [HttpGet]
        public IActionResult Blank(CredencialesRequest userInfo)
        {
            try
            {
                var validarCartera = "S";
                var valueBytes = System.Convert.FromBase64String(userInfo.Usuario);
                var usrdes = encryptInfo.DecryptStringAES(Encoding.UTF8.GetString(valueBytes));
                // var usrdes = encryptInfo.DecryptStringAES(userInfo.Usuario);
                userInfo.Usuario = usrdes;
                ViewData["cartera"] = validarCartera;
                ObjUsuario usuario = usuarioOperations.ObtenerUsuario(userInfo.Usuario);
                var rolPerfil = rolOperations.ConsultarRol(usuario.RolID).FirstOrDefault()!.RolPerfiles.FirstOrDefault();
                if (rolPerfil != null)
                {
                    //var perfil = perfilOperations.ConsultarPerfil(rolPerfil.PerfilID, null, 0).FirstOrDefault();
                    //usuario.perfil = perfil.Perfil;

                    //obtiene el id del lab de cada usuario                    
                    object obj = perfilOperations.ConsultarLaboratorio(usuario.EmpresaID);
                    
                    if (obj != null)
                    {
                        LaboratorioResponse lstLab = JsonConvert.DeserializeObject<LaboratorioResponse>(obj.ToString()!)!;
                        foreach (var lab in lstLab.laboratorio)
                        {
                            foreach (var labFinal in lab.details)
                            {
                                usuario.EmpresaID = labFinal.IdLaboratorio;
                            }
                        }
                    }

                    //valida si existe cartera vencida del cliente
                    //validarCartera = clienteOperation.ValidarCartera(usuario.RolID, usuario.Identificacion);
                    validarCartera = clienteOperationNew.ValidarCartera(usuario.RolID, usuario.Identificacion);

                    switch (validarCartera)
                    {
                        case "A":
                            usuario.EstadoCartera = "Activo";
                            usuario.EstadoColor = "success";
                            usuario.Color = "#1CBB8C";
                            usuario.BloqueoCartera = "A";
                            break;
                        case "S":
                            usuario.EstadoCartera = "Suspendido";
                            usuario.EstadoColor = "danger";
                            usuario.Color = "#DC3545";
                            usuario.BloqueoCartera = "S";
                            break;
                        case "T":
                            usuario.EstadoCartera = "Activo Temporal";
                            usuario.EstadoColor = "warning";
                            usuario.Color = "#FCB92C";
                            usuario.BloqueoCartera = "T";
                            break;
                    }
                    EstablecerSesionMenuOpciones(usuario.RolID, true);
                    ViewData["cartera"] = validarCartera;
                }
                else
                {
                    EstablecerSesionMenuOpciones(usuario.RolID, false);
                }

                EstablecerSesionUsuario(usuario);


                return View();
            }
            catch (Exception)
            {

                return RedirectToAction("Index", "Login");
            }
        }

        private void EstablecerSesionMenuOpciones(int roldId, bool exito)
        {
            if (exito)
            {
                var menuOpciones = ObtenerMenu(roldId);
                HttpContext.Session.SetString("menu", JsonConvert.SerializeObject(menuOpciones).ToString());
            }
            else
            {
                var menuOpciones = JsonConvert.DeserializeObject<List<MenuItem>>("[{}]")!.FirstOrDefault(x => x.OpcionID == 0)!.Items;
                HttpContext.Session.SetString("menu", JsonConvert.SerializeObject(menuOpciones).ToString());
            }

        }
        private void EstablecerSesionUsuario(ObjUsuario usuario)
        {
            HttpContext.Session.SetString("usuario", JsonConvert.SerializeObject(usuario).ToString());
        }
        private void EstablecerSesionCartera(string cartera)
        {
            HttpContext.Session.SetString("cartera", cartera);
        }

        private IEnumerable<MenuItem> ObtenerMenu(int roldId)
        {
            // Cargar el menu.json en el objeto final
            var menu = new List<MenuItem>();

            var menuJson = System.IO.File.ReadAllText("MenuHumaLab.json", Encoding.UTF8);
            var menuInicial = JsonConvert.DeserializeObject<List<MenuItem>>(menuJson)!.FirstOrDefault(x => x.OpcionID == 0)!.Items;


            foreach (var item in menuInicial)
            {
                int[] list = item.IdRolGrupo.Split(',').Select(Int32.Parse).ToArray();

                if (Array.IndexOf(list, roldId) != -1)
                {
                    menu.Add(item);
                }
            }

            return menu.OrderBy(x => x.OpcionID).ToList();
        }

        public List<Factura> facturasPendientes(ClienteRequest cliente)
        {
            List<Factura> ordenesFactura = facturasOperacion.facturasPendientes(cliente);
            return ordenesFactura;
        }

        public List<ClienteResponse> informacionCliente(ClienteRequest cliente)
        {
            List<ClienteResponse> clientesHuma = facturasOperacion.informacionCliente(cliente);
            return clientesHuma;
        }

        public Portal.Humalab.Servicio.Modelos.Facturas.FacturaBase64 pdfFactura(DatosFactura facturas)
        {
            PdfHumalab pdfHumalab = new PdfHumalab();
            return pdfHumalab.GenerarDocumFactu(facturas);

            //return facturasOperacion.pdfFactura(facturas);
        }


        public Task<FacturaFlexResponse> Factura(ConsolidarFacturacion facturas)
        {
            var ctx = this.HttpContext.Session.GetString("usuario");
            ObjUsuario infoUsuario = JsonConvert.DeserializeObject<ObjUsuario>(ctx!)!;

            return facturasOperacion.facturar(facturas, infoUsuario);
        }


        //Seccion para el administrador de pruebas
        //carga de pruebas orginal
        public Task<string> CatalogoPruebasAdmin()
        {
            string path = "catalogoMuestras.json";
            string pathNew = "catalogoMuestrasFinal.json";
            Task<string>? catalogo = null;
            Task<string>? menuJson = null;

            //menuJson = System.IO.File.ReadAllTextAsync(path, Encoding.UTF8);
            //catalogo = menuJson;
            FileInfo fileInfo = new FileInfo(pathNew);
            if (fileInfo.Exists)
            {
                menuJson = System.IO.File.ReadAllTextAsync(pathNew, Encoding.UTF8);
                catalogo = menuJson;
            }
            else
            {
                menuJson = System.IO.File.ReadAllTextAsync(path, Encoding.UTF8);
                catalogo = menuJson;
            }

            return catalogo!;
        }

        //guarda un nuevo archivo con las pruebas
        //a utilizar
        public async Task<string> JsonFinalPruebas(string pruebaFinalJSON)
        {
            string pathFinal = "catalogoMuestrasFinal.json";
            string json = pruebaFinalJSON;
            string result;

            try
            {
                await WriteFileAsync("", pathFinal, json);
                result = "00";
            }
            catch (Exception ex)
            {
                result = "01";
            }            
            return result;
        }

        public async Task<string> JsonFinalPruebasNew(string pruebaFinalJSONew)
        {
            string pathFinal = "catalogoMuestrasFinalNew.json";
            string json = pruebaFinalJSONew;
            string result;

            try
            {
                await WriteFileAsync("", pathFinal, json);
                result = "00";
            }
            catch (Exception ex)
            {
                result = "01";
            }
            return result;
        }

        public static async Task WriteFileAsync(string dir, string file, string content)
        {
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(dir, file)))
            {
                await outputFile.WriteAsync(content);
            }
        }

    }
}
