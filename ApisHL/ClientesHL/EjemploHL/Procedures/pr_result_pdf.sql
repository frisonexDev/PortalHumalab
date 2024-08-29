/************************************************************************
*	Stored procedure: pr_result_pdf										*
*	Base de datos: DbPortalHumalab						  			    *
*	Elaborado por: José Guarnizo						                *
*----------------------------------------------------------------------	*
*					DESCRIPCION DEL PROCEDIMIENTO						*
*	En este Procedimiento consulta el base64 en base al id del lab y    *
*   el numero de orden.												    *
*	                                                                    *
*----------------------------------------------------------------------	*
*					BITACORA DE MODIFICACIONES							*
*	FECHA AUTOR RAZON													*
*----------------------------------------------------------------------	*/
IF NOT EXISTS (SELECT * FROM  sys.procedures WHERE NAME = 'pr_result_pdf')	
	EXEC('Create Procedure dbo.pr_result_pdf As')
go

ALTER PROCEDURE [dbo].[pr_result_pdf](
	@i_accion char,	
	@i_idLab int = null,
	@i_codOrden varchar(30) = null,
	@i_identificacion varchar(20) = null
)

as

if @i_accion = 'C'
begin
	
	if exists (select IdResultados from Resultados where NumeroOrden = @i_codOrden and IdLaboratorio = @i_idLab)
	begin
		select Informe as Resultado
		from Resultados
		where IdLaboratorio = @i_idLab
		and NumeroOrden = @i_codOrden
	end
	else
	begin
		select '01' as Resultado
	end

end

if @i_accion = 'P'
BEGIN
	
	if exists (select NumeroOrden from Resultados where IdentificacionPac = @i_identificacion)
	begin
		if @i_idLab != null or @i_idLab != ''
		begin
			-- datos del paciente en base al lab
			if @i_idLab = 18 or @i_idLab = 31
			begin
				select IdentificacionPac as Cedula,
					NombrePaciente as NombrePaciente,
					NumeroOrden as NumeroOrden,
					Estado as Estado,
					FechaIngreso as FechaIngreso,
					IdLaboratorio as IdLaboratorio
				from Resultados
				where IdentificacionPac = @i_identificacion
				and IdLaboratorio in (18, 31)
			end
			else
			begin
				select IdentificacionPac as Cedula,
					NombrePaciente as NombrePaciente,
					NumeroOrden as NumeroOrden,
					Estado as Estado,
					FechaIngreso as FechaIngreso,
					IdLaboratorio as IdLaboratorio
				from Resultados
				where IdentificacionPac = @i_identificacion
				and IdLaboratorio = @i_idLab
			end			
		end
		else
		begin
			--datos del paciente total
			select IdentificacionPac as Cedula,
				NombrePaciente as NombrePaciente,
				NumeroOrden as NumeroOrden,
				Estado as Estado,
				FechaIngreso as FechaIngreso,
				IdLaboratorio as IdLaboratorio
			from Resultados
			where IdentificacionPac = @i_identificacion
		end
	end

END

GO
