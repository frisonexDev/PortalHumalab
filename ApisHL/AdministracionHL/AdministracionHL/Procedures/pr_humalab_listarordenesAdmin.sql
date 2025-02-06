/************************************************************************
*	Archivo Fisico: pr_humalab_listarordenesAdmin.sql					*
*	Stored procedure: pr_humalab_listarordenesAdmin						*
*	Base de datos: DbPortalHumalab						  			    *
*	Producto: Portal Clientes Humalab					                *
*	Elaborado por: Jose Guarnizo						                *
*----------------------------------------------------------------------	*
*					DESCRIPCION DEL PROCEDIMIENTO						*
*	En este Procedimiento consulta ordenes de todos los clientes para   *
*	el administrador                                                    *
*----------------------------------------------------------------------	*
*					BITACORA DE MODIFICACIONES							*
*	FECHA AUTOR RAZON													*
*						                                                *
*----------------------------------------------------------------------	*/
IF NOT EXISTS (SELECT * FROM sys.procedures WHERE name = 'pr_humalab_listarordenesAdmin')	
	EXEC('Create Procedure dbo.pr_humalab_listarordenesAdmin As')
GO

ALTER PROCEDURE [dbo].pr_humalab_listarordenesAdmin(
	@i_accion CHAR,	
	@opcionBusqueda INT=NULL,
	@opcionEstado varchar(100)=null,
	@datoBusqueda VARCHAR(50)=NULL,
	@idOrden INT=NULL,
	@codigoBarra Varchar(20)=NULL,
	@fechaInicio DATE=NULL,
	@fechaFin DATE = NULL			
)

AS

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

IF(@i_accion='C')
Begin
	
	IF(@opcionBusqueda= @estadoCero)
	BEGIN
			Select O.IdOrden AS 'NOrden', O.CodigoBarra AS 'CodigoBarra', O.FechaCreacion AS 'FechaIngreso', 
				P.Nombres+' '+P.Apellidos AS 'NombrePaciente', CD.Nombre AS 'Estado',
				SUM(Pr.Precio) AS 'Precio', O.Observacion AS 'Observacion', O.Resultados as 'CodigoGalileo'
			From Orden O
			INNER JOIN Paciente P ON O.Identificacion = P.Identificacion
			INNER JOIN Prueba Pr ON O.IdOrden = Pr.IdOrden
			INNER JOIN CatalogoMaestro CM ON CM.IdCatalogoMaestro = @estadoCatalogoM
			INNER JOIN CatalogoDetalle CD ON CD.IdCatalogoMaestro = CM.IdCatalogoMaestro			
			WHERE O.FechaCreacion >= CONVERT(DATETIME, CONVERT(DATE, GETDATE())) 
			AND O.FechaCreacion < CONVERT(DATETIME, DATEADD(DAY, 1, CONVERT(DATE, GETDATE())))			
			AND O.Estado = CD.IdCatalogoDetalle						
			and Pr.Eliminado != 1 --nuevo 11/01/2024
			and O.Eliminado !=1 --nuevo 24/01/2024
			GROUP BY O.IdOrden, O.CodigoBarra,P.Nombres, P.Apellidos, O.FechaCreacion, Cd.Nombre, 
				O.Observacion, O.Resultados
			order by O.FechaCreacion desc
	END

	--Busqueda de todas las ordenes
	if @opcionBusqueda = @estadoTrece
	begin
		select @fechaFin = DATEADD(DAY, 1, @fechaFin)

		SELECT O.IdOrden AS 'NOrden', O.CodigoBarra AS 'CodigoBarra', 
			O.FechaCreacion AS 'FechaIngreso', P.Nombres+' '+P.Apellidos AS 'NombrePaciente', 
			CD.Nombre AS 'Estado', SUM(Pr.Precio)AS'Precio', 
			O.Observacion AS 'Observacion',O.Resultados as 'CodigoGalileo', cl.aux1 as 'NombreCliente'  
		From Orden O
		INNER JOIN Paciente P ON O.Identificacion = P.Identificacion
		INNER JOIN Prueba Pr ON O.IdOrden = Pr.IdOrden
		inner join CatalogoDetalle CD ON O.Estado = CD.IdCatalogoDetalle
		inner join Usuario us on O.IdUsuarioGalileo = us.idGalileo
		inner join Cliente cl on us.idUsuario = cl.IdUsuario
		where O.Estado not in (@i_idCance, @i_idGenen)				
		AND O.FechaCreacion BETWEEN @fechaInicio AND @fechaFin
		and Pr.Eliminado != 1 --nuevo 11/01/2024
		and O.Eliminado !=1
		GROUP BY O.IdOrden, O.CodigoBarra, P.Nombres, P.Apellidos, O.FechaCreacion, Cd.Nombre, 
			O.Observacion, O.Resultados, cl.aux1
		order by O.FechaCreacion desc

	end

		--Busqueda por nombre, cedula o apellidos paciente
	IF(@opcionBusqueda = @estadoDos)
	BEGIN			
			select @fechaFin = DATEADD(DAY, 1, @fechaFin)

			IF @datoBusqueda != null or @datoBusqueda != ''
			begin
				SELECT O.IdOrden AS 'NOrden', O.CodigoBarra AS 'CodigoBarra', O.FechaCreacion AS 'FechaIngreso', 
					P.Nombres+' '+P.Apellidos AS 'NombrePaciente', CD.Nombre AS 'Estado', SUM(Pr.Precio)AS'Precio', 
					O.Observacion AS 'Observacion', O.Resultados as 'CodigoGalileo', cl.aux1 as 'NombreCliente'
				FROM Orden O 
				INNER JOIN Paciente P ON O.Identificacion = P.Identificacion
				INNER JOIN Prueba Pr ON O.IdOrden = Pr.IdOrden
				inner join CatalogoDetalle CD ON O.Estado = CD.IdCatalogoDetalle
				inner join Usuario us on O.IdUsuarioGalileo = us.idGalileo
				inner join Cliente cl on us.idUsuario = cl.IdUsuario
				WHERE (
					(P.Nombres + p.Apellidos like '%' + @datoBusqueda + '%')
					OR P.Identificacion = @datoBusqueda
				)												
				AND O.FechaCreacion BETWEEN @fechaInicio AND @fechaFin
				and Pr.Eliminado != 1
				and O.Eliminado !=1
				GROUP BY O.IdOrden, O.CodigoBarra,P.Nombres, P.Apellidos, O.FechaCreacion, Cd.Nombre, 
						O.Observacion, O.Resultados, cl.aux1
				order by O.FechaCreacion desc
			end
			else
			begin
				
				SELECT O.IdOrden AS 'NOrden', O.CodigoBarra AS 'CodigoBarra', O.FechaCreacion AS 'FechaIngreso', 
					P.Nombres+' '+P.Apellidos AS 'NombrePaciente', CD.Nombre AS 'Estado', SUM(Pr.Precio)AS'Precio', 
					O.Observacion AS 'Observacion', O.Resultados as 'CodigoGalileo', cl.aux1 as 'NombreCliente'  
				FROM Orden O 
				INNER JOIN Paciente P ON O.Identificacion = P.Identificacion
				INNER JOIN Prueba Pr ON O.IdOrden = Pr.IdOrden
				inner join CatalogoDetalle CD ON O.Estado = CD.IdCatalogoDetalle
				inner join Usuario us on O.IdUsuarioGalileo = us.idGalileo
				inner join Cliente cl on us.idUsuario = cl.IdUsuario
				WHERE CD.IdCatalogoDetalle = @i_EstadoId								
				AND O.FechaCreacion BETWEEN @fechaInicio AND @fechaFin
				and Pr.Eliminado != 1
				and O.Eliminado !=1
				GROUP BY O.IdOrden, O.CodigoBarra,P.Nombres, P.Apellidos, O.FechaCreacion, Cd.Nombre, 
					O.Observacion, O.Resultados, cl.aux1
				order by O.FechaCreacion desc

			end

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
				O.Observacion AS 'Observacion', 
				O.Resultados as 'CodigoGalileo',
				cl.aux1 as 'NombreCliente'
			FROM Orden O 
			INNER JOIN Paciente P ON O.Identificacion = P.Identificacion
			INNER JOIN Prueba Pr ON O.IdOrden = Pr.IdOrden
			inner join CatalogoDetalle CD ON O.Estado = CD.IdCatalogoDetalle
			inner join Usuario us on O.IdUsuarioGalileo = us.idGalileo
			inner join Cliente cl on us.idUsuario = cl.IdUsuario
			WHERE CD.Valor in ('RCTL', 'RCTP') 						
			AND O.FechaCreacion BETWEEN @fechaInicio AND @fechaFin
			and P.Identificacion = @datoBusqueda
			and Pr.Eliminado != 1
			and O.Eliminado !=1
			GROUP BY O.IdOrden, O.CodigoBarra,P.Nombres, P.Apellidos, O.FechaCreacion, Cd.Nombre, 
				O.Observacion, O.Resultados, cl.aux1
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
				O.Observacion AS 'Observacion',
				O.Resultados as 'CodigoGalileo',
				cl.aux1 as 'NombreCliente'
			FROM Orden O 
			INNER JOIN Paciente P ON O.Identificacion = P.Identificacion
			INNER JOIN Prueba Pr ON O.IdOrden = Pr.IdOrden
			inner join CatalogoDetalle CD ON O.Estado = CD.IdCatalogoDetalle	
			inner join Usuario us on O.IdUsuarioGalileo = us.idGalileo
			inner join Cliente cl on us.idUsuario = cl.IdUsuario
			WHERE CD.Valor in ('RCTL', 'RCTP') 						
			AND O.FechaCreacion BETWEEN @fechaInicio AND @fechaFin
			and Pr.Eliminado != 1
			and O.Eliminado !=1
			GROUP BY O.IdOrden, O.CodigoBarra, P.Nombres, P.Apellidos, O.FechaCreacion, Cd.Nombre, 
				O.Observacion, O.Resultados, cl.aux1
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
				O.Observacion AS 'Observacion',
				O.Resultados as 'CodigoGalileo',
				cl.aux1 as 'NombreCliente'
			FROM Orden O 
			INNER JOIN Paciente P ON O.Identificacion = P.Identificacion
			INNER JOIN Prueba Pr ON O.IdOrden = Pr.IdOrden
			inner join CatalogoDetalle CD ON O.Estado = CD.IdCatalogoDetalle		
			inner join Usuario us on O.IdUsuarioGalileo = us.idGalileo
			inner join Cliente cl on us.idUsuario = cl.IdUsuario
			WHERE CD.Valor in ('ENV', 'ENVP') 			
			AND O.FechaCreacion BETWEEN @fechaInicio AND @fechaFin
			and P.Identificacion = @datoBusqueda
			and Pr.Eliminado != 1
			and O.Eliminado !=1
			GROUP BY O.IdOrden, O.CodigoBarra,P.Nombres, P.Apellidos, O.FechaCreacion, Cd.Nombre, 
				O.Observacion, O.Resultados, cl.aux1
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
				O.Observacion AS 'Observacion',
				O.Resultados as 'CodigoGalileo',
				cl.aux1 as 'NombreCliente'
			FROM Orden O 
			INNER JOIN Paciente P ON O.Identificacion = P.Identificacion
			INNER JOIN Prueba Pr ON O.IdOrden = Pr.IdOrden
			inner join CatalogoDetalle CD ON O.Estado = CD.IdCatalogoDetalle	
			inner join Usuario us on O.IdUsuarioGalileo = us.idGalileo
			inner join Cliente cl on us.idUsuario = cl.IdUsuario
			WHERE CD.Valor in ('ENV', 'ENVP') 			
			AND O.FechaCreacion BETWEEN @fechaInicio AND @fechaFin
			and Pr.Eliminado != 1
			and O.Eliminado !=1
			GROUP BY O.IdOrden, O.CodigoBarra,P.Nombres, P.Apellidos, O.FechaCreacion, Cd.Nombre, 
				O.Observacion, O.Resultados, cl.aux1
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
				O.Observacion AS 'Observacion',
				O.Resultados as 'CodigoGalileo',
				cl.aux1 as 'NombreCliente'
			FROM Orden O 
			INNER JOIN Paciente P ON O.Identificacion = P.Identificacion
			INNER JOIN Prueba Pr ON O.IdOrden = Pr.IdOrden
			inner join CatalogoDetalle CD ON O.Estado = CD.IdCatalogoDetalle
			inner join Usuario us on O.IdUsuarioGalileo = us.idGalileo
			inner join Cliente cl on us.idUsuario = cl.IdUsuario
			WHERE CD.Valor in ('RCBD', 'RCBP') 			
			AND O.FechaCreacion BETWEEN @fechaInicio AND @fechaFin
			and P.Identificacion = @datoBusqueda
			and Pr.Eliminado != 1
			and O.Eliminado !=1
			GROUP BY O.IdOrden, O.CodigoBarra,P.Nombres, P.Apellidos, O.FechaCreacion, Cd.Nombre, 
				O.Observacion, O.Resultados, cl.aux1
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
				O.Observacion AS 'Observacion',
				O.Resultados as 'CodigoGalileo',
				cl.aux1 as 'NombreCliente'
			FROM Orden O 
			INNER JOIN Paciente P ON O.Identificacion = P.Identificacion
			INNER JOIN Prueba Pr ON O.IdOrden = Pr.IdOrden
			inner join CatalogoDetalle CD ON O.Estado = CD.IdCatalogoDetalle
			inner join Usuario us on O.IdUsuarioGalileo = us.idGalileo
			inner join Cliente cl on us.idUsuario = cl.IdUsuario
			WHERE CD.Valor in ('RCBD', 'RCBP') 
			AND O.FechaCreacion BETWEEN @fechaInicio AND @fechaFin
			and Pr.Eliminado != 1
			and O.Eliminado !=1
			GROUP BY O.IdOrden, O.CodigoBarra,P.Nombres, P.Apellidos, O.FechaCreacion, Cd.Nombre, 
				O.Observacion, O.Resultados, cl.aux1
			order by O.FechaCreacion desc

		end

	end

end

GO