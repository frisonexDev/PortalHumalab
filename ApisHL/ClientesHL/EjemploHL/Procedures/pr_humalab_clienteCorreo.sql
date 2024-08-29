/************************************************************************
*	Stored procedure: pr_humalab_clienteCorreo							*
*	Base de datos: DbPortalHumalab						  			    *
*	Elaborado por: José Guarnizo							            *
*----------------------------------------------------------------------	*
*					DESCRIPCION DEL PROCEDIMIENTO						*
*	En este Procedimiento consulta el correo del cliente                *
*	                                                                    *
*----------------------------------------------------------------------	*
*					BITACORA DE MODIFICACIONES							*
*	FECHA AUTOR RAZON													*
*						                                                *
*----------------------------------------------------------------------	*/
IF NOT EXISTS (SELECT * FROM  sys.procedures WHERE NAME = 'pr_humalab_clienteCorreo')	
	EXEC('Create Procedure dbo.pr_humalab_clienteCorreo As')
go

ALTER PROCEDURE [dbo].[pr_humalab_clienteCorreo](
	@i_accion char(1),
	@idOrden int
)

as

if @i_accion = 'C'
begin
	
	if exists (select top 1 IdOrden from Orden)
	begin
		
		SELECT c.NombreCliente AS Cliente,
			c.Telefono AS Telefono,
			cd.Direccion AS DireccionCliente,
			us.Email AS Correo
		FROM dbo.Orden o
		INNER JOIN dbo.Usuario us ON us.idGalileo = o.UsuarioCreacion
		--INNER JOIN dbo.Cliente c ON c.IdUsuario = us.idUsuario		
		INNER JOIN dbo.Cliente c ON c.CodClienteCta = us.CodClienteCta --nuevo 2024/01/25
		INNER JOIN dbo.ClienteDireccion cd ON c.IdCliente = cd.IdCliente
		AND COALESCE(o.Eliminado,0) = 0
		WHERE o.IdOrden = @idOrden

		select COUNT(*) as totalMuestras
		from Muestra
		where IdOrden = @idOrden 

	end

end

GO
