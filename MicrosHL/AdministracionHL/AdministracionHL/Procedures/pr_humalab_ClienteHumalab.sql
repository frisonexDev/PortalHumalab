/************************************************************************
*	Archivo Fisico: pr_humalab_ClienteHumalab.sql					    *
*	Stored procedure: pr_humalab_ClienteHumalab						    *
*	Base de datos: DbPortalHumalab						  			    *
*	Producto: Portal Clientes Humalab					                *
*	Elaborado por: Jose Guarnizo						                *
*----------------------------------------------------------------------	*
*					DESCRIPCION DEL PROCEDIMIENTO						*
*	En este Procedimiento consulta ruc del cliente                      *
*	                                                                    *
*----------------------------------------------------------------------	*
*					BITACORA DE MODIFICACIONES							*
*	FECHA AUTOR RAZON													*
*						                                                *
*----------------------------------------------------------------------	*/
IF NOT EXISTS (SELECT * FROM sys.procedures WHERE name = 'pr_humalab_ClienteHumalab')	
	EXEC('Create Procedure dbo.pr_humalab_ClienteHumalab As')
GO

ALTER PROCEDURE [dbo].[pr_humalab_ClienteHumalab](
	@i_accion CHAR(1),		
	----Usuario--	

	@i_ruc VARCHAR(15)=NULL				
)

AS

declare  @i_IdOrdenDetalle int
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
select top 1 idUsuario,idGalileo,Usuario,Identificacion,IdRol,Estado
from usuario where Identificacion =  @i_ruc-- @ruc and cast(f.FechaCreacion as date) between cast(@fdesde as date) and cast( @ffhasta as date)


	RETURN 0
END

IF @i_accion = 'G'
BEGIN
	

	RETURN 0
END

RAISERROR ('El código de la acción es incorrecto.',16,1)

GO