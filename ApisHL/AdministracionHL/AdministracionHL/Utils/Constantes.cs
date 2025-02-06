namespace AdministracionHL.Utils;

public class Constantes
{
    public struct Transaccion
    {
        public const int Default = 0;
        public const int Correcta = 201;
        public const int Error = 400;
    }

    public struct Estados
    {
        public const string Orden = "EstadoOrden";
        public const string Prueba = "EstadoPrueba";
        public const string Muestra = "EstadoMuestra";
        public const string Pedido = "EstadoPedido";

        public const string Generada = "GENE";
        public const string Cancelada = "CANC";
        public const string PorRecolectar = "PREC";
        public const string RecolectadoTotal = "RCTL";
        public const string RecolectadoParcial = "RCTP";
        public const string Rechazada = "RCHZ";
        public const string EnAnalisis = "ANLS";
        public const string Validado = "VALD";
        public const string Facturadas = "FACT";
        public const string Anulado = "ANUL";

        public const string Recolectada = "RECT";
        public const string Recibida = "RECB";
        public const string RechazaOp = "RCHO";
        public const string RechazaLb = "RCHL";
        public const string NoProcesada = "NPRC";
    }

    public struct Numeros
    {
        public const int Cero = 0;
        public const int Uno = 1;
        public const int Dos = 2;
        public const int Tres = 3;
        public const int Cuatro = 4;
        public const int Cinco = 5;
        public const int Seis = 6;
        public const int Siete = 7;
        public const int Ocho = 8;
        public const int Nueve = 9;
    }

    public struct Caracteres
    {
        public const string Guion = "-";
    }
}
