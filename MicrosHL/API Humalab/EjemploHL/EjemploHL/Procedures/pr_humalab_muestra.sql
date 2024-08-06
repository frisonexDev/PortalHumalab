/************************************************************************
*	Stored procedure: pr_humalab_muestra								*
*	Base de datos: DbPortalHumalab						  			    *
*	Elaborado por: José Guarnizo						                *
*----------------------------------------------------------------------	*
*					DESCRIPCION DEL PROCEDIMIENTO						*
*	En este Procedimiento realiza acciones de las muestras              *
*	                                                                    *
*----------------------------------------------------------------------	*
*					BITACORA DE MODIFICACIONES							*
*	FECHA AUTOR RAZON													*
*						                                                *
*----------------------------------------------------------------------	*/
IF NOT EXISTS (SELECT * FROM  sys.procedures WHERE NAME = 'pr_humalab_muestra')	
	EXEC('Create Procedure dbo.pr_humalab_muestra As')
go

ALTER PROCEDURE [dbo].[pr_humalab_muestra](
	@i_accion CHAR(2),
	@idMuestra INT=NULL,
	@idOrden INT=NULL,
	@idMuestraGalileo INT=NULL,
	@nombre VARCHAR(50)=NULL,
	@muestraAlterna VARCHAR(50)=NULL,
	@recipiente VARCHAR(20)=NULL,
	@codigoBarra VARCHAR(20)=NULL,
	@estadoMuestra INT=NULL,
	@usuarioCreacion INT=NULL,
	@fechaCreacion DATETIME=NULL,
	@idMuestra2 INT=NULL
)

as

DECLARE @contar AS INT,
		@elimanodLogico AS INT=1,
		@codigoBarraOrden AS VARCHAR(15),
		@idMuestraGalileoNew int,
		@idOrdenNew int,
		@nombreNew varchar(50),
		@recipienteNew varchar(20),
		@muestraAlternaNew varchar(50),
		@codigoBarraNew varchar(20),
		@estadoMuestraNew int,
		@usuarioCreacionNew int,
		@fechaCreacionNew datetime

select @idMuestraGalileoNew = @idMuestraGalileo
select @idOrdenNew = @idOrden
select @nombreNew = @nombre
select @recipienteNew = @recipiente
select @muestraAlternaNew = @muestraAlterna
select @codigoBarraNew = @codigoBarra
select @estadoMuestraNew = @estadoMuestra
select @usuarioCreacionNew = @usuarioCreacion
select @fechaCreacionNew = @fechaCreacion

BEGIN

--Insertar
	IF @i_accion = 'I'
	BEGIN	

		--INSERT INTO Muestra 
		--VALUES(@idMuestraGalileo, @idOrden, @nombre, 
		--	   @recipiente, @muestraAlterna,@codigoBarra,
		--	   @estadoMuestra, @usuarioCreacion, @fechaCreacion, 
		--	   NULL, NULL, NULL, NULL, 0)				
		--SELECT SCOPE_IDENTITY() as IdMuestra
		INSERT INTO Muestra 
		VALUES(@idMuestraGalileoNew, @idOrdenNew, @nombreNew, 
			   @recipienteNew, @muestraAlternaNew, @codigoBarraNew,
			   @estadoMuestraNew, @usuarioCreacionNew, @fechaCreacionNew, 
			   NULL, NULL, NULL, NULL, 0)				
		SELECT SCOPE_IDENTITY() as IdMuestra
		
	END

	--Modificar
	IF @i_accion = 'M'
	BEGIN	
			
		IF(@contar>0)
			BEGIN
				UPDATE Muestra 
				SET	EstadoMuestra=@estadoMuestra, 
					UsuarioModificacion=@usuarioCreacion, 
					FechaModificacion=@fechaCreacion
				WHERE IdMuestraGalileo=@idMuestraGalileo 
				AND CodigoBarra = @codigoBarra

			END
		
	END

	--Consultar
	IF @i_accion = 'C'
	BEGIN	
			
		SELECT IdMuestra AS 'IdMuestra' 
		FROM Muestra 
		WHERE IdOrden = @idOrden 
		AND IdMuestraGalileo = @idMuestraGalileo 
		AND Eliminado <> @elimanodLogico
		
	END

	IF @i_accion = 'C1'
	BEGIN	
			
		SELECT COUNT(IdMuestra) AS 'Cantidad' 
		FROM Muestra 
		WHERE IdOrden = @idOrden
		AND Eliminado != 1 --16/01/2024
		
	END


	--Eliminar
	IF @i_accion = 'E'
	BEGIN	
	
		UPDATE Muestra SET
				EstadoMuestra=@estadoMuestra,
				UsuarioEliminacion=@usuarioCreacion,
				FechaEliminacion=@fechaCreacion,
				Eliminado=@elimanodLogico
		WHERE IdMuestra=@idMuestra2 
		AND IdOrden=@idOrden
		
	END

END

GO