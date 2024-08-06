/************************************************************************
*	Stored procedure: pr_humalab_pedidoAct						        *
*	Base de datos: DbPortalHumalab						  			    *
*	Elaborado por: Jose Guarnizo						                *
*----------------------------------------------------------------------	*
*					DESCRIPCION DEL PROCEDIMIENTO						*
*	En este Procedimiento consulta si un pedido fue actualizado         *
*	                                                                    *
*----------------------------------------------------------------------	*
*					BITACORA DE MODIFICACIONES							*
*	FECHA AUTOR RAZON													*
*						                                                *
*----------------------------------------------------------------------	*/
IF NOT EXISTS (SELECT * FROM  sys.procedures WHERE NAME = 'pr_humalab_pedidoAct')	
	EXEC('Create Procedure dbo.pr_humalab_pedidoAct As')
go

ALTER PROCEDURE [dbo].[pr_humalab_pedidoAct](
	@i_accion Char(2),
	@i_idPedido int
)

as

declare @i_numOrden int,
	@i_fechaMod varchar(20)

if(@i_accion='C')
BEGIN

	if exists( select 1 from Pedido where IdPedido = @i_idPedido)
	begin
		
		select @i_fechaMod = FechaModificacion
		from Orden
		where IdPedido = @i_idPedido

		if @i_fechaMod != NULL or @i_fechaMod != ''
		begin
						
			select @i_numOrden = IdOrden 
			from Orden
			where IdPedido = @i_idPedido

			select @i_fechaMod as Actualizado

		end
		else
		begin			
			select 'NO'	as Actualizado
			
		end

	end

END

GO