/************************************************************************
*	Stored procedure: pr_resultados_MedicoLab					        *
*	Base de datos: DbPortalHumalab						  			    *
*	Elaborado por: José Guarnizo						                *
*----------------------------------------------------------------------	*
*					DESCRIPCION DEL PROCEDIMIENTO						*
*	En este Procedimiento ver los pdf y resultados del medico           *
*	                                                                    *
*----------------------------------------------------------------------	*
*					BITACORA DE MODIFICACIONES							*
*	FECHA AUTOR RAZON													*
*----------------------------------------------------------------------	*/
IF NOT EXISTS (SELECT * FROM  sys.procedures WHERE NAME = 'pr_resultados_MedicoLab')	
	EXEC('Create Procedure dbo.pr_resultados_MedicoLab As')
go

ALTER PROCEDURE [dbo].[pr_resultados_MedicoLab](
	@i_accion char,
	@opcionBusqueda int = null,
	@opcionEstado int = null,
	@datoBusqueda varchar(20) = null,
	@codigoBarra varchar(100) = null,
	@fechaInicio date = null,
	@fechaFin date = null,
	@idLab int = null,
	@idResult int = null,
	@sedes varchar(200) = null
)

as

declare @estadoBuscar varchar(100)

--abierta
if @opcionEstado = 1
begin
	set @estadoBuscar = 'Abierta'
end

--resultado pendiente
 if @opcionEstado = 2
begin
	set @estadoBuscar = 'Resultados Pendiente'
end

--validacion pendiente
if @opcionEstado = 3
begin
	set @estadoBuscar = 'Validación Pendiente'
end

--validada
if @opcionEstado = 4
begin
	set @estadoBuscar = 'Validada'
end

--tabla temporal para las sedes
CREATE TABLE #sedes (Sede varchar)

INSERT INTO #sedes (Sede)
SELECT value
FROM STRING_SPLIT(@sedes, ',')

if @i_accion = 'C'
begin
	
	--en todos los estados
	if @opcionBusqueda = 0
	begin
		
		select @fechaInicio = DATEADD(DAY, -1, GETDATE())
		select @fechaFin = DATEADD(DAY, 1, GETDATE())

		select r.IdResultados as idResultados,
			r.IdentificacionPac as Identificacion,
			r.NumeroOrden as NumeroOrden,
			r.Genero as Genero,
			r.Estado as Estado,
			LEFT(r.FechaIngreso, CHARINDEX(' ', r.FechaIngreso) - 1) as FechaIngreso,
			r.NombrePaciente as NombrePaciente,
			r.aux1 as IdSede,
			r.aux2 as NombreSede
		from Resultados r		
		where r.IdLaboratorio = @idLab		
		AND (COALESCE(@sedes, '') = '' OR r.aux1 IN (SELECT Sede FROM #sedes))		
		AND CONVERT(DATETIME, FechaIngreso, 103) BETWEEN CONVERT(DATETIME, @fechaInicio, 103) AND CONVERT(DATETIME, @fechaFin, 103)
		ORDER BY r.FechaRegistro desc

	end

	--busqueda por numero de orden
	if @opcionBusqueda = 1
	begin
		select @fechaFin = DATEADD(DAY, 1, @fechaFin)

		if @datoBusqueda != null or @datoBusqueda != ''
		begin
			select r.IdResultados as idResultados,
				r.IdentificacionPac as Identificacion,
				r.NumeroOrden as NumeroOrden,
				r.Genero as Genero,
				r.Estado as Estado,
				LEFT(r.FechaIngreso, CHARINDEX(' ', r.FechaIngreso) - 1) as FechaIngreso,
				r.NombrePaciente as NombrePaciente,
				r.aux1 as IdSede,
				r.aux2 as NombreSede
			from Resultados r
			where r.IdLaboratorio = @idLab
			and r.NumeroOrden = @datoBusqueda			
			--AND r.aux1 IN (SELECT Sede FROM #sedes)
			AND (COALESCE(@sedes, '') = '' OR r.aux1 IN (SELECT Sede FROM #sedes))
			AND CONVERT(DATETIME, r.FechaIngreso, 103) BETWEEN CONVERT(DATETIME, @fechaInicio, 103) AND CONVERT(DATETIME, @fechaFin, 103)
			ORDER BY r.FechaRegistro desc
		end
		else
		begin	
			
			if @opcionEstado != 0
			begin
				select r.IdResultados as idResultados,
					r.IdentificacionPac as Identificacion,
					r.NumeroOrden as NumeroOrden,
					r.Genero as Genero,
					r.Estado as Estado,
					LEFT(r.FechaIngreso, CHARINDEX(' ', r.FechaIngreso) - 1) as FechaIngreso,
					r.NombrePaciente as NombrePaciente,
					r.aux1 as IdSede,
					r.aux2 as NombreSede
				from Resultados r
				where r.IdLaboratorio = @idLab
				and r.Estado = @estadoBuscar
				--AND r.aux1 IN (SELECT Sede FROM #sedes)
				AND (COALESCE(@sedes, '') = '' OR r.aux1 IN (SELECT Sede FROM #sedes))	
				AND CONVERT(DATETIME, r.FechaIngreso, 103) BETWEEN CONVERT(DATETIME, @fechaInicio, 103) AND CONVERT(DATETIME, @fechaFin, 103)
				ORDER BY r.FechaRegistro desc
			end
			else
			begin
				select r.IdResultados as idResultados,
					r.IdentificacionPac as Identificacion,
					r.NumeroOrden as NumeroOrden,
					r.Genero as Genero,
					r.Estado as Estado,
					LEFT(r.FechaIngreso, CHARINDEX(' ', r.FechaIngreso) - 1) as FechaIngreso,
					r.NombrePaciente as NombrePaciente,
					r.aux1 as IdSede,
					r.aux2 as NombreSede
				from Resultados r
				where r.IdLaboratorio = @idLab				
				--AND r.aux1 IN (SELECT Sede FROM #sedes)
				AND (COALESCE(@sedes, '') = '' OR r.aux1 IN (SELECT Sede FROM #sedes))	
				AND CONVERT(DATETIME, r.FechaIngreso, 103) BETWEEN CONVERT(DATETIME, @fechaInicio, 103) AND CONVERT(DATETIME, @fechaFin, 103)
				ORDER BY r.FechaRegistro desc	
			end

		end
		
	end

	--busqueda por nombre
	if @opcionBusqueda = 2
	begin
		select @fechaFin = DATEADD(DAY, 1, @fechaFin)

		if @datoBusqueda != null or @datoBusqueda != ''
		begin
			select r.IdResultados as idResultados,
				r.IdentificacionPac as Identificacion,
				r.NumeroOrden as NumeroOrden,
				r.Genero as Genero,
				r.Estado as Estado,
				LEFT(r.FechaIngreso, CHARINDEX(' ', r.FechaIngreso) - 1) as FechaIngreso,
				r.NombrePaciente as NombrePaciente,
				r.aux1 as IdSede,
				r.aux2 as NombreSede
			from Resultados r
			where r.IdLaboratorio = @idLab			
			and r.NombrePaciente like '%' + @datoBusqueda + '%'			
			--AND r.aux1 IN (SELECT Sede FROM #sedes)
			AND (COALESCE(@sedes, '') = '' OR r.aux1 IN (SELECT Sede FROM #sedes))	
			AND CONVERT(DATETIME, r.FechaIngreso, 103) BETWEEN CONVERT(DATETIME, @fechaInicio, 103) AND CONVERT(DATETIME, @fechaFin, 103)
			ORDER BY r.FechaRegistro desc
		end
		else
		begin	
			
			if @opcionEstado != 0
			begin
				select r.IdResultados as idResultados,
					r.IdentificacionPac as Identificacion,
					r.NumeroOrden as NumeroOrden,
					r.Genero as Genero,
					r.Estado as Estado,
					LEFT(r.FechaIngreso, CHARINDEX(' ', r.FechaIngreso) - 1) as FechaIngreso,
					r.NombrePaciente as NombrePaciente,
					r.aux1 as IdSede,
					r.aux2 as NombreSede
				from Resultados r
				where r.IdLaboratorio = @idLab
				and r.Estado = @estadoBuscar
				--AND r.aux1 IN (SELECT Sede FROM #sedes)
				AND (COALESCE(@sedes, '') = '' OR r.aux1 IN (SELECT Sede FROM #sedes))	
				AND CONVERT(DATETIME, r.FechaIngreso, 103) BETWEEN CONVERT(DATETIME, @fechaInicio, 103) AND CONVERT(DATETIME, @fechaFin, 103)
				ORDER BY r.FechaRegistro desc
			end
			else
			begin				
				select r.IdResultados as idResultados,
					r.IdentificacionPac as Identificacion,
					r.NumeroOrden as NumeroOrden,
					r.Genero as Genero,
					r.Estado as Estado,
					LEFT(r.FechaIngreso, CHARINDEX(' ', r.FechaIngreso) - 1) as FechaIngreso,
					r.NombrePaciente as NombrePaciente,
					r.aux1 as IdSede,
					r.aux2 as NombreSede
				from Resultados r
				where r.IdLaboratorio = @idLab				
				--AND r.aux1 IN (SELECT Sede FROM #sedes)
				AND (COALESCE(@sedes, '') = '' OR r.aux1 IN (SELECT Sede FROM #sedes))	
				AND CONVERT(DATETIME, r.FechaIngreso, 103) BETWEEN CONVERT(DATETIME, @fechaInicio, 103) AND CONVERT(DATETIME, @fechaFin, 103)
				ORDER BY r.FechaRegistro desc				
			end

		end
		
	end

	--busqueda por cedula
	if @opcionBusqueda = 3
	begin		
		select @fechaFin = DATEADD(DAY, 1, @fechaFin)

		if @datoBusqueda != null or @datoBusqueda != ''
		begin
			select r.IdResultados as idResultados,
				r.IdentificacionPac as Identificacion,
				r.NumeroOrden as NumeroOrden,
				r.Genero as Genero,
				r.Estado as Estado,
				LEFT(r.FechaIngreso, CHARINDEX(' ', r.FechaIngreso) - 1) as FechaIngreso,
				r.NombrePaciente as NombrePaciente,
				r.aux1 as IdSede,
				r.aux2 as NombreSede
			from Resultados r
			where r.IdLaboratorio = @idLab
			and r.IdentificacionPac = @datoBusqueda				
			--AND aux1 IN (SELECT Sede FROM #sedes)
			AND (COALESCE(@sedes, '') = '' OR r.aux1 IN (SELECT Sede FROM #sedes))	
			AND CONVERT(DATETIME, r.FechaIngreso, 103) BETWEEN CONVERT(DATETIME, @fechaInicio, 103) AND CONVERT(DATETIME, @fechaFin, 103)
			ORDER BY r.FechaRegistro desc

		end
		else
		begin			
			
			if @opcionEstado != 0
			begin
				select r.IdResultados as idResultados,
					r.IdentificacionPac as Identificacion,
					r.NumeroOrden as NumeroOrden,
					r.Genero as Genero,
					r.Estado as Estado,
					LEFT(r.FechaIngreso, CHARINDEX(' ', r.FechaIngreso) - 1) as FechaIngreso,
					r.NombrePaciente as NombrePaciente,
					r.aux1 as IdSede,
					r.aux2 as NombreSede
				from Resultados r
				where r.IdLaboratorio = @idLab
				and r.Estado = @estadoBuscar
				--AND aux1 IN (SELECT Sede FROM #sedes)
				AND (COALESCE(@sedes, '') = '' OR r.aux1 IN (SELECT Sede FROM #sedes))	
				AND CONVERT(DATETIME, r.FechaIngreso, 103) BETWEEN CONVERT(DATETIME, @fechaInicio, 103) AND CONVERT(DATETIME, @fechaFin, 103)
				ORDER BY r.FechaRegistro desc
			end
			else
			begin
				select r.IdResultados as idResultados,
					r.IdentificacionPac as Identificacion,
					r.NumeroOrden as NumeroOrden,
					r.Genero as Genero,
					r.Estado as Estado,
					LEFT(r.FechaIngreso, CHARINDEX(' ', r.FechaIngreso) - 1) as FechaIngreso,
					r.NombrePaciente as NombrePaciente,
					r.aux1 as IdSede,
					r.aux2 as NombreSede
				from Resultados r
				where r.IdLaboratorio = @idLab				
				--AND aux1 IN (SELECT Sede FROM #sedes)
				AND (COALESCE(@sedes, '') = '' OR r.aux1 IN (SELECT Sede FROM #sedes))	
				AND CONVERT(DATETIME, r.FechaIngreso, 103) BETWEEN CONVERT(DATETIME, @fechaInicio, 103) AND CONVERT(DATETIME, @fechaFin, 103)
				ORDER BY r.FechaRegistro desc	
			end
			
		end

	end

	drop table #sedes

end

if @i_accion = 'P'
BEGIN
	
	select Informe as Resultado
	from Resultados
	where IdResultados = @idResult

end

GO


