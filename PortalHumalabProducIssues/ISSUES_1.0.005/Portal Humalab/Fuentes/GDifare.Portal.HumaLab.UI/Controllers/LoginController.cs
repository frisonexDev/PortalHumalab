using GDifare.Portal.Humalab.Seguridad.Modelos;
using GDifare.Portal.Humalab.Seguridad.Operation;
using GDifare.Portal.Humalab.Servicio.Seguridad;
using GDifare.Portal.HumaLab.UI.Models;
using GDifare.Portal.HumaLab.UI.Utils;
using GDifare.Portales.HumaLab.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace GDifare.Portales.HumaLab.UI.Controllers
{
    public class LoginController : Controller
    {
        private readonly UsuarioOperation usuarioOperations;
        private readonly SeguridadOperations seguridadOperations;

        private string UserAvalab;
        private string PassAvalab;

        public LoginController(AppSettings settings)
        {
            //Microservicio Difare de seguridad
            usuarioOperations = new UsuarioOperation(settings.ServerSeguridad, settings.PortSeguridad, settings.Token, settings.UserAvalab, settings.PassAvalab);           
        }


        // GET: LoginController
        public ActionResult Index()
        {

            return View();
        }
        public int Validation(CredencialesRequest userInfo)
        {
            try
            {
                if (userInfo.Usuario != null || userInfo.Usuario != "")
                {

                    var usrdes = encryptInfo.DecryptStringAES(userInfo.Usuario);
                    //var usrpas = encryptInfo.DecryptStringAES(userInfo.Clave);

                    userInfo.Usuario = usrdes;
                    //userInfo.Clave = usrpas;

                    bool valido = usuarioOperations.Autenticar(userInfo);
                    
                    if (valido)
                    {
                        ObjUsuario usuario = usuarioOperations.ObtenerUsuario(userInfo.Usuario);
                     //   bool validoClienteHumalab = seguridadOperations.ConsultaCliente(usuario.Identificacion);

                        var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Sid, usuario.UsuarioID.ToString()),
                                new Claim(ClaimTypes.UserData, usuario.Identificacion),
                                new Claim(ClaimTypes.Name, usuario.NombreCompleto),
                                new Claim(ClaimTypes.Role, usuario.RolID.ToString()),
                                new Claim(ClaimTypes.Email, usuario.Correo)
                            };

                        var claimsIdentity = new ClaimsIdentity(claims, "Identity.Application");

                        var authProperties = new AuthenticationProperties
                        {
                            AllowRefresh = true,
                            IsPersistent = true
                        };

                        HttpContext.SignInAsync(
                            "Identity.Application",
                            new ClaimsPrincipal(claimsIdentity),
                            authProperties
                        );
                        return 200;
                    }
                    else
                    {
                       return 404;
                    }
                }
                return 404;
            }
            catch (Exception)
            {

                return 500;
            }

        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]

       
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
