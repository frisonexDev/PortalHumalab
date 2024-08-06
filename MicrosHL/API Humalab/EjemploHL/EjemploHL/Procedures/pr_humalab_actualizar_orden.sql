/************************************************************************
*	Stored procedure: pr_humalab_actualizar_orden						*
*	Base de datos: DbPortalHumalab						  			    *
*	Elaborado por: José Guarnizo							            *
*----------------------------------------------------------------------	*
*					DESCRIPCION DEL PROCEDIMIENTO						*
*	En este Procedimiento actualiza la orden                            *
*	                                                                    *
*----------------------------------------------------------------------	*
*					BITACORA DE MODIFICACIONES							*
*	FECHA AUTOR RAZON													*
*						                                                *
*----------------------------------------------------------------------	*/
IF NOT EXISTS (SELECT * FROM  sys.procedures WHERE NAME = 'pr_humalab_actualizar_orden')	
	EXEC('Create Procedure dbo.pr_humalab_actualizar_orden As')
go

ALTER PROCEDURE [dbo].[pr_humalab_actualizar_orden](
    @i_accion CHAR(2),
	@idOrden INT=NULL,
	@idPedido INT=NULL,
	@estadoOrden INT=NULL,
	@usuarioCreacion INT=NULL,
	@fechaCreacion DATE=NULL
)

as

IF(@i_accion='M')
BEGIN

	UPDATE Orden 
	SET IdPedido=@idPedido, Estado =  @estadoOrden, UsuarioModificacion = @usuarioCreacion, FechaModificacion=@fechaCreacion
	WHERE IdOrden=@idOrden

END

IF(@i_accion='M2')
BEGIN

	UPDATE Orden 
	SET IdPedido=null, Estado =  @estadoOrden, UsuarioModificacion = @usuarioCreacion, FechaModificacion=@fechaCreacion
	WHERE IdOrden=@idOrden

END

GO