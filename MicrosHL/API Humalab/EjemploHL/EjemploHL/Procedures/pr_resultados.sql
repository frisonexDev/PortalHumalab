/************************************************************************
*	Stored procedure: pr_resultados					                    *
*	Base de datos: DbPortalHumalab						  			    *
*	Elaborado por: José Guarnizo						                *
*----------------------------------------------------------------------	*
*					DESCRIPCION DEL PROCEDIMIENTO						*
*	En este Procedimiento actualiza e inserta resultados del medico     *
*                                                                       *
*	                                                                    *
*----------------------------------------------------------------------	*
*					BITACORA DE MODIFICACIONES							*
*	FECHA AUTOR RAZON													*
*																		*
*----------------------------------------------------------------------	*/
IF NOT EXISTS (SELECT * FROM  sys.procedures WHERE NAME = 'pr_resultados')	
	EXEC('Create Procedure dbo.pr_resultados As')
go

ALTER PROCEDURE [dbo].[pr_resultados] (
	@i_accion char,
	@i_idLab int,
	@i_idenpac varchar(100),
	@i_numOrden varchar(100),
	@i_informe xml,
	@i_estado varchar(100),
	@i_idEstado int = null,
	@i_genero varchar(20) = null,
	@i_fechaNacimiento varchar(20) = null,
	@i_fechaIngrenso varchar(100) = null,
	@i_usuario varchar(50) = null,
	@i_nombrePaciente varchar(100) = null,
	@i_idSede varchar(100) = null,
	@i_nombreSede varchar(200) = null
)

as

declare @estadoIni varchar(50),
	@estValidado int,
	@estValidadoPend int,
	@estResultPend int,
	@estOrden int,
	@estValPrue int,	
	@estaPrueba int,
	@numOrden int

select @estadoIni = Estado
from Resultados
where NumeroOrden = @i_numOrden
and IdLaboratorio = @i_idLab

--Obtener el id del estado de la orden a validado, validacion pendiente y resultado pendiente
select @estOrden = IdCatalogoMaestro
from CatalogoMaestro
where Nombre = 'EstadoOrden'

select @estValidado = IdCatalogoDetalle
from CatalogoDetalle
where IdCatalogoMaestro = @estOrden
and Valor = 'VALD'

select @estValidadoPend = IdCatalogoDetalle
from CatalogoDetalle
where IdCatalogoMaestro = @estOrden
and Valor = 'VALDP'

select @estResultPend = IdCatalogoDetalle
from CatalogoDetalle
where IdCatalogoMaestro = @estOrden
and Valor = 'RESTP'

--Obtener el id del estado de las pruebas de la orden en validado
select @estaPrueba = IdCatalogoMaestro
from CatalogoMaestro
where Nombre = 'EstadoPrueba'

select @estValPrue = IdCatalogoDetalle
from CatalogoDetalle
where IdCatalogoMaestro = @estaPrueba
and Valor = 'VALD'


if @i_accion = 'I'
begin

	if not exists (select IdResultados from Resultados where IdLaboratorio = @i_idLab
			   and NumeroOrden = @i_numOrden)
	begin		
		--inserta el nuevo registro
		insert into Resultados
		values(@i_idLab, @i_idenpac, @i_numOrden,
			   @i_informe, GETDATE(), @i_estado,
			   null, @i_idSede, @i_nombreSede,
			   null, null, null, 
			   @i_genero, @i_fechaNacimiento, 
			   @i_fechaIngrenso, @i_usuario, @i_nombrePaciente)

		select '00' Resultado
	end
	else
	begin

		if exists (select IdResultados from Resultados where IdLaboratorio = @i_idLab
			   and NumeroOrden = @i_numOrden)
		begin
			
			if @estadoIni != @i_estado or @estadoIni = @i_estado
			begin
				
				select @numOrden = IdOrden
				from Orden
				where Resultados = @i_numOrden

				--actualiza el estado en la tabla Orden y Prueba
				if @i_estado = 'Validada'
				begin					
					Update Orden
					set Estado = @estValidado
					where IdOrden = @numOrden
					
					update Prueba
					set Estado = @estValPrue
					where IdOrden = @numOrden
				end

				if @i_estado = 'Resultados Pendiente'
				begin					
					Update Orden
					set Estado = @estResultPend
					where IdOrden = @numOrden
				end

				if @i_estado = 'Validación Pendiente'
				begin
					Update Orden
					set Estado = @estValidadoPend
					where IdOrden = @numOrden
				end

				--actualiza el estado del registro, el base64 en la tabla Resultados
				--y el id de la sede y nombre se la sede
				Update Resultados
				set Estado = @i_estado, Informe = @i_informe,
					aux1 = @i_idSede, aux2 = @i_nombreSede
				where NumeroOrden = @i_numOrden
				and IdLaboratorio = @i_idLab

				select '00' Resultado

			end
			else
			begin
				select '01' Resultado
			end

		end		
	end

end

GO