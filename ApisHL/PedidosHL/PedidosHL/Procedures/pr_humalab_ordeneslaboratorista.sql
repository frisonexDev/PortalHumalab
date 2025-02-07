/************************************************************************
*	Stored procedure: pr_humalab_ordeneslaboratorista					*
*	Base de datos: DbPortalHumalab						  			    *
*	Elaborado por: José Guarnizo						                *
*----------------------------------------------------------------------	*
*					DESCRIPCION DEL PROCEDIMIENTO						*
*	En este Procedimiento consulta el catalogo detalle                   *
*	                                                                    *
*----------------------------------------------------------------------	*
*					BITACORA DE MODIFICACIONES							*
* FECHA AUTOR RAZON													    *
* 2024/08/27 José Guarnizo Se agrega campos de nombre de cliente y      *
*						   direccion del cliente para el excel masivo   *
* 2024/09/24 José Guarnizo Se modifica para que tambien envie el estado *
*						   de la orden y también para que no se duplique*
*						   la información de las ordenes del lab.       *
* 2024/10/17 José Guarnizo Se modifica para que valida tambien en base  *
*						   al cliente y obtenga el nombre del lab.		*
* 2024/10/18 José Guarnizo Se modifica para que salga ruc, nombre lab,  *
*						   tipo cliente, cod lab, fecha creacion y      *
*						   asesor.                                      *
* 2024/11/08 José Guarnizo Se modifica para que salga el estado de la   *
*						   prueba.										*
* 2024/12/17 José Guarnizo Se modifica para que tambien busque por		*
*						   operador.									*
*----------------------------------------------------------------------	*/
IF NOT EXISTS (SELECT * FROM  sys.procedures WHERE NAME = 'pr_humalab_ordeneslaboratorista')	
	EXEC('Create Procedure dbo.pr_humalab_ordeneslaboratorista As')
go

ALTER PROCEDURE [dbo].[pr_humalab_ordeneslaboratorista](
	@i_accion CHAR(2)
	,@i_fecha_desde DATE
	,@i_fecha_hasta DATE
	,@i_operador VARCHAR(100)
	,@i_es_ruc BIT
	,@i_cliente VARCHAR(200)
	,@i_estado VARCHAR(50)
	,@i_pedido INT
	,@i_orden INT
	,@i_codExterniLis varchar(100) = null
	,@i_idAsesor int = null
)

as

DECLARE @fechaActual DATETIME,
	@i_idEstadoOrd int,
	@i_idEstadoPed int,
	@i_idEstadoMues int,
	@i_idGenero int,
	@i_idEstadoPrue int,
	@i_idEstadoReco int,
	@i_idEstadoRecPar int,
	@i_idEstadoEnv int,
	@i_idEstadoEnvp int,
	@i_idEstadoRec int,
	@i_idEstadoRecp int,
	@idDniCliente int

SET @fechaActual = GETDATE()


select @i_idEstadoOrd = IdCatalogoMaestro
from CatalogoMaestro
where Nombre = 'EstadoOrden'

select @i_idEstadoPed = IdCatalogoMaestro
from CatalogoMaestro
where Nombre = 'EstadoPedido'

select @i_idEstadoMues = IdCatalogoMaestro
from CatalogoMaestro
where Nombre = 'EstadoMuestra'

select @i_idGenero = IdCatalogoMaestro
from CatalogoMaestro
where Nombre = 'Genero'

select @i_idEstadoPrue = IdCatalogoMaestro
from CatalogoMaestro
where Nombre = 'EstadoPrueba'

-------------------------
select @i_idEstadoReco = IdCatalogoDetalle
from CatalogoDetalle
where IdCatalogoMaestro = @i_idEstadoOrd
and Valor = 'RCTL'

select @i_idEstadoRecPar = IdCatalogoDetalle
from CatalogoDetalle
where IdCatalogoMaestro = @i_idEstadoOrd
and Valor = 'RCTP'

---------------------------
select @i_idEstadoEnv = IdCatalogoDetalle
FROM CatalogoDetalle
where IdCatalogoMaestro = @i_idEstadoOrd
and Valor = 'ENV'

select @i_idEstadoEnvp = IdCatalogoDetalle
from CatalogoDetalle
where IdCatalogoMaestro = @i_idEstadoOrd
and Valor = 'ENVP'

----------------------------

select @i_idEstadoRec = IdCatalogoDetalle
from CatalogoDetalle
where IdCatalogoMaestro = @i_idEstadoOrd
and Valor = 'RCBD'

select @i_idEstadoRecp = IdCatalogoDetalle
from CatalogoDetalle
where IdCatalogoMaestro = @i_idEstadoOrd
and Valor = 'RCBP'


IF @i_accion = 'I'
BEGIN
	RETURN 0
END
-- CONSULTAR PEDIDOS
ELSE IF @i_accion = 'C'
BEGIN
	DECLARE @nombre_operador VARCHAR(200), @ruc VARCHAR(200), @cliente VARCHAR(300), @estado INT
	DECLARE @ordenes TABLE(IdOrden INT, IdPedido INT, Resultados varchar(100), 
						   CodigoBarra VARCHAR(50), FechaCreacion DATETIME, UsuarioOperador VARCHAR(50), 
						   EstadoPedido VARCHAR(50), ObservacionMuestras VARCHAR(2000), 
						   IdentificacionPac varchar(20), NombresPac varchar(50), ClienteNombre varchar(100))
	IF @i_cliente IS NOT NULL AND @i_es_ruc = 1
		SET @ruc = @i_cliente
	ELSE
		SET @cliente = '%' + @i_cliente + '%'

	SET @nombre_operador = '%' + @i_operador + '%'

	IF @i_estado IS NOT NULL
	BEGIN
		
		if @i_estado = 'GENE'
		begin
			SET @estado = (SELECT TOP 1 IdCatalogoDetalle FROM dbo.CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoOrd and Valor = @i_estado)

			INSERT INTO @ordenes
			select o.IdOrden
				,o.IdPedido
				,o.Resultados
				,CodigoBarra
				,o.FechaCreacion
				,'' as UsuarioOperador
				,(SELECT cd.Nombre 
					FROM dbo.CatalogoDetalle cd 
					WHERE cd.IdCatalogoDetalle = o.Estado) AS EstadoPedido
				,(
					SELECT ISNULL(STRING_AGG(Descripcion,'¬'),'')
					FROM
						(SELECT DISTINCT ob.Descripcion 
						 FROM dbo.Prueba pr 
						 INNER JOIN dbo.PruebaMuestra pm ON pm.IdPrueba = pr.IdPrueba
						 INNER JOIN dbo.Muestra m ON m.IdMuestra = pm.IdMuestra
						 INNER JOIN dbo.ObservacionM ob ON ob.IdMuestra = m.IdMuestra
						 WHERE pr.IdOrden = o.IdOrden 
					AND COALESCE(ob.Eliminado, 0)<>1) A ) AS ObservacionMuestras,
				pa.Identificacion as IdentificacionPac,
				pa.Nombres+' '+pa.Apellidos as NombresPac,
				c.aux1 as ClienteNombre				
			from Orden o
			inner join Pedido p on o.IdPedido = p.IdPedido
			inner join Cliente c on p.IdOperador = c.IdOperadorLogistico
			inner join Paciente pa on o.Identificacion = pa.Identificacion			
			where o.Estado = @estado
			and c.IdCliente = p.IdCliente
			and (@i_idAsesor IS NULL OR p.IdOperador like @i_idAsesor)
			ORDER BY o.IdOrden, o.FechaCreacion desc
			
		end


		if (@i_estado != 'RCTL_RCPC' or @i_estado != 'ENV_ENVP' or @i_estado != 'RCBD_RCBP')
		begin

			SET @estado = (SELECT TOP 1 IdCatalogoDetalle FROM dbo.CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoOrd and Valor = @i_estado)

			INSERT INTO @ordenes
			SELECT o.IdOrden
				,o.IdPedido
				,o.Resultados
				,CodigoBarra
				,o.FechaCreacion
				,p.UsuarioOperador
				,(SELECT cd.Nombre FROM dbo.CatalogoDetalle cd WHERE cd.IdCatalogoDetalle = o.Estado) AS EstadoPedido
				,(
				SELECT ISNULL(STRING_AGG(Descripcion,'¬'),'')
				FROM
				(SELECT DISTINCT ob.Descripcion FROM dbo.Prueba pr INNER JOIN dbo.PruebaMuestra pm ON pm.IdPrueba = pr.IdPrueba
				INNER JOIN dbo.Muestra m ON m.IdMuestra = pm.IdMuestra
				INNER JOIN dbo.ObservacionM ob ON ob.IdMuestra = m.IdMuestra
				WHERE pr.IdOrden = o.IdOrden AND COALESCE(ob.Eliminado, 0)<>1) A ) AS ObservacionMuestras,
				pa.Identificacion as IdentificacionPac,
				pa.Nombres+' '+pa.Apellidos as NombresPac,
				c.aux1 as ClienteNombre				
				--INTO #TEMP1
			FROM dbo.Pedido p
			INNER JOIN dbo.Orden o ON p.IdPedido = o.IdPedido						
			INNER JOIN dbo.Cliente c ON c.IdOperadorLogistico = p.IdOperador --nuevo 11/01/2024	
			inner join dbo.Paciente pa on o.Identificacion = pa.Identificacion --04/04/2024
			--inner join dbo.Usuario u on o.IdUsuarioGalileo = u.idGalileo --24/09/2024
			--INNER JOIN dbo.Cliente c ON u.idUsuario = c.IdUsuario --24/09/2024
			WHERE p.EstadoPedido IN (SELECT IdCatalogoDetalle FROM dbo.CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoPed AND Valor IN ('ENV','ENVP', 'PREC', 'RCTL','RCPC')) AND
			p.FechaRetiro >= @i_fecha_desde AND p.FechaRetiro < dateadd(DAY, 1, @i_fecha_hasta)
			and c.IdCliente = p.IdCliente
			AND COALESCE(p.Eliminado, 0) = 0 AND COALESCE(o.Eliminado, 0) = 0
			AND (
				o.Estado = @estado)
			AND (
				@nombre_operador IS NULL
				OR p.UsuarioOperador LIKE @nombre_operador
			)
			AND (
				@ruc IS NULL
				OR c.Identificacion = @ruc
			)
			AND (
				@cliente IS NULL
				OR c.NombreCliente LIKE @cliente
				)
			and (@i_idAsesor IS NULL OR p.IdOperador like @i_idAsesor)
			group by o.IdOrden
				,o.IdPedido, o.Resultados
				,CodigoBarra, o.FechaCreacion
				,p.UsuarioOperador, o.Estado, 
				p.IdPedido, p.FechaCreacion, pa.Identificacion,
				pa.Nombres, pa.Apellidos, c.aux1
			ORDER BY p.IdPedido, o.IdOrden, p.FechaCreacion desc

		end

		if @i_estado = 'RCTL_RCPC'
		begin

			INSERT INTO @ordenes
			SELECT o.IdOrden
				,o.IdPedido
				,o.Resultados
				,CodigoBarra
				,o.FechaCreacion
				,p.UsuarioOperador
				,(SELECT cd.Nombre FROM dbo.CatalogoDetalle cd WHERE cd.IdCatalogoDetalle = o.Estado) AS EstadoPedido
				,(
				SELECT ISNULL(STRING_AGG(Descripcion,'¬'),'')
				FROM
				(SELECT DISTINCT ob.Descripcion FROM dbo.Prueba pr INNER JOIN dbo.PruebaMuestra pm ON pm.IdPrueba = pr.IdPrueba
				INNER JOIN dbo.Muestra m ON m.IdMuestra = pm.IdMuestra
				INNER JOIN dbo.ObservacionM ob ON ob.IdMuestra = m.IdMuestra
				WHERE pr.IdOrden = o.IdOrden AND COALESCE(ob.Eliminado, 0)<>1) A ) AS ObservacionMuestras,
				pa.Identificacion as IdentificacionPac,
				pa.Nombres+' '+pa.Apellidos as NombresPac,
				c.aux1 as ClienteNombre				
				--INTO #TEMP1
			FROM dbo.Pedido p
			INNER JOIN dbo.Orden o ON p.IdPedido = o.IdPedido						
			INNER JOIN dbo.Cliente c ON c.IdOperadorLogistico = p.IdOperador --nuevo 11/01/2024	
			inner join dbo.Paciente pa on o.Identificacion = pa.Identificacion --04/04/2024
			--inner join dbo.Usuario u on o.IdUsuarioGalileo = u.idGalileo --24/09/2024
			--INNER JOIN dbo.Cliente c ON u.idUsuario = c.IdUsuario --24/09/2024
			WHERE p.EstadoPedido IN (SELECT IdCatalogoDetalle FROM dbo.CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoPed AND Valor IN ('ENV','ENVP', 'PREC', 'RCTL','RCPC')) AND
			p.FechaRetiro >= @i_fecha_desde AND p.FechaRetiro < dateadd(DAY, 1, @i_fecha_hasta)
			and c.IdCliente = p.IdCliente
			AND COALESCE(p.Eliminado, 0) = 0 AND COALESCE(o.Eliminado, 0) = 0
			AND (
				o.Estado in (@i_idEstadoReco, @i_idEstadoRecPar))
			AND (
				@nombre_operador IS NULL
				OR p.UsuarioOperador LIKE @nombre_operador
			)
			AND (
				@ruc IS NULL
				OR c.Identificacion = @ruc
			)
			AND (
				@cliente IS NULL
				OR c.NombreCliente LIKE @cliente
				)
			and (@i_idAsesor IS NULL OR p.IdOperador like @i_idAsesor)
			group by o.IdOrden
				,o.IdPedido, o.Resultados
				,CodigoBarra, o.FechaCreacion
				,p.UsuarioOperador, o.Estado, 
				p.IdPedido, p.FechaCreacion, pa.Identificacion,
				pa.Nombres, pa.Apellidos, c.aux1
			ORDER BY p.IdPedido, o.IdOrden, p.FechaCreacion desc

		end

		if @i_estado = 'ENV_ENVP'
		begin

			INSERT INTO @ordenes
			SELECT o.IdOrden
				,o.IdPedido
				,o.Resultados
				,CodigoBarra
				,o.FechaCreacion
				,p.UsuarioOperador
				,(SELECT cd.Nombre FROM dbo.CatalogoDetalle cd WHERE cd.IdCatalogoDetalle = o.Estado) AS EstadoPedido
				,(
				SELECT ISNULL(STRING_AGG(Descripcion,'¬'),'')
				FROM
				(SELECT DISTINCT ob.Descripcion FROM dbo.Prueba pr INNER JOIN dbo.PruebaMuestra pm ON pm.IdPrueba = pr.IdPrueba
				INNER JOIN dbo.Muestra m ON m.IdMuestra = pm.IdMuestra
				INNER JOIN dbo.ObservacionM ob ON ob.IdMuestra = m.IdMuestra
				WHERE pr.IdOrden = o.IdOrden AND COALESCE(ob.Eliminado, 0)<>1) A ) AS ObservacionMuestras,
				pa.Identificacion as IdentificacionPac,
				pa.Nombres+' '+pa.Apellidos as NombresPac,
				c.aux1 as ClienteNombre				
				--INTO #TEMP1
			FROM dbo.Pedido p
			INNER JOIN dbo.Orden o ON p.IdPedido = o.IdPedido						
			INNER JOIN dbo.Cliente c ON c.IdOperadorLogistico = p.IdOperador --nuevo 11/01/2024		
			inner join dbo.Paciente pa on o.Identificacion = pa.Identificacion --04/04/2024
			--inner join dbo.Usuario u on o.IdUsuarioGalileo = u.idGalileo --24/09/2024
			--INNER JOIN dbo.Cliente c ON u.idUsuario = c.IdUsuario --24/09/2024
			WHERE p.EstadoPedido IN (SELECT IdCatalogoDetalle FROM dbo.CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoPed AND Valor IN ('ENV','ENVP', 'PREC', 'RCTL','RCPC')) AND
			p.FechaRetiro >= @i_fecha_desde AND p.FechaRetiro < dateadd(DAY, 1, @i_fecha_hasta)
			and c.IdCliente = p.IdCliente
			AND COALESCE(p.Eliminado, 0) = 0 AND COALESCE(o.Eliminado, 0) = 0
			AND (
				o.Estado in (@i_idEstadoEnv, @i_idEstadoEnvp))
			AND (
				@nombre_operador IS NULL
				OR p.UsuarioOperador LIKE @nombre_operador
			)
			AND (
				@ruc IS NULL
				OR c.Identificacion = @ruc
			)
			AND (
				@cliente IS NULL
				OR c.NombreCliente LIKE @cliente
				)
			and (@i_idAsesor IS NULL OR p.IdOperador like @i_idAsesor)
			group by o.IdOrden
				,o.IdPedido, o.Resultados
				,CodigoBarra, o.FechaCreacion
				,p.UsuarioOperador, o.Estado, 
				p.IdPedido, p.FechaCreacion, pa.Identificacion,
				pa.Nombres, pa.Apellidos, c.aux1
			ORDER BY p.IdPedido, o.IdOrden, p.FechaCreacion desc

		end

		if  @i_estado = 'RCBD_RCBP'
		begin
			
			INSERT INTO @ordenes
			SELECT o.IdOrden
				,o.IdPedido
				,o.Resultados
				,CodigoBarra
				,o.FechaCreacion
				,p.UsuarioOperador
				,(SELECT cd.Nombre FROM dbo.CatalogoDetalle cd WHERE cd.IdCatalogoDetalle = o.Estado) AS EstadoPedido
				,(
				SELECT ISNULL(STRING_AGG(Descripcion,'¬'),'')
				FROM
				(SELECT DISTINCT ob.Descripcion FROM dbo.Prueba pr INNER JOIN dbo.PruebaMuestra pm ON pm.IdPrueba = pr.IdPrueba
				INNER JOIN dbo.Muestra m ON m.IdMuestra = pm.IdMuestra
				INNER JOIN dbo.ObservacionM ob ON ob.IdMuestra = m.IdMuestra
				WHERE pr.IdOrden = o.IdOrden AND COALESCE(ob.Eliminado, 0)<>1) A ) AS ObservacionMuestras,
				pa.Identificacion as IdentificacionPac,
				pa.Nombres+' '+pa.Apellidos as NombresPac,
				c.aux1 as ClienteNombre				
				--INTO #TEMP1
			FROM dbo.Pedido p
			INNER JOIN dbo.Orden o ON p.IdPedido = o.IdPedido						
			INNER JOIN dbo.Cliente c ON c.IdOperadorLogistico = p.IdOperador --nuevo 11/01/2024	
			inner join dbo.Paciente pa on o.Identificacion = pa.Identificacion --04/04/2024
			--inner join dbo.Usuario u on o.IdUsuarioGalileo = u.idGalileo --24/09/2024
			--INNER JOIN dbo.Cliente c ON u.idUsuario = c.IdUsuario --24/09/2024
			WHERE p.EstadoPedido IN (SELECT IdCatalogoDetalle FROM dbo.CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoPed AND Valor IN ('ENV','ENVP', 'PREC', 'RCTL','RCPC')) AND
			p.FechaRetiro >= @i_fecha_desde AND p.FechaRetiro < dateadd(DAY, 1, @i_fecha_hasta)
			and c.IdCliente = p.IdCliente
			AND COALESCE(p.Eliminado, 0) = 0 AND COALESCE(o.Eliminado, 0) = 0
			AND (
				o.Estado in (@i_idEstadoRec, @i_idEstadoRecp))
			AND (
				@nombre_operador IS NULL
				OR p.UsuarioOperador LIKE @nombre_operador
			)
			AND (
				@ruc IS NULL
				OR c.Identificacion = @ruc
			)
			AND (
				@cliente IS NULL
				OR c.NombreCliente LIKE @cliente
				)
			and (@i_idAsesor IS NULL OR p.IdOperador like @i_idAsesor)
			group by o.IdOrden
				,o.IdPedido, o.Resultados
				,CodigoBarra, o.FechaCreacion
				,p.UsuarioOperador, o.Estado, 
				p.IdPedido, p.FechaCreacion, pa.Identificacion,
				pa.Nombres, pa.Apellidos, c.aux1
			ORDER BY p.IdPedido, o.IdOrden, p.FechaCreacion desc
			
		end
		
	END
	ELSE
	BEGIN
		INSERT INTO @ordenes
		SELECT o.IdOrden
		,o.IdPedido
		,o.Resultados
		,CodigoBarra
		,o.FechaCreacion
		,p.UsuarioOperador
		,(SELECT cd.Nombre FROM dbo.CatalogoDetalle cd WHERE cd.IdCatalogoDetalle = o.Estado) AS EstadoPedido
		,(
		SELECT ISNULL(STRING_AGG(Descripcion,'¬'),'')
		FROM
		(SELECT DISTINCT ob.Descripcion FROM dbo.Prueba pr INNER JOIN dbo.PruebaMuestra pm ON pm.IdPrueba = pr.IdPrueba
		INNER JOIN dbo.Muestra m ON m.IdMuestra = pm.IdMuestra
		INNER JOIN dbo.ObservacionM ob ON ob.IdMuestra = m.IdMuestra
		WHERE pr.IdOrden = o.IdOrden AND COALESCE(ob.Eliminado, 0)<>1) A ) AS ObservacionMuestras,
		o.Identificacion as IdentificacionPac,
		pa.Nombres+' '+pa.Apellidos as NombresPac,
		c.aux1 as ClienteNombre		
		--INTO #TEMP2
		FROM dbo.Pedido p
		INNER JOIN dbo.Orden o ON p.IdPedido = o.IdPedido				
		INNER JOIN dbo.Cliente c ON c.IdOperadorLogistico = p.IdOperador --nuevo 11/01/2024		
		inner join dbo.Paciente pa on pa.Identificacion = o.Identificacion --04/04/2024
		--inner join dbo.Usuario u on o.IdUsuarioGalileo = u.idGalileo --24/09/2024
		--INNER JOIN dbo.Cliente c ON u.idUsuario = c.IdUsuario --24/09/2024
		WHERE p.EstadoPedido IN (SELECT IdCatalogoDetalle FROM dbo.CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoPed AND Valor IN ('ENV','ENVP', 'PREC', 'RCTL','RCPC')) AND
		p.FechaRetiro >= @i_fecha_desde AND p.FechaRetiro < dateadd(DAY, 1, @i_fecha_hasta)
		and c.IdCliente = p.IdCliente
		AND COALESCE(p.Eliminado, 0) = 0 AND COALESCE(o.Eliminado, 0) = 0
		AND (			
			o.Estado IN (SELECT IdCatalogoDetalle FROM CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoOrd AND Valor IN ('RCTL','RCTP',
																															  'PREC', 'RCHZ', 
																															  'ANLS', 'VALD', 
																															  'FACT', 'RCBD',
																															  'RCBP', 'ENV', 
																															  'ENVP', 'RESTP', 
																															  'VALDP', 'GENE'))
			)
		AND (
			@nombre_operador IS NULL
			OR p.UsuarioOperador LIKE @nombre_operador
			)
		AND (
			@ruc IS NULL
			OR c.Identificacion = @ruc
			)
		AND (
			@cliente IS NULL
			OR c.NombreCliente LIKE @cliente
			)
		and (@i_idAsesor IS NULL OR p.IdOperador like @i_idAsesor)
		group by o.IdOrden
			,o.IdPedido, o.Resultados
			,CodigoBarra, o.FechaCreacion
			,p.UsuarioOperador, o.Estado, 
			p.IdPedido, p.FechaCreacion, o.Identificacion,
			pa.Nombres, pa.Apellidos, c.aux1
		ORDER BY p.IdPedido, o.IdOrden, p.FechaCreacion desc
	END


	SELECT * FROM @ordenes
	
	SELECT 
	(
		SELECT COUNT(*) FROM 
		(SELECT m.IdMuestra
		FROM @ordenes t INNER JOIN Prueba pr on pr.IdOrden = t.IdOrden
		INNER JOIN PruebaMuestra pm ON pm.IdPrueba = pr.IdPrueba
		INNER JOIN Muestra m ON m.IdMuestra = pm.IdMuestra
		WHERE COALESCE(m.Eliminado, 0) <> 1
		AND m.EstadoMuestra IN (SELECT IdCatalogoDetalle FROM dbo.CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoMues AND Valor IN ('RECT','RECB'))
		GROUP BY m.IdMuestra) A
		) AS TotalMuestrasEntregadas
	,(
	SELECT COUNT(*) FROM 
	(SELECT m.IdMuestra 
	FROM @ordenes t INNER JOIN Prueba pr on pr.IdOrden = t.IdOrden
	INNER JOIN PruebaMuestra pm ON pm.IdPrueba = pr.IdPrueba
	INNER JOIN Muestra m ON m.IdMuestra = pm.IdMuestra
	WHERE COALESCE(m.Eliminado, 0) <> 1
	AND m.EstadoMuestra = (SELECT IdCatalogoDetalle FROM dbo.CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoMues AND Valor = 'RECB')
	GROUP BY m.IdMuestra) A
	) AS TotalMuestrasRecibidas

	,(	SELECT COUNT(*) FROM 
	(SELECT m.IdMuestra
	FROM @ordenes t INNER JOIN Prueba pr on pr.IdOrden = t.IdOrden
	INNER JOIN PruebaMuestra pm ON pm.IdPrueba = pr.IdPrueba
	INNER JOIN Muestra m ON m.IdMuestra = pm.IdMuestra
	WHERE pr.IdOrden = t.IdOrden AND COALESCE(m.Eliminado, 0) <> 1
	AND m.EstadoMuestra in (SELECT IdCatalogoDetalle FROM dbo.CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoMues AND Valor IN ('RCHO','RCHL'))
	GROUP BY m.IdMuestra) A
	) AS TotalMuestrasRechazadas

	,(SELECT DISTINCT COUNT(pr.IdPrueba)
	FROM @ordenes t INNER JOIN Prueba pr on pr.IdOrden = t.IdOrden
	LEFT JOIN PruebaMuestra pm ON pm.IdPrueba = pr.IdPrueba
	WHERE pr.IdOrden = t.IdOrden AND pm.IdPrueba IS NULL --Pruebas sin muestras
	)  AS TotalSinMuestra



END
ELSE IF @i_accion = 'CD'
BEGIN
	
	declare @cedula varchar(100),
			@genero bit

	select @cedula = Identificacion
	from Orden
	where IdOrden = @i_orden

	select @genero = Genero
	from Paciente
	where Identificacion = @cedula

	if @genero = 1
	begin
		select @genero = 0
	end
	else
	begin
		select @genero = 1
	end
	
	--Consulta cabecera de Orden Laboratorista
	SELECT (SELECT Nombre FROM dbo.CatalogoDetalle WHERE IdCatalogoMaestro = @i_idGenero AND Valor = @genero) AS Genero
	,FechaNacimiento
	,Edad
	,Diagnostico
	,Medicamento
	,Observacion
	,p.Nombres + ' ' + p.Apellidos as NombresPaciente
	,p.Identificacion --nuevo
	,(SELECT ob.Descripcion FROM Observacion ob INNER JOIN PedidoObservacion pob ON ob.IdObservacion = pob.IdObservacion AND pob.IdPedido = o.IdPedido) AS ObservacionCliente
	,(SELECT ObservacionOpLogistico FROM Pedido pd WHERE pd.IdPedido = o.IdPedido) AS ObservacionOpLogistico
	,(select FechaRetiro from Pedido pd where pd.IdPedido = o.IdPedido) as FechaEnvio --verficar prueba	
	,(select aux1 from Cliente cl
	  inner join Usuario us on cl.IdUsuario = us.idUsuario
	  inner join Orden o on us.idGalileo = o.IdUsuarioGalileo
	  where o.IdOrden = @i_orden) as NombreCliente
	,(select Ciudad from ClienteDireccion cd
	  inner join Cliente cl on cd.IdCliente = cl.IdCliente
	  inner join Usuario us on cl.IdUsuario = us.idUsuario
      inner join Orden o on us.idGalileo = o.IdUsuarioGalileo
	  where o.IdOrden = @i_orden) as CiudadCliente
	,(select u.Identificacion from Usuario u
	  inner join Orden o on u.idGalileo = o.IdUsuarioGalileo
	  where o.IdOrden = @i_orden) as RucLab
	,(select NombreOperadorLogistico from Cliente c
	  inner join Usuario us on c.IdUsuario = us.idUsuario
	  inner join Orden o on us.idGalileo = o.IdUsuarioGalileo
	  where o.IdOrden = @i_orden) as Operador
	,(select NombreCliente from Cliente cl
	  inner join Usuario us on cl.IdUsuario = us.idUsuario
	  inner join Orden o on us.idGalileo = o.IdUsuarioGalileo
	  where o.IdOrden = @i_orden) as ClienteNombre
	,p.CodLaboratorio
	,(select Nombre from CatalogoDetalle 
	  where IdCatalogoDetalle = p.TipoPaciente) as TipoPaciente
	FROM Paciente p INNER JOIN Orden o ON p.Identificacion = o.Identificacion	
	WHERE O.IdOrden = @i_orden
	
	--Consulta detalle pruebas/muestras por orden Laboratorista
	SELECT 
	pr.IdOrden
	,pr.IdPrueba
	,IdPruebaGalileo AS Codigo
	,pr.Nombre AS PruebaPerfil
	,pr.CodigoExamen as CodigoExamen
	,ISNULL(
        CASE 
            WHEN (SELECT Valor FROM CatalogoDetalle cd WHERE cd.IdCatalogoMaestro = @i_idEstadoPrue AND cd.IdCatalogoDetalle = pr.Estado) IN ('RCHZ')  THEN CAST(1 AS BIT)
            ELSE CAST(0 AS BIT)
        END, CAST(0 AS BIT)
    ) AS PruebaRechazada
	,pr.Observacion AS ObservacionPrueba
	,m.Nombre AS Muestra
	,m.CodigoBarra
	,m.IdMuestra
	,ISNULL(
            CASE 
                WHEN (SELECT Valor FROM CatalogoDetalle cd WHERE cd.IdCatalogoMaestro = @i_idEstadoMues AND cd.IdCatalogoDetalle = m.EstadoMuestra) IN ('RECB')  THEN CAST(1 AS BIT)
                WHEN (SELECT Valor FROM CatalogoDetalle cd WHERE cd.IdCatalogoMaestro = @i_idEstadoMues AND cd.IdCatalogoDetalle = m.EstadoMuestra) IN ('RECT') THEN CAST(0 AS BIT)
            END, CAST(0 AS BIT)
        ) AS Recibido,
        ISNULL(
            CASE 
                WHEN (SELECT Valor  FROM CatalogoDetalle cd WHERE cd.IdCatalogoMaestro = @i_idEstadoMues AND cd.IdCatalogoDetalle = m.EstadoMuestra) IN ('RCHO','RCHL') THEN CAST(1 AS BIT)
                WHEN (SELECT Valor  FROM CatalogoDetalle cd WHERE cd.IdCatalogoMaestro = @i_idEstadoMues AND cd.IdCatalogoDetalle = m.EstadoMuestra) IN ('RECT') THEN CAST(0 AS BIT)
            END, CAST(0 AS BIT)
        ) AS Rechazado
	,(SELECT om.Descripcion FROM dbo.ObservacionM om WHERE om.IdMuestra = m.IdMuestra AND Operador = 0 AND COALESCE(om.Eliminado,0) = 0) AS ObservacionMuestra --ob.Descripcion
	,(SELECT Nombre FROM dbo.CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoMues AND IdCatalogoDetalle = M.EstadoMuestra) AS EstadoMuestra
	,(select Nombre from dbo.CatalogoDetalle where IdCatalogoMaestro = @i_idEstadoOrd and IdCatalogoDetalle = o.Estado) as EstadoOrden
	,o.FechaCreacion
	,o.Resultados as CodLis
	,(select Nombre from dbo.CatalogoDetalle where IdCatalogoMaestro = @i_idEstadoPrue and IdCatalogoDetalle = pr.Estado) as EstadoPrueba --08/11/2024
	FROM Orden o INNER JOIN Prueba pr ON pr.IdOrden = o.IdOrden AND COALESCE(pr.Eliminado,0) = 0
	LEFT JOIN PruebaMuestra pm ON pr.IdPrueba = pm.IdPrueba AND COALESCE(pm.Eliminado,0) = 0
	LEFT JOIN Muestra m ON m.IdMuestra = pm.IdMuestra AND COALESCE(m.Eliminado,0) = 0
	WHERE o.IdOrden = @i_orden

END
ELSE IF @i_accion = 'M'
BEGIN
	
BEGIN TRANSACTION;

BEGIN TRY

	DECLARE @muestras_totales INT
			,@muestras_aceptadas INT
			,@muestras_rechazadas INT
			,@estado_orden INT

		

		SET @muestras_totales = (
				SELECT count(*)
				FROM (
					SELECT m.IdMuestra
					FROM dbo.Prueba pr
					INNER JOIN dbo.Orden o ON o.IdOrden = pr.IdOrden
					INNER JOIN dbo.PruebaMuestra pm ON pr.IdPrueba = pm.IdPrueba
					INNER JOIN dbo.Muestra m on pm.IdMuestra = m.IdMuestra
					WHERE o.IdOrden = @i_orden
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
					WHERE o.IdOrden = @i_orden AND m.EstadoMuestra = (SELECT IdCatalogoDetalle FROM dbo.CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoMues AND Valor = 'RECB')
					GROUP BY m.IdMuestra
					) a
				)
				
		IF @muestras_totales = @muestras_aceptadas			
			SET @estado_orden = (SELECT IdCatalogoDetalle FROM CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoOrd AND Valor = 'RCBD')
		ELSE			
			SET @estado_orden = (SELECT IdCatalogoDetalle FROM CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoOrd AND Valor = 'RCBP')

		UPDATE [dbo].Orden
		SET Estado = @estado_orden, Resultados = @i_codExterniLis
		,Observacion = 'Orden ' + (SELECT Nombre FROM CatalogoDetalle WHERE IdCatalogoMaestro = @i_idEstadoOrd AND IdCatalogoDetalle = @estado_orden)
			,[UsuarioModificacion] = @i_operador
			,[FechaModificacion] = @fechaActual
		WHERE IdOrden = @i_orden
		COMMIT;
		END TRY
		BEGIN CATCH
		    -- En caso de error, deshacer la transacción
			ROLLBACK;

			SELECT @i_orden AS IdOrden, 'Error en la transacción: ' + ERROR_MESSAGE() AS Mensaje
		END CATCH;

	SELECT @i_orden AS IdOrden, 'Orden enviada exitosamente' AS Mensaje

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