/************************************************************************
*	Stored procedure: pr_operador_muestras								*
*	Base de datos: DbPortalHumalab						  			    *
*	Elaborado por: José Guarnizo						                *
*----------------------------------------------------------------------	*
*					DESCRIPCION DEL PROCEDIMIENTO						*
*	En este Procedimiento muestras del operador                         *
*	                                                                    *
*----------------------------------------------------------------------	*
*					BITACORA DE MODIFICACIONES							*
*	FECHA AUTOR RAZON													*
*	29/01/2025 José Guarnizo Se comenta para que no se actualice el     *
*							 precio de la prueba a 0.00. 			    *
*----------------------------------------------------------------------	*/
IF NOT EXISTS (SELECT * FROM  sys.procedures WHERE NAME = 'pr_operador_muestras')	
	EXEC('Create Procedure dbo.pr_operador_muestras As')
go

ALTER PROCEDURE [dbo].[pr_operador_muestras](
	@i_accion CHAR(2)
	,@i_id_prueba_muestra INT
	,@i_recolecta BIT
	,@i_rechaza BIT
	,@i_es_operador BIT
	,@i_usuario_operador INT
	,@i_nombre_usuario VARCHAR(100)
	,@i_observacion VARCHAR(200)
)

as

DECLARE @fecha_actual DATETIME = GETDATE(),
	@i_idEstadoOrd int,
	@i_idEstadoMues int,
	@i_idEstadoPrue int

select @i_idEstadoOrd = IdCatalogoMaestro
from CatalogoMaestro
where Nombre = 'EstadoOrden'

select @i_idEstadoMues = IdCatalogoMaestro
from CatalogoMaestro
where Nombre = 'EstadoMuestra'

select @i_idEstadoPrue = IdCatalogoMaestro
from CatalogoMaestro
where Nombre = 'EstadoPrueba'

IF @i_accion = 'I'
BEGIN
	RETURN 0
END
ELSE IF @i_accion = 'M'
BEGIN
	BEGIN TRAN ActualizarMuestra
	DECLARE @id_estado_muestra varchar(50), @id_prueba int, @muestras_prueba int, @m_rech_antes int, @m_rech_despues int,
	@id_orden int, @pruebas_orden int, @p_rech_antes int, @p_rech_despues int, @estado_prueba varchar(50), @estado_orden varchar(50),
	@id_pedido int,  @estado_pedido varchar(50), @ordenes_pedido int, @o_rech_antes int, @o_rech_despues int

	IF(@i_es_operador = 1)
	BEGIN
		IF(@i_recolecta = 0 AND @i_rechaza = 0) 
		BEGIN
			SET @id_estado_muestra = 'PREC'
		END
		ELSE IF(@i_recolecta = 1 AND @i_rechaza = 0) 
		BEGIN
			SET @id_estado_muestra = 'RECT'
		END
		ELSE IF(@i_recolecta = 0 AND @i_rechaza = 1) 
		BEGIN
			SET @id_estado_muestra = 'RCHO'
		END
		ELSE
			RAISERROR ('No se puede recolectar y rechazar a la vez',16,1);
	END
	ELSE
	BEGIN
		IF(@i_recolecta = 0 AND @i_rechaza = 0) 
		BEGIN
			SET @id_estado_muestra = 'RECT'
		END
		ELSE IF(@i_recolecta = 1 AND @i_rechaza = 0) 
		BEGIN
			SET @id_estado_muestra = 'RECB'
		END
		ELSE IF(@i_recolecta = 0 AND @i_rechaza = 1) 
		BEGIN
			SET @id_estado_muestra = 'RCHL'
		END
		ELSE
			RAISERROR ('No se puede recibir y rechazar a la vez',16,1);
	END
	
	--Estado inicial de pedido y orden
	SET @estado_pedido = 'PREC'
	SET @estado_orden = 'PREC'

	--Consulta prueba de la muestra
	SELECT @id_prueba = pm.IdPrueba, @estado_prueba = (SELECT cd.Nombre FROM CatalogoDetalle cd WHERE cd.IdCatalogoDetalle = pr.Estado)
	FROM dbo.Prueba pr INNER JOIN dbo.PruebaMuestra pm ON pr.IdPrueba = pm.IdPrueba
	INNER JOIN dbo.Muestra m ON m.IdMuestra = pm.IdMuestra
	WHERE m.IdMuestra = @i_id_prueba_muestra AND COALESCE(pr.Eliminado,0) = 0 AND COALESCE(m.Eliminado,0) = 0
	
	--Consulta estado de la orden
	SELECT @id_orden = pr.IdOrden
	FROM dbo.Orden o INNER JOIN dbo.Prueba pr ON o.IdOrden = pr.IdOrden
	INNER JOIN dbo.PruebaMuestra pm ON pr.IdPrueba = pm.IdPrueba
	INNER JOIN dbo.Muestra m ON m.IdMuestra = pm.IdMuestra
	WHERE m.IdMuestra = @i_id_prueba_muestra AND COALESCE(pr.Eliminado,0) = 0 AND COALESCE(m.Eliminado,0) = 0

	--Consulta estado del pedido
	SELECT @id_pedido = o.IdPedido
	FROM dbo.Pedido p INNER JOIN dbo.Orden o ON o.IdPedido = p.IdPedido
	WHERE o.IdOrden = @id_orden AND COALESCE(p.Eliminado,0) = 0 AND COALESCE(o.Eliminado,0) = 0

	--Ordenes del pedido
	SET @ordenes_pedido = (SELECT COUNT(IdOrden) FROM dbo.Orden WHERE IdPedido = @id_pedido AND COALESCE(Eliminado,0) = 0 )
	
	--Muestras de la prueba
	SET @muestras_prueba = (SELECT COUNT(pm.IdMuestra) FROM dbo.Prueba pr INNER JOIN dbo.PruebaMuestra pm ON pr.IdPrueba = pm.IdPrueba
	INNER JOIN dbo.Muestra m ON m.IdMuestra = pm.IdMuestra
	WHERE pm.IdPrueba = @id_prueba AND COALESCE(pr.Eliminado,0) = 0 AND COALESCE(m.Eliminado,0) = 0)

	--consulta ordenes rechazadas del pedido
	SET @o_rech_antes = (SELECT COUNT(IdOrden) FROM dbo.Orden WHERE IdPedido = @id_pedido AND Estado = (SELECT IdCatalogoDetalle FROM CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoOrd AND Valor ='RCHZ')
	AND COALESCE(Eliminado,0) = 0 )

	--consulta muestras rechazadas
	SET @m_rech_antes = (SELECT COUNT(pm.IdMuestra) FROM dbo.Prueba pr INNER JOIN dbo.PruebaMuestra pm ON pr.IdPrueba = pm.IdPrueba
	INNER JOIN dbo.Muestra m ON m.IdMuestra = pm.IdMuestra
	WHERE pm.IdPrueba = @id_prueba AND COALESCE(m.Eliminado,0) = 0
	AND m.EstadoMuestra IN (SELECT IdCatalogoDetalle FROM CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoMues AND Valor IN ('RCHO','RCHL')))

	--Consulta pruebas de la orden
	SET @pruebas_orden = (SELECT COUNT(pr.IdOrden) FROM dbo.Prueba pr INNER JOIN dbo.Orden o ON pr.IdOrden = o.IdOrden
	WHERE o.IdOrden = @id_orden AND COALESCE(pr.Eliminado,0) = 0)

	--Consulta pruebas rechazadas
	SET @p_rech_antes = (SELECT COUNT(pr.IdPrueba) FROM dbo.Prueba pr INNER JOIN dbo.Orden o ON pr.IdOrden = o.IdOrden
	WHERE o.IdOrden = @id_orden AND COALESCE(pr.Eliminado,0) = 0 AND pr.Estado = (SELECT IdCatalogoDetalle FROM CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoPrue AND Valor ='RCHZ'))

	--Actualiza el estado de la muestra
	UPDATE [dbo].[Muestra]
	SET [EstadoMuestra] = (SELECT IdCatalogoDetalle FROM CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoMues AND Valor = @id_estado_muestra)
		,[UsuarioModificacion] = @i_usuario_operador
		,[FechaModificacion] = @fecha_actual
	WHERE [IdMuestra] = @i_id_prueba_muestra

	--Actualiza el estado de las pruebas atadas a la muestra
	IF(@i_es_operador = 1)
	BEGIN
		UPDATE p
		SET Estado = CASE WHEN (@id_estado_muestra = 'RCHO' OR @id_estado_muestra = 'RCHL') THEN (SELECT IdCatalogoDetalle FROM CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoPrue AND Valor = 'RCHZ')		
		WHEN (@id_estado_muestra = 'PREC') THEN (SELECT IdCatalogoDetalle FROM CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoPrue AND Valor = 'GENE')
		WHEN (@id_estado_muestra = 'RECB') THEN (SELECT IdCatalogoDetalle FROM CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoPrue AND Valor = 'PPRC')
		WHEN (@id_estado_muestra = 'RECT') THEN (SELECT IdCatalogoDetalle FROM CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoPrue AND Valor = 'RECT')
		END
		,p.Observacion = @i_observacion
		,p.UsuarioModificacion = @i_usuario_operador
		,p.FechaModificacion = @fecha_actual
		FROM dbo.Prueba p INNER JOIN dbo.PruebaMuestra pm ON p.IdPrueba = pm.IdPrueba
		WHERE pm.IdMuestra = @i_id_prueba_muestra

		--UPDATE p
		--SET Precio = 0.00
		--FROM dbo.Prueba p 
		--INNER JOIN dbo.PruebaMuestra pm ON p.IdPrueba = pm.IdPrueba
		--WHERE pm.IdMuestra = @i_id_prueba_muestra	
	END
	ELSE
	BEGIN
		UPDATE p
		SET Estado = CASE WHEN (@id_estado_muestra = 'RCHO' OR @id_estado_muestra = 'RCHL') THEN (SELECT IdCatalogoDetalle FROM CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoPrue AND Valor = 'RCHZ')
		WHEN (@id_estado_muestra = 'PREC') THEN (SELECT IdCatalogoDetalle FROM CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoPrue AND Valor = 'RECT')
		WHEN (@id_estado_muestra = 'RECB') THEN (SELECT IdCatalogoDetalle FROM CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoPrue AND Valor = 'PPRC')
		WHEN (@id_estado_muestra = 'RECT') THEN (SELECT IdCatalogoDetalle FROM CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoPrue AND Valor = 'RECT')
		END
		,p.Observacion = @i_observacion
		,p.UsuarioModificacion = @i_usuario_operador
		,p.FechaModificacion = @fecha_actual
		FROM dbo.Prueba p INNER JOIN dbo.PruebaMuestra pm ON p.IdPrueba = pm.IdPrueba
		WHERE pm.IdMuestra = @i_id_prueba_muestra
	END


	--EN CASO QUE RECHAZA
	IF (@id_estado_muestra = 'RCHO' OR @id_estado_muestra = 'RCHL')
	BEGIN
		IF EXISTS (
				SELECT Descripcion
				FROM [dbo].[ObservacionM]
				WHERE IdMuestra = @i_id_prueba_muestra
					AND UsuarioCreacion = @i_usuario_operador AND COALESCE(Eliminado,0) = 0
				)
		BEGIN
			UPDATE [dbo].[ObservacionM]
			SET [Descripcion] = @i_observacion
				,[UsuarioModificacion] = @i_usuario_operador
				,[FechaModificacion] = @fecha_actual
				,Eliminado = 0
			WHERE IdMuestra = @i_id_prueba_muestra
				AND UsuarioCreacion = @i_usuario_operador AND COALESCE(Eliminado,0) = 0								

		END
		ELSE
		BEGIN
			INSERT INTO [dbo].[ObservacionM] (
				[IdMuestra]
				,[Descripcion]
				,[NombreUsuario]
				,[UsuarioCreacion]
				,[Operador]
				,[FechaCreacion]
				,[UsuarioModificacion]
				,[FechaModificacion]
				,[UsuarioEliminacion]
				,[FechaEliminacion]
				,[Eliminado]
				)
			VALUES (
				@i_id_prueba_muestra
				,@i_observacion
				,@i_nombre_usuario
				,@i_usuario_operador
				,@i_es_operador
				,@fecha_actual
				,NULL
				,NULL
				,NULL
				,NULL
				,0
				)
		END
	END
	ELSE
	BEGIN
		IF EXISTS (
				SELECT Descripcion
				FROM [dbo].[ObservacionM]
				WHERE IdMuestra = @i_id_prueba_muestra
					AND UsuarioCreacion = @i_usuario_operador AND COALESCE(Eliminado,0) = 0
				)
		BEGIN
			UPDATE [dbo].[ObservacionM]
			SET [UsuarioEliminacion] = @i_usuario_operador
				,[FechaEliminacion] = @fecha_actual
				,Eliminado = 1
			WHERE IdMuestra = @i_id_prueba_muestra
				AND UsuarioCreacion = @i_usuario_operador  AND COALESCE(Eliminado,0) = 0
		END
	END

	--Consulta muestras y pruebas rechazadas luego de la actualización de la muestra
	SET @m_rech_despues = (SELECT COUNT(pm.IdMuestra) FROM dbo.Prueba pr INNER JOIN dbo.PruebaMuestra pm ON pr.IdPrueba = pm.IdPrueba
	INNER JOIN dbo.Muestra m ON m.IdMuestra = pm.IdMuestra
	WHERE pm.IdPrueba = @id_prueba AND COALESCE(m.Eliminado,0) = 0
	AND m.EstadoMuestra IN (SELECT IdCatalogoDetalle FROM CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoMues AND Valor IN ('RCHO','RCHL')))

	SET @p_rech_despues = (SELECT COUNT(pr.IdPrueba) FROM dbo.Prueba pr INNER JOIN dbo.Orden o ON pr.IdOrden = o.IdOrden
	WHERE o.IdOrden = @id_orden AND COALESCE(pr.Eliminado,0) = 0 AND pr.Estado = (SELECT IdCatalogoDetalle FROM CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoPrue AND Valor ='RCHZ'))

	--Si todas las pruebas de la orden están rechazadas entonces la orden se rechaza
	IF (@pruebas_orden = @p_rech_despues)
	BEGIN
		UPDATE dbo.Orden
		SET Estado = (SELECT IdCatalogoDetalle FROM CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoOrd AND Valor = 'RCHZ')
		,[UsuarioModificacion] = @i_usuario_operador
		,[FechaModificacion] = @fecha_actual
		,Observacion = 'Orden rechazada porque todas las muestras fueron rechazadas'
		WHERE IdOrden = @id_orden

	END


	COMMIT TRAN ActualizarMuestra
	SELECT IdMuestra = @i_id_prueba_muestra, EstadoMuestra = (SELECT Nombre from [dbo].[CatalogoDetalle] where IdCatalogoMaestro=@i_idEstadoMues and Valor= @id_estado_muestra), Observacion=@i_observacion
END
ELSE IF @i_accion = 'C'
BEGIN
	RETURN 0
END
ELSE IF @i_accion = 'G'
BEGIN
	RETURN 0
END
ELSE
BEGIN
	RAISERROR (
			'El código de la acción es incorrecto.'
			,16
			,1
			)
END

GO