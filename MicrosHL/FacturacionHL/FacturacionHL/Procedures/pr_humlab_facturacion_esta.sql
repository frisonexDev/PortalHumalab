/************************************************************************
*	Stored procedure: pr_humlab_facturacion_esta					    *
*	Base de datos: DbPortalHumalab						  			    *
*	Elaborado por: Jose Guarnizo						                *
*----------------------------------------------------------------------	*
*					DESCRIPCION DEL PROCEDIMIENTO						*
*	En este Procedimiento consulta estados de factura                   *
*	                                                                    *
*----------------------------------------------------------------------	*
*					BITACORA DE MODIFICACIONES							*
*	FECHA AUTOR RAZON													*
*						                                                *
*----------------------------------------------------------------------	*/
IF NOT EXISTS (SELECT * FROM sys.procedures WHERE name = 'pr_humlab_facturacion_esta')	
	EXEC('Create Procedure dbo.pr_humlab_facturacion_esta As')
GO

ALTER PROCEDURE [dbo].[pr_humlab_facturacion_esta](
	@i_accion char
)

as

declare @i_idCatalogoMaestro int


IF @i_accion = 'C'
BEGIN
	
	select @i_idCatalogoMaestro = IdCatalogoMaestro
	from CatalogoMaestro
	where Nombre = 'EstadoOrden'

	select IdCatalogoDetalle as Validadas
	from CatalogoDetalle
	where IdCatalogoMaestro = @i_idCatalogoMaestro
	and Valor = 'VALD'

	select IdCatalogoDetalle as Facturadas
	from CatalogoDetalle
	where IdCatalogoMaestro = @i_idCatalogoMaestro
	and Valor = 'FACT'

END
GO