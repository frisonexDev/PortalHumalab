/************************************************************************
*	Stored procedure: pr_humalab_listarordenes							*
*	Base de datos: DbPortalHumalab						  			    *
*	Elaborado por: José Guarnizo							            *
*----------------------------------------------------------------------	*
*					DESCRIPCION DEL PROCEDIMIENTO						*
*	En este Procedimiento lista ordenes                                 *
*	                                                                    *
*----------------------------------------------------------------------	*
*					BITACORA DE MODIFICACIONES							*
*	FECHA AUTOR RAZON													*
*	2024/09/24 Jose Guarnizo Se cambia para que traiga las ordenes por  *
*							 usuario individual es decir sus ordenes.   *
*----------------------------------------------------------------------	*/
IF NOT EXISTS (SELECT * FROM  sys.procedures WHERE NAME = 'pr_humalab_listarordenes')	
	EXEC('Create Procedure dbo.pr_humalab_listarordenes As')
go

ALTER PROCEDURE [dbo].[pr_humalab_listarordenes](
	@i_accion CHAR,
	@idUsuarioGalileo INT,
	@opcionBusqueda INT=NULL,
	@opcionEstado varchar(100)=null,
	@datoBusqueda VARCHAR(50)=NULL,
	@idOrden INT=NULL,
	@codigoBarra Varchar(20)=NULL,
	@fechaInicio DATE=NULL,
	@fechaFin DATE = NULL
)

as

declare @estadoEliminado AS INT =1,
		@fechaDefault AS DATE='1/1/0001 0:00:00',
		@estadoGenerado AS VARCHAR(10) ='Generada',
		@estadoCero AS INT=0,
		@estadoUno AS INT =1,
		@estadoDos AS INT =2,
		@estadoTres AS INT =3,
		@estadoCuatro AS INT =4,
		@estadoCinco AS INT =5,
		@estadoSeis AS INT =6,
		@estadoSiete AS INT =7,
		@estadoOcho AS INT =8,
		@estadoCatalogoM int,		
		@estadoDiez as int = 10, --Recolectado total/parcial
		@estadoOnce as int = 11, --Enviado total/parcial
		@estadoDoce as int = 12, --Recibida/parcial
		@i_EstadoId int,
		@estadoTrece as int = 13, --todas las ordenes
		@i_idCance as int, --canceladas
		@i_idGenen as int,
		@idenCliente as varchar(20),
		@sucursal varchar(100)

select @estadoCatalogoM = IdCatalogoMaestro
from CatalogoMaestro 
where Nombre = 'EstadoOrden'

--estados general
select @i_EstadoId = IdCatalogoDetalle
from CatalogoDetalle
where IdCatalogoMaestro = @estadoCatalogoM
and Valor = @opcionEstado

select @i_idCance = IdCatalogoDetalle
from CatalogoDetalle
where Valor = 'CANC'
and IdCatalogoMaestro = @estadoCatalogoM

select @i_idGenen = IdCatalogoDetalle
from CatalogoDetalle
where Valor = 'GENE'
and IdCatalogoMaestro = @estadoCatalogoM

select @idenCliente = Identificacion
from Usuario
where idGalileo = @idUsuarioGalileo

--nuevo 2024/01/24
select @sucursal = CodClienteCta
from Usuario
where idGalileo = @idUsuarioGalileo

IF(@i_accion='C')
Begin
	
	IF(@opcionBusqueda= @estadoCero)
	BEGIN
			Select /*top 25*/ O.IdOrden AS 'NOrden', O.CodigoBarra AS 'CodigoBarra', O.FechaCreacion AS 'FechaIngreso', P.Nombres+' '+P.Apellidos AS 'NombrePaciente', CD.Nombre AS 'Estado',
				SUM(Pr.Precio) AS 'Precio', 
				O.Observacion AS 'Observacion',
				O.Resultados as 'CodigoGalileo'
			From Orden O
			INNER JOIN Paciente P ON O.Identificacion = P.Identificacion
			INNER JOIN Prueba Pr ON O.IdOrden = Pr.IdOrden
			INNER JOIN CatalogoMaestro CM ON CM.IdCatalogoMaestro=@estadoCatalogoM
			INNER JOIN CatalogoDetalle CD ON CD.IdCatalogoMaestro = CM.IdCatalogoMaestro
			inner join Usuario US ON O.IdUsuarioGalileo = US.idGalileo --nuevo			
			--where cast(O.FechaCreacion as date) = CAST(GETDATE() AS DATE) 
			WHERE O.FechaCreacion >= CONVERT(DATETIME, CONVERT(DATE, GETDATE())) 
			AND O.FechaCreacion < CONVERT(DATETIME, DATEADD(DAY, 1, CONVERT(DATE, GETDATE())))
			AND CD.Nombre=@estadoGenerado 
			AND O.Estado = CD.IdCatalogoDetalle
			--and US.Identificacion = @idenCliente
			--and US.CodClienteCta = @sucursal --nuevo 24/01/2024
			and US.idGalileo = @idUsuarioGalileo --nuevo 24/09/2024
			and Pr.Eliminado != 1 --nuevo 11/01/2024
			and O.Eliminado !=1 --nuevo 24/01/2024
			GROUP BY O.IdOrden, O.CodigoBarra,P.Nombres, P.Apellidos, O.FechaCreacion, Cd.Nombre, 
				O.Observacion, O.Resultados
			order by O.FechaCreacion desc
	END

	--Busqueda por numero de orden
	IF(@opcionBusqueda = @estadoUno)
	BEGIN
			select @fechaFin = DATEADD(DAY, 1, @fechaFin)

			SELECT O.IdOrden AS 'NOrden',O.CodigoBarra AS 'CodigoBarra', O.FechaCreacion AS 'FechaIngreso', P.Nombres+' '+P.Apellidos AS 'NombrePaciente', CD.Nombre AS 'Estado', 
			SUM(Pr.Precio)AS'Precio' ,O.Observacion AS 'Observacion', O.Resultados as 'CodigoGalileo'  
			FROM Orden O
			INNER JOIN Paciente P ON O.Identificacion = P.Identificacion
			INNER JOIN Prueba Pr ON O.IdOrden = Pr.IdOrden
			inner join CatalogoDetalle CD ON O.Estado = CD.IdCatalogoDetalle
			inner join Usuario US ON O.IdUsuarioGalileo = US.idGalileo --nuevo
			--WHERE O.IdUsuarioGalileo = @idUsuarioGalileo 
			where  O.CodigoBarra=@codigoBarra
			AND CD.IdCatalogoDetalle = @i_EstadoId
			--and US.Identificacion = @idenCliente
			--and US.CodClienteCta = @sucursal --nuevo 24/01/2024
			and US.idGalileo = @idUsuarioGalileo --nuevo 24/09/2024
			AND O.FechaCreacion BETWEEN @fechaInicio AND @fechaFin
			and Pr.Eliminado != 1 --nuevo 11/01/2024
			GROUP BY O.IdOrden, O.CodigoBarra,P.Nombres, P.Apellidos, O.FechaCreacion, Cd.Nombre, 
			O.Observacion, O.Resultados
			order by O.FechaCreacion desc

	END

	--Busqueda por nombre paciente
	IF(@opcionBusqueda = @estadoDos)
	BEGIN			
			select @fechaFin = DATEADD(DAY, 1, @fechaFin)

			IF @datoBusqueda != null or @datoBusqueda != ''
			begin
				SELECT O.IdOrden AS 'NOrden',O.CodigoBarra AS 'CodigoBarra', O.FechaCreacion AS 'FechaIngreso', P.Nombres+' '+P.Apellidos AS 'NombrePaciente', CD.Nombre AS 'Estado',SUM(Pr.Precio)AS'Precio' , O.Observacion AS 'Observacion', O.Resultados as 'CodigoGalileo'   FROM Orden O 
				INNER JOIN Paciente P ON O.Identificacion = P.Identificacion
				INNER JOIN Prueba Pr ON O.IdOrden = Pr.IdOrden
				inner join CatalogoDetalle CD ON O.Estado = CD.IdCatalogoDetalle
				inner join Usuario US ON O.IdUsuarioGalileo = US.idGalileo --nuevo
				WHERE (
					(P.Nombres + p.Apellidos like '%' + @datoBusqueda + '%')
					OR P.Identificacion = @datoBusqueda --2024/11/21
				) 
				--AND O.IdUsuarioGalileo = @idUsuarioGalileo
				--AND CD.IdCatalogoDetalle = @i_EstadoId --2024/11/21
				--and US.Identificacion = @idenCliente --nuevo
				--and US.CodClienteCta = @sucursal --nuevo 24/01/2024
				and US.idGalileo = @idUsuarioGalileo --nuevo 24/09/2024
				AND O.FechaCreacion BETWEEN @fechaInicio AND @fechaFin
				and Pr.Eliminado != 1 --nuevo 11/01/2024
				GROUP BY O.IdOrden, O.CodigoBarra,P.Nombres, P.Apellidos, O.FechaCreacion, Cd.Nombre, 
					O.Observacion, O.Resultados
				order by O.FechaCreacion desc
			end
			else
			begin
				
				SELECT O.IdOrden AS 'NOrden',O.CodigoBarra AS 'CodigoBarra', O.FechaCreacion AS 'FechaIngreso', P.Nombres+' '+P.Apellidos AS 'NombrePaciente', CD.Nombre AS 'Estado',SUM(Pr.Precio)AS'Precio' , O.Observacion AS 'Observacion',O.Resultados as 'CodigoGalileo'  FROM Orden O 
				INNER JOIN Paciente P ON O.Identificacion = P.Identificacion
				INNER JOIN Prueba Pr ON O.IdOrden = Pr.IdOrden
				inner join CatalogoDetalle CD ON O.Estado = CD.IdCatalogoDetalle
				inner join Usuario US ON O.IdUsuarioGalileo = US.idGalileo --nuevo
				WHERE CD.IdCatalogoDetalle = @i_EstadoId
				--and US.Identificacion = @idenCliente --nuevo
				--and US.CodClienteCta = @sucursal --nuevo 24/01/2024
				and US.idGalileo = @idUsuarioGalileo --nuevo 24/09/2024
				AND O.FechaCreacion BETWEEN @fechaInicio AND @fechaFin
				and Pr.Eliminado != 1 --nuevo 11/01/2024
				GROUP BY O.IdOrden, O.CodigoBarra,P.Nombres, P.Apellidos, O.FechaCreacion, Cd.Nombre, 
				O.Observacion, O.Resultados
				order by O.FechaCreacion desc

			end

	END

	--Busqueda por cedula paciente
	IF(@opcionBusqueda =@estadoTres)
	BEGIN
			select @fechaFin = DATEADD(DAY, 1, @fechaFin)

			SELECT O.IdOrden AS 'NOrden',O.CodigoBarra AS 'CodigoBarra', O.FechaCreacion AS 'FechaIngreso', 
				P.Nombres+' '+P.Apellidos AS 'NombrePaciente', CD.Nombre AS 'Estado',
				SUM(Pr.Precio)AS'Precio' , O.Observacion AS 'Observacion',O.Resultados as 'CodigoGalileo'  FROM Orden O 
			INNER JOIN Paciente P ON O.Identificacion = P.Identificacion
			INNER JOIN Prueba Pr ON O.IdOrden = Pr.IdOrden
			inner join CatalogoDetalle CD ON O.Estado = CD.IdCatalogoDetalle
			inner join Usuario US ON O.IdUsuarioGalileo = US.idGalileo --nuevo
			WHERE P.Identificacion = @datoBusqueda 
			--AND O.IdUsuarioGalileo = @idUsuarioGalileo
			AND CD.IdCatalogoDetalle = @i_EstadoId
			--and US.Identificacion = @idenCliente --nuevo
			--and US.CodClienteCta = @sucursal --nuevo 24/01/2024
			and US.idGalileo = @idUsuarioGalileo --nuevo 24/09/2024
			AND O.FechaCreacion BETWEEN @fechaInicio AND @fechaFin
			and Pr.Eliminado != 1 --nuevo 11/01/2024
			GROUP BY O.IdOrden, O.CodigoBarra,P.Nombres, P.Apellidos, O.FechaCreacion, Cd.Nombre, 
			O.Observacion, O.Resultados
			order by O.FechaCreacion desc

	END

	IF(@opcionBusqueda = @estadoCuatro)
	BEGIN
			SELECT O.IdOrden AS 'NOrden',O.CodigoBarra AS 'CodigoBarra', O.FechaCreacion AS 'FechaIngreso', 
				P.Nombres+' '+P.Apellidos AS 'NombrePaciente', CD.Nombre AS 'Estado',
				SUM(Pr.Precio)AS'Precio' , O.Observacion AS 'Observacion',O.Resultados as 'CodigoGalileo' FROM Orden O 
			INNER JOIN Paciente P ON O.Identificacion = P.Identificacion
			INNER JOIN Prueba Pr ON O.IdOrden = Pr.IdOrden
			INNER JOIN CatalogoMaestro CM ON CM.IdCatalogoMaestro=@estadoCatalogoM
			INNER JOIN CatalogoDetalle CD ON CD.IdCatalogoMaestro = CM.IdCatalogoMaestro
			inner join Usuario US ON O.IdUsuarioGalileo = US.idGalileo --nuevo
			WHERE P.Nombres+p.Apellidos like '%'+@datoBusqueda+'%' 
			--AND O.IdUsuarioGalileo=@idUsuarioGalileo
			--and US.Identificacion = @idenCliente --nuevo
			--and US.CodClienteCta = @sucursal --nuevo 24/01/2024
			and US.idGalileo = @idUsuarioGalileo --nuevo 24/09/2024
			AND  O.FechaCreacion BETWEEN @fechaInicio AND @fechaFin 
			AND O.Estado = CD.IdCatalogoDetalle
			and Pr.Eliminado != 1 --nuevo 11/01/2024
			GROUP BY O.IdOrden, O.CodigoBarra,P.Nombres, P.Apellidos, O.FechaCreacion, Cd.Nombre, 
			O.Observacion, O.Resultados
			order by O.FechaCreacion desc
	END

	IF(@opcionBusqueda = @estadoCinco)
	BEGIN
			SELECT O.IdOrden AS 'NOrden',O.CodigoBarra AS 'CodigoBarra', O.FechaCreacion AS 'FechaIngreso', 
				P.Nombres+' '+P.Apellidos AS 'NombrePaciente', CD.Nombre AS 'Estado',
				SUM(Pr.Precio)AS'Precio' , O.Observacion AS 'Observacion',O.Resultados as 'CodigoGalileo' FROM Orden O 
			INNER JOIN Paciente P ON O.Identificacion = P.Identificacion
			INNER JOIN Prueba Pr ON O.IdOrden = Pr.IdOrden
			INNER JOIN CatalogoMaestro CM ON CM.IdCatalogoMaestro=@estadoCatalogoM
			INNER JOIN CatalogoDetalle CD ON CD.IdCatalogoMaestro = CM.IdCatalogoMaestro
			inner join Usuario US ON O.IdUsuarioGalileo = US.idGalileo --nuevo
			WHERE P.Identificacion=@datoBusqueda 
			--AND O.IdUsuarioGalileo=@idUsuarioGalileo 
			--and US.Identificacion = @idenCliente --nuevo
			--and US.CodClienteCta = @sucursal --nuevo 24/01/2024
			and US.idGalileo = @idUsuarioGalileo --nuevo 24/09/2024
			AND O.FechaCreacion BETWEEN @fechaInicio AND @fechaFin 
			AND O.Estado = CD.IdCatalogoDetalle
			and Pr.Eliminado != 1 --nuevo 11/01/2024
			GROUP BY O.IdOrden, O.CodigoBarra,P.Nombres, P.Apellidos, O.FechaCreacion, Cd.Nombre, 
			O.Observacion, O.Resultados
			order by O.FechaCreacion desc
	END

	IF(@opcionBusqueda = @estadoSeis)
	BEGIN
	
			select @fechaFin = DATEADD(DAY, 1, @fechaFin)

			SELECT O.IdOrden AS 'NOrden',O.CodigoBarra AS 'CodigoBarra', O.FechaCreacion AS 'FechaIngreso', 
				P.Nombres+' '+P.Apellidos AS 'NombrePaciente', CD.Nombre AS 'Estado',
				SUM(Pr.Precio)AS'Precio' , O.Observacion AS 'Observacion',O.Resultados as 'CodigoGalileo' FROM Orden O 
			INNER JOIN Paciente P ON O.Identificacion = P.Identificacion
			INNER JOIN Prueba Pr ON O.IdOrden = Pr.IdOrden
			INNER JOIN CatalogoMaestro CM ON CM.IdCatalogoMaestro=@estadoCatalogoM
			INNER JOIN CatalogoDetalle CD ON CD.IdCatalogoMaestro = CM.IdCatalogoMaestro
			inner join Usuario US ON O.IdUsuarioGalileo = US.idGalileo --nuevo
			WHERE CD.Valor=@datoBusqueda 
			--AND O.IdUsuarioGalileo= @idUsuarioGalileo  
			--and US.Identificacion = @idenCliente --nuevo
			--and US.CodClienteCta = @sucursal --nuevo 24/01/2024
			and US.idGalileo = @idUsuarioGalileo --nuevo 24/09/2024
			AND O.Estado = CD.IdCatalogoDetalle
			AND O.FechaCreacion BETWEEN @fechaInicio AND @fechaFin
			and Pr.Eliminado != 1 --nuevo 11/01/2024
			GROUP BY O.IdOrden, O.CodigoBarra,P.Nombres, P.Apellidos, O.FechaCreacion, Cd.Nombre, 
			O.Observacion, O.Resultados
			order by O.FechaCreacion desc
	END

	
	IF(@opcionBusqueda = @estadoSiete)
	BEGIN
			select @fechaFin = DATEADD(DAY, 1, @fechaFin)

			SELECT O.IdOrden AS 'NOrden',O.CodigoBarra AS 'CodigoBarra', O.FechaCreacion AS 'FechaIngreso', 
				P.Nombres+' '+P.Apellidos AS 'NombrePaciente', CD.Nombre AS 'Estado',
				SUM(Pr.Precio)AS'Precio' , O.Observacion AS 'Observacion',O.Resultados as 'CodigoGalileo' FROM Orden O 
			INNER JOIN Paciente P ON O.Identificacion = P.Identificacion
			INNER JOIN Prueba Pr ON O.IdOrden = Pr.IdOrden
			INNER JOIN CatalogoMaestro CM ON CM.IdCatalogoMaestro=@estadoCatalogoM
			INNER JOIN CatalogoDetalle CD ON CD.IdCatalogoMaestro = CM.IdCatalogoMaestro
			inner join Usuario US ON O.IdUsuarioGalileo = US.idGalileo --nuevo
			WHERE CD.Valor=@datoBusqueda 
			--AND O.IdUsuarioGalileo=@idUsuarioGalileo  
			--and US.Identificacion = @idenCliente --nuevo
			--and US.CodClienteCta = @sucursal --nuevo 24/01/2024
			and US.idGalileo = @idUsuarioGalileo --nuevo 24/09/2024
			AND O.Estado = CD.IdCatalogoDetalle 
			AND O.FechaCreacion BETWEEN @fechaInicio AND @fechaFin
			and Pr.Eliminado != 1 --nuevo 11/01/2024
			GROUP BY O.IdOrden, O.CodigoBarra,P.Nombres, P.Apellidos, O.FechaCreacion, Cd.Nombre, 
			O.Observacion, O.Resultados
			order by O.FechaCreacion desc
	END

	--busqueda todos
	IF(@opcionBusqueda = @estadoOcho)
	BEGIN
		select @fechaFin = DATEADD(DAY, 1, @fechaFin)

		SELECT O.IdOrden AS 'NOrden', O.CodigoBarra AS 'CodigoBarra', 
			O.FechaCreacion AS 'FechaIngreso', P.Nombres+' '+P.Apellidos AS 'NombrePaciente', 
			CD.Nombre AS 'Estado', SUM(Pr.Precio)AS'Precio', 
			O.Observacion AS 'Observacion',O.Resultados as 'CodigoGalileo' 
		From Orden O
		INNER JOIN Paciente P ON O.Identificacion = P.Identificacion
		INNER JOIN Prueba Pr ON O.IdOrden = Pr.IdOrden
		inner join CatalogoDetalle CD ON O.Estado = CD.IdCatalogoDetalle
		inner join Usuario US ON O.IdUsuarioGalileo = US.idGalileo --nuevo
		--WHERE IdUsuarioGalileo = @idUsuarioGalileo
		where O.Estado not in (@i_idCance, @i_idGenen)
		--and US.Identificacion = @idenCliente --nuevo
		--and US.CodClienteCta = @sucursal --nuevo 24/01/2024
		and US.idGalileo = @idUsuarioGalileo --nuevo 24/09/2024
		AND O.FechaCreacion BETWEEN @fechaInicio AND @fechaFin
		and Pr.Eliminado != 1 --nuevo 11/01/2024
		GROUP BY O.IdOrden, O.CodigoBarra,P.Nombres, P.Apellidos, O.FechaCreacion, Cd.Nombre, 
		O.Observacion, O.Resultados
		order by O.FechaCreacion desc

	END
	
	--recolectado total y parcial
	if(@opcionBusqueda = @estadoDiez)
	begin

		select @fechaFin = DATEADD(DAY, 1, @fechaFin)

		--busqueda por cedula
		if @datoBusqueda != null or @datoBusqueda != ''
		begin
			--busqueda por cedula
			SELECT O.IdOrden AS 'NOrden',
				O.CodigoBarra AS 'CodigoBarra', 
				O.FechaCreacion AS 'FechaIngreso', 
				P.Nombres+' '+P.Apellidos AS 'NombrePaciente', 
				CD.Nombre AS 'Estado',
				SUM(Pr.Precio) AS 'Precio', 
				O.Observacion AS 'Observacion',O.Resultados as 'CodigoGalileo'  
			FROM Orden O 
			INNER JOIN Paciente P ON O.Identificacion = P.Identificacion
			INNER JOIN Prueba Pr ON O.IdOrden = Pr.IdOrden
			inner join CatalogoDetalle CD ON O.Estado = CD.IdCatalogoDetalle
			inner join Usuario US ON O.IdUsuarioGalileo = US.idGalileo --nuevo
			WHERE CD.Valor in ('RCTL', 'RCTP') 
			--AND O.IdUsuarioGalileo = @idUsuarioGalileo
			--and US.Identificacion = @idenCliente --nuevo
			--and US.CodClienteCta = @sucursal --nuevo 24/01/2024
			and US.idGalileo = @idUsuarioGalileo --nuevo 24/09/2024
			AND O.FechaCreacion BETWEEN @fechaInicio AND @fechaFin
			and P.Identificacion = @datoBusqueda
			and Pr.Eliminado != 1 --nuevo 11/01/2024
			GROUP BY O.IdOrden, O.CodigoBarra,P.Nombres, P.Apellidos, O.FechaCreacion, Cd.Nombre, 
			O.Observacion, O.Resultados
			order by O.FechaCreacion desc

		end
		else
		begin
			--busqueda por fechas
			SELECT O.IdOrden AS 'NOrden',
				O.CodigoBarra AS 'CodigoBarra', 
				O.FechaCreacion AS 'FechaIngreso', 
				P.Nombres+' '+P.Apellidos AS 'NombrePaciente', 
				CD.Nombre AS 'Estado',
				SUM(Pr.Precio) AS 'Precio', 
				O.Observacion AS 'Observacion',O.Resultados as 'CodigoGalileo'  
			FROM Orden O 
			INNER JOIN Paciente P ON O.Identificacion = P.Identificacion
			INNER JOIN Prueba Pr ON O.IdOrden = Pr.IdOrden
			inner join CatalogoDetalle CD ON O.Estado = CD.IdCatalogoDetalle
			inner join Usuario US ON O.IdUsuarioGalileo = US.idGalileo --nuevo
			WHERE CD.Valor in ('RCTL', 'RCTP') 
			--AND O.IdUsuarioGalileo = @idUsuarioGalileo
			--and US.Identificacion = @idenCliente --nuevo
			--and US.CodClienteCta = @sucursal --nuevo 24/01/2024
			and US.idGalileo = @idUsuarioGalileo --nuevo 24/09/2024
			AND O.FechaCreacion BETWEEN @fechaInicio AND @fechaFin
			and Pr.Eliminado != 1 --nuevo 11/01/2024
			GROUP BY O.IdOrden, O.CodigoBarra,P.Nombres, P.Apellidos, O.FechaCreacion, Cd.Nombre, 
			O.Observacion, O.Resultados
			order by O.FechaCreacion desc

		end
		
	end

	--enviado total y parcial
	if(@opcionBusqueda = @estadoOnce)
	begin

		select @fechaFin = DATEADD(DAY, 1, @fechaFin)
		
		if @datoBusqueda != null or @datoBusqueda != ''
		begin			
			--busqueda por cedula
			SELECT O.IdOrden AS 'NOrden',
				O.CodigoBarra AS 'CodigoBarra', 
				O.FechaCreacion AS 'FechaIngreso', 
				P.Nombres+' '+P.Apellidos AS 'NombrePaciente', 
				CD.Nombre AS 'Estado',
				SUM(Pr.Precio)AS'Precio', 
				O.Observacion AS 'Observacion',O.Resultados as 'CodigoGalileo'  
			FROM Orden O 
			INNER JOIN Paciente P ON O.Identificacion = P.Identificacion
			INNER JOIN Prueba Pr ON O.IdOrden = Pr.IdOrden
			inner join CatalogoDetalle CD ON O.Estado = CD.IdCatalogoDetalle
			--inner join Usuario US ON O.IdUsuarioGalileo = US.idGalileo --nuevo
			WHERE CD.Valor in ('ENV', 'ENVP') 
			--AND O.IdUsuarioGalileo = @idUsuarioGalileo
			--and US.Identificacion = @idenCliente --nuevo
			AND O.FechaCreacion BETWEEN @fechaInicio AND @fechaFin
			and P.Identificacion = @datoBusqueda
			and Pr.Eliminado != 1 --nuevo 11/01/2024
			GROUP BY O.IdOrden, O.CodigoBarra,P.Nombres, P.Apellidos, O.FechaCreacion, Cd.Nombre, 
			O.Observacion, O.Resultados
			order by O.FechaCreacion desc

		end
		else
		begin			
			--busqueda por fechas
			SELECT O.IdOrden AS 'NOrden',
				O.CodigoBarra AS 'CodigoBarra', 
				O.FechaCreacion AS 'FechaIngreso', 
				P.Nombres+' '+P.Apellidos AS 'NombrePaciente', 
				CD.Nombre AS 'Estado',
				SUM(Pr.Precio)AS'Precio', 
				O.Observacion AS 'Observacion',O.Resultados as 'CodigoGalileo'  
			FROM Orden O 
			INNER JOIN Paciente P ON O.Identificacion = P.Identificacion
			INNER JOIN Prueba Pr ON O.IdOrden = Pr.IdOrden
			inner join CatalogoDetalle CD ON O.Estado = CD.IdCatalogoDetalle
			inner join Usuario US ON O.IdUsuarioGalileo = US.idGalileo --nuevo
			WHERE CD.Valor in ('ENV', 'ENVP') 
			--AND O.IdUsuarioGalileo = @idUsuarioGalileo
			--and US.Identificacion = @idenCliente --nuevo
			--and US.CodClienteCta = @sucursal --nuevo 24/01/2024
			and US.idGalileo = @idUsuarioGalileo --nuevo 24/09/2024
			AND O.FechaCreacion BETWEEN @fechaInicio AND @fechaFin
			and Pr.Eliminado != 1 --nuevo 11/01/2024
			GROUP BY O.IdOrden, O.CodigoBarra,P.Nombres, P.Apellidos, O.FechaCreacion, Cd.Nombre, 
			O.Observacion, O.Resultados
			order by O.FechaCreacion desc
			
		end

	end

	--Recibida/parcial
	if(@opcionBusqueda = @estadoDoce)
	begin
	
		select @fechaFin = DATEADD(DAY, 1, @fechaFin)
		
		if @datoBusqueda != null or @datoBusqueda != ''
		begin			
			--busqueda por cedula
			SELECT O.IdOrden AS 'NOrden',
				O.CodigoBarra AS 'CodigoBarra', 
				O.FechaCreacion AS 'FechaIngreso', 
				P.Nombres+' '+P.Apellidos AS 'NombrePaciente', 
				CD.Nombre AS 'Estado',
				SUM(Pr.Precio)AS'Precio', 
				O.Observacion AS 'Observacion',O.Resultados as 'CodigoGalileo'  
			FROM Orden O 
			INNER JOIN Paciente P ON O.Identificacion = P.Identificacion
			INNER JOIN Prueba Pr ON O.IdOrden = Pr.IdOrden
			inner join CatalogoDetalle CD ON O.Estado = CD.IdCatalogoDetalle
			inner join Usuario US ON O.IdUsuarioGalileo = US.idGalileo --nuevo
			WHERE CD.Valor in ('RCBD', 'RCBP') 
			--AND O.IdUsuarioGalileo=@idUsuarioGalileo
			--and US.Identificacion = @idenCliente --nuevo
			--and US.CodClienteCta = @sucursal --nuevo 24/01/2024
			and US.idGalileo = @idUsuarioGalileo --nuevo 24/09/2024
			AND O.FechaCreacion BETWEEN @fechaInicio AND @fechaFin
			and P.Identificacion = @datoBusqueda
			and Pr.Eliminado != 1 --nuevo 11/01/2024
			GROUP BY O.IdOrden, O.CodigoBarra,P.Nombres, P.Apellidos, O.FechaCreacion, Cd.Nombre, 
			O.Observacion, O.Resultados
			order by O.FechaCreacion desc

		end
		else
		begin
			--busqueda por fechas
			SELECT O.IdOrden AS 'NOrden',
				O.CodigoBarra AS 'CodigoBarra', 
				O.FechaCreacion AS 'FechaIngreso', 
				P.Nombres+' '+P.Apellidos AS 'NombrePaciente', 
				CD.Nombre AS 'Estado',
				SUM(Pr.Precio)AS'Precio', 
				O.Observacion AS 'Observacion',O.Resultados as 'CodigoGalileo'  
			FROM Orden O 
			INNER JOIN Paciente P ON O.Identificacion = P.Identificacion
			INNER JOIN Prueba Pr ON O.IdOrden = Pr.IdOrden
			inner join CatalogoDetalle CD ON O.Estado = CD.IdCatalogoDetalle
			inner join Usuario US ON O.IdUsuarioGalileo = US.idGalileo --nuevo
			WHERE CD.Valor in ('RCBD', 'RCBP') 
			--AND O.IdUsuarioGalileo=@idUsuarioGalileo
			--and US.Identificacion = @idenCliente --nuevo
			--and US.CodClienteCta = @sucursal --nuevo 24/01/2024
			and US.idGalileo = @idUsuarioGalileo --nuevo 24/09/2024
			AND O.FechaCreacion BETWEEN @fechaInicio AND @fechaFin
			and Pr.Eliminado != 1 --nuevo 11/01/2024
			GROUP BY O.IdOrden, O.CodigoBarra,P.Nombres, P.Apellidos, O.FechaCreacion, Cd.Nombre, 
			O.Observacion, O.Resultados
			order by O.FechaCreacion desc

		end

	end

	--Busqueda de todas las ordenes
	if @opcionBusqueda = @estadoTrece
	begin
		select @fechaFin = DATEADD(DAY, 1, @fechaFin)

		SELECT O.IdOrden AS 'NOrden', O.CodigoBarra AS 'CodigoBarra', 
			O.FechaCreacion AS 'FechaIngreso', P.Nombres+' '+P.Apellidos AS 'NombrePaciente', 
			CD.Nombre AS 'Estado', SUM(Pr.Precio)AS'Precio', 
			O.Observacion AS 'Observacion',O.Resultados as 'CodigoGalileo'  
		From Orden O
		INNER JOIN Paciente P ON O.Identificacion = P.Identificacion
		INNER JOIN Prueba Pr ON O.IdOrden = Pr.IdOrden
		inner join CatalogoDetalle CD ON O.Estado = CD.IdCatalogoDetalle
		inner join Usuario US ON O.IdUsuarioGalileo = US.idGalileo --nuevo
		--WHERE IdUsuarioGalileo = @idUsuarioGalileo
		where O.Estado not in (@i_idCance, @i_idGenen)
		--and US.Identificacion = @idenCliente --nuevo
		--and US.CodClienteCta = @sucursal --nuevo 24/01/2024
		and US.idGalileo = @idUsuarioGalileo --nuevo 24/09/2024
		AND O.FechaCreacion BETWEEN @fechaInicio AND @fechaFin
		and Pr.Eliminado != 1 --nuevo 11/01/2024
		GROUP BY O.IdOrden, O.CodigoBarra,P.Nombres, P.Apellidos, O.FechaCreacion, Cd.Nombre, 
		O.Observacion, O.Resultados
		order by O.FechaCreacion desc

	end

END

GO