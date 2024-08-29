/************************************************************************
*	Stored procedure: pr_humalab_facturas						        *
*	Base de datos: DbPortalHumalab						  			    *
*	Elaborado por: Jose Guarnizo						                *
*----------------------------------------------------------------------	*
*					DESCRIPCION DEL PROCEDIMIENTO						*
*	En este Procedimiento facturas del cliente                          *
*	                                                                    *
*----------------------------------------------------------------------	*
*					BITACORA DE MODIFICACIONES							*
*	FECHA AUTOR RAZON													*
*						                                                *
*----------------------------------------------------------------------	*/
IF NOT EXISTS (SELECT * FROM sys.procedures WHERE name = 'pr_humalab_facturas')	
	EXEC('Create Procedure dbo.pr_humalab_facturas As')
GO

ALTER PROCEDURE [dbo].[pr_humalab_facturas](
	@i_accion CHAR(1),		
	----Usuario--	

	@i_identificador VARCHAR(15)=NULL,
	@i_tipo VARCHAR(15)=NULL,
	@i_desde VARCHAR(100)=NULL,
	@i_hasta VARCHAR(100)=NULL,
	@i_todos varchar(10) = null,
	

	@i_numeroFactura VARCHAR(100)=1,
	@i_identificacionUsuario VARCHAR(100)=2,
	@i_totalFactura  VARCHAR(100)=3,
	@i_totalMuestras  VARCHAR(100)=4,
	@i_estado  VARCHAR(100)=NULL,
	@i_usuarioCreacion  VARCHAR(100)=5,
	@i_idOrden varchar(100) = null --15/01/2024		
)

AS

declare @i_IdGalileo int,@i_itemsActualizados int
declare @estadoOrdenValidado int,@estadoPruebaValidado int
declare @estadoOrdenFacturado int,@estadoPruebaFacturado int,
	@idOrdenFinal int, @idOrdenEstados int, @estadoFacturadas int,
	@idEstadoPrueba int, @idRechazada int

--15/01/2024
select @idOrdenFinal = IdOrden
from Orden
where CodigoBarra = @i_idOrden

select @idOrdenEstados = IdCatalogoMaestro
from CatalogoMaestro
where Nombre = 'EstadoOrden'

select @estadoFacturadas = IdCatalogoDetalle
from CatalogoDetalle
where IdCatalogoMaestro = @idOrdenEstados
and Valor = 'FACT'

--estados prueba
select @idEstadoPrueba = IdCatalogoMaestro
from CatalogoMaestro
where Nombre = 'EstadoPrueba'

select @idRechazada = IdCatalogoDetalle
from CatalogoDetalle
where IdCatalogoMaestro = @idEstadoPrueba
and Valor = 'RCHZ'

select @estadoOrdenValidado = IdCatalogoDetalle from CatalogoDetalle where IdCatalogoMaestro = (select IdCatalogoMaestro from CatalogoMaestro where nombre = 'EstadoOrden')
and valor = 'VALD'

select @estadoOrdenFacturado = IdCatalogoDetalle from CatalogoDetalle where IdCatalogoMaestro = (select IdCatalogoMaestro from CatalogoMaestro where nombre = 'EstadoOrden')
and valor = 'FACT'



select @estadoPruebaValidado = IdCatalogoDetalle from CatalogoDetalle where IdCatalogoMaestro = (select IdCatalogoMaestro from CatalogoMaestro where nombre = 'EstadoPrueba')
and valor = 'VALD'

select @estadoPruebaFacturado = IdCatalogoDetalle from CatalogoDetalle where IdCatalogoMaestro = (select IdCatalogoMaestro from CatalogoMaestro where nombre = 'EstadoPrueba')
and valor = 'FACT'


IF @i_accion = 'I'
BEGIN	
	RETURN 0
END

IF @i_accion = 'M'
BEGIN
  BEGIN TRANSACTION;  
BEGIN TRY  
			set @i_itemsActualizados = 0
			
			insert into Facturacion (NumeroFactura,IdentificacionUsuario,TotalFactura,TotalMuestras,Estado,UsuarioCreacion,FechaCreacion ,Elimanado)
			values(@i_numeroFactura,@i_identificacionUsuario,@i_totalFactura,@i_totalMuestras,@estadoFacturadas,@i_usuarioCreacion,getdate(),'False')
					
			if @@identity>=0
			begin					

					insert into FacturaOrden (IdOrden,IdFactura)
					SELECT 
					distinct o.idOrden as IdOrden, @@identity
					FROM Pedido p INNER JOIN Orden o ON p.IdPedido = o.IdPedido
					INNER JOIN Prueba pr ON o.IdOrden = pr.IdOrden
					where p.IdOperador = (SELECT top 1 IdOperadorLogistico from Cliente where CodClienteCta = @i_identificador)
					and o.Estado = @estadoOrdenValidado
					AND o.IdOrden = @idOrdenFinal --15/01/2024
					and (cast(o.FechaCreacion as date) >= cast(@i_desde as date) 
					and cast(o.FechaCreacion as date)<=cast(@i_hasta as date))								

				if @@ROWCOUNT>=0
				begin	
		
						IF OBJECT_ID(N'tempdb..#items') IS NOT NULL
						BEGIN
						DROP TABLE #items
						END								
									SELECT 	pr.idPrueba,o.idOrden as orden,o.CodigoBarra as IdOrden,CodigoExamen as codigo,pr.Nombre AS Nombre,1 cantidad,pr.Precio,o.Estado,(SELECT Nombre FROM CatalogoDetalle where IdCatalogoDetalle = o.Estado) AS EstadoOrden
									into #items
									FROM Pedido p INNER JOIN Orden o ON p.IdPedido = o.IdPedido
									INNER JOIN Prueba pr ON o.IdOrden = pr.IdOrden
									where p.IdOperador = (SELECT top 1 IdOperadorLogistico from Cliente where Identificacion = @i_identificador)
									and o.Estado = @estadoOrdenValidado 
									AND o.IdOrden = @idOrdenFinal --15/01/2024
									and (cast(o.FechaCreacion as date) >= cast(@i_desde as date) 
									and cast(o.FechaCreacion as date)<=cast(@i_hasta as date))
									and pr.Estado != @idRechazada

								update orden
									set estado  = @estadoOrdenFacturado,fechaModificacion=getdate()
								where  idorden in ( select  distinct orden from #items)

								update Prueba
									set estado  = @estadoPruebaFacturado,fechaModificacion=getdate()
								where  idprueba in (select distinct idprueba from #items)


					set @i_itemsActualizados = @@ROWCOUNT
		
				end

			end 
			 COMMIT TRANSACTION;  
			select	@i_itemsActualizados as actualizados
		END TRY  
		BEGIN CATCH  
		select	@i_itemsActualizados as actualizados
		 ROLLBACK TRANSACTION;  
		END CATCH;  
	RETURN 0
END

IF @i_accion = 'C'
BEGIN

	if @i_todos = '-1'
	begin
		
		SELECT 
			pr.idPrueba
			,o.CodigoBarra as IdOrden
			,CodigoExamen as codigo
			,pr.Nombre AS Nombre,
			1 cantidad,
			pr.Precio,
			o.Estado ,
			(SELECT Nombre FROM CatalogoDetalle where IdCatalogoDetalle = o.Estado) AS estadoFactura
		FROM Pedido p INNER JOIN Orden o ON p.IdPedido = o.IdPedido
		INNER JOIN Prueba pr ON o.IdOrden = pr.IdOrden
		where o.Estado = @i_tipo  
		and (cast(o.FechaCreacion as date) >= cast(@i_desde as date) 
		and cast(o.FechaCreacion as date)<=cast(@i_hasta as date))
		AND pr.Estado != @idRechazada
		RETURN 0

	end
	else
	begin
	
		SELECT 
			pr.idPrueba
			,o.CodigoBarra as IdOrden
			,CodigoExamen as codigo
			,pr.Nombre AS Nombre,
			1 cantidad,
			pr.Precio,
			o.Estado ,
			(SELECT Nombre FROM CatalogoDetalle where IdCatalogoDetalle = o.Estado) AS estadoFactura
		FROM Pedido p INNER JOIN Orden o ON p.IdPedido = o.IdPedido
		INNER JOIN Prueba pr ON o.IdOrden = pr.IdOrden
		where p.IdOperador = (SELECT top 1 IdOperadorLogistico from Cliente where Identificacion =  @i_identificador)
		and o.Estado = @i_tipo  
		and (cast(o.FechaCreacion as date) >= cast(@i_desde as date) 
		and cast(o.FechaCreacion as date)<=cast(@i_hasta as date))
		AND pr.Estado != @idRechazada
		RETURN 0

	end

END

IF @i_accion = 'G'
BEGIN

  RETURN 0
END

RAISERROR ('El código de la acción es incorrecto.',16,1)

GO
