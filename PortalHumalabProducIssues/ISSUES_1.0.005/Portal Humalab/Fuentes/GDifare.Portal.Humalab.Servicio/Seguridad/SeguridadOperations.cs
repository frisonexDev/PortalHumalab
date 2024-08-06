using GDifare.Portal.Humalab.Servicio.Modelos.Cliente;
using GDifare.Portal.Humalab.Servicio.Resources;
using GDifare.Portales.Comunicaciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace GDifare.Portal.Humalab.Servicio.Seguridad
{
    public class SeguridadOperations
    {
        private readonly Communicator CommunicatorSeguridad;
        public SeguridadOperations(string server, int port, string token)
        {
            CommunicatorSeguridad = new Communicator(server, port, Routes.PathServicioCliente, token);
        }

        public bool ConsultaCliente(string cliente)
        {
            ClienteResponse? infocliente = null;
            bool existeCliente = false;
            try
            {
                var metodo = "consultarClienteHumalab";

                if (!string.IsNullOrWhiteSpace(cliente))
                {
                    metodo += string.Format("?Ruc={0}", HttpUtility.UrlEncode(cliente));
                }


                infocliente = CommunicatorSeguridad.InvokeOperation<ClienteResponse>(metodo, TipoOperacion.GET);

                if(infocliente!=null) existeCliente = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return existeCliente!;
        }
    }
}
