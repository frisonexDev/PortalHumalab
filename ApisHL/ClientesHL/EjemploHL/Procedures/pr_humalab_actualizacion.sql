/************************************************************************
*	Stored procedure: pr_humalab_actualizacion							*
*	Base de datos: DbPortalHumalab						  			    *
*	Elaborado por: José Guarnizo						                *
*----------------------------------------------------------------------	*
*					DESCRIPCION DEL PROCEDIMIENTO						*
*	En este Procedimiento realiza actualizacion                         *
*	                                                                    *
*----------------------------------------------------------------------	*
*					BITACORA DE MODIFICACIONES							*
*	FECHA AUTOR RAZON													*
*						                                                *
*----------------------------------------------------------------------	*/
IF NOT EXISTS (SELECT * FROM  sys.procedures WHERE NAME = 'pr_humalab_actualizacion')	
	EXEC('Create Procedure dbo.pr_humalab_actualizacion As')
go

ALTER PROCEDURE [dbo].[pr_humalab_actualizacion](
	@i_accion CHAR(2),
	@idOrden INT = NULL,
	@idPrueba INT=NULL,
	@codigoBarra VARCHAR=NULL,
	@estado INT=NULL,
	@usuarioCreacion INT=NULL,
	@fechaCreacion DATETIME=NULL
)

as

DECLARE @elimandoLogico AS INT=1,
		@estadoPorProcesar AS INT =0
BEGIN

	IF @i_accion = 'M'
	BEGIN	

	select @estadoPorPRocesar=CD.IdCatalogoDetalle from CatalogoMaestro CM 
	INNER JOIN CatalogoDetalle CD ON CM.IdCatalogoMaestro = CD.IdCatalogoMaestro
	where CM.Nombre='EstadoPrueba' AND CD.Valor='PPRC'


		UPDATE Prueba SET
		Estado=@estado,UsuarioModificacion=@usuarioCreacion, FechaModificacion=@fechaCreacion
		WHERE  IdPrueba=@idPrueba AND Estado=@estadoPorProcesar
		
	END

	

END

GO