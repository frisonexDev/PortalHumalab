/************************************************************************
*	Stored procedure: pr_operador_pedidos_new							*
*	Base de datos: DbPortalHumalab						  			    *
*	Elaborado por: José Guarnizo						                *
*----------------------------------------------------------------------	*
*					DESCRIPCION DEL PROCEDIMIENTO						*
*	En este Procedimiento pedidos del operador                          *
*	                                                                    *
*----------------------------------------------------------------------	*
*					BITACORA DE MODIFICACIONES							*
*	FECHA AUTOR RAZON													*
*						                                                *
*----------------------------------------------------------------------	*/
IF NOT EXISTS (SELECT * FROM  sys.procedures WHERE NAME = 'pr_operador_pedidos_new')	
	EXEC('Create Procedure dbo.pr_operador_pedidos_new As')
go

ALTER PROCEDURE [dbo].[pr_operador_pedidos_new](
	@i_accion CHAR(2)
	,@i_fecha_desde DATE
	,@i_fecha_hasta DATE
	,@i_cliente VARCHAR(100)
	,@i_es_identificacion BIT
	,@i_operador_logistico INT
	,@i_estado VARCHAR(50)
	,@i_pedido INT
	,@i_observacion_operador VARCHAR(100)
	,@i_pedidos_entrega VARCHAR(MAX)
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
	@sucursal varchar(100)

SET @fechaActual = GETDATE()


select @i_idEstadoPedi = IdCatalogoMaestro
from CatalogoMaestro
where Nombre = 'EstadoPedido'

select @i_idEstadoMues = IdCatalogoMaestro
from CatalogoMaestro
where Nombre = 'EstadoMuestra'

select @i_idEstadoOrden = IdCatalogoMaestro
from CatalogoMaestro
where Nombre = 'EstadoOrden'

--enviado parcial y total
select @idEnv = IdCatalogoDetalle
from CatalogoDetalle
where IdCatalogoMaestro = @i_idEstadoPedi
and Valor = 'ENV'

select @idEnvP = IdCatalogoDetalle
from CatalogoDetalle
where IdCatalogoMaestro = @i_idEstadoPedi
and Valor = 'ENVP'

--recolectado total y parcial
select @idRec = IdCatalogoDetalle
from CatalogoDetalle
where IdCatalogoMaestro = @i_idEstadoPedi
and Valor = 'RCTL'

select @idRecP = IdCatalogoDetalle
from CatalogoDetalle
where IdCatalogoMaestro = @i_idEstadoPedi
and Valor = 'RCPC'

--anulados
select @idAnu = IdCatalogoDetalle
from CatalogoDetalle
where IdCatalogoMaestro = @i_idEstadoPedi
and Valor = 'ANUL'

--por recolectar
select @idPorRec = IdCatalogoDetalle
from CatalogoDetalle
where IdCatalogoMaestro = @i_idEstadoPedi
and Valor = 'PREC'

--identificacion del cliente
select @idDniCliente = Identificacion
from Cliente
where IdOperadorLogistico = @i_operador_logistico

--sucursal del cliente y usuario
select @sucursal = CodClienteCta
from Cliente
where IdOperadorLogistico = @i_operador_logistico


IF @i_accion = 'I'
BEGIN
	RETURN 0
END
ELSE IF @i_accion = 'M' -- RECOGE EL PEDIDO
BEGIN
	IF @i_pedidos_entrega IS NULL
	BEGIN

BEGIN TRANSACTION;

BEGIN TRY

		DECLARE @muestras_totales INT
			,@muestras_aceptadas INT
			,@muestras_rechazadas INT
		DECLARE @estado_pedido INT

		

		SET @muestras_totales = (
				SELECT count(*)
				FROM (
					SELECT m.IdMuestra
					FROM dbo.Prueba pr
					INNER JOIN dbo.Orden o ON o.IdOrden = pr.IdOrden
					INNER JOIN dbo.PruebaMuestra pm ON pr.IdPrueba = pm.IdPrueba
					INNER JOIN dbo.Muestra m on pm.IdMuestra = m.IdMuestra
					WHERE o.IdPedido = @i_pedido
					GROUP BY m.IdMuestra
					) a
				)
		SET @muestras_aceptadas = (
				SELECT count(*)
				FROM (
					SELECT m.IdMuestra
					FROM dbo.Prueba pr
					INNER JOIN dbo.Orden o ON o.IdOrden = pr.IdOrden
					INNER JOIN dbo.PruebaMuestra pm ON pr.IdPrueba = pm.IdPrueba
					INNER JOIN dbo.Muestra m on pm.IdMuestra = m.IdMuestra
					WHERE o.IdPedido = @i_pedido AND m.EstadoMuestra = (SELECT IdCatalogoDetalle FROM dbo.CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoMues AND Valor = 'RECT')
					GROUP BY m.IdMuestra
					) a
				)
				
		IF @muestras_totales = @muestras_aceptadas			
			SET @estado_pedido = (SELECT IdCatalogoDetalle FROM CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoPedi AND Valor = 'ENV')
		ELSE			
			SET @estado_pedido = (SELECT IdCatalogoDetalle FROM CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoPedi AND Valor = 'ENVP')

		UPDATE [dbo].[Pedido]
		SET [EstadoPedido] = @estado_pedido
			,[ObservacionOpLogistico] = @i_observacion_operador
			,[UsuarioModificacion] = @i_operador_logistico
			,[FechaRetiro] = @fechaActual
			,[FechaModificacion] = @fechaActual
		WHERE [IdPedido] = @i_pedido

		
SELECT r.IdOrden, TotalMuestras, Recolectadas
INTO #TEMP1
FROM
--Muestras totales por orden
(SELECT a.IdOrden, COUNT(IdMuestra) AS TotalMuestras
FROM(

SELECT o.IdOrden, m.IdMuestra
					FROM dbo.Prueba pr
					INNER JOIN dbo.Orden o ON o.IdOrden = pr.IdOrden AND COALESCE(pr.Eliminado,0) = 0
					INNER JOIN dbo.PruebaMuestra pm ON pr.IdPrueba = pm.IdPrueba AND COALESCE(pm.Eliminado,0) = 0
					INNER JOIN dbo.Muestra m on pm.IdMuestra = m.IdMuestra AND COALESCE(m.Eliminado,0) = 0
					WHERE o.IdPedido = @i_pedido
					GROUP BY o.IdOrden,m.IdMuestra) a
GROUP BY a.IdOrden) t
					LEFT JOIN 

--Muestras recolectadas por orden
(SELECT a.IdOrden, COUNT(IdMuestra) AS Recolectadas
FROM(
--Muestras por orden
SELECT o.IdOrden, m.IdMuestra
					FROM dbo.Orden o
					INNER JOIN dbo.Prueba pr ON o.IdOrden = pr.IdOrden AND COALESCE(pr.Eliminado,0) = 0
					INNER JOIN dbo.PruebaMuestra pm ON pr.IdPrueba = pm.IdPrueba AND COALESCE(pm.Eliminado,0) = 0
					LEFT JOIN dbo.Muestra m on pm.IdMuestra = m.IdMuestra AND COALESCE(m.Eliminado,0) = 0 AND m.EstadoMuestra = (SELECT IdCatalogoDetalle FROM dbo.CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoMues AND Valor = 'RECT')
					WHERE o.IdPedido = @i_pedido
					GROUP BY o.IdOrden,m.IdMuestra) a
GROUP BY a.IdOrden) r
 ON t.IdOrden = r.IdOrden


UPDATE o
SET Estado = CASE 	
	WHEN Recolectadas = TotalMuestras THEN (SELECT IdCatalogoDetalle FROM CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoOrden AND Valor = 'ENV')	
	WHEN Recolectadas > 0 AND Recolectadas < TotalMuestras THEN (SELECT IdCatalogoDetalle FROM CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoOrden AND Valor = 'ENVP')
	ELSE (SELECT IdCatalogoDetalle FROM CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoOrden AND Valor = 'PREC')
END
FROM Orden o INNER JOIN #TEMP1 t ON o.IdOrden = t.IdOrden AND Estado <> (SELECT IdCatalogoDetalle FROM CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoOrden AND Valor = 'RCHZ' )


COMMIT;
END TRY
BEGIN CATCH
    -- En caso de error, deshacer la transacción
    ROLLBACK;
    PRINT 'Error en la transacción: ' + ERROR_MESSAGE();
END CATCH;

		SELECT @i_pedido AS numPedido
	END
	ELSE -- ENTREGAR PEDIDO
	BEGIN
		DECLARE @entregado BIT, @estado_entregado INT
		SET @entregado = 1
		SET @estado_entregado = (SELECT IdCatalogoDetalle FROM CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoPedi AND Valor = 'ENTR')

		BEGIN TRANSACTION;

		UPDATE [dbo].[Pedido]
		SET [EstadoPedido] = @estado_entregado,
			[ObservacionOpLogistico] = @i_observacion_operador,
			[UsuarioModificacion] = @i_operador_logistico,
			[FechaModificacion] = GETDATE()
		WHERE [IdPedido] IN (
			SELECT CAST(value AS INT)
			FROM STRING_SPLIT(@i_pedidos_entrega, ',')
			WHERE TRY_CAST(value AS INT) IS NOT NULL
		);

		-- Verificar si ocurrió algún error durante la actualización
		IF @@ERROR <> 0
		BEGIN
			ROLLBACK TRANSACTION; -- Deshacer la transacción en caso de error
			SET @entregado = 0
		END

		COMMIT TRANSACTION;
		SELECT @entregado AS Entregado
	END
END

-- CONSULTAR DETALLE DEL PEDIDO
ELSE IF @i_accion = 'CD'
BEGIN

DECLARE @estado_recolectado INT 
	SET @estado_recolectado = (SELECT IdCatalogoDetalle FROM CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoMues AND Valor = 'RECT')

	SELECT p.IdPedido
		,p.NumeroRemision
		,(SELECT Nombre FROM CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoPedi and IdCatalogoDetalle = p.EstadoPedido) as EstadoPedido
		,(SELECT c.NombreCliente FROM Cliente c INNER JOIN Usuario u ON c.IdUsuario = u.IdUsuario WHERE u.idGalileo = p.UsuarioCreacion ) AS Cliente
		,p.FechaCreacion --nuevo 18/01/2024
		,(SELECT ob.Descripcion FROM Observacion ob INNER JOIN PedidoObservacion pob ON ob.IdObservacion = pob.IdObservacion AND pob.IdPedido = p.IdPedido) AS ObservacionCliente
		,(SELECT Usuario FROM Usuario where idGalileo = p.UsuarioCreacion) AS Usuario
		,p.UsuarioOperador  AS OperadorLogistico
		,(SELECT u.Email FROM Usuario u WHERE u.idGalileo = p.UsuarioCreacion) AS CorreoCliente
		,(select c.Telefono from Cliente c INNER JOIN Usuario u ON c.IdUsuario = u.IdUsuario WHERE u.idGalileo = p.UsuarioCreacion) AS Telefono		
		,p.ObservacionOpLogistico AS ObservacionOperador
		,(select cd.Direccion 
		  from ClienteDireccion cd 
		  inner join Cliente c on c.IdCliente = cd.IdCliente
		  inner join Usuario u on c.IdUsuario = u.idUsuario
		  where u.idGalileo = p.UsuarioCreacion) as ClienteDireccion
		,(select cd.Ciudad 
		  from ClienteDireccion cd 
		  inner join Cliente c on cd.IdCliente = c.IdCliente
		  inner join Usuario u on c.IdUsuario = u.idUsuario
		  where u.idGalileo = p.UsuarioCreacion) as ClienteCiudad
		,(SELECT count(*)
				FROM (
					SELECT m.IdMuestra
					FROM dbo.Prueba pr
					INNER JOIN dbo.Orden o ON o.IdOrden = pr.IdOrden										
					inner join Muestra m on o.IdOrden = m.IdOrden --16/01/2024
					AND COALESCE(m.Eliminado,0) = 0
					WHERE o.IdPedido = p.IdPedido
					GROUP BY m.IdMuestra
					) a
				) AS TotalMuestras
		,(SELECT count(*)
				FROM (
					SELECT m.IdMuestra
					FROM dbo.Prueba pr
					INNER JOIN dbo.Orden o ON o.IdOrden = pr.IdOrden
					INNER JOIN dbo.PruebaMuestra pm ON pr.IdPrueba = pm.IdPrueba
					INNER JOIN dbo.Muestra m on pm.IdMuestra = m.IdMuestra AND COALESCE(m.Eliminado,0) = 0
					WHERE o.IdPedido = p.IdPedido AND m.EstadoMuestra = @estado_recolectado
					GROUP BY m.IdMuestra
					) a
				) AS MuestrasRecolectadas
	FROM [DbPortalHumalab].[dbo].[Pedido] p
	WHERE p.IdPedido = @i_pedido

SELECT
    ROW_NUMBER() OVER (ORDER BY x.CodigoBarra) AS Numero,
    x.CodigoBarra,
    x.TipoMuestra,
	x.FechaOrden,
    x.Identificacion,
    x.Retirado,
    x.Rechazado,
    x.ObservacionOperador,
    x.EstadoMuestra,
    x.IdPruebaMuestra,
	x.Paciente
FROM (
    SELECT DISTINCT
        m.CodigoBarra,
        m.Nombre AS TipoMuestra,
		o.FechaCreacion AS FechaOrden,
        p.Identificacion,
		p.Nombres +' '+p.Apellidos as Paciente,
        ISNULL(
            CASE 
                WHEN (SELECT Valor  FROM CatalogoDetalle cd WHERE cd.IdCatalogoMaestro = @i_idEstadoMues AND cd.IdCatalogoDetalle = m.EstadoMuestra) IN ('RECT','RECB')  THEN CAST(1 AS BIT)
                WHEN (SELECT Valor  FROM CatalogoDetalle cd WHERE cd.IdCatalogoMaestro = @i_idEstadoMues AND cd.IdCatalogoDetalle = m.EstadoMuestra) IN ('PREC') THEN CAST(0 AS BIT)
            END, CAST(0 AS BIT)
        ) AS Retirado,
        ISNULL(
            CASE 
                WHEN (SELECT Valor  FROM CatalogoDetalle cd WHERE cd.IdCatalogoMaestro = @i_idEstadoMues AND cd.IdCatalogoDetalle = m.EstadoMuestra) IN ('RCHO','RCHL') THEN CAST(1 AS BIT)
                WHEN (SELECT Valor  FROM CatalogoDetalle cd WHERE cd.IdCatalogoMaestro = @i_idEstadoMues AND cd.IdCatalogoDetalle = m.EstadoMuestra) IN ('PREC') THEN CAST(0 AS BIT)
            END, CAST(0 AS BIT)
        ) AS Rechazado,
       mo.Descripcion AS ObservacionOperador,
	   (SELECT cat.Nombre FROM CatalogoDetalle cat WHERE cat.IdCatalogoMaestro = @i_idEstadoMues and IdCatalogoDetalle = m.EstadoMuestra) AS EstadoMuestra,
        m.IdMuestra AS IdPruebaMuestra
    FROM dbo.Prueba pr
    INNER JOIN dbo.Orden o ON o.IdOrden = pr.IdOrden        
	inner join Muestra m on o.IdOrden = m.IdOrden --16/01/2024
	INNER JOIN dbo.Paciente p ON p.Identificacion = o.Identificacion
    LEFT JOIN dbo.ObservacionM mo ON mo.IdMuestra = m.IdMuestra AND mo.Eliminado = 0 AND mo.Operador = 1
    LEFT JOIN dbo.Usuario usu ON usu.idUsuario = mo.UsuarioCreacion
	WHERE o.IdPedido = @i_pedido) AS x;

END

-- CONSULTAR PEDIDOS
ELSE IF @i_accion = 'C'
BEGIN
	DECLARE @i_identificacion VARCHAR(50), 
			@i_nombre_cliente VARCHAR(50), 
			@i_op INT	

	IF @i_es_identificacion IS NOT NULL
		AND @i_es_identificacion = 1
		SET @i_identificacion = @i_cliente
	ELSE
		SET @i_nombre_cliente = '%' + @i_cliente + '%'

	--Valida si el usuario tiene Rol cliente para mostrar todos los pedidos
	IF EXISTS (SELECT TOP 1 idUsuario FROM dbo.Usuario WHERE idGalileo = @i_operador_logistico AND IdRol = 100)
		SET @i_op = NULL
	ELSE
		SET @i_op = @i_operador_logistico		

		if @i_estado = 'RCTL_RCPC'
		begin

			SELECT IdPedido
				,p.UsuarioCreacion AS IdCliente
				,c.NombreCliente AS Cliente								
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
				inner join Muestra m on o.IdOrden = m.IdOrden --16/01/2024
				AND COALESCE(m.Eliminado,0) = 0
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
				INNER JOIN dbo.Muestra m on pm.IdMuestra = m.IdMuestra AND COALESCE(m.Eliminado,0) = 0
				WHERE o.IdPedido = p.IdPedido
				AND m.EstadoMuestra IN (SELECT IdCatalogoDetalle FROM dbo.CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoMues AND Valor IN ('RECT','RECB','ENTR'))
				GROUP BY m.IdMuestra
				) muestrasRetiradas
			) AS TotalRetiradas
			,p.FechaRetiro
			,(SELECT Nombre FROM CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoPedi and IdCatalogoDetalle = p.EstadoPedido) as EstadoPedido
			FROM [DbPortalHumalab].[dbo].[Pedido] p with (nolock)						
			--INNER JOIN dbo.Cliente c ON c.IdOperadorLogistico = p.IdOperador --2024/07/30
			inner join Usuario u on u.idGalileo = p.UsuarioCreacion --2024/07/30
			inner join Cliente c on u.idUsuario = c.IdUsuario --2024/07/30
			WHERE p.FechaCreacion >= @i_fecha_desde 			
			and c.CodClienteCta = @sucursal --nuevo 2024/01/25
			AND p.FechaCreacion < dateadd(DAY, 1, @i_fecha_hasta)			
			AND(p.EstadoPedido  in (@idRec, @idRecP))
			AND (
				@i_identificacion IS NULL
				OR c.Identificacion = @i_identificacion
			)
			AND (
				@i_nombre_cliente IS NULL
				OR c.NombreCliente LIKE @i_nombre_cliente
			)
			AND (
				(@i_op IS NULL AND p.UsuarioCreacion = @i_operador_logistico)
				OR IdOperador = @i_op
			)
			order by p.FechaCreacion desc

		end

		if @i_estado = 'ENV_ENVP'
		begin
			
			SELECT IdPedido
				,p.UsuarioCreacion AS IdCliente
				,c.NombreCliente AS Cliente								
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
				inner join Muestra m on o.IdOrden = m.IdOrden --16/01/2024
				AND COALESCE(m.Eliminado,0) = 0
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
				INNER JOIN dbo.Muestra m on pm.IdMuestra = m.IdMuestra AND COALESCE(m.Eliminado,0) = 0
				WHERE o.IdPedido = p.IdPedido
				AND m.EstadoMuestra IN (SELECT IdCatalogoDetalle FROM dbo.CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoMues AND Valor IN ('RECT','RECB','ENTR'))
				GROUP BY m.IdMuestra
				) muestrasRetiradas
			) AS TotalRetiradas
			,p.FechaRetiro
			,(SELECT Nombre FROM CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoPedi and IdCatalogoDetalle = p.EstadoPedido) as EstadoPedido
			FROM [DbPortalHumalab].[dbo].[Pedido] p with (nolock)						
			--INNER JOIN dbo.Cliente c ON c.IdOperadorLogistico = p.IdOperador --2024/07/30
			inner join Usuario u on u.idGalileo = p.UsuarioCreacion --2024/07/30
			inner join Cliente c on u.idUsuario = c.IdUsuario --2024/07/30
			WHERE p.FechaCreacion >= @i_fecha_desde 			
			and c.CodClienteCta = @sucursal --nuevo 2024/01/25
			AND p.FechaCreacion < dateadd(DAY, 1, @i_fecha_hasta)			
			AND(p.EstadoPedido  in (@idEnv, @idEnvP))
			AND (
				@i_identificacion IS NULL
				OR c.Identificacion = @i_identificacion
			)
			AND (
				@i_nombre_cliente IS NULL
				OR c.NombreCliente LIKE @i_nombre_cliente
			)
			AND (
				(@i_op IS NULL AND p.UsuarioCreacion = @i_operador_logistico)
				OR IdOperador = @i_op
			)
			order by p.FechaCreacion desc

		end

		if @i_estado = 'PREC' --(@i_estado != 'ENV_ENVP' or @i_estado != 'RCTL_RCPC')
		begin
			
			SELECT IdPedido
				,p.UsuarioCreacion AS IdCliente
				,c.NombreCliente AS Cliente								
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
				inner join Muestra m on o.IdOrden = m.IdOrden --16/01/2024
				AND COALESCE(m.Eliminado,0) = 0
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
				INNER JOIN dbo.Muestra m on pm.IdMuestra = m.IdMuestra AND COALESCE(m.Eliminado,0) = 0
				WHERE o.IdPedido = p.IdPedido
				AND m.EstadoMuestra IN (SELECT IdCatalogoDetalle FROM dbo.CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoMues AND Valor IN ('RECT','RECB','ENTR'))
				GROUP BY m.IdMuestra
				) muestrasRetiradas
			) AS TotalRetiradas
			,p.FechaRetiro
			,(SELECT Nombre FROM CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoPedi and IdCatalogoDetalle = p.EstadoPedido) as EstadoPedido
			FROM [DbPortalHumalab].[dbo].[Pedido] p with (nolock)						
			--INNER JOIN dbo.Cliente c ON c.IdOperadorLogistico = p.IdOperador --2024/07/30
			inner join Usuario u on u.idGalileo = p.UsuarioCreacion --2024/07/30
			inner join Cliente c on u.idUsuario = c.IdUsuario --2024/07/30
			WHERE p.FechaCreacion >= @i_fecha_desde			
			and c.CodClienteCta = @sucursal --nuevo 2024/01/25
			AND p.FechaCreacion < dateadd(DAY, 1, @i_fecha_hasta)			
			AND (p.EstadoPedido = @idPorRec)
			AND (
				@i_identificacion IS NULL
				OR c.Identificacion = @i_identificacion
			)
			AND (
				@i_nombre_cliente IS NULL
				OR c.NombreCliente LIKE @i_nombre_cliente
			)
			AND (
				(@i_op IS NULL AND p.UsuarioCreacion = @i_operador_logistico)
				OR IdOperador = @i_op
			)
			order by p.FechaCreacion desc

		end

		if @i_estado = 'ANUL'
		begin
			
			SELECT IdPedido
				,p.UsuarioCreacion AS IdCliente
				,c.NombreCliente AS Cliente				
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
				inner join Muestra m on o.IdOrden = m.IdOrden --16/01/2024
				AND COALESCE(m.Eliminado,0) = 0
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
				INNER JOIN dbo.Muestra m on pm.IdMuestra = m.IdMuestra AND COALESCE(m.Eliminado,0) = 0
				WHERE o.IdPedido = p.IdPedido
				AND m.EstadoMuestra IN (SELECT IdCatalogoDetalle FROM dbo.CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoMues AND Valor IN ('RECT','RECB','ENTR'))
				GROUP BY m.IdMuestra
				) muestrasRetiradas
			) AS TotalRetiradas
			,p.FechaRetiro
			,(SELECT Nombre FROM CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoPedi and IdCatalogoDetalle = p.EstadoPedido) as EstadoPedido
			FROM [DbPortalHumalab].[dbo].[Pedido] p with (nolock)						
			--INNER JOIN dbo.Cliente c ON c.IdOperadorLogistico = p.IdOperador --2024/07/30
			inner join Usuario u on u.idGalileo = p.UsuarioCreacion --2024/07/30
			inner join Cliente c on u.idUsuario = c.IdUsuario --2024/07/30
			WHERE p.FechaCreacion >= @i_fecha_desde 			
			and c.CodClienteCta = @sucursal --nuevo 2024/01/25
			AND p.FechaCreacion < dateadd(DAY, 1, @i_fecha_hasta)			
			AND(p.EstadoPedido  = @idAnu)
			AND (
				@i_identificacion IS NULL
				OR c.Identificacion = @i_identificacion
			)
			AND (
				@i_nombre_cliente IS NULL
				OR c.NombreCliente LIKE @i_nombre_cliente
			)
			AND (
				(@i_op IS NULL AND p.UsuarioCreacion = @i_operador_logistico)
				OR IdOperador = @i_op
			)
			order by p.FechaCreacion desc

		end

		if @i_estado = '-1'
		begin
			
			SELECT IdPedido
				,p.UsuarioCreacion AS IdCliente
				,c.NombreCliente AS Cliente				
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
				inner join Muestra m on o.IdOrden = m.IdOrden
				AND COALESCE(m.Eliminado,0) = 0
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
				INNER JOIN dbo.Muestra m on pm.IdMuestra = m.IdMuestra AND COALESCE(m.Eliminado,0) = 0
				WHERE o.IdPedido = p.IdPedido
				AND m.EstadoMuestra IN (SELECT IdCatalogoDetalle FROM dbo.CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoMues AND Valor IN ('RECT','RECB','ENTR'))
				GROUP BY m.IdMuestra
				) muestrasRetiradas
			) AS TotalRetiradas
			,p.FechaRetiro
			,(SELECT Nombre FROM CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoPedi and IdCatalogoDetalle = p.EstadoPedido) as EstadoPedido
			FROM [DbPortalHumalab].[dbo].[Pedido] p with (nolock)
			--INNER JOIN dbo.Cliente c ON c.IdOperadorLogistico = p.IdOperador --2024/07/30
			inner join Usuario u on u.idGalileo = p.UsuarioCreacion --2024/07/30
			inner join Cliente c on u.idUsuario = c.IdUsuario --2024/07/30
			WHERE p.FechaCreacion >= @i_fecha_desde 			
			--and c.CodClienteCta = @sucursal --nuevo 2024/07/30
			AND p.FechaCreacion < dateadd(DAY, 1, @i_fecha_hasta)			
			AND(p.EstadoPedido in (@idRec, @idRecP, @idPorRec))
			AND (
				@i_identificacion IS NULL
				OR c.Identificacion = @i_identificacion
			)
			AND (
				@i_nombre_cliente IS NULL
				OR c.NombreCliente LIKE @i_nombre_cliente
			)
			AND (
				(@i_op IS NULL AND p.UsuarioCreacion = @i_operador_logistico)
				OR IdOperador = @i_op
			)
			order by p.FechaCreacion desc

		end

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