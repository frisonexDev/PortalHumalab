
/****************************************************************************
 *	Archivo Físico:		pr_cliente_nombre.sql								*
 *	Stored Procedure:	prm_cliente_nombre									*
 *	Base de Datos:		DbPortalHumalab									    *
 *	Producto:			Portales Clientes Humalab						    *
 *	Elaborado por:		Jose Guarnizo  								        *
 *--------------------------------------------------------------------------*
 *						DESCRIPCION DEL PROCEDIMIENTO						*
 *	Obtiene el nombre del cliente                                        	*
 *--------------------------------------------------------------------------*
 *						BITACORA DE MODIFICACIONES							*
 *	FECHA		AUTOR				RAZON									*
 *	03/12/2024	Jose Guarnizo      Se modifica para que busque por id de    *
 *								   muestra									*
 *--------------------------------------------------------------------------*/
 IF NOT EXISTS (SELECT * FROM sys.procedures WHERE name = 'pr_cliente_nombre')	
	EXEC('Create Procedure dbo.pr_cliente_nombre As')
GO

ALTER PROCEDURE [dbo].[pr_cliente_nombre] (
	@i_accion char(1),
	@i_id int,
	@i_codbarra varchar(100),
	@id_muestra int = null
)
AS

declare @nombre varchar(100),
		@idUsuario int,
		@i_idGalileo int,
		@i_idOrden int,
		@i_identi varchar(20)

IF @i_accion = 'C'
BEGIN
	
	if exists (select 1 from Usuario where idGalileo =  @i_id and IdRol = 100)
	begin
		
		select @i_identi = Identificacion
		from Usuario
		where idGalileo = @i_id

		select @nombre = NombreCliente
		from Cliente
		where Identificacion = @i_identi

		select @nombre as NombreCliente

		select @i_idGalileo = IdMuestraGalileo,
			   @i_idOrden = IdOrden
		from Muestra
		where CodigoBarra = @i_codbarra
		and IdMuestra = @id_muestra
		and Eliminado != 1

		select pc.Nombres + ' ' + pc.Apellidos as Nombres, pc.Identificacion, mt.Nombre
		from Paciente pc
		inner join Orden od on pc.Identificacion = od.Identificacion
		inner join Muestra mt on od.IdOrden = mt.IdOrden
		where od.IdOrden = @i_idOrden
		and mt.IdMuestraGalileo = @i_idGalileo

	end
END

GO