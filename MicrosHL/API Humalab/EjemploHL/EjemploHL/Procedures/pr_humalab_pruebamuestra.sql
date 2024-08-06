/************************************************************************
*	Stored procedure: pr_humalab_pruebamuestra							*
*	Base de datos: DbPortalHumalab						  			    *
*	Elaborado por: José Guarnizo							            *
*----------------------------------------------------------------------	*
*					DESCRIPCION DEL PROCEDIMIENTO						*
*	En este Procedimiento realiza acciones de pruebas y muestras        *
*	                                                                    *
*----------------------------------------------------------------------	*
*					BITACORA DE MODIFICACIONES							*
*	FECHA AUTOR RAZON													*
*						                                                *
*----------------------------------------------------------------------	*/
IF NOT EXISTS (SELECT * FROM  sys.procedures WHERE NAME = 'pr_humalab_pruebamuestra')	
	EXEC('Create Procedure dbo.pr_humalab_pruebamuestra As')
go

ALTER PROCEDURE [dbo].[pr_humalab_pruebamuestra](
	@i_accion CHAR(2),
	@idPrueba INT=NULL,
	@idMuestra INT=NULL,
	@usuarioCreacion INT=NULL,
	@fechaCreacion DATETIME=NULL
)

as

DECLARE @contar AS INT,
		@elimanodLogico AS INT=1,
		@idPruebaNew INT,
		@idMuestraNew INT,
		@usuarioCreacionNew INT,
		@fechaCreacionNew DATETIME

--SELECT @contar = COUNT(IdPruebaMuestra) 
--FROM PruebaMuestra 
--WHERE IdPrueba = @idPrueba 
--AND IdMuestra = @idMuestra

select @idPruebaNew = @idPrueba
select @idMuestraNew = @idMuestra
select @usuarioCreacionNew = @usuarioCreacion
select @fechaCreacionNew = @fechaCreacion

BEGIN

--Insertar
	IF @i_accion = 'I'
	BEGIN	

		--IF(@contar<1)
		--BEGIN
		--	--INSERT INTO PruebaMuestra 
		--	--VALUES(@idPrueba, @idMuestra, @usuarioCreacion, 
		--	--	   @fechaCreacion, NULL, NULL, 
		--	--	   NULL, NULL, 0)
		--	INSERT INTO PruebaMuestra 
		--	VALUES(@idPruebaNew, @idMuestraNew, @usuarioCreacionNew, 
		--		   @fechaCreacionNew, NULL, NULL, 
		--		   NULL, NULL, 0)
		--END
		IF not exists (select IdPruebaMuestra from PruebaMuestra where IdPrueba = @idPrueba
					   AND IdMuestra = @idMuestra)
		BEGIN
			INSERT INTO PruebaMuestra 
			VALUES(@idPruebaNew, @idMuestraNew, @usuarioCreacionNew, 
				   @fechaCreacionNew, NULL, NULL, 
				   NULL, NULL, 0)
		END
		
	END

	--Modificar
	IF @i_accion = 'M'
	BEGIN	
			
		--IF(@contar>0)
		--	BEGIN

		--		UPDATE PruebaMuestra 
		--		SET IdPrueba=@idPrueba ,
		--			IdMuestra=@idMuestra, 
		--			UsuarioModificacion=@usuarioCreacion, 
		--			FechaModificacion=@fechaCreacion
		--		WHERE IdMuestra=@idMuestra 
		--		AND IdPrueba=@idPrueba

		--	END
		IF exists (select IdPruebaMuestra from PruebaMuestra where IdPrueba = @idPrueba
				   AND IdMuestra = @idMuestra)
		BEGIN
			UPDATE PruebaMuestra 
			SET IdPrueba=@idPrueba ,
				IdMuestra=@idMuestra, 
				UsuarioModificacion=@usuarioCreacion, 
				FechaModificacion=@fechaCreacion
			WHERE IdMuestra=@idMuestra 
			AND IdPrueba=@idPrueba

		END
		
	END

	--Consultar
	IF @i_accion = 'C'
	BEGIN	
			
		SELECT IdPrueba, IdMuestra 
		FROM PruebaMuestra
		WHERE IdMuestra=@idMuestra OR IdPrueba=@idPrueba
		
	END

	--Consultar
	IF @i_accion = 'C1'
	BEGIN	
			
		SELECT IdPrueba, IdMuestra 
		FROM PruebaMuestra 
		WHERE IdPrueba=@idPrueba 
		
	END

	--Eliminar
	IF @i_accion = 'E'
	BEGIN	
					
		UPDATE PruebaMuestra 
		SET UsuarioEliminacion=@usuarioCreacion,
			FechaEliminacion=@fechaCreacion,
			Eliminado=@elimanodLogico
		WHERE IdMuestra=@idMuestra 
		AND IdPrueba=@idPrueba
		
	END

END

GO