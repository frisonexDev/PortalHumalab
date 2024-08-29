/************************************************************************
*	Stored procedure: pr_humalab_muestraobs								*
*	Base de datos: DbPortalHumalab						  			    *
*	Elaborado por: José Guarnizo							            *
*----------------------------------------------------------------------	*
*					DESCRIPCION DEL PROCEDIMIENTO						*
*	En este Procedimiento realiza acciones de muestras observacion      *
*	                                                                    *
*----------------------------------------------------------------------	*
*					BITACORA DE MODIFICACIONES							*
*	FECHA AUTOR RAZON													*
*						                                                *
*----------------------------------------------------------------------	*/
IF NOT EXISTS (SELECT * FROM  sys.procedures WHERE NAME = 'pr_humalab_muestraobs')	
	EXEC('Create Procedure dbo.pr_humalab_muestraobs As')
go

ALTER PROCEDURE [dbo].[pr_humalab_muestraobs](
	@i_accion CHAR(1),
	@idObservacionM INT=NULL,
	@idMuestra INT=NULL,
	@descripcion VARCHAR(100)=NULL,
	@usuarioCreacion INT=NULL,
	@fechaCreacion DATETIME=NULL
)

as

DECLARE @contarMuestraObs AS INT,
		@elimanodLogico AS INT=1,
		@retiradas AS INT

SELECT @contarMuestraObs=COUNT(IdObservacionM) FROM ObservacionM WHERE IdMuestra=@idMuestra

BEGIN

--Insertar
	IF @i_accion = 'I'
	BEGIN	

		IF(@contarMuestraObs<1)
			BEGIN

				INSERT INTO ObservacionM VALUES(@idMuestra, @descripcion,null, @usuarioCreacion, @fechaCreacion, NULL, NULL, NULL, NULL,NULL,0)

			END
		
	END

	--Modificar
	IF @i_accion = 'M'
	BEGIN	
			
		IF(@contarMuestraObs>0)
			BEGIN

				UPDATE ObservacionM SET
				
				Descripcion = @descripcion, UsuarioModificacion=@usuarioCreacion, FechaModificacion=@fechaCreacion
				WHERE IdObservacionM=@idObservacionM

			END
		
	END

	--Consultar
	IF @i_accion = 'C'
	BEGIN	
			
		SELECT * FROM ObservacionM WHERE IdMuestra=IdMuestra
		
	END


	--Eliminar
	IF @i_accion = 'E'
	BEGIN	
			
		
		UPDATE ObservacionM SET
				UsuarioEliminacion=@usuarioCreacion,
				FechaEliminacion=@fechaCreacion,
				Eliminado=@elimanodLogico
				WHERE IdMuestra=@idMuestra
		
	END

END

GO