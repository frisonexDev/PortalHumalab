/************************************************************************
*	Stored procedure: pr_operador_pedidosActu					        *
*	Base de datos: DbPortalHumalab						  			    *
*	Elaborado por: José Guarnizo						                *
*----------------------------------------------------------------------	*
*					DESCRIPCION DEL PROCEDIMIENTO						*
*	En este Procedimiento actualiza el pedido a recolectado total o     *
	parcial                                                             *
*	                                                                    *
*----------------------------------------------------------------------	*
*					BITACORA DE MODIFICACIONES							*
* FECHA AUTOR RAZON													    *
* 2024/12/10 José Guarnizo Se modifica para que no se actualice 	    *
*						   el pediddo cuando ya haya sido enviado o no  *
*----------------------------------------------------------------------	*/
IF NOT EXISTS (SELECT * FROM  sys.procedures WHERE NAME = 'pr_operador_pedidosActu')	
	EXEC('Create Procedure dbo.pr_operador_pedidosActu As')
go

ALTER PROCEDURE [dbo].[pr_operador_pedidosActu](
	 @i_accion CHAR(2),		 
	 @i_operador_logistico INT,
	 @i_idPedido int
)

as

DECLARE @fechaActual DATETIME,
	@i_idOrden int,
	@i_idEstadoPedi int,
	@i_idEstadoMues int,
	@i_idEstadoOrden int,
	@idEnv int,
	@idEnvP int,
	@idRec int,
	@idRecP int,
	@idAnu int,
	@idPorRec int,
	@idDniCliente varchar(20),
	@muestras_totales int,
	@muestras_aceptadas int,
	@estado_pedido int,
	@idRecol int,
	@idRecolPar int

select @i_idEstadoPedi = IdCatalogoMaestro
from CatalogoMaestro
where Nombre = 'EstadoPedido'

select @i_idEstadoMues = IdCatalogoMaestro
from CatalogoMaestro
where Nombre = 'EstadoMuestra'

select @i_idEstadoOrden = IdCatalogoMaestro
from CatalogoMaestro
where Nombre = 'EstadoOrden'

--anulados
select @idAnu = IdCatalogoDetalle
from CatalogoDetalle
where IdCatalogoMaestro = @i_idEstadoPedi
and Valor = 'ANUL'

--recoltectado total o parcial
select @idRecol = IdCatalogoDetalle
from CatalogoDetalle
where IdCatalogoMaestro = @i_idEstadoPedi
and Valor = 'RCTL'

select @idRecolPar = IdCatalogoDetalle
from CatalogoDetalle
where IdCatalogoMaestro = @i_idEstadoPedi
and Valor = 'RCPC'

--por recolectar 
select @idPorRec = IdCatalogoDetalle
from CatalogoDetalle
where IdCatalogoMaestro = @i_idEstadoPedi
and Valor = 'PREC'

--identificacion del cliente
select @idDniCliente = Identificacion
from Cliente
where IdOperadorLogistico = @i_operador_logistico

--pedido enviado total o parcial
select @idEnvTotal = IdCatalogoDetalle
from CatalogoDetalle
where IdCatalogoMaestro = @i_idEstadoPedi
and Valor = 'ENV'

select @idEnvParcial = IdCatalogoDetalle
from CatalogoDetalle
where IdCatalogoMaestro = @i_idEstadoPedi
and Valor = 'ENVP'


IF @i_accion = 'M'
begin

	if exists (SELECT 1 FROM Pedido WHERE IdPedido = @i_idPedido 
			   AND IdOperador = @i_operador_logistico 
			   and EstadoPedido = @idPorRec)
	begin

		set @muestras_totales = (
			select COUNT(*)
			from(
				select m.IdMuestra
				from Prueba pr
				inner join Orden o on o.IdOrden = pr.IdOrden
				inner join PruebaMuestra pm on pr.IdPrueba = pm.IdPrueba
				inner join Muestra m on pm.IdMuestra = m.IdMuestra
				where o.IdPedido = @i_idPedido
				and pr.Eliminado != 1
				and o.Eliminado != 1
				and pm.Eliminado != 1
				and m.Eliminado != 1
				GROUP BY m.IdMuestra
			) a
		)

		set @muestras_aceptadas = (
			select COUNT(*)
			from(
				SELECT m.IdMuestra
				from Prueba pr
				inner join Orden o on o.IdOrden = pr.IdOrden
				inner join PruebaMuestra pm on pr.IdPrueba = pm.IdPrueba
				inner join Muestra m on pm.IdMuestra = m.IdMuestra
				WHERE o.IdPedido = @i_idPedido 
				AND m.EstadoMuestra = (SELECT IdCatalogoDetalle FROM dbo.CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoMues AND Valor = 'RECT')
				and pr.Eliminado != 1
				and o.Eliminado != 1
				and pm.Eliminado != 1
				and m.Eliminado != 1
				GROUP BY m.IdMuestra
			) a		
		)

		IF @muestras_totales != 0 AND @muestras_aceptadas != 0
		BEGIN

			if @muestras_totales = @muestras_aceptadas
			BEGIN
				SET @estado_pedido = (SELECT IdCatalogoDetalle FROM CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoPedi AND Valor = 'RCTL')
			END
			ELSE
			BEGIN
				SET @estado_pedido = (SELECT IdCatalogoDetalle FROM CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoPedi AND Valor = 'RCPC')
			END

		END

		IF @estado_pedido != 0 
		BEGIN
			--Actualiza el pedido a recolectado o recolectado parcial
			update Pedido
			set EstadoPedido = @estado_pedido,
				UsuarioModificacion = @i_operador_logistico,
				FechaModificacion = GETDATE()
			where IdPedido = @i_idPedido
			and Eliminado != 1

			SELECT '00' as Resultado
			
		END
		else
		begin
			SELECT '01' as Resultado		
		end
	end
	else
	begin
		
		set @muestras_totales = (
		select COUNT(*)
		from(
			select m.IdMuestra
			from Prueba pr
			inner join Orden o on o.IdOrden = pr.IdOrden
			inner join PruebaMuestra pm on pr.IdPrueba = pm.IdPrueba
			inner join Muestra m on pm.IdMuestra = m.IdMuestra
			where o.IdPedido = @i_idPedido
			and pr.Eliminado != 1
			and o.Eliminado != 1
			and pm.Eliminado != 1
			and m.Eliminado != 1
			GROUP BY m.IdMuestra
			) a
		)

		set @muestras_aceptadas = (
		select COUNT(*)
		from(
			SELECT m.IdMuestra
			from Prueba pr
			inner join Orden o on o.IdOrden = pr.IdOrden
			inner join PruebaMuestra pm on pr.IdPrueba = pm.IdPrueba
			inner join Muestra m on pm.IdMuestra = m.IdMuestra
			WHERE o.IdPedido = @i_idPedido 
			AND m.EstadoMuestra = (SELECT IdCatalogoDetalle FROM dbo.CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoMues AND Valor = 'RECT')
			and pr.Eliminado != 1
			and o.Eliminado != 1
			and pm.Eliminado != 1
			and m.Eliminado != 1
			GROUP BY m.IdMuestra
			) a		
		)

		IF @muestras_totales != 0 AND @muestras_aceptadas = 0
		BEGIN			
			SET @estado_pedido = (SELECT IdCatalogoDetalle FROM CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoPedi AND Valor = 'PREC')
			
			--seleciona el estado actual del pedido
			select @idPedidoFinal = EstadoPedido
			from Pedido
			where IdPedido = @i_idPedido
			AND IdOperador = @i_operador_logistico

			if @idEnvTotal != @idPedidoFinal and @idEnvParcial != @idPedidoFinal
			begin
				--actualiza el pedido a por recolectar
				update Pedido
				set EstadoPedido = @estado_pedido,
					UsuarioModificacion = @i_operador_logistico,
					FechaModificacion = GETDATE()
				where IdPedido = @i_idPedido
				and Eliminado != 1

				SELECT '00' as Resultado
			end

		END		
		else
		begin
			
			if @muestras_totales = @muestras_aceptadas
			BEGIN
				SET @estado_pedido = (SELECT IdCatalogoDetalle FROM CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoPedi AND Valor = 'RCTL')

				--seleciona el estado actual del pedido
				select @idPedidoFinal = EstadoPedido
				from Pedido
				where IdPedido = @i_idPedido
				AND IdOperador = @i_operador_logistico

				--veririfica si el pedido esta enviado sea total o parcial
				--y direfente del estado del pedido actualiza
				--caso contrario no actualiza el pedido porque ya fue enviado

				if @idEnvTotal != @idPedidoFinal and @idEnvParcial != @idPedidoFinal
				begin
					--Actualiza el pedido a recolectado o recolectado parcial
					update Pedido
					set EstadoPedido = @estado_pedido,
						UsuarioModificacion = @i_operador_logistico,
						FechaModificacion = GETDATE()
					where IdPedido = @i_idPedido
					and Eliminado != 1

					SELECT '00' as Resultado
				end

			END
			ELSE
			BEGIN
				SET @estado_pedido = (SELECT IdCatalogoDetalle FROM CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoPedi AND Valor = 'RCPC')

				--seleciona el estado actual del pedido
				select @idPedidoFinal = EstadoPedido
				from Pedido
				where IdPedido = @i_idPedido
				AND IdOperador = @i_operador_logistico

				if @idEnvTotal != @idPedidoFinal and @idEnvParcial != @idPedidoFinal
				begin
					--Actualiza el pedido a recolectado o recolectado parcial
					update Pedido
					set EstadoPedido = @estado_pedido,
						UsuarioModificacion = @i_operador_logistico,
						FechaModificacion = GETDATE()
					where IdPedido = @i_idPedido
					and Eliminado != 1

					SELECT '00' as Resultado
				end
				
			END						
		end
	end

end

GO