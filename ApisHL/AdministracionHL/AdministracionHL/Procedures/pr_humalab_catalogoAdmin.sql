/************************************************************************
*	Archivo Fisico: pr_humalab_catalogoAdmin.sql					    *
*	Stored procedure: pr_humalab_catalogoAdmin						    *
*	Base de datos: DbPortalHumalab						  			    *
*	Producto: Portal Clientes Humalab					                *
*	Elaborado por: Jose Guarnizo						                *
*----------------------------------------------------------------------	*
*					DESCRIPCION DEL PROCEDIMIENTO						*
*	En este Procedimiento consulta los estados de las ordenes           *
*----------------------------------------------------------------------	*
*					BITACORA DE MODIFICACIONES							*
*	FECHA AUTOR RAZON													*
*						                                                *
*----------------------------------------------------------------------	*/
IF NOT EXISTS (SELECT * FROM sys.procedures WHERE name = 'pr_humalab_catalogoAdmin')	
	EXEC('Create Procedure dbo.pr_humalab_catalogoAdmin As')
GO

ALTER PROCEDURE [dbo].pr_humalab_catalogoAdmin(
	@i_accion Char(2),
	@estado  VARCHAR(20)=NULL,
	@valor VARCHAR(10)=NULL
)

AS

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