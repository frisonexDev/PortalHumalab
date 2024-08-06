/************************************************************************
*	Stored procedure: pr_humalab_pedido_observacion						*
*	Base de datos: DbPortalHumalab						  			    *
*	Elaborado por: José Guarnizo							            *
*----------------------------------------------------------------------	*
*					DESCRIPCION DEL PROCEDIMIENTO						*
*	En este Procedimiento consulta el catalogo detalle                   *
*	                                                                    *
*----------------------------------------------------------------------	*
*					BITACORA DE MODIFICACIONES							*
*	FECHA AUTOR RAZON													*
*						                                                *
*----------------------------------------------------------------------	*/
IF NOT EXISTS (SELECT * FROM  sys.procedures WHERE NAME = 'pr_humalab_pedido_observacion')	
    EXEC('Create Procedure dbo.pr_humalab_pedido_observacion As')
go

ALTER PROCEDURE [dbo].[pr_humalab_pedido_observacion](
    @i_accion CHAR(1),
    @idPedido INT,
    @idObservacion INT,
    @usuarioCreacion INT,
    @fechaCreacion Date
)

as

IF(@i_accion = 'I')
BEGIN

	INSERT PedidoObservacion Values (@idPedido,@idObservacion,@usuarioCreacion,@fechaCreacion,null,null,null,null,null)

END

GO