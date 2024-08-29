/************************************************************************
*	Stored procedure: pr_humalab_rolcliente						        *
*	Base de datos: DbPortalHumalab						  			    *
*	Elaborado por: Jose Guarnizo						                *
*----------------------------------------------------------------------	*
*					DESCRIPCION DEL PROCEDIMIENTO						*
*	En este Procedimiento consulta rol del cliente                      *
*	                                                                    *
*----------------------------------------------------------------------	*
*					BITACORA DE MODIFICACIONES							*
*	FECHA AUTOR RAZON													*
*						                                                *
*----------------------------------------------------------------------	*/
IF NOT EXISTS (SELECT * FROM sys.procedures WHERE name = 'pr_humalab_rolcliente')	
	EXEC('Create Procedure dbo.pr_humalab_rolcliente As')
GO

ALTER PROCEDURE [dbo].[pr_humalab_rolcliente](
	@i_ruc varchar(13)
)

as

declare @existe int  = 0
		
select @existe =count(*) from Usuario where Identificacion = @i_ruc

if @existe != 0 or @existe != null
begin
	select 'Activo' as estado
end
else
begin
	select 'Inactivo' as estado
end

GO