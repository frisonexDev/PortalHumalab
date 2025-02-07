/************************************************************************
*	Stored procedure: pr_humalab_orden								    *
*	Base de datos: DbPortalHumalab						  			    *
*	Elaborado por: José Guarnizo							            *
*----------------------------------------------------------------------	*
*					DESCRIPCION DEL PROCEDIMIENTO						*
*	En este Procedimiento realiza acciones para las ordenes del portal  *
*	                                                                    *
*----------------------------------------------------------------------	*
*					BITACORA DE MODIFICACIONES							*
*	FECHA AUTOR RAZON													*
*	29/01/2025 Jose Guarnizo Se modifica para ver si existe una orden   *
*						     que se elimino atada a un pedido se debe   *
*							 anular el pedido tambien.					*
*----------------------------------------------------------------------	*/
IF NOT EXISTS (SELECT * FROM  sys.procedures WHERE NAME = 'pr_humalab_orden')	
	EXEC('Create Procedure dbo.pr_humalab_orden As')
go

ALTER PROCEDURE [dbo].[pr_humalab_orden](
	@i_accion CHAR(2),
	@idOrden INT=NULL,
	@idPedido INT=NULL,
	@idUsuarioGalileo INT=NULL,
	@identificacionPaciente VARCHAR(15)=NULL,
	@codigoBarra VARCHAR(50)=NULL,
	@medico VARCHAR(50)=NULL,
	@diagnostico VARCHAR(50)=NULL,
	@observacion VARCHAR(MAX)=NULL,
	@estado INT=NULL,
	--@resultados XML=NULL,
	@resultados varchar(100)=NULL,
	@usuarioCreacion INT=NULL,
	@fechaCreacion DATETIME=Null,
	@empresaId int = null
)

As

select @fechaCreacion = GETDATE() --de prueba

DECLARE @contar AS INT,
		@elimanodLogico AS INT=1,
		@identi varchar(20),
		@codCta varchar(100),
		@idUsuarioGalileoNew int,
		@identificacionPacienteNew VARCHAR(15),
		@codigoBarraNew VARCHAR(50),
		@medicoNew VARCHAR(50),
		@diagnosticoNew VARCHAR(50),
		@observacionNew VARCHAR(MAX),
		@estadoNew INT,
		@resultadosNew varchar(100),
		@usuarioCreacionNew INT,
		@fechaCreacionNew DATETIME,
		@empresaIdNew int,
		@idPedidoOrden int,
		@idPedidoAnulado int,
		@idCatalogoMaestroPedido int

select @identi = Identificacion
from Usuario
where idGalileo = @idUsuarioGalileo

select @idUsuarioGalileoNew = @idUsuarioGalileo
select @identificacionPacienteNew = @identificacionPaciente
select @codigoBarraNew = @codigoBarra
select @medicoNew = @medico
select @diagnosticoNew = @diagnostico
select @observacionNew = @observacion
select @estadoNew = @estado
select @resultadosNew = @resultados
select @usuarioCreacionNew = @usuarioCreacion
select @fechaCreacionNew = @fechaCreacion
select @empresaIdNew = @empresaId

select @idCatalogoMaestroPedido = IdCatalogoMaestro
from CatalogoMaestro
where Nombre = 'EstadoPedido'

--estado pedido anulado
select @idPedidoAnulado = IdCatalogoDetalle
from CatalogoDetalle
where IdCatalogoMaestro = @idCatalogoMaestroPedido
and Valor = 'ANUL'

BEGIN

--Insertar
	IF @i_accion = 'I'
	BEGIN			
		--INSERT INTO Orden VALUES(null,@idUsuarioGalileo,@identificacionPaciente,
		--						 @codigoBarra,@medico,@diagnostico,@observacion,
		--						 @estado,@resultados,@usuarioCreacion, 
		--						 @fechaCreacion, 0, null, 0, null, 0, @empresaId, null)
		--SELECT SCOPE_IDENTITY() as IdOrden
		INSERT INTO Orden VALUES(null, @idUsuarioGalileoNew, @identificacionPacienteNew,
								 @codigoBarraNew, @medicoNew, @diagnosticoNew, @observacionNew,
								 @estadoNew, @resultadosNew, @usuarioCreacionNew, 
								 @fechaCreacionNew, 0, null, 0, null, 0, @empresaIdNew, null)
		SELECT SCOPE_IDENTITY() as IdOrden
		
	END

	--Modificar
	IF @i_accion = 'M'
	BEGIN	
		
		IF exists(select 1 from Orden where CodigoBarra = @codigoBarra)
			BEGIN

				UPDATE Orden 
				SET Identificacion=@identificacionPaciente, 
					Medicamento=@medico, 
					Diagnostico=@diagnostico, 
					Observacion=@observacion,
					Estado=@estado, 
					UsuarioModificacion=@usuarioCreacion, 
					FechaModificacion=GETDATE()
				WHERE IdOrden=@idOrden

			END

		--IF(@contar>0)
		--	BEGIN

		--		UPDATE Orden SET
		--		Identificacion=@identificacionPaciente, Medicamento=@medico, Diagnostico=@diagnostico, Observacion=@observacion,
		--		Estado=@estado, UsuarioModificacion=@usuarioCreacion, FechaModificacion=GETDATE()
		--		WHERE IdOrden=@idOrden

		--	END
		
	END

	--Modificar
	IF @i_accion = 'M2'
	BEGIN	
		UPDATE Orden 
		SET Resultados=@resultados,
			Estado=@estado, 
			UsuarioModificacion=@usuarioCreacion, 
			FechaModificacion=GETDATE()
		WHERE CodigoBarra=@codigoBarra

		select IdOrden from Orden where CodigoBarra=@codigoBarra
	END

	--Consultar
	IF @i_accion = 'C'
	BEGIN	
		--nuevo 2024/07/26
		DECLARE @today DATE = CAST(GETDATE() AS DATE);

		--nuevo 2024/01/24
		select @codCta = CodClienteCta
		from Usuario
		where idGalileo = @idUsuarioGalileo

		SELECT COUNT(o.IdOrden) AS NumeroOrden
		FROM Orden o
		inner join Usuario u on o.IdUsuarioGalileo = u.idGalileo		
		where u.CodClienteCta = @codCta
		and o.Eliminado != 1
		AND CAST(o.FechaCreacion AS DATE) = @today; --2024/07/26
		
	END

	--Eliminar
	IF @i_accion = 'E'
		BEGIN	
			
			UPDATE Orden 
			SET Estado=@estado,
				Eliminado=@elimanodLogico, 
				UsuarioEliminacion=@usuarioCreacion, 
				FechaEliminacion=GETDATE()
			WHERE IdOrden=@idOrden

			select @idPedidoOrden = IdPedido
			from Orden
			where IdOrden = @idOrden

			if @idPedidoOrden != null
			begin
				UPDATE Pedido
				SET EstadoPedido = @idPedidoAnulado 
				where IdPedido = @idPedidoOrden
			end

		END

END
GO