namespace PedidosHL.Utils;

public class StringHandler
{
	// Nombres de módulo y proyecto
	internal const string ModuleName = "Humalab";
	internal const string ProjectName = "Pedidos";

	// Nombres de procedimientos
	internal const string ProcedureExample = "pr_ejemplo";
	internal const string ProcedureOperadorPedidos = "pr_operador_pedidos";
	internal const string ProcedureOperadorPedidosNew = "pr_operador_pedidos_new";
	internal const string ProcedureOperadorPedidosActu = "pr_operador_pedidosActu";
	internal const string ProcedureOperadorMuestras = "pr_operador_muestras";
	internal const string ProcedureOrdenesLaboratorista = "pr_humalab_ordeneslaboratorista";
	internal const string ProcedureEstadosCatalogo = "pr_humalab_consulta_catalogo";
	internal const string ProcedureCambiaPrueba = "pr_humalab_pruebaslaboratorista";
	internal const string ProcedureConsultaOrdenGalileo = "pr_humalab_consultaOrdenGalileo";
	internal const string ProcedureVerificarActPedi = "pr_humalab_pedidoAct";


	internal const string DATABASEDEV = "DATABASEHUMALAB";

	//Valores por defecto para armar Orden Galileo
	internal const int IdLaboratorio = 0;
	internal const int IdSede = 1;
	internal const string Laboratorio = "HUMALAB S.A.";
	internal const string CieDiagnostico = "U00-U99";
	internal const string IdSistemaExterno = "2";
	internal const string PrioridadOrden = "R";
	internal const string Usuario = "SOPORTE_HL";
	internal const string CampoMedicamentos = "Medicamentos";
	internal const string CampoDiagnostico = "Diagnóstico Específicos";
	internal const bool TomaRemota = true;
	internal const string DireccionToma = null;
	internal const bool TomaMuestraActivo = true;
	internal const bool InfoOrdenActivo = true;
}
