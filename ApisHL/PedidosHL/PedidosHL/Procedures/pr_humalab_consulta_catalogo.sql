/************************************************************************
*	Stored procedure: pr_humalab_consulta_catalogo						*
*	Base de datos: DbPortalHumalab						  			    *
*	Elaborado por: José Guarnizo						                *
*----------------------------------------------------------------------	*
*					DESCRIPCION DEL PROCEDIMIENTO						*
*	En este Procedimiento consulta catalogo                             *
*	                                                                    *
*----------------------------------------------------------------------	*
*					BITACORA DE MODIFICACIONES							*
*	FECHA AUTOR RAZON													*
*						                                                *
*----------------------------------------------------------------------	*/
IF NOT EXISTS (SELECT * FROM  sys.procedures WHERE NAME = 'pr_humalab_consulta_catalogo')	
	EXEC('Create Procedure dbo.pr_humalab_consulta_catalogo As')
go

ALTER PROCEDURE [dbo].[pr_humalab_consulta_catalogo](
	@i_accion Char(2),
	@i_maestro  VARCHAR(100)
)

as

if(@i_accion='C')
BEGIN

	Select ctd.IdCatalogoDetalle
	,ctd.Nombre
	,ctd.Valor
	from [dbo].[CatalogoMaestro] ctm
	inner join [dbo].[CatalogoDetalle] ctd on ctm.IdCatalogoMaestro=ctd.IdCatalogoMaestro
	where ctm.Nombre = @i_maestro

END

GO