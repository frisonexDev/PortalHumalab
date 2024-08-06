/************************************************************************
*	Stored procedure: pr_humalab_tipoCliente					        *
*	Base de datos: DbPortalHumalab						  			    *
*	Elaborado por: José Guarnizo						                *
*----------------------------------------------------------------------	*
*					DESCRIPCION DEL PROCEDIMIENTO						*
*	En este Procedimiento consulta el tipo de cliente para la orden     *
*	                                                                    *
*----------------------------------------------------------------------	*
*					BITACORA DE MODIFICACIONES							*
*	FECHA AUTOR RAZON													*
*						                                                *
*----------------------------------------------------------------------	*/
IF NOT EXISTS (SELECT * FROM  sys.procedures WHERE NAME = 'pr_humalab_tipoCliente')	
	EXEC('Create Procedure dbo.pr_humalab_tipoCliente As')
go

ALTER PROCEDURE [dbo].[pr_humalab_tipoCliente](
	@i_accion char
)

as

declare @idMaestroTipo int

select @idMaestroTipo = IdCatalogoMaestro
from CatalogoMaestro
where Nombre = 'TipoCliente'

if @i_accion = 'C'
begin

	select IdCatalogoDetalle as IdCatalogo,
		Nombre as NombreCatalogo,
		Valor as ValorCatalogo
	from CatalogoDetalle
	where IdCatalogoMaestro = @idMaestroTipo

end

GO