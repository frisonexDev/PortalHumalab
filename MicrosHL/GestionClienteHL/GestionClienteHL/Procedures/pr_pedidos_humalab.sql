/************************************************************************
*	Archivo Fisico: pr_pedidos_humalab.sql								*
*	Stored procedure: pr_pedidos_humalab								*
*	Base de datos: DbPortalHumalab						  			    *
*	Producto: Portal Clientes Humalab administracion					*
*	Elaborado por: José Guarnizo							            *
*----------------------------------------------------------------------	*
*					DESCRIPCION DEL PROCEDIMIENTO						*
*	En este Procedimiento consulta todos los id de los pedidos          *
*	                                                                    *
*----------------------------------------------------------------------	*
*					BITACORA DE MODIFICACIONES							*
*	FECHA AUTOR RAZON													*
*						                                                *
*----------------------------------------------------------------------	*/
IF NOT EXISTS (SELECT * FROM  sysobjects WHERE NAME = 'pr_pedidos_humalab')	
	EXEC('Create Procedure dbo.pr_pedidos_humalab As')
go

ALTER PROCEDURE [dbo].[pr_pedidos_humalab](
	@i_accion char(1),
	@i_tipo_consulta varchar(10),
	--@i_idPedido int = null,
	@i_numRemision varchar(100) = null,
	@i_idOrden int = null,
	@i_idGalileo int = null,
	@i_diagnostico varchar(100) = null
)

as

declare @fecha varchar(100),
	@idMaestroPedido int,
	@idMaestroOrden int, 
	@idMaestroPrueba int,
	@idRecibidasOrden int,
	@idPorRecolectarPedido int,
	@idPorRecolectarOrden int,
	@idPorRecolectarPrueba int,
	@idGeneradaOrden int,
	@idGeneradaPrueba int,
	@idValidadaOrden int,
	@idFacturadaOrden int,
	@idAnuladoPedido int,
	@idRechazada int,
	@idRecolectada int,
	@idRecoTotal int,
	@idRecolectadoPed int,
	@idRecolecParPed int,
	@idRecoParcOrden int,
	@idRecolecParcPrueba int,
	@idReciParOrden int,
	@idPorProcPrueba int,
	@idValidPrueba int,
	@idFactPrueba int

--catalogo maestro estados orden, pedido y prueba
select @idMaestroPedido = IdCatalogoMaestro 
from CatalogoMaestro
where Nombre = 'EstadoPedido'

SELECT @idMaestroOrden = IdCatalogoMaestro 
from CatalogoMaestro
where Nombre = 'EstadoOrden'

select @idMaestroPrueba = IdCatalogoMaestro
from CatalogoMaestro
where Nombre = 'EstadoPrueba'

--------------------------------------------

--Pedidos ANULADAS, RECOLECTADA, RECOLECTADAS PARCIAL y POR RECOLECTAR

select @idAnuladoPedido = IdCatalogoDetalle
from CatalogoDetalle
where IdCatalogoMaestro = @idMaestroPedido
and Valor = 'ANUL'

select @idRecolectadoPed = IdCatalogoDetalle
from CatalogoDetalle
where IdCatalogoMaestro = @idMaestroPedido
and Valor = 'RCTL'

select @idRecolecParPed = IdCatalogoDetalle
from CatalogoDetalle
where IdCatalogoMaestro = @idMaestroPedido
and Valor = 'RCPC'

select @idPorRecolectarPedido = IdCatalogoDetalle
from CatalogoDetalle
where IdCatalogoMaestro = @idMaestroPedido
and Valor = 'PREC'

------------------------------------------------

--tabla ORDEN estado VALIDADO, FACTURADAS, RECIBIDAS, RECOLECTADO TOTAL, 
-- RECIBIDAS PARCIAL, POR RECOLECTAR y GENERADA

select @idPorRecolectarOrden = IdCatalogoDetalle
from CatalogoDetalle
where IdCatalogoMaestro = @idMaestroOrden
and Valor = 'PREC'

select @idValidadaOrden = IdCatalogoDetalle
from CatalogoDetalle
where IdCatalogoMaestro = @idMaestroOrden
and Valor = 'VALD'

select @idFacturadaOrden = IdCatalogoDetalle
from CatalogoDetalle
where IdCatalogoMaestro = @idMaestroOrden
and Valor = 'FACT'

select @idRecibidasOrden = IdCatalogoDetalle
from CatalogoDetalle
where IdCatalogoMaestro = @idMaestroOrden
and Valor = 'RCBD' 

select @idRecoTotal = IdCatalogoDetalle
from CatalogoDetalle
where IdCatalogoMaestro = @idMaestroOrden
and Valor = 'RCTL' 

select @idRecoParcOrden = IdCatalogoDetalle
from CatalogoDetalle
where IdCatalogoMaestro = @idMaestroOrden
and Valor = 'RCTP' 

select @idGeneradaOrden = IdCatalogoDetalle
from CatalogoDetalle
where IdCatalogoMaestro = @idMaestroOrden
and Valor = 'GENE'

select @idReciParOrden = IdCatalogoDetalle
from CatalogoDetalle
where IdCatalogoMaestro = @idMaestroOrden
and Valor = 'RCBP'

-------------------------------
--Pruebas RECHAZADAS, RECOLECTADAS, RECOLECTADA PARCIAL, POR RECOLECTAR,
-- VALIDADAS, FACTURADAS y GENERADA

select @idPorRecolectarPrueba = IdCatalogoDetalle
from CatalogoDetalle
where IdCatalogoMaestro = @idMaestroPrueba
and Valor = 'PREC'

select @idRechazada = IdCatalogoDetalle
from CatalogoDetalle
where IdCatalogoMaestro = @idMaestroPrueba
and Valor = 'RCHZ'

select @idRecolectada = IdCatalogoDetalle
from CatalogoDetalle
where IdCatalogoMaestro = @idMaestroPrueba
and Valor = 'RECT'

select @idRecolecParcPrueba = IdCatalogoDetalle
from CatalogoDetalle
where IdCatalogoMaestro = @idMaestroPrueba
and Valor = 'RCTP'

select @idGeneradaPrueba = IdCatalogoDetalle
from CatalogoDetalle
where IdCatalogoMaestro = @idMaestroPrueba
and Valor = 'GENE'

select @idPorProcPrueba = IdCatalogoDetalle
from CatalogoDetalle
where IdCatalogoMaestro = @idMaestroPrueba
and Valor = 'PPRC'

select @idValidPrueba = IdCatalogoDetalle
from CatalogoDetalle
where IdCatalogoMaestro = @idMaestroPrueba
and Valor = 'VALD'

select @idFactPrueba = IdCatalogoDetalle
from CatalogoDetalle
where IdCatalogoMaestro = @idMaestroPrueba
and Valor = 'FACT'

-----------------------------------------------

select @fecha = FORMAT(GETDATE(), 'yyyy-MM-dd')

begin
	
	if @i_accion = 'C'
	begin
		if exists (select 1 from Pedido)
		begin
			
			if @i_tipo_consulta = 'P'
			begin				

				select NumeroRemision
				from Pedido
				where CAST(FechaCreacion as date) = @fecha
				and UsuarioCreacion = @i_idGalileo
				and Eliminado = 0

			end

			if @i_tipo_consulta = 'PO'
			begin				

				--total de ordenes del pedido
				select count(*) as totalOrdenes
				from Pedido pd
				join Orden od on pd.IdPedido = od.IdPedido
				where pd.NumeroRemision = @i_numRemision
				and CAST(pd.FechaCreacion as date) = @fecha
				and CAST(od.FechaCreacion as date) = @fecha				
				and pd.UsuarioCreacion = @i_idGalileo
				and od.UsuarioCreacion = @i_idGalileo
				and pd.Eliminado = 0
				and od.Eliminado = 0								

				--diagnotiscos del pedido
				select distinct pb.Nombre
				from Pedido pd
				join Orden od on pd.IdPedido = od.IdPedido
				join Prueba pb on od.IdOrden = pb.IdOrden
				where pd.NumeroRemision = @i_numRemision
				and CAST(pd.FechaCreacion as date) = @fecha
				and CAST(od.FechaCreacion as date) = @fecha
				and CAST(pb.FechaCreacion as date) = @fecha
				and pd.UsuarioCreacion = @i_idGalileo
				and od.UsuarioCreacion = @i_idGalileo
				and pb.UsuarioCreacion = @i_idGalileo
				and pd.Eliminado = 0
				and od.Eliminado = 0
				and pb.Eliminado = 0				
			end

			if @i_tipo_consulta = 'CDP'
			begin				

				if @i_diagnostico != null or @i_diagnostico != ''
				begin					
					--PENDIENTES ordenes, pedidos y pruebas
					select COUNT(*) as totalPendientes
					from Pedido pd
					join Orden od on pd.IdPedido = od.IdPedido
					join Prueba pb on od.IdOrden = pb.IdOrden
					where CAST(pd.FechaCreacion as date) = @fecha
					and CAST(od.FechaCreacion as date) = @fecha
					and CAST(pb.FechaCreacion as date) = @fecha
					and pd.NumeroRemision = @i_numRemision
					and pd.EstadoPedido = @idPorRecolectarPedido
					and od.Estado in (@idPorRecolectarOrden, @idGeneradaOrden)
					and pb.Estado in (@idPorRecolectarPrueba, @idGeneradaPrueba)					
					and pd.UsuarioCreacion = @i_idGalileo
					and od.UsuarioCreacion = @i_idGalileo
					and pb.UsuarioCreacion = @i_idGalileo
					and pb.Nombre = @i_diagnostico
					and pd.Eliminado = 0
					and od.Eliminado = 0
					and pb.Eliminado = 0

					--PROCESADAS ordenes, pedidos y pruebas (validado y facturdas)
					select COUNT(*) as totalProcesadas
					from Pedido pd
					join Orden od on pd.IdPedido = od.IdPedido
					join Prueba pb on od.IdOrden = pb.IdOrden
					where CAST(pd.FechaCreacion as date) = @fecha
					and CAST(od.FechaCreacion as date) = @fecha
					and CAST(pb.FechaCreacion as date) = @fecha
					and pd.NumeroRemision = @i_numRemision
					and pd.EstadoPedido in (@idRecolectadoPed, @idRecolecParPed)
					and od.Estado in (@idValidadaOrden, @idFacturadaOrden)
					and pb.Estado in (@idValidPrueba, @idFactPrueba)
					and pd.UsuarioCreacion = @i_idGalileo					
					and od.UsuarioCreacion = @i_idGalileo
					and pb.UsuarioCreacion = @i_idGalileo
					and pb.Nombre = @i_diagnostico
					and pd.Eliminado = 0
					and od.Eliminado = 0
					and pb.Eliminado = 0

					--ENVIADAS ordenes, pedidos y pruebas
					select COUNT(*) as totalEnviadas
					from Pedido pd
					join Orden od on pd.IdPedido = od.IdPedido
					join Prueba pb on od.IdOrden = pb.IdOrden
					where CAST(pd.FechaCreacion as date) = @fecha
					and CAST(od.FechaCreacion as date) = @fecha
					and CAST(pb.FechaCreacion as date) = @fecha
					and pd.NumeroRemision = @i_numRemision
					and pd.EstadoPedido in (@idRecolectadoPed, @idRecolecParPed)
					and od.Estado in (@idRecibidasOrden, @idRecoTotal, @idRecoParcOrden, @idReciParOrden)
					and pb.Estado in (@idRecolectada, @idRecolecParcPrueba, @idPorProcPrueba)
					and pd.UsuarioCreacion = @i_idGalileo					
					and od.UsuarioCreacion = @i_idGalileo
					and pb.UsuarioCreacion = @i_idGalileo
					and pb.Nombre = @i_diagnostico
					and pd.Eliminado = 0
					and od.Eliminado = 0
					and pb.Eliminado = 0			
				end
				else
				begin					
					--PENDIENTES ordenes, pedidos y pruebas
					select COUNT(*) as totalPendientes
					from Pedido pd
					join Orden od on pd.IdPedido = od.IdPedido
					join Prueba pb on od.IdOrden = pb.IdOrden
					where CAST(pd.FechaCreacion as date) = @fecha
					and CAST(od.FechaCreacion as date) = @fecha
					and CAST(pb.FechaCreacion as date) = @fecha
					and pd.NumeroRemision = @i_numRemision
					and pd.EstadoPedido = @idPorRecolectarPedido
					and od.Estado in (@idPorRecolectarOrden, @idGeneradaOrden)
					and pb.Estado in (@idPorRecolectarPrueba, @idGeneradaPrueba)
					and pd.UsuarioCreacion = @i_idGalileo
					and od.UsuarioCreacion = @i_idGalileo
					and pb.UsuarioCreacion = @i_idGalileo					
					and pd.Eliminado = 0
					and od.Eliminado = 0
					and pb.Eliminado = 0

					--PROCESADAS ordenes, pedidos y pruebas (validado y facturdas)
					select COUNT(*) as totalProcesadas
					from Pedido pd
					join Orden od on pd.IdPedido = od.IdPedido
					join Prueba pb on od.IdOrden = pb.IdOrden
					where CAST(pd.FechaCreacion as date) = @fecha
					and CAST(od.FechaCreacion as date) = @fecha
					and CAST(pb.FechaCreacion as date) = @fecha
					and pd.NumeroRemision = @i_numRemision
					and pd.EstadoPedido in (@idRecolectadoPed, @idRecolecParPed)
					and od.Estado in (@idValidadaOrden, @idFacturadaOrden)
					and pb.Estado in (@idValidPrueba, @idFactPrueba)
					and pd.UsuarioCreacion = @i_idGalileo					
					and od.UsuarioCreacion = @i_idGalileo
					and pb.UsuarioCreacion = @i_idGalileo					
					and pd.Eliminado = 0
					and od.Eliminado = 0
					and pb.Eliminado = 0

					--ENVIADAS ordenes, pedidos y pruebas
					select COUNT(*) as totalEnviadas
					from Pedido pd
					join Orden od on pd.IdPedido = od.IdPedido
					join Prueba pb on od.IdOrden = pb.IdOrden
					where CAST(pd.FechaCreacion as date) = @fecha
					and CAST(od.FechaCreacion as date) = @fecha
					and CAST(pb.FechaCreacion as date) = @fecha
					and pd.NumeroRemision = @i_numRemision
					and pd.EstadoPedido in (@idRecolectadoPed, @idRecolecParPed)
					and od.Estado in (@idRecibidasOrden, @idRecoTotal, @idRecoParcOrden, @idReciParOrden)
					and pb.Estado in (@idRecolectada, @idRecolecParcPrueba, @idPorProcPrueba)
					and pd.UsuarioCreacion = @i_idGalileo					
					and od.UsuarioCreacion = @i_idGalileo
					and pb.UsuarioCreacion = @i_idGalileo					
					and pd.Eliminado = 0
					and od.Eliminado = 0
					and pb.Eliminado = 0
				end
			end

			if @i_tipo_consulta = 'CTP'
			begin
				
				--PENDIENTES ordenes, pedidos y pruebas
				select COUNT(*) as totalPendientes
				from Pedido pd
				join Orden od on pd.IdPedido = od.IdPedido
				join Prueba pb on od.IdOrden = pb.IdOrden
				where CAST(pd.FechaCreacion as date) = @fecha
				and CAST(od.FechaCreacion as date) = @fecha
				and CAST(pb.FechaCreacion as date) = @fecha					
				and pd.EstadoPedido = @idPorRecolectarPedido
				and od.Estado in (@idPorRecolectarOrden, @idGeneradaOrden)
				and pb.Estado in (@idPorRecolectarPrueba, @idGeneradaPrueba)
				and pd.UsuarioCreacion = @i_idGalileo
				and od.UsuarioCreacion = @i_idGalileo
				and pb.UsuarioCreacion = @i_idGalileo
				and pd.Eliminado = 0
				and od.Eliminado = 0
				and pb.Eliminado = 0

				--PROCESADAS ordenes, pedidos y pruebas (validado y facturdas)
				select COUNT(*) as totalProcesadas
				from Pedido pd
				join Orden od on pd.IdPedido = od.IdPedido
				join Prueba pb on od.IdOrden = pb.IdOrden
				where CAST(pd.FechaCreacion as date) = @fecha
				and CAST(od.FechaCreacion as date) = @fecha
				and CAST(pb.FechaCreacion as date) = @fecha					
				and pd.EstadoPedido in (@idRecolectadoPed, @idRecolecParPed)
				and od.Estado in (@idValidadaOrden, @idFacturadaOrden)
				and pb.Estado in (@idValidPrueba, @idFactPrueba)
				and pd.UsuarioCreacion = @i_idGalileo					
				and od.UsuarioCreacion = @i_idGalileo
				and pb.UsuarioCreacion = @i_idGalileo
				and pd.Eliminado = 0
				and od.Eliminado = 0
				and pb.Eliminado = 0

				--ENVIADAS ordenes, pedidos y pruebas
				select COUNT(*) as totalEnviadas
				from Pedido pd
				join Orden od on pd.IdPedido = od.IdPedido
				join Prueba pb on od.IdOrden = pb.IdOrden
				where CAST(pd.FechaCreacion as date) = @fecha
				and CAST(od.FechaCreacion as date) = @fecha
				and CAST(pb.FechaCreacion as date) = @fecha
				and pd.EstadoPedido in (@idRecolectadoPed, @idRecolecParPed)
				and od.Estado in (@idRecibidasOrden, @idRecoTotal, @idRecoParcOrden, @idReciParOrden)
				and pb.Estado in (@idRecolectada, @idRecolecParcPrueba, @idPorProcPrueba)
				and pd.UsuarioCreacion = @i_idGalileo					
				and od.UsuarioCreacion = @i_idGalileo
				and pb.UsuarioCreacion = @i_idGalileo
				and pd.Eliminado = 0
				and od.Eliminado = 0
				and pb.Eliminado = 0			
			end
		end
	end
end

GO