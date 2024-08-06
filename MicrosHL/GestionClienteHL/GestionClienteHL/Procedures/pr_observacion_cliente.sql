/****************************************************************************
 *	Archivo Físico:		pr_observacion_cliente.sql							*
 *	Stored Procedure:	pr_observacion_cliente								*
 *	Base de Datos:		DbPortalHumalab									    *
 *	Producto:			Portales Clientes Humalab						    *
 *	Elaborado por:		Jose Guarnizo  								        *
 *--------------------------------------------------------------------------*
 *						DESCRIPCION DEL PROCEDIMIENTO						*
 *	Obtiene el nombre del cliente                                        	*
 *--------------------------------------------------------------------------*
 *						BITACORA DE MODIFICACIONES							*
 *	FECHA		AUTOR				RAZON									*
 *	DD/MM/YYYY	[Desarrollador]		Versión inicial							* 
 *--------------------------------------------------------------------------*/
IF NOT EXISTS (SELECT * FROM sys.procedures WHERE name = 'pr_observacion_cliente')	
	EXEC('Create Procedure dbo.pr_observacion_cliente As')
GO

ALTER PROCEDURE [dbo].[pr_observacion_cliente] (
	@i_accion char(1),
	@idUsuario int
)
AS

if exists (select 1 from Usuario where idUsuario = @idUsuario)
begin

	select Observacion
	from Usuario
	where idUsuario = @idUsuario	

end

GO