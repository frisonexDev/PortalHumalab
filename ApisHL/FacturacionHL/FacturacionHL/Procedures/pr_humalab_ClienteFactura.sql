/************************************************************************
*	Stored procedure: pr_humalab_ClienteFactura						    *
*	Base de datos: DbPortalHumalab						  			    *
*	Elaborado por: Jose Guarnizo						                *
*----------------------------------------------------------------------	*
*					DESCRIPCION DEL PROCEDIMIENTO						*
*	En este Procedimiento consulta cliente para la factura              *
*	                                                                    *
*----------------------------------------------------------------------	*
*					BITACORA DE MODIFICACIONES							*
*	FECHA AUTOR RAZON													*
*						                                                *
*----------------------------------------------------------------------	*/
IF NOT EXISTS (SELECT * FROM sys.procedures WHERE name = 'pr_humalab_ClienteFactura')	
	EXEC('Create Procedure dbo.pr_humalab_ClienteFactura As')
GO

ALTER PROCEDURE [dbo].[pr_humalab_ClienteFactura](
	@i_accion CHAR(1),		
	----Usuario--	

	@identificador VARCHAR(15)=NULL				
)

AS

declare  @i_IdOrdenDetalle int,
	@i_idCatalogoMaestro int


IF @i_accion = 'I'
BEGIN	
	RETURN 0
END

IF @i_accion = 'M'
BEGIN
	
	RETURN 0
END

IF @i_accion = 'C'
BEGIN

	select  distinct c.CodClienteCta,c.NombreCliente,c.IdOperadorLogistico,c.NombreOperadorLogistico,c.identificacion
	from cliente c

	RETURN 0
END

IF @i_accion = 'G'
BEGIN
	

	RETURN 0
END

RAISERROR ('El código de la acción es incorrecto.',16,1)

GO