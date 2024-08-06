/************************************************************************
*	Stored procedure: pr_humalab_codLis					                *
*	Base de datos: DbPortalHumalab						  			    *
*	Elaborado por: José Guarnizo						                *
*----------------------------------------------------------------------	*
*					DESCRIPCION DEL PROCEDIMIENTO						*
*	En este Procedimiento consulta el codigo lis de la orden            *
*	                                                                    *
*----------------------------------------------------------------------	*
*					BITACORA DE MODIFICACIONES							*
*	FECHA AUTOR RAZON													*
*						                                                *
*----------------------------------------------------------------------	*/
IF NOT EXISTS (SELECT * FROM  sys.procedures WHERE NAME = 'pr_humalab_codLis')	
	EXEC('Create Procedure dbo.pr_humalab_codLis As')
go

ALTER PROCEDURE [dbo].[pr_humalab_codLis](
	@i_accion char,
	@codigoBarra varchar(100)
)

as

declare @codLis varchar(100)

if @i_accion = 'C'
begin

	select @codLis = Resultados 
	from Orden
	where CodigoBarra = @codigoBarra
	and Eliminado != 1 --15/01/2024

	select @codLis

end

GO