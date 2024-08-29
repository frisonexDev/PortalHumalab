/************************************************************************
*	Archivo Fisico: pr_modificacion_cliente.sql							*
*	Stored procedure: pr_modificacion_cliente							*
*	Base de datos: DbPortalHumalab						  			    *
*	Producto: Portal Clientes Humalab Clientes							*
*	Elaborado por: José Guarnizo 		                                *
*----------------------------------------------------------------------	*
*					DESCRIPCION DEL PROCEDIMIENTO						*
* En este Procedimiento va a actualizar el estado de un cliente         *
*                                                                       *
*----------------------------------------------------------------------	*
*					BITACORA DE MODIFICACIONES							*
*	FECHA AUTOR RAZON													*
*						                                                *
*----------------------------------------------------------------------	*/
IF NOT EXISTS (SELECT * FROM  sys.procedures WHERE NAME = 'pr_modificacion_cliente')	
	EXEC('Create Procedure dbo.pr_modificacion_cliente As')
GO

ALTER PROCEDURE [dbo].[pr_modificacion_cliente](
	@i_idCliente int,
	@i_cliente varchar(200),
	@i_fechaVigencia varchar(100) = null,
	@i_idEstadoNuevo int,
	@i_observacion varchar(max) = null,
	@i_usuModifica int,
	@i_accion char
)

AS

declare @valorEstadoNuevo char,
	@valorEstadoAnterior char,
	@mensajeError varchar(200),
	@codigoError varchar(10)

--valor estado anterior del cliente
select @valorEstadoAnterior = Estado 
from Usuario
where idUsuario = @i_idCliente

--valor del nuevo estado
select @valorEstadoNuevo = Valor
from CatalogoDetalle
where IdCatalogoDetalle = @i_idEstadoNuevo

BEGIN
	--Modificar
	IF @i_accion = 'M'
	BEGIN		
		if exists (select 1 from Usuario where idUsuario = @i_idCliente and Estado = @valorEstadoAnterior and IdRol = 100)
		begin	
			--cliente con estado temporal cambia a nuevo estado 
			if @i_fechaVigencia != null or @i_fechaVigencia != ''
			begin				
				update Usuario
				set Estado = @valorEstadoNuevo, Fechavigencia = convert(date, @i_fechaVigencia),
					UsuarioModificacion = @i_usuModifica, FechaModificacion = getdate(),
					Observacion = @i_observacion
				where Identificacion = @i_cliente
				and Estado = @valorEstadoAnterior
				and idUsuario = @i_idCliente

				select @mensajeError  = 'Cliente actualizado', @codigoError = '00'
				SELECT @mensajeError, @codigoError
			end
			else
			begin
				--cliente con estado suspendido o activo cambia a nuevo estado 
				update Usuario
				set Estado = @valorEstadoNuevo, UsuarioModificacion = @i_usuModifica,
					FechaModificacion = getdate(), Observacion = @i_observacion
				where Identificacion = @i_cliente
				and Estado = @valorEstadoAnterior
				and idUsuario = @i_idCliente

				select @mensajeError  = 'Cliente actualizado', @codigoError = '00'
				SELECT @mensajeError, @codigoError
			end
		end
		else
		begin
			select @mensajeError  = 'Cliente no existe', @codigoError = '01'			
			SELECT @mensajeError, @codigoError
		end				
	END
END

GO