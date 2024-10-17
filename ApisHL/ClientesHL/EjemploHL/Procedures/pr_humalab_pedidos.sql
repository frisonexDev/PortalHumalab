/************************************************************************
*	Stored procedure: pr_humalab_pedidos								*
*	Base de datos: DbPortalHumalab						  			    *
*	Elaborado por: José Guarnizo							            *
*----------------------------------------------------------------------	*
*					DESCRIPCION DEL PROCEDIMIENTO						*
*	En este Procedimiento realiza acciones para pedidos cliente         *
*	                                                                    *
*----------------------------------------------------------------------	*
*					BITACORA DE MODIFICACIONES							*
*	FECHA AUTOR RAZON													*
*	2024/10/16 Jose Guarnizo Se actualiza para que se guarde el id		*
*							 del cliente.								*
*----------------------------------------------------------------------	*/
IF NOT EXISTS (SELECT * FROM  sys.procedures WHERE NAME = 'pr_humalab_pedidos')	
	EXEC('Create Procedure dbo.pr_humalab_pedidos As')
go

ALTER PROCEDURE [dbo].[pr_humalab_pedidos](
	@i_accion CHAR(2),
	@idPedido INT=NULL,
	@idOperador INT=NULL,
	@usuarioOperador Varchar(50)=NULL,
	@numeroRemision VARCHAR(50)=NULL,
	@fechaRetiro DATETIME=NULL,
	@estadoPedido INT=NULL,
	@observacionCliente VARCHAR(MAX)=NULL,
	@observacionOperador VARCHAR(MAX)=NULL,
	@usuarioCreacion INT=NULL,
	@fechaCreacion DATETIME=NULL,
	@idCliente int = null
)

as

DECLARE @contarPedido AS INT,
		@elimanodLogico AS INT=1,
		@idPedidoB AS INT,
		@idObservacion AS INT,
		@retiradas AS INT,
		@i_idEstadoPedido int,
		@i_idEstadoMue int,
		@idOperadorNew INT,
		@usuarioOperadorNew Varchar(50),
		@numeroRemisionNew VARCHAR(50),
		@estadoPedidoNew INT,
		@observacionClienteNew VARCHAR(MAX),
		@observacionOperadorNew VARCHAR(MAX),
		@usuarioCreacionNew INT,
		@fechaCreacionNew DATETIME

select @i_idEstadoPedido = IdCatalogoMaestro
from CatalogoMaestro
where Nombre = 'EstadoPedido'

select @i_idEstadoMue = IdCatalogoMaestro
from CatalogoMaestro
where Nombre = 'EstadoMuestra'

SELECT @contarPedido = COUNT(IdPedido) 
FROM Pedido 
WHERE NumeroRemision=@numeroRemision

select @idOperadorNew = @idOperador  
select @usuarioOperadorNew = @usuarioOperador 
select @numeroRemisionNew = @numeroRemision 
select @fechaCreacionNew = @fechaCreacion
select @estadoPedidoNew = @estadoPedido
select @observacionClienteNew = @observacionCliente
select @observacionOperadorNew = @observacionOperador
select @usuarioCreacionNew = @usuarioCreacion

BEGIN

--Insertar
	IF @i_accion = 'I'
	BEGIN	
			--INSERT INTO Pedido 
			--VALUES(@idOperador, @usuarioOperador, @numeroRemision, 
			--	   @fechaCreacion, @estadoPedido, @observacionCliente, 
			--	   @observacionOperador, @usuarioCreacion, GETDATE(), 
			--	   NULL, NULL, NULL, NULL, 0)
			--SELECT SCOPE_IDENTITY() as idPedido
			INSERT INTO Pedido 
			VALUES(@idOperadorNew, @usuarioOperadorNew, @numeroRemisionNew, 
				   @fechaCreacionNew, @estadoPedidoNew, @observacionClienteNew, 
				   @observacionOperadorNew, @usuarioCreacionNew, GETDATE(), 
				   NULL, NULL, NULL, NULL, 0, @idCliente)
			SELECT SCOPE_IDENTITY() as idPedido						
	END

	--Modificar
	IF @i_accion = 'M'
	BEGIN	
			
		IF(@contarPedido>0)
		BEGIN
			
			UPDATE Pedido 
			SET EstadoPedido=@estadoPedido, 
				FechaRetiro=@fechaRetiro,
				ObservacionCliente=@observacionCliente, 
				ObservacionOpLogistico=@observacionOperador, 
				UsuarioModificacion=@usuarioCreacion, 
				FechaModificacion=GETDATE()
			WHERE IdPedido=@idPedido OR NumeroRemision=@numeroRemision

		END
		
	END

	--Consultar
	IF @i_accion = 'C'
	BEGIN	
			
		Select count(IdPedido) AS 'NumeroPedido'  
		from Pedido 
		where UsuarioCreacion=@usuarioCreacion

	END

	IF @i_accion = 'C1'
		BEGIN	
			
			SELECT IdPedido
				,p.NumeroRemision
				,p.FechaCreacion
				,(
					SELECT count(*)
					FROM (
						SELECT o.IdOrden
						FROM dbo.Orden o
						WHERE o.IdPedido = p.IdPedido
						GROUP BY o.IdOrden
						) a
					) AS TotalOrdenes
				,(
					SELECT count(*)
					FROM (
							SELECT m.IdMuestra
							FROM dbo.Prueba pr
							INNER JOIN dbo.Orden o ON o.IdOrden = pr.IdOrden
							INNER JOIN dbo.PruebaMuestra pm ON pr.IdPrueba = pm.IdPrueba
							INNER JOIN dbo.Muestra m on pm.IdMuestra = m.IdMuestra
							WHERE o.IdPedido = p.IdPedido
							GROUP BY m.IdMuestra

						) muestras
					) AS TotalMuestras
				,(
					SELECT count(*)
					FROM (
							SELECT m.IdMuestra
							FROM dbo.Prueba pr
							INNER JOIN dbo.Orden o ON o.IdOrden = pr.IdOrden
							INNER JOIN dbo.PruebaMuestra pm ON pr.IdPrueba = pm.IdPrueba
							INNER JOIN dbo.Muestra m on pm.IdMuestra = m.IdMuestra
							WHERE o.IdPedido = p.IdPedido
							AND m.EstadoMuestra = (SELECT TOP 1 IdCatalogoDetalle FROM dbo.CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoMue and Valor = 'RECT')
							GROUP BY m.IdMuestra
						) muestrasRetiradas
					) AS TotalRetiradas
				,p.FechaRetiro
				,(SELECT Nombre FROM CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoPedido and IdCatalogoDetalle = p.EstadoPedido) as EstadoPedido
			FROM Pedido p 
			Where p.UsuarioCreacion = @usuarioCreacion AND p.Eliminado<>@elimanodLogico AND p.FechaCreacion = CONVERT(DATE,GETDATE())
		
	END


	--Eliminar
	IF @i_accion = 'E'
	BEGIN	
	
		UPDATE Pedido 
		SET UsuarioEliminacion=@usuarioCreacion,
			FechaEliminacion=GETDATE(),
			Eliminado=@elimanodLogico
		WHERE IdPedido=@idPedido OR NumeroRemision=@numeroRemision
		
	END

	--Eliminar
	IF @i_accion = 'E1'
	BEGIN	
		
		UPDATE Pedido 
		SET EstadoPedido =@estadoPedido,
			UsuarioEliminacion=@usuarioCreacion,
			FechaEliminacion=GETDATE(),
			Eliminado=@elimanodLogico
		WHERE IdPedido=@idPedido
		
	END

END

GO