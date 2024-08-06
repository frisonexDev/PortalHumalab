/************************************************************************
*	Stored procedure: pr_humalab_listarpedidos							*
*	Base de datos: DbPortalHumalab						  			    *
*	Elaborado por: José Guarnizo							            *
*----------------------------------------------------------------------	*
*					DESCRIPCION DEL PROCEDIMIENTO						*
*	En este Procedimiento listas los pedidos                            *
*	                                                                    *
*----------------------------------------------------------------------	*
*					BITACORA DE MODIFICACIONES							*
*	FECHA AUTOR RAZON													*
*						                                                *
*----------------------------------------------------------------------	*/
IF NOT EXISTS (SELECT * FROM  sys.procedures WHERE NAME = 'pr_humalab_listarpedidos')	
	EXEC('Create Procedure dbo.pr_humalab_listarpedidos As')
go

ALTER PROCEDURE [dbo].[pr_humalab_listarpedidos](
	@i_accion CHAR,
	@idUsuario INT,
	@opcionBusqueda INT=NULL,
	@datoBusqueda VARCHAR(50)=NULL,
	@fechaInicio DATE=NULL,
	@fechaFin DATE = NULL,
	@numRemision varchar(100) = null
)

as

declare @estadoEliminado AS INT =1,
		@idCatalogoMaestro AS INT = 9,
		@estadoCero AS INT=0,
		@estadoUno AS INT =1,
		@estadoDos AS INT =2,
		@estadoTres AS INT =3,
		@estadoCuatro AS INT =4,
		@i_idEstadoMue int,
		@i_idEstadoPed int,
		@i_idPed int,
		@i_idEstadoReco int,
		@i_idEstadoRecp int,
		@i_idEstadoEnv int,
		@i_idEstadoEnvp int,
		@i_idEstadoPrec int,
		@i_idEstadoAnu int


select @i_idEstadoMue = IdCatalogoMaestro
from CatalogoMaestro
where Nombre = 'EstadoMuestra'

SELECT @i_idEstadoPed = IdCatalogoMaestro
from CatalogoMaestro
where Nombre = 'EstadoPedido'

select @i_idPed = IdCatalogoDetalle
FROM CatalogoDetalle
where Valor = @datoBusqueda
and IdCatalogoMaestro = @i_idEstadoPed

--------------------------
select @i_idEstadoPrec = IdCatalogoDetalle
from CatalogoDetalle
where Valor = 'PREC'
and IdCatalogoMaestro = @i_idEstadoPed

--------------------------
select @i_idEstadoAnu = IdCatalogoDetalle
from CatalogoDetalle
where Valor = 'ANUL'
and IdCatalogoMaestro = @i_idEstadoPed

--------------------------
select @i_idEstadoReco = IdCatalogoDetalle
from CatalogoDetalle
where Valor = 'RCTL'
and IdCatalogoMaestro = @i_idEstadoPed

select @i_idEstadoRecp = IdCatalogoDetalle
from CatalogoDetalle
where Valor = 'RCPC'
and IdCatalogoMaestro = @i_idEstadoPed

--------------------------
select @i_idEstadoEnv = IdCatalogoDetalle
from CatalogoDetalle
where Valor = 'ENV'
and IdCatalogoMaestro = @i_idEstadoPed

select @i_idEstadoEnvp = IdCatalogoDetalle
from CatalogoDetalle
where Valor = 'ENVP'
and IdCatalogoMaestro = @i_idEstadoPed
---------------------


IF(@i_accion='C')
Begin
	
	IF(@opcionBusqueda= @estadoCero)
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
								--INNER JOIN dbo.PruebaMuestra pm ON pr.IdPrueba = pm.IdPrueba 16/01/2024
								--INNER JOIN dbo.Muestra m on pm.IdMuestra = m.IdMuestra 16/01/2024
								inner join Muestra m on o.IdOrden = m.IdOrden --16/01/2024
								WHERE o.IdPedido = p.IdPedido
								and m.Eliminado != 1 --nuevo 11/01/2024
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
							INNER JOIN CatalogoMaestro CM ON CM.IdCatalogoMaestro = @i_idEstadoMue
							INNER JOIN CatalogoDetalle CD ON CM.IdCatalogoMaestro = CD.IdCatalogoMaestro
							WHERE o.IdPedido = p.IdPedido AND m.EstadoMuestra =CD.IdCatalogoDetalle AND (CD.Valor ='RECT' OR CD.Valor ='RECB' OR CD.Valor ='ENTR')
							GROUP BY m.IdMuestra
							) muestrasRetiradas
						) AS TotalRetiradas
					,p.FechaRetiro
					,(SELECT Nombre FROM CatalogoDetalle WHERE IdCatalogoMaestro = @idCatalogoMaestro and IdCatalogoDetalle = p.EstadoPedido) as EstadoPedido
				FROM Pedido p 
				Where p.UsuarioCreacion = @idUsuario 
				AND CAST(p.FechaCreacion AS DATE) = CAST(GETDATE() AS DATE)
				order by p.FechaCreacion desc

	END

	IF(@opcionBusqueda = @estadoUno)
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
								--INNER JOIN dbo.PruebaMuestra pm ON pr.IdPrueba = pm.IdPrueba 16/01/2024
								--INNER JOIN dbo.Muestra m on pm.IdMuestra = m.IdMuestra 16/01/2024
								inner join Muestra m on o.IdOrden = m.IdOrden --16/01/2024
								WHERE o.IdPedido = p.IdPedido
								and m.Eliminado != 1 --nuevo 11/01/2024
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
							INNER JOIN CatalogoMaestro CM ON CM.IdCatalogoMaestro = @i_idEstadoMue
							INNER JOIN CatalogoDetalle CD ON CM.IdCatalogoMaestro = CD.IdCatalogoMaestro
							WHERE o.IdPedido = p.IdPedido AND m.EstadoMuestra =CD.IdCatalogoDetalle AND (CD.Valor ='RECT' OR CD.Valor ='RECB' OR CD.Valor ='ENTR')
							GROUP BY m.IdMuestra
							) muestrasRetiradas
						) AS TotalRetiradas
					,p.FechaRetiro
					,(SELECT Nombre FROM CatalogoDetalle WHERE IdCatalogoMaestro = @idCatalogoMaestro and IdCatalogoDetalle = p.EstadoPedido) as EstadoPedido
				FROM Pedido p 
				Where p.UsuarioCreacion = @idUsuario AND p.NumeroRemision=@datoBusqueda
				order by p.FechaCreacion desc
	END


	IF(@opcionBusqueda = @estadoDos)
	BEGIN		
		select @fechaFin = DATEADD(DAY, 1, @fechaFin)

		if @numRemision != '' or @numRemision != null
		begin
			
			--busqueda por recolectar
			if @datoBusqueda = 'PREC'
			begin
				
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
								--INNER JOIN dbo.PruebaMuestra pm ON pr.IdPrueba = pm.IdPrueba 16/01/2024
								--INNER JOIN dbo.Muestra m on pm.IdMuestra = m.IdMuestra 16/01/2024
								inner join Muestra m on o.IdOrden = m.IdOrden --16/01/2024
								WHERE o.IdPedido = p.IdPedido
								and m.Eliminado != 1 --nuevo 11/01/2024
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
							INNER JOIN CatalogoMaestro CM ON CM.IdCatalogoMaestro = @i_idEstadoMue
							INNER JOIN CatalogoDetalle CD ON CM.IdCatalogoMaestro = CD.IdCatalogoMaestro
							WHERE o.IdPedido = p.IdPedido AND m.EstadoMuestra =CD.IdCatalogoDetalle AND (CD.Valor ='RECT' OR CD.Valor ='RECB' OR CD.Valor ='ENTR')
							GROUP BY m.IdMuestra
							) muestrasRetiradas
						) AS TotalRetiradas
					,p.FechaRetiro
					,(SELECT Nombre FROM CatalogoDetalle WHERE IdCatalogoMaestro = @idCatalogoMaestro and IdCatalogoDetalle = p.EstadoPedido) as EstadoPedido
				FROM Pedido p 
				--INNER JOIN CatalogoMaestro CM ON CM.IdCatalogoMaestro=@idCatalogoMaestro
				--INNER JOIN CatalogoDetalle CD ON CD.IdCatalogoMaestro = CM.IdCatalogoMaestro
				Where p.UsuarioCreacion = @idUsuario 
				and p.EstadoPedido = @i_idEstadoPrec				
				AND p.NumeroRemision = @numRemision
				AND p.FechaCreacion BETWEEN @fechaInicio AND @fechaFin
				order by p.FechaCreacion desc

			end

			--Busqueda anulado
			if @datoBusqueda = 'ANU'
			begin
				
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
								--INNER JOIN dbo.PruebaMuestra pm ON pr.IdPrueba = pm.IdPrueba 16/01/2024
								--INNER JOIN dbo.Muestra m on pm.IdMuestra = m.IdMuestra 16/01/2024
								inner join Muestra m on o.IdOrden = m.IdOrden --16/01/2024
								WHERE o.IdPedido = p.IdPedido
								and m.Eliminado != 1 --nuevo 11/01/2024
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
							INNER JOIN CatalogoMaestro CM ON CM.IdCatalogoMaestro = @i_idEstadoMue
							INNER JOIN CatalogoDetalle CD ON CM.IdCatalogoMaestro = CD.IdCatalogoMaestro
							WHERE o.IdPedido = p.IdPedido AND m.EstadoMuestra =CD.IdCatalogoDetalle AND (CD.Valor ='RECT' OR CD.Valor ='RECB' OR CD.Valor ='ENTR')
							GROUP BY m.IdMuestra
							) muestrasRetiradas
						) AS TotalRetiradas
					,p.FechaRetiro
					,(SELECT Nombre FROM CatalogoDetalle WHERE IdCatalogoMaestro = @idCatalogoMaestro and IdCatalogoDetalle = p.EstadoPedido) as EstadoPedido
				FROM Pedido p 
				--INNER JOIN CatalogoMaestro CM ON CM.IdCatalogoMaestro=@idCatalogoMaestro
				--INNER JOIN CatalogoDetalle CD ON CD.IdCatalogoMaestro = CM.IdCatalogoMaestro
				Where p.UsuarioCreacion = @idUsuario 
				and p.EstadoPedido = @i_idEstadoAnu			
				AND p.NumeroRemision = @numRemision
				AND p.FechaCreacion BETWEEN @fechaInicio AND @fechaFin
				order by p.FechaCreacion desc

			end

			--Busqueda recolectado total/parcial
			if @datoBusqueda = 'RCTL_RCPC'
			begin
				
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
								--INNER JOIN dbo.PruebaMuestra pm ON pr.IdPrueba = pm.IdPrueba 16/01/2024
								--INNER JOIN dbo.Muestra m on pm.IdMuestra = m.IdMuestra 16/01/2024
								inner join Muestra m on o.IdOrden = m.IdOrden --16/01/2024
								WHERE o.IdPedido = p.IdPedido
								and m.Eliminado != 1 --nuevo 11/01/2024
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
							INNER JOIN CatalogoMaestro CM ON CM.IdCatalogoMaestro = @i_idEstadoMue
							INNER JOIN CatalogoDetalle CD ON CM.IdCatalogoMaestro = CD.IdCatalogoMaestro
							WHERE o.IdPedido = p.IdPedido AND m.EstadoMuestra =CD.IdCatalogoDetalle AND (CD.Valor ='RECT' OR CD.Valor ='RECB' OR CD.Valor ='ENTR')
							GROUP BY m.IdMuestra
							) muestrasRetiradas
						) AS TotalRetiradas
					,p.FechaRetiro
					,(SELECT Nombre FROM CatalogoDetalle WHERE IdCatalogoMaestro = @idCatalogoMaestro and IdCatalogoDetalle = p.EstadoPedido) as EstadoPedido
				FROM Pedido p 
				--INNER JOIN CatalogoMaestro CM ON CM.IdCatalogoMaestro=@idCatalogoMaestro
				--INNER JOIN CatalogoDetalle CD ON CD.IdCatalogoMaestro = CM.IdCatalogoMaestro
				Where p.UsuarioCreacion = @idUsuario 
				and p.EstadoPedido in (@i_idEstadoReco, @i_idEstadoRecp)		
				AND p.NumeroRemision = @numRemision
				AND p.FechaCreacion BETWEEN @fechaInicio AND @fechaFin
				order by p.FechaCreacion desc

			end

			--Busqueda enviado total/parcial
			if @datoBusqueda = 'ENV_ENVP'
			begin
				
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
								--INNER JOIN dbo.PruebaMuestra pm ON pr.IdPrueba = pm.IdPrueba 16/01/2024
								--INNER JOIN dbo.Muestra m on pm.IdMuestra = m.IdMuestra 16/01/2024
								inner join Muestra m on o.IdOrden = m.IdOrden --16/01/2024
								WHERE o.IdPedido = p.IdPedido
								and m.Eliminado != 1 --nuevo 11/01/2024
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
							INNER JOIN CatalogoMaestro CM ON CM.IdCatalogoMaestro = @i_idEstadoMue
							INNER JOIN CatalogoDetalle CD ON CM.IdCatalogoMaestro = CD.IdCatalogoMaestro
							WHERE o.IdPedido = p.IdPedido AND m.EstadoMuestra =CD.IdCatalogoDetalle AND (CD.Valor ='RECT' OR CD.Valor ='RECB' OR CD.Valor ='ENTR')
							GROUP BY m.IdMuestra
							) muestrasRetiradas
						) AS TotalRetiradas
					,p.FechaRetiro
					,(SELECT Nombre FROM CatalogoDetalle WHERE IdCatalogoMaestro = @idCatalogoMaestro and IdCatalogoDetalle = p.EstadoPedido) as EstadoPedido
				FROM Pedido p 
				--INNER JOIN CatalogoMaestro CM ON CM.IdCatalogoMaestro=@idCatalogoMaestro
				--INNER JOIN CatalogoDetalle CD ON CD.IdCatalogoMaestro = CM.IdCatalogoMaestro
				Where p.UsuarioCreacion = @idUsuario 
				and p.EstadoPedido in (@i_idEstadoEnv, @i_idEstadoEnvp)		
				AND p.NumeroRemision = @numRemision
				AND p.FechaCreacion BETWEEN @fechaInicio AND @fechaFin
				order by p.FechaCreacion desc

			end

		end
			
	END

	IF(@opcionBusqueda =@estadoTres)
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
								--INNER JOIN dbo.PruebaMuestra pm ON pr.IdPrueba = pm.IdPrueba 16/01/2024
								--INNER JOIN dbo.Muestra m on pm.IdMuestra = m.IdMuestra 16/01/2024
								inner join Muestra m on o.IdOrden = m.IdOrden --16/01/2024
								WHERE o.IdPedido = p.IdPedido
								and m.Eliminado != 1 --nuevo 11/01/2024
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
							INNER JOIN CatalogoMaestro CM ON CM.IdCatalogoMaestro = @i_idEstadoMue
							INNER JOIN CatalogoDetalle CD ON CM.IdCatalogoMaestro = CD.IdCatalogoMaestro
							WHERE o.IdPedido = p.IdPedido AND m.EstadoMuestra =CD.IdCatalogoDetalle AND (CD.Valor ='RECT' OR CD.Valor ='RECB' OR CD.Valor ='ENTR')
							GROUP BY m.IdMuestra
							) muestrasRetiradas
						) AS TotalRetiradas
					,p.FechaRetiro
					,(SELECT Nombre FROM CatalogoDetalle WHERE IdCatalogoMaestro = @idCatalogoMaestro and IdCatalogoDetalle = p.EstadoPedido) as EstadoPedido
				FROM Pedido p 
				INNER JOIN CatalogoMaestro CM ON CM.IdCatalogoMaestro=@idCatalogoMaestro
				INNER JOIN CatalogoDetalle CD ON CD.IdCatalogoMaestro = CM.IdCatalogoMaestro
				Where p.UsuarioCreacion = @idUsuario AND CD.Valor=@datoBusqueda AND p.EstadoPedido = CD.IdCatalogoDetalle AND p.FechaCreacion BETWEEN @fechaInicio AND @fechaFin
				order by p.FechaCreacion desc
	END
	
	IF(@opcionBusqueda =@estadoCuatro)
	BEGIN
		
		--Busqueda todos los pedidos
		if @datoBusqueda = null or @datoBusqueda = '0'
		begin
			
			if /*@numRemision = null or*/ @numRemision = ''
			begin
				
				select @fechaFin = DATEADD(DAY, 1, @fechaFin)

				SELECT DISTINCT IdPedido
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
								--INNER JOIN dbo.PruebaMuestra pm ON pr.IdPrueba = pm.IdPrueba 16/01/2024
								--INNER JOIN dbo.Muestra m on pm.IdMuestra = m.IdMuestra 16/01/2024
								inner join Muestra m on o.IdOrden = m.IdOrden --16/01/2024
								WHERE o.IdPedido = p.IdPedido
								and m.Eliminado != 1 --nuevo 11/01/2024
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
							INNER JOIN CatalogoMaestro CM ON CM.IdCatalogoMaestro = @i_idEstadoMue
							INNER JOIN CatalogoDetalle CD ON CM.IdCatalogoMaestro = CD.IdCatalogoMaestro
							WHERE o.IdPedido = p.IdPedido AND m.EstadoMuestra =CD.IdCatalogoDetalle AND (CD.Valor ='RECT' OR CD.Valor ='RECB' OR CD.Valor ='ENTR')
							GROUP BY m.IdMuestra
							) muestrasRetiradas
						) AS TotalRetiradas
					,p.FechaRetiro
					,(SELECT Nombre FROM CatalogoDetalle WHERE IdCatalogoMaestro = @idCatalogoMaestro and IdCatalogoDetalle = p.EstadoPedido) as EstadoPedido
				FROM Pedido p 
				INNER JOIN CatalogoMaestro CM ON CM.IdCatalogoMaestro=@idCatalogoMaestro
				INNER JOIN CatalogoDetalle CD ON CD.IdCatalogoMaestro = CM.IdCatalogoMaestro
				Where p.UsuarioCreacion = @idUsuario				
				AND p.FechaCreacion BETWEEN @fechaInicio AND @fechaFin
				order by p.FechaCreacion desc
			end
			else
			begin
				select @fechaFin = DATEADD(DAY, 1, @fechaFin)

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
								--INNER JOIN dbo.PruebaMuestra pm ON pr.IdPrueba = pm.IdPrueba 16/01/2024
								--INNER JOIN dbo.Muestra m on pm.IdMuestra = m.IdMuestra 16/01/2024
								inner join Muestra m on o.IdOrden = m.IdOrden --16/01/2024
								WHERE o.IdPedido = p.IdPedido
								and m.Eliminado != 1 --nuevo 11/01/2024
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
							INNER JOIN CatalogoMaestro CM ON CM.IdCatalogoMaestro = @i_idEstadoMue
							INNER JOIN CatalogoDetalle CD ON CM.IdCatalogoMaestro = CD.IdCatalogoMaestro
							WHERE o.IdPedido = p.IdPedido AND m.EstadoMuestra =CD.IdCatalogoDetalle AND (CD.Valor ='RECT' OR CD.Valor ='RECB' OR CD.Valor ='ENTR')
							GROUP BY m.IdMuestra
							) muestrasRetiradas
						) AS TotalRetiradas
					,p.FechaRetiro
					,(SELECT Nombre FROM CatalogoDetalle WHERE IdCatalogoMaestro = @idCatalogoMaestro and IdCatalogoDetalle = p.EstadoPedido) as EstadoPedido
				FROM Pedido p 
				INNER JOIN CatalogoMaestro CM ON CM.IdCatalogoMaestro=@idCatalogoMaestro
				INNER JOIN CatalogoDetalle CD ON CD.IdCatalogoMaestro = CM.IdCatalogoMaestro
				Where p.UsuarioCreacion = @idUsuario				
				AND p.EstadoPedido = CD.IdCatalogoDetalle
				AND p.NumeroRemision = @numRemision
				order by p.FechaCreacion desc

			end
		end
		
		--Busqueda por recolectar
		if @datoBusqueda = 'PREC' /*and @numRemision = '' or @numRemision = null*/
		begin
			select @fechaFin = DATEADD(DAY, 1, @fechaFin)

			SELECT DISTINCT IdPedido
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
								--INNER JOIN dbo.PruebaMuestra pm ON pr.IdPrueba = pm.IdPrueba 16/01/2024
								--INNER JOIN dbo.Muestra m on pm.IdMuestra = m.IdMuestra 16/01/2024
								inner join Muestra m on o.IdOrden = m.IdOrden --16/01/2024
								WHERE o.IdPedido = p.IdPedido
								and m.Eliminado != 1 --nuevo 11/01/2024
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
							INNER JOIN CatalogoMaestro CM ON CM.IdCatalogoMaestro = @i_idEstadoMue
							INNER JOIN CatalogoDetalle CD ON CM.IdCatalogoMaestro = CD.IdCatalogoMaestro
							WHERE o.IdPedido = p.IdPedido AND m.EstadoMuestra =CD.IdCatalogoDetalle AND (CD.Valor ='RECT' OR CD.Valor ='RECB' OR CD.Valor ='ENTR')
							GROUP BY m.IdMuestra
							) muestrasRetiradas
						) AS TotalRetiradas
					,p.FechaRetiro
					,(SELECT Nombre FROM CatalogoDetalle WHERE IdCatalogoMaestro = @idCatalogoMaestro and IdCatalogoDetalle = p.EstadoPedido) as EstadoPedido
				FROM Pedido p 
				INNER JOIN CatalogoMaestro CM ON CM.IdCatalogoMaestro=@idCatalogoMaestro
				INNER JOIN CatalogoDetalle CD ON CD.IdCatalogoMaestro = CM.IdCatalogoMaestro
				Where p.UsuarioCreacion = @idUsuario
				and p.EstadoPedido = @i_idEstadoPrec
				AND p.FechaCreacion BETWEEN @fechaInicio AND @fechaFin
				order by p.FechaCreacion desc
		end		
		
		--Busqueda anulado
		if @datoBusqueda = 'ANUL' /*and @numRemision = '' or @numRemision = null*/
		begin
			select @fechaFin = DATEADD(DAY, 1, @fechaFin)

			SELECT DISTINCT IdPedido
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
								--INNER JOIN dbo.PruebaMuestra pm ON pr.IdPrueba = pm.IdPrueba 16/01/2024
								--INNER JOIN dbo.Muestra m on pm.IdMuestra = m.IdMuestra 16/01/2024
								inner join Muestra m on o.IdOrden = m.IdOrden --16/01/2024
								WHERE o.IdPedido = p.IdPedido
								and m.Eliminado != 1 --nuevo 11/01/2024
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
							INNER JOIN CatalogoMaestro CM ON CM.IdCatalogoMaestro = @i_idEstadoMue
							INNER JOIN CatalogoDetalle CD ON CM.IdCatalogoMaestro = CD.IdCatalogoMaestro
							WHERE o.IdPedido = p.IdPedido AND m.EstadoMuestra =CD.IdCatalogoDetalle AND (CD.Valor ='RECT' OR CD.Valor ='RECB' OR CD.Valor ='ENTR')
							GROUP BY m.IdMuestra
							) muestrasRetiradas
						) AS TotalRetiradas
					,p.FechaRetiro
					,(SELECT Nombre FROM CatalogoDetalle WHERE IdCatalogoMaestro = @idCatalogoMaestro and IdCatalogoDetalle = p.EstadoPedido) as EstadoPedido
				FROM Pedido p 
				INNER JOIN CatalogoMaestro CM ON CM.IdCatalogoMaestro=@idCatalogoMaestro
				INNER JOIN CatalogoDetalle CD ON CD.IdCatalogoMaestro = CM.IdCatalogoMaestro
				Where p.UsuarioCreacion = @idUsuario
				and p.EstadoPedido = @i_idEstadoAnu
				AND p.FechaCreacion BETWEEN @fechaInicio AND @fechaFin
				order by p.FechaCreacion desc
		end

		--busqueda recolectado total/parcial
		IF @datoBusqueda = 'RCTL_RCPC' /*and @numRemision = '' or @numRemision = null*/
		begin
			select @fechaFin = DATEADD(DAY, 1, @fechaFin)			

			SELECT DISTINCT IdPedido
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
								--INNER JOIN dbo.PruebaMuestra pm ON pr.IdPrueba = pm.IdPrueba 16/01/2024
								--INNER JOIN dbo.Muestra m on pm.IdMuestra = m.IdMuestra 16/01/2024
								inner join Muestra m on o.IdOrden = m.IdOrden --16/01/2024
								WHERE o.IdPedido = p.IdPedido
								and m.Eliminado != 1 --nuevo 11/01/2024
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
							INNER JOIN CatalogoMaestro CM ON CM.IdCatalogoMaestro = @i_idEstadoMue
							INNER JOIN CatalogoDetalle CD ON CM.IdCatalogoMaestro = CD.IdCatalogoMaestro
							WHERE o.IdPedido = p.IdPedido AND m.EstadoMuestra =CD.IdCatalogoDetalle AND (CD.Valor ='RECT' OR CD.Valor ='RECB' OR CD.Valor ='ENTR')
							GROUP BY m.IdMuestra
							) muestrasRetiradas
						) AS TotalRetiradas
					,p.FechaRetiro
					,(SELECT Nombre FROM CatalogoDetalle WHERE IdCatalogoMaestro = @idCatalogoMaestro and IdCatalogoDetalle = p.EstadoPedido) as EstadoPedido
				FROM Pedido p 
				INNER JOIN CatalogoMaestro CM ON CM.IdCatalogoMaestro=@idCatalogoMaestro
				INNER JOIN CatalogoDetalle CD ON CD.IdCatalogoMaestro = CM.IdCatalogoMaestro
				Where p.UsuarioCreacion = @idUsuario
				and p.EstadoPedido in (@i_idEstadoReco, @i_idEstadoRecp)
				AND p.FechaCreacion BETWEEN @fechaInicio AND @fechaFin
				order by p.FechaCreacion desc			
		end
		
		if @datoBusqueda = 'ENV_ENVP' /*and @numRemision = '' or @numRemision = null*/
		begin
			select @fechaFin = DATEADD(DAY, 1, @fechaFin)
			
			SELECT DISTINCT IdPedido
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
								--INNER JOIN dbo.PruebaMuestra pm ON pr.IdPrueba = pm.IdPrueba 16/01/2024
								--INNER JOIN dbo.Muestra m on pm.IdMuestra = m.IdMuestra 16/01/2024
								inner join Muestra m on o.IdOrden = m.IdOrden --16/01/2024
								WHERE o.IdPedido = p.IdPedido
								and m.Eliminado != 1 --nuevo 11/01/2024
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
							INNER JOIN CatalogoMaestro CM ON CM.IdCatalogoMaestro = @i_idEstadoMue
							INNER JOIN CatalogoDetalle CD ON CM.IdCatalogoMaestro = CD.IdCatalogoMaestro
							WHERE o.IdPedido = p.IdPedido AND m.EstadoMuestra =CD.IdCatalogoDetalle AND (CD.Valor ='RECT' OR CD.Valor ='RECB' OR CD.Valor ='ENTR')
							GROUP BY m.IdMuestra
							) muestrasRetiradas
						) AS TotalRetiradas
					,p.FechaRetiro
					,(SELECT Nombre FROM CatalogoDetalle WHERE IdCatalogoMaestro = @idCatalogoMaestro and IdCatalogoDetalle = p.EstadoPedido) as EstadoPedido
				FROM Pedido p 
				INNER JOIN CatalogoMaestro CM ON CM.IdCatalogoMaestro=@idCatalogoMaestro
				INNER JOIN CatalogoDetalle CD ON CD.IdCatalogoMaestro = CM.IdCatalogoMaestro
				Where p.UsuarioCreacion = @idUsuario
				and p.EstadoPedido in (@i_idEstadoEnv, @i_idEstadoEnvp)
				AND p.FechaCreacion BETWEEN @fechaInicio AND @fechaFin
				order by p.FechaCreacion desc
			
		end

	END

END

GO