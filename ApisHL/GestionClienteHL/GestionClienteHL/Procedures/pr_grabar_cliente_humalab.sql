/************************************************************************
*	Archivo Fisico: pr_grabar_cliente_humalab.sql						*
*	Stored procedure: pr_grabar_cliente_humalab							*
*	Base de datos: DbPortalHumalab						  			    *
*	Producto: Portal Clientes Humalab administracion					*
*	Elaborado por: José Guarnizo							            *
*----------------------------------------------------------------------	*
*					DESCRIPCION DEL PROCEDIMIENTO						*
*	En este Procedimiento va a poder registrar un nuevo cliente         *
*	de Humalab									                        *
*----------------------------------------------------------------------	*
*					BITACORA DE MODIFICACIONES							*
*	FECHA AUTOR RAZON													*
*	2024/08/27 Jose Guarnizo Se agrega campo para nombre comercial		*
*							 laboratorio.								*
*   2024/10/31 Jose Guarnizo Se modifica para que tambien se actualice  *
*							 el nombre comercial del laboratorio.       *
*----------------------------------------------------------------------	*/
IF NOT EXISTS (SELECT * FROM  sys.procedures WHERE NAME = 'pr_grabar_cliente_humalab')	
	EXEC('Create Procedure dbo.pr_grabar_cliente_humalab As')
go

ALTER PROCEDURE [dbo].[pr_grabar_cliente_humalab] (
	@i_id_galileo int,
	@i_usuario varchar(200),
	@i_identificacion varchar(100),
	@i_id_rol int,
	@i_email varchar(200) = null,
	@i_usuario_creacion int,
	@i_usuario_asesor varchar(10) = null,
	@i_nombre_asesor varchar(100),
	@i_nombre_galileo varchar(100),
	@i_accion char(1),
	@i_codCliente varchar(100),
	@i_direccion varchar(100),
	@i_provincia varchar(100),
	@i_ciudad varchar(50),
	@i_latitud varchar(50) = null,
	@i_longitud varchar(50) = null,
	@id_OperadorLis varchar(50) = null,
	@i_telefono varchar(50) = null,
	@i_labComercial varchar(MAX) = null
)

as

declare @idEstadoCliente int,
	@valorEstado char,
	@mensajeError varchar(200),
	@codigoError varchar(10),
	@idUsuario int,
	@idCliente int		

select @idEstadoCliente = idCatalogoMaestro 
from CatalogoMaestro
where Nombre = 'EstadoCliente'

select @valorEstado = Valor
from CatalogoDetalle
where IdCatalogoMaestro = @idEstadoCliente
and Nombre = 'Activo'

begin
	if @i_accion = 'I'
	begin
		
		if not exists (select 1 from Usuario where Usuario = @i_nombre_galileo)
		begin

			insert into Usuario (
				idGalileo, Usuario,
				Identificacion,IdRol, 
				Estado, Email, 
				Fechavigencia, Observacion, 
				UsuarioCreacion, FechaCreacion, 
				UsuarioModificacion, FechaModificacion, 
				UsuarioEliminacion, FechaEliminacion, Eliminado,
				CodClienteCta
			)
			values(
				@i_id_galileo, @i_nombre_galileo,
				@i_identificacion, @i_id_rol, 
				@valorEstado, @i_email, 
				null, null, 
				@i_usuario_creacion, getdate(), 
				null, null, 
				null, null, null, @i_codCliente
			)

			select @idUsuario = idUsuario
			from Usuario
			where Identificacion = @i_identificacion
			and Estado = @valorEstado

			if not exists (select 1 from Cliente where CodClienteCta = @i_codCliente)
			begin
				
				insert into Cliente(
					IdUsuario, Identificacion,
					NombreCliente, IdOperadorLogistico,
					NombreOperadorLogistico, UsuarioCreacion,
					FechaCreacion, UsuarioModificacion,
					FechaModificacion, UsuarioEliminacion,
					FechaEliminacion, Eliminado, 
					CodClienteCta, IdOperadorLis, Telefono,
					aux1
				)
				values(
					@idUsuario, @i_identificacion,
					@i_usuario, @i_usuario_asesor,
					@i_nombre_asesor, @i_usuario_creacion,
					getdate(), null,
					null, null,
					null, null, 
					@i_codCliente, @id_OperadorLis, @i_telefono,
					@i_labComercial
				)

				select @idCliente = IdCliente
				from Cliente
				where Identificacion = @i_identificacion
				and IdUsuario = @idUsuario

			end

			insert into ClienteDireccion(
				IdCliente, Direccion, Provincia,
				Ciudad, Latitud, Longitud,
				FechaCreacion, UsuarioModificacion,
				FechaModificacion, UsuarioEliminacion,
				FechaEliminacion, Eliminado,
				UsuarioCreacion
			)
			values(
				@idCliente, @i_direccion, @i_provincia,
				@i_ciudad, @i_latitud, @i_longitud,
				GETDATE(), null,
				null, null,
				null, null,
				@i_usuario_creacion
			)

			set @mensajeError  = 'Cliente registrado'
			set @codigoError = '00'
			
			select @mensajeError, @codigoError

		end
		else
		begin
			
			set @mensajeError  = 'Cliente ya se encuetra registrado'
			set @codigoError = '01'
			
			select @mensajeError, @codigoError

		end
	end

	if @i_accion = 'M'
	begin
		
		if exists (select 1 from Usuario where idGalileo = @i_id_galileo)
		begin
			update Usuario
			set Usuario = @i_nombre_galileo, 
				Email = @i_email,
				UsuarioModificacion = @i_usuario_creacion, 
				FechaModificacion = getdate()				
			where idGalileo = @i_id_galileo			

			select @idUsuario = idUsuario
			from Usuario
			where idGalileo = @i_id_galileo			

			update Cliente
			set NombreCliente = @i_usuario, IdOperadorLogistico = @i_usuario_asesor,
				NombreOperadorLogistico = @i_nombre_asesor, 
				UsuarioModificacion = @i_usuario_creacion,
				FechaModificacion = GETDATE(), 
				Telefono = @i_telefono,
				aux1 = @i_labComercial
			where IdUsuario = @idUsuario			

			select @idCliente = IdCliente
			from Cliente
			where IdUsuario = @idUsuario			

			update ClienteDireccion
			set Direccion = @i_direccion, Provincia = @i_provincia,
				Ciudad = @i_ciudad, Latitud = @i_latitud, Longitud = @i_longitud,
				FechaModificacion = GETDATE(), UsuarioModificacion = @i_usuario_creacion
			where IdCliente = @idCliente

			set @mensajeError  = 'Cliente actualizado'
			set @codigoError = '00'
			
			select @mensajeError, @codigoError

		end
		else
		begin

			set @mensajeError  = 'Cliente no actualizado'
			set @codigoError = '01'
			
			select @mensajeError, @codigoError

		end
		
	end

end

GO