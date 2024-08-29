/************************************************************************
*	Stored procedure: pr_humalab_clienteCorreoElim						*
*	Base de datos: DbPortalHumalab						  			    *
*	Elaborado por: José Guarnizo							            *
*----------------------------------------------------------------------	*
*					DESCRIPCION DEL PROCEDIMIENTO						*
*	En este Procedimiento correo cuando se elimina un pedido            *
*	                                                                    *
*----------------------------------------------------------------------	*
*					BITACORA DE MODIFICACIONES							*
*	FECHA AUTOR RAZON													*
*						                                                *
*----------------------------------------------------------------------	*/
IF NOT EXISTS (SELECT * FROM  sys.procedures WHERE NAME = 'pr_humalab_clienteCorreoElim')	
	EXEC('Create Procedure dbo.pr_humalab_clienteCorreoElim As')
go

ALTER PROCEDURE [dbo].[pr_humalab_clienteCorreoElim](
	@i_accion char(1),
	@i_idPedido int
)

as

declare @idOrden int

if @i_accion = 'C'
begin
	
	if exists (select top 1 IdPedido from Pedido)
	begin
		
		select @idOrden = IdOrden
		from Orden
		where IdPedido = @i_idPedido

		SELECT c.NombreCliente AS Cliente,
			c.Telefono AS Telefono,
			cd.Direccion AS DireccionCliente,
			us.Email AS Correo
		FROM dbo.Orden o
		INNER JOIN dbo.Usuario us ON us.idGalileo = o.UsuarioCreacion
		--INNER JOIN dbo.Cliente c ON c.IdUsuario = us.idUsuario nuevo 2024/01/25	
		INNER JOIN dbo.Cliente c ON c.CodClienteCta = us.CodClienteCta --nuevo 2024/01/25
		INNER JOIN dbo.ClienteDireccion cd ON c.IdCliente = cd.IdCliente
		AND COALESCE(o.Eliminado,0) = 0
		WHERE o.IdOrden = @idOrden

		select NumeroRemision
		from Pedido
		where IdPedido = @i_idPedido

	end

end


GO