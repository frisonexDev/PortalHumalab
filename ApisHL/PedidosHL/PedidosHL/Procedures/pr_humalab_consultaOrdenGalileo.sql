/************************************************************************
*	Stored procedure: pr_humalab_consultaOrdenGalileo					*
*	Base de datos: DbPortalHumalab						  			    *
*	Elaborado por: José Guarnizo						                *
*----------------------------------------------------------------------	*
*					DESCRIPCION DEL PROCEDIMIENTO						*
*	En este Procedimiento consulta orden galileo                        *
*	                                                                    *
*----------------------------------------------------------------------	*
*					BITACORA DE MODIFICACIONES							*
*	FECHA AUTOR RAZON													*
*	23/09/2024 Jose Guarnizo Se cambia que en ves del ruc envia el id   *
							 cliente.					                *
*----------------------------------------------------------------------	*/
IF NOT EXISTS (SELECT * FROM  sys.procedures WHERE NAME = 'pr_humalab_consultaOrdenGalileo')	
	EXEC('Create Procedure dbo.pr_humalab_consultaOrdenGalileo As')
go

ALTER PROCEDURE [dbo].[pr_humalab_consultaOrdenGalileo](
	@i_accion CHAR(1),		
	@i_orden INT	
)

as

declare @i_idEstadoPrue int

select @i_idEstadoPrue = IdCatalogoMaestro
from CatalogoMaestro
where Nombre = 'EstadoPrueba'

IF @i_accion = 'I'
BEGIN	
	RETURN 0
END

ELSE IF @i_accion = 'M'
BEGIN
	
	RETURN 0
END

ELSE IF @i_accion = 'C'
BEGIN

	SELECT c.NombreCliente AS Cliente
	,o.FechaCreacion AS fechaIngreso
	,o.Observacion AS comentario
	,o.CodigoBarra AS codigoExterno
	--datos paciente
	,pc.Identificacion AS Identificador
	,pc.Nombres AS NombrePaciente
	,pc.Apellidos AS ApellidoPaciente
	,pc.FechaNacimiento AS FechaNacimiento
	,CASE WHEN pc.Genero = 0 THEN 'F' ELSE 'M' END AS Genero
	,pc.Email AS CorreoElectronico
	--campos adicionales
	,'Telefonoo' AS nombreCampoAdicional
	,pc.Telefono AS valor
	--medico
	,c.NombreCliente AS Nombre
	,c.NombreCliente AS Apellido
	,c.Identificacion AS Matricula
	,c.Identificacion AS Telefono
	--Cliente
	,u.Usuario as RucCliente
	--Usuario
	,u.Email
	--informacionOrden
	,o.Medicamento AS medicamento	
	,o.Diagnostico AS diagnostico	
	--tomaMuestra
	--detalleOrden
	,pr.CodigoExamen AS CodigoExamen
	,pr.Nombre AS NombreExamenPerfil
	FROM dbo.Orden o INNER JOIN dbo.Pedido p ON o.IdPedido = p.IdPedido AND COALESCE(p.Eliminado,0) = 0 AND COALESCE(o.Eliminado,0) = 0
	INNER JOIN dbo.Usuario u ON u.idGalileo = p.UsuarioCreacion	
	inner join Cliente c on u.CodClienteCta = c.CodClienteCta --nuevo 2024/01/25
	INNER JOIN dbo.Paciente pc ON pc.Identificacion = o.Identificacion
	INNER JOIN dbo.Prueba pr ON pr.IdOrden = o.IdOrden AND COALESCE(pr.Eliminado,0) = 0
	INNER JOIN dbo.PruebaMuestra pm ON pm.IdPrueba = pr.IdPrueba AND pr.Estado = (SELECT IdCatalogoDetalle FROM CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoPrue AND Valor = 'PPRC')
	INNER JOIN dbo.Muestra m ON m.IdMuestra = pm.IdMuestra AND COALESCE(m.Eliminado,0) = 0	
	WHERE o.IdOrden = @i_orden

END

ELSE IF @i_accion = 'G'
BEGIN
	RETURN 0
END
ELSE 
	RAISERROR ('El código de la acción es incorrecto.',16,1)

GO