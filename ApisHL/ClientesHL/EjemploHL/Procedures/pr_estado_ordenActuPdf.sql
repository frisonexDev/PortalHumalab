/************************************************************************
*	Stored procedure: pr_estado_ordenActuPdf					        *
*	Base de datos: DbPortalHumalab						  			    *
*	Elaborado por: José Guarnizo						                *
*----------------------------------------------------------------------	*
*					DESCRIPCION DEL PROCEDIMIENTO						*
*	En este Procedimiento actualiza el estado de la orden cuando lis    *
*   valida las pruebas                                                  *
*	                                                                    *
*----------------------------------------------------------------------	*
*					BITACORA DE MODIFICACIONES							*
*	FECHA AUTOR RAZON													*
*						                                                *
*----------------------------------------------------------------------	*/
IF NOT EXISTS (SELECT * FROM  sys.procedures WHERE NAME = 'pr_estado_ordenActuPdf')	
	EXEC('Create Procedure dbo.pr_estado_ordenActuPdf As')
go

ALTER PROCEDURE [dbo].[pr_estado_ordenActuPdf](
	@i_accion char,
	@i_idEstado int = null,
	@codigoBarra varchar(100) = null,
	@basePdf XML = null,
	@codigoBarraHuma varchar(100) = null,
	@idLab int = null

)

as

declare @i_nombreEstado varchar(50),
	@idResultadoPend int, 
	@idValidPen int,
	@idValidado int,
	@idOrden int,
	@idValidadoPrueba int,
	@idPruebaOrd int

if @i_accion = 'M'
BEGIN
	
	--actualiza estado a resultado pendiente
	if @i_idEstado = 3
	begin
				
		select @idResultadoPend = CD.IdCatalogoDetalle 
		from CatalogoMaestro CM 
		INNER JOIN CatalogoDetalle CD ON CM.IdCatalogoMaestro = CD.IdCatalogoMaestro
		where CM.Nombre='EstadoOrden' AND CD.Valor='RESTP'

		--actualiza la orden
		update Orden
		set Estado = @idResultadoPend
		--where CodigoBarra = @codigoBarra
		where Resultados =  @codigoBarra

		select '00' as resultado
	end

	--actualiza estado a validacion pendiente
	if @i_idEstado = 4
	begin
		select @idValidPen = CD.IdCatalogoDetalle 
		from CatalogoMaestro CM 
		INNER JOIN CatalogoDetalle CD ON CM.IdCatalogoMaestro = CD.IdCatalogoMaestro
		where CM.Nombre='EstadoOrden' AND CD.Valor='VALDP'

		--actualiza la orden
		update Orden
		set Estado = @idValidPen
		--where CodigoBarra = @codigoBarra
		where Resultados =  @codigoBarra

		select '00' as resultado

	end

	--actualiza estado a validado
	if @i_idEstado = 5
	begin
		
		--obtiene id de la orden
		select @idOrden = IdOrden
		from Orden
		--where CodigoBarra = @codigoBarra
		where Resultados =  @codigoBarra

		--actualiza la orden a validado
		select @idValidado = CD.IdCatalogoDetalle 
		from CatalogoMaestro CM 
		INNER JOIN CatalogoDetalle CD ON CM.IdCatalogoMaestro = CD.IdCatalogoMaestro
		where CM.Nombre='EstadoOrden' AND CD.Valor='VALD'

		update Orden
		set Estado = @idValidado
		--where CodigoBarra = @codigoBarra	
		where Resultados =  @codigoBarra

		--actualiza las pruebas de la orden a validadas
		select @idValidadoPrueba = CD.IdCatalogoDetalle 
		from CatalogoMaestro CM 
		INNER JOIN CatalogoDetalle CD ON CM.IdCatalogoMaestro = CD.IdCatalogoMaestro
		where CM.Nombre='EstadoPrueba' AND CD.Valor='VALD'

		update Prueba
		set Estado = @idValidadoPrueba
		where IdOrden = @idOrden		

		select '00' as resultado

	end

END


if @i_accion = 'P'
BEGIN
	
	if exists (select IdOrden from Orden where Resultados = @codigoBarra)
	begin

		if @i_idEstado = 3
		begin
			
			select @idResultadoPend = CD.IdCatalogoDetalle 
			from CatalogoMaestro CM 
			INNER JOIN CatalogoDetalle CD ON CM.IdCatalogoMaestro = CD.IdCatalogoMaestro
			where CM.Nombre='EstadoOrden' AND CD.Valor='RESTP'

			--obtiene id de la orden
			select @idOrden = IdOrden
			from Orden			
			where Resultados = @codigoBarra
			and CodigoBarra = @codigoBarraHuma
			and idLaboratorio = @idLab
			and Eliminado != 1

			if @idOrden != null or @idOrden != ''
			begin
				--actualiza la orden
				update Orden
					set Estado = @idResultadoPend,
					ResultadoFinal = @basePdf
				where Resultados =  @codigoBarra
				and CodigoBarra = @codigoBarraHuma
				and idLaboratorio = @idLab

				select '00' as resultado
			end
			else
			begin
				select '01' as resultado
			end

			
		end

		if @i_idEstado = 4
		begin

			select @idValidPen = CD.IdCatalogoDetalle 
			from CatalogoMaestro CM 
			INNER JOIN CatalogoDetalle CD ON CM.IdCatalogoMaestro = CD.IdCatalogoMaestro
			where CM.Nombre='EstadoOrden' AND CD.Valor='VALDP'

			--obtiene id de la orden
			select @idOrden = IdOrden
			from Orden			
			where Resultados = @codigoBarra
			and CodigoBarra = @codigoBarraHuma
			and idLaboratorio = @idLab
			and Eliminado != 1

			if @idOrden != null or @idOrden != ''
			begin
				--actualiza la orden
				update Orden
					set Estado = @idValidPen,
					ResultadoFinal = @basePdf
				where Resultados =  @codigoBarra
				and CodigoBarra = @codigoBarraHuma
				and idLaboratorio = @idLab

				select '00' as resultado
			end
			else
			begin
				select '01' as resultado
			end

		end

		if @i_idEstado = 5
		begin
		
			--obtiene id de la orden
			select @idOrden = IdOrden
			from Orden			
			where Resultados = @codigoBarra
			and CodigoBarra = @codigoBarraHuma
			and idLaboratorio = @idLab
			and Eliminado != 1

			if @idOrden != null or @idOrden != ''
			begin
				--actualiza la orden a validado
				select @idValidado = CD.IdCatalogoDetalle 
				from CatalogoMaestro CM 
				INNER JOIN CatalogoDetalle CD ON CM.IdCatalogoMaestro = CD.IdCatalogoMaestro
				where CM.Nombre='EstadoOrden' AND CD.Valor='VALD'

				update Orden
					set Estado = @idValidado,
					ResultadoFinal = @basePdf
				where Resultados =  @codigoBarra
				and CodigoBarra = @codigoBarraHuma
				and idLaboratorio = @idLab

				--actualiza las pruebas de la orden a validadas
				select @idValidadoPrueba = CD.IdCatalogoDetalle 
				from CatalogoMaestro CM 
				INNER JOIN CatalogoDetalle CD ON CM.IdCatalogoMaestro = CD.IdCatalogoMaestro
				where CM.Nombre='EstadoPrueba' AND CD.Valor='VALD'

				update Prueba
					set Estado = @idValidadoPrueba
				where IdOrden = @idOrden		

				select '00' as resultado

			end
			else
			begin
				select '01' as resultado
			end

		end

	end
	else
	begin
		select '01' as resultado		
	end

end

if @i_accion = 'C'
BEGIN
	
	declare @base64 xml

	if exists (select top 1 IdOrden from Orden where CodigoBarra = @codigoBarraHuma)
	begin
		
		select @base64 = ResultadoFinal
		from Orden
		where CodigoBarra = @codigoBarraHuma
		and Eliminado != 1

		select @base64 as resultado

	end
	else
	begin
		select '01' as resultado
	end

END

GO