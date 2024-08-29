/************************************************************************
*	Stored procedure: pr_humalab_prueba								    *
*	Base de datos: DbPortalHumalab						  			    *
*	Elaborado por: José Guarnizo							            *
*----------------------------------------------------------------------	*
*					DESCRIPCION DEL PROCEDIMIENTO						*
*	En este Procedimiento realiza acciones para las pruebas             *
*	                                                                    *
*----------------------------------------------------------------------	*
*					BITACORA DE MODIFICACIONES							*
*	FECHA AUTOR RAZON													*
*						                                                *
*----------------------------------------------------------------------	*/
IF NOT EXISTS (SELECT * FROM  sys.procedures WHERE NAME = 'pr_humalab_prueba')	
	EXEC('Create Procedure dbo.pr_humalab_prueba As')
go

ALTER PROCEDURE [dbo].[pr_humalab_prueba](
	@i_accion CHAR(2),
	@idOrden2 INT = NULL,
	@idPrueba INT=NULL,
	@idPruebaGalileo INT=NULL,
	@codigoBarra VARCHAR(100)=NULL,
	@esPerfil BIT=NULL,
	@codigoExamen VARCHAR(100)=NULL,
	@nombre VARCHAR(50)=NULL,
	@abreviatura VARCHAR(5)=NULL,
	@metodologia Varchar(10)=NULL,
	@precio MONEY=NULL,
	@estado INT=NULL,
	@usuarioCreacion INT=NULL,
	@fechaCreacion DATETIME=NULL
)

as

DECLARE @contarPrueba AS INT,
		@elimanodLogico AS INT=1,
		@eliminadoLogicoO AS INT =0,
		@idOrden AS INT,
		@contarPruebaMod AS INT,
		@idPruebaGalileoNew int,
		@codigoExamenNew VARCHAR(100),
		@esPerfilNew bit,
		@nombreNew VARCHAR(50),
		@abreviaturaNew varchar(50),
		@metodologiaNew Varchar(10),
		@precioNew MONEY,
		@estadoNew int,
		@usuarioCreacionNew int,
		@fechaCreacionNew datetime,
		@idOrdenNew int

SELECT @idOrden = IdOrden 
FROM Orden
WHERE CodigoBarra = @codigoBarra
and Eliminado = 0

--SELECT @contarPrueba = COUNT(IdPrueba) 
--FROM Prueba 
--WHERE IdOrden = @idOrden 
--AND IdPruebaGalileo = @idPruebaGalileo
--and Eliminado != 1

--para modificar el estado de la prueba
--select @contarPruebaMod = COUNT(IdPrueba) 
--FROM Prueba WHERE IdPrueba = @idPrueba
--AND Eliminado != 1

select @idPruebaGalileoNew = @idPruebaGalileo
select @codigoExamenNew = @codigoExamen
select @esPerfilNew = @esPerfil
select @nombreNew = @nombre
select @abreviaturaNew = @abreviatura
select @metodologiaNew = @metodologia
select @precioNew = @precio
select @estadoNew = @estado
select @usuarioCreacionNew = @usuarioCreacion
select @fechaCreacionNew = @fechaCreacion
select @idOrdenNew = @idOrden

BEGIN

--Insertar
	IF @i_accion = 'I'
	BEGIN			

		--IF(@contarPrueba<1)
		--	BEGIN				
		--		--INSERT INTO Prueba 
		--		--VALUES(@idOrden, @idPruebaGalileo,@codigoExamen,
		--		--	   @esPerfil,@nombre,@abreviatura,
		--		--	   @metodologia, @precio,@estado,
		--		--	   @usuarioCreacion, @fechaCreacion, NULL, 
		--		--	   NULL, NULL, NULL, 
		--		--	   @eliminadoLogicoO, NULL)			
		--		--SELECT SCOPE_IDENTITY() as IdPrueba
		--		INSERT INTO Prueba 
		--		VALUES(@idOrdenNew, @idPruebaGalileoNew, @codigoExamenNew,
		--			   @esPerfilNew, @nombreNew, @abreviaturaNew,
		--			   @metodologiaNew, @precioNew, @estadoNew,
		--			   @usuarioCreacionNew, @fechaCreacionNew, NULL, 
		--			   NULL, NULL, NULL, 
		--			   @eliminadoLogicoO, NULL)
			
		--		SELECT SCOPE_IDENTITY() as IdPrueba
			
		--	END
		IF NOT EXISTS (SELECT IdPrueba FROM Prueba WHERE IdOrden = @idOrden 
					   AND IdPruebaGalileo = @idPruebaGalileo AND Eliminado != 1)
		BEGIN				
			INSERT INTO Prueba 
			VALUES(@idOrdenNew, @idPruebaGalileoNew, @codigoExamenNew,
				   @esPerfilNew, @nombreNew, @abreviaturaNew,
				   @metodologiaNew, @precioNew, @estadoNew,
				   @usuarioCreacionNew, @fechaCreacionNew, NULL, 
				   NULL, NULL, NULL, 
				   @eliminadoLogicoO, NULL)
			
			SELECT SCOPE_IDENTITY() as IdPrueba
			
		END
		
	END

	--Modificar
	IF @i_accion = 'M'
	BEGIN	
		
		--if @contarPruebaMod > 0
		--begin

		--	update Prueba
		--	set Estado = @estado, 
		--		UsuarioModificacion = @usuarioCreacion,
		--		FechaModificacion = GETDATE()
		--	where IdPrueba = @idPrueba

		--end
		if exists (select IdPrueba from Prueba 
				   where IdPrueba = @idPrueba
				   AND Eliminado != 1 )
		begin

			update Prueba
			set Estado = @estado, 
				UsuarioModificacion = @usuarioCreacion,
				FechaModificacion = GETDATE()
			where IdPrueba = @idPrueba

		end
		
	END

	IF @i_accion = 'M2'
	BEGIN	
		UPDATE Prueba SET
			   Estado=@estado,
			   UsuarioModificacion=@usuarioCreacion, 
			   FechaModificacion=@fechaCreacion
			   --WHERE IdOrden = (SELECT IdOrden FROM Orden where CodigoBarra=@codigoBarra)
			   WHERE IdOrden = @idOrden
		
	END

	--Consultar
	IF @i_accion = 'C'
	BEGIN	
			
		SELECT * FROM Prueba 
		WHERE IdPruebaGalileo = @idPruebaGalileo 
		AND IdOrden = @idOrden
		
	END

	--Consultar
	IF @i_accion = 'C1'
	BEGIN	
			
		SELECT IdOrden, IdPruebaGalileo, IdPrueba 
		FROM Prueba 
		WHERE IdOrden = @idOrden2 
		AND Eliminado <> @elimanodLogico
		
	END

	--Consultar
	IF @i_accion = 'C2'
	BEGIN	
			
		SELECT IdPrueba ,Estado 
		FROM Prueba 
		WHERE IdOrden = @idOrden2
		
	END


	--Eliminar
	IF @i_accion = 'E'
	BEGIN	
					
		UPDATE Prueba
		SET Estado = @estado,
			UsuarioEliminacion=@usuarioCreacion,
			FechaEliminacion=GETDATE(),
			Eliminado=@elimanodLogico
			WHERE IdPrueba = @idPrueba		
	END

END

GO