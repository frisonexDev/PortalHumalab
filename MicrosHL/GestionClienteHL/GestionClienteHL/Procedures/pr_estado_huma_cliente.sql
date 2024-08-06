/************************************************************************
*	Archivo Fisico: pr_estado_huma_cliente.sql							*
*	Stored procedure: pr_estado_huma_cliente							*
*	Base de datos: DbPortalHumalab						  			    *
*	Producto: Portal Clientes Humalab administracion					*
*	Elaborado por: José Guarnizo							            *
*----------------------------------------------------------------------	*
*					DESCRIPCION DEL PROCEDIMIENTO						*
*	En este Procedimiento se va a obtener todos los estados de los      *
*   clientes.                                                           *
*----------------------------------------------------------------------	*
*					BITACORA DE MODIFICACIONES							*
*	FECHA AUTOR RAZON													*
*						                                                *
*----------------------------------------------------------------------	*/
IF NOT EXISTS (SELECT * FROM  sys.procedures WHERE NAME = 'pr_estado_huma_cliente')	
	EXEC('Create Procedure dbo.pr_estado_huma_cliente As')
go

ALTER PROCEDURE [dbo].[pr_estado_huma_cliente](	
	@i_accion char(1)
)

as

declare @i_maestro int

if @i_accion = 'C'
begin
	
	select @i_maestro = IdCatalogoMaestro
	from CatalogoMaestro
	where Nombre = 'EstadoCliente'

	if exists (select top 1 IdCatalogoDetalle from CatalogoDetalle)
	begin
		select cd.IdCatalogoDetalle, cd.Nombre, cd.Valor
		from CatalogoDetalle cd
		join CatalogoMaestro cm on cd.IdCatalogoMaestro = cm.IdCatalogoMaestro
		where cd.IdCatalogoMaestro = @i_maestro 
	end

end

GO