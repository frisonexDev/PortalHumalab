/************************************************************************
*	Stored procedure: pr_humalab_catalogo								*
*	Base de datos: DbPortalHumalab						  			    *
*	Elaborado por: Daniel Nicolalde							            *
*----------------------------------------------------------------------	*
*					DESCRIPCION DEL PROCEDIMIENTO						*
*	En este Procedimiento consulta el catalogo detalle                  *
*	                                                                    *
*----------------------------------------------------------------------	*
*					BITACORA DE MODIFICACIONES							*
*	FECHA AUTOR RAZON													*
*						                                                *
*----------------------------------------------------------------------	*/
IF NOT EXISTS (SELECT * FROM  sys.procedures WHERE NAME = 'pr_humalab_catalogo')	
	EXEC('Create Procedure dbo.pr_humalab_catalogo As')
go

ALTER PROCEDURE [dbo].[pr_humalab_catalogo](
	@i_accion Char(2),
	@estado  VARCHAR(20)=NULL,
	@valor VARCHAR(10)=NULL
)

as

if(@i_accion='C')
BEGIN

	Select ctd.IdCatalogoDetalle AS 'IdDetalle' from [dbo].[CatalogoMaestro] ctm
	inner join [dbo].[CatalogoDetalle] ctd on ctm.IdCatalogoMaestro=ctd.IdCatalogoMaestro
	where ctm.Nombre=@estado AND ctd.Valor = @valor

END

if(@i_accion='C1')
BEGIN

	Select ctd.Valor AS 'Abreviatura', ctd.Nombre AS 'Estados' from [dbo].[CatalogoMaestro] ctm
	inner join [dbo].[CatalogoDetalle] ctd on ctm.IdCatalogoMaestro=ctd.IdCatalogoMaestro
	where ctm.Nombre=@estado

END

GO