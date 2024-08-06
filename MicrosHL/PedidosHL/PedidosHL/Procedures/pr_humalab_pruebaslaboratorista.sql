/************************************************************************
*	Stored procedure: pr_humalab_pruebaslaboratorista					*
*	Base de datos: DbPortalHumalab						  			    *
*	Elaborado por: José Guarnizo						                *
*----------------------------------------------------------------------	*
*					DESCRIPCION DEL PROCEDIMIENTO						*
*	En este Procedimiento pruebas del laboratorista                     *
*	                                                                    *
*----------------------------------------------------------------------	*
*					BITACORA DE MODIFICACIONES							*
*	FECHA AUTOR RAZON													*
*						                                                *
*----------------------------------------------------------------------	*/
IF NOT EXISTS (SELECT * FROM  sys.procedures WHERE NAME = 'pr_humalab_pruebaslaboratorista')	
	EXEC('Create Procedure dbo.pr_humalab_pruebaslaboratorista As')
go

ALTER PROCEDURE [dbo].[pr_humalab_pruebaslaboratorista](
	@i_accion CHAR(2)
	,@i_id_prueba INT
	,@i_id_muestra INT
	,@i_rechaza BIT
	,@i_usuario INT
	,@i_observacion VARCHAR(200)
)

as

DECLARE @fechaActual DATETIME,
	@i_idEstadoMues int,
	@i_idEstadoPrue int

SET @fechaActual = GETDATE()

select @i_idEstadoMues = IdCatalogoMaestro
from CatalogoMaestro
where Nombre = 'EstadoMuestra'

select @i_idEstadoPrue = IdCatalogoMaestro
from CatalogoMaestro
where Nombre = 'EstadoPrueba'

DECLARE @id_estado_muestra INT, @id_estado_prueba INT, @pruebas_restantes INT, @pruebas_orden INT, @id_orden INT, @p_rech_despues INT, @p_rech_antes INT, @estado_prueba INT,
@rechazaMuestra BIT, @muestrasRechazadas INT, @activaMuestra BIT, @estadoMuestra INT


IF @i_accion = 'M'
BEGIN
BEGIN TRANSACTION;

BEGIN TRY	

	SET @rechazaMuestra = 0
	SET @activaMuestra = 0

	SET @id_orden = (SELECT TOP 1 IdOrden FROM dbo.Prueba WHERE IdPrueba = @i_id_prueba)

	SET @estadoMuestra = (SELECT EstadoMuestra FROM Muestra WHERE IdMuestra = @i_id_muestra)

	--Valida el estado inicial de la prueba dependiendo del estado de la muestra


	--Consulta el número de pruebas de la orden
	SELECT @pruebas_orden = (SELECT COUNT(pr.IdOrden)
	FROM dbo.Prueba pr INNER JOIN dbo.Orden o ON pr.IdOrden = o.IdOrden
	WHERE o.IdOrden = @id_orden AND COALESCE(pr.Eliminado,0) = 0)
	
	--Consulta estado de la prueba
	SELECT @estado_prueba = pr.Estado
	FROM dbo.Prueba pr WHERE IdPrueba = @i_id_prueba

	--Consulta si la prueba tiene una muestra rechazada
	SET @muestrasRechazadas = (SELECT COUNT(*)
			FROM Prueba pr INNER JOIN PruebaMuestra pm ON pr.IdPrueba = pm.IdPrueba AND COALESCE(pm.Eliminado,0) = 0
			INNER JOIN Muestra m ON m.IdMuestra = pm.IdMuestra AND COALESCE(m.Eliminado,0) = 0 AND m.EstadoMuestra = (SELECT IdCatalogoDetalle FROM CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoMues AND Valor ='RCHL')
			WHERE pr.IdPrueba = @i_id_prueba)

		--Consulta pruebas rechazadas
	SET @p_rech_antes = (SELECT COUNT(pr.IdPrueba) FROM dbo.Prueba pr INNER JOIN dbo.Orden o ON pr.IdOrden = o.IdOrden
	WHERE o.IdOrden = @id_orden AND COALESCE(pr.Eliminado,0) = 0 AND pr.Estado = (SELECT IdCatalogoDetalle FROM CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoPrue AND Valor ='RCHZ'))

		IF(@i_rechaza = 0)
		BEGIN
			SET @id_estado_muestra = (SELECT EstadoMuestra FROM dbo.Muestra WHERE IdMuestra = @i_id_muestra)
			IF(@id_estado_muestra = (SELECT IdCatalogoDetalle FROM CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoMues AND Valor = 'RECT'))
				SET @id_estado_prueba = (SELECT IdCatalogoDetalle FROM CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoPrue AND Valor = 'RECT')
			ELSE IF(@id_estado_muestra = (SELECT IdCatalogoDetalle FROM CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoMues AND Valor = 'RECB'))
				SET @id_estado_prueba = (SELECT IdCatalogoDetalle FROM CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoPrue AND Valor = 'PPRC')

			IF(@muestrasRechazadas > 0)
				SET @activaMuestra = 1
		END
		ELSE 
		BEGIN
			SET @id_estado_muestra = (SELECT IdCatalogoDetalle FROM CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoMues AND Valor = 'RCHL')
			SET @id_estado_prueba = (SELECT IdCatalogoDetalle FROM CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoPrue AND Valor = 'RCHZ')

		END

			SET @pruebas_restantes = (SELECT  COUNT(*)
			FROM Prueba pr INNER JOIN PruebaMuestra pm ON pr.IdPrueba = pm.IdPrueba AND COALESCE(pm.Eliminado,0) = 0
			AND pr.Estado <> (SELECT IdCatalogoDetalle FROM CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoPrue AND Valor = 'RCHZ' )
			INNER JOIN Muestra m ON m.IdMuestra = pm.IdMuestra AND COALESCE(m.Eliminado,0) = 0
			WHERE pm.IdMuestra = @i_id_muestra AND pr.IdPrueba <> @i_id_prueba)

			UPDATE DBO.Prueba
			SET Estado = @id_estado_prueba
			,Observacion = @i_observacion
			,FechaModificacion = @fechaActual
			,UsuarioModificacion = @i_usuario
			WHERE IdPrueba = @i_id_prueba

			IF(@pruebas_restantes = 0 AND @i_rechaza = 1)
			BEGIN
				SET @rechazaMuestra = 1
			END

		COMMIT;
		SELECT IdMuestra = @i_id_muestra, IdPrueba = @i_id_prueba, RechazaMuestra = @rechazaMuestra, ActivaMuestra = @activaMuestra, EstadoPrueba = (SELECT Nombre from [dbo].[CatalogoDetalle] where IdCatalogoMaestro=@i_idEstadoMues and IdCatalogoDetalle= @id_estado_prueba)
		END TRY
		BEGIN CATCH

			ROLLBACK;
			SELECT IdMuestra = 0, IdPrueba = 0, RechazaMuestra = 0, ActivaMuestra = 0, EstadoPrueba = ERROR_MESSAGE()

		END CATCH;

END
ELSE IF @i_accion = 'C'
BEGIN
	SELECT 
		pr.IdOrden
		,pr.IdPrueba
		,IdPruebaGalileo AS Codigo
		,pr.Nombre AS PruebaPerfil
		,ISNULL(
			CASE 
				WHEN (SELECT Valor FROM CatalogoDetalle cd WHERE cd.IdCatalogoMaestro = @i_idEstadoPrue AND cd.IdCatalogoDetalle = pr.Estado) IN ('RCHZ')  THEN CAST(1 AS BIT)
				ELSE CAST(0 AS BIT)
			END, CAST(0 AS BIT)
		) AS PruebaRechazada
		,pr.Observacion AS ObservacionPrueba
		FROM Prueba pr INNER JOIN PruebaMuestra pm ON pr.IdPrueba = pm.IdPrueba AND COALESCE(pm.Eliminado,0) = 0 AND COALESCE(pr.Eliminado,0) = 0
		INNER JOIN Muestra m ON m.IdMuestra = pm.IdMuestra AND COALESCE(m.Eliminado,0) = 0
		WHERE m.IdMuestra = @i_id_muestra
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