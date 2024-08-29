/************************************************************************
*	Stored procedure: pr_humalab_pacientes								*
*	Base de datos: DbPortalHumalab						  			    *
*	Elaborado por: José Guarnizo							            *
*----------------------------------------------------------------------	*
*					DESCRIPCION DEL PROCEDIMIENTO						*
*	En este Procedimiento realiza acciones para los pacientes           *
*	                                                                    *
*----------------------------------------------------------------------	*
*					BITACORA DE MODIFICACIONES							*
*	FECHA AUTOR RAZON													*
*																		*
*----------------------------------------------------------------------	*/
IF NOT EXISTS (SELECT * FROM  sys.procedures WHERE NAME = 'pr_humalab_pacientes')	
	EXEC('Create Procedure dbo.pr_humalab_pacientes As')
go

ALTER PROCEDURE [dbo].[pr_humalab_pacientes](
	@i_accion CHAR(1),
	@identificacion VARCHAR(15)=NULL,
	@nombres VARCHAR(50)=NULL,
	@apellidos VARCHAR(50)=NULL,
	@genero Bit=NULL,
	@fechaNacimiento Date=NULL,
	@edad INT=NULL,
	@telefono VARCHAR(13)=NULL,
	@email VARCHAR(70)=NULL,
	@usuarioCreacion INT=NULL,
	@fechaCreacion DATETIME=NULL,
	@tipoPaciente int = null,
	@codLab varchar(50) = null
)

as

DECLARE @contar AS INT,
		@elimanodLogico AS INT=1,
		@identificacionNew VARCHAR(15),
		@nombresNew VARCHAR(50),
		@apellidosNew VARCHAR(50),
		@generoNew Bit,
		@fechaNacimientoNew Date,
		@edadNew INT,
		@telefonoNew VARCHAR(13),
		@emailNew VARCHAR(70),
		@usuarioCreacionNew INT,
		@fechaCreacionNew DATETIME,
		@tipoPacienteNew int,
		@codLabNew varchar(50)


SELECT @contar = COUNT(Identificacion) 
FROM Paciente 
WHERE Identificacion = @identificacion

select @identificacionNew = @identificacion
select @nombresNew = @nombres
select @apellidosNew = @apellidos
select @generoNew = @genero
select @fechaNacimientoNew = @fechaNacimiento
select @edadNew = @edad
select @telefonoNew = @telefono
select @emailNew = @email
select @usuarioCreacionNew = @usuarioCreacion
select @fechaCreacionNew = @fechaCreacion
select @tipoPacienteNew = @tipoPaciente
select @codLabNew = @codLab

BEGIN

--Insertar
	IF @i_accion = 'I'
	BEGIN	

		IF(@contar<1)
			BEGIN
				INSERT INTO Paciente 
				VALUES(@identificacionNew, @nombresNew, @apellidosNew,
					   @generoNew, @fechaNacimientoNew, @edadNew,
					   @telefonoNew, @emailNew, @usuarioCreacionNew, 
					   @fechaCreacionNew, null, null, 
					   null, null, null, @tipoPacienteNew, @codLabNew)
			END
		
	END

	--Modificar
	IF @i_accion = 'M'
	BEGIN	
			
		IF(@contar>0)
			BEGIN

				UPDATE Paciente 
				SET Identificacion=@identificacion,
					Nombres= @nombres,
					Apellidos= @apellidos,
					@Telefono=@telefono, 
					Email=@email, 
					UsuarioModificacion=@usuarioCreacion,
					FechaModificacion=@fechaCreacion, 
					TipoPaciente = @tipoPaciente,
					CodLaboratorio = @codLab
				WHERE Identificacion=@identificacion

			END
		
	END

	--Consultar
	IF @i_accion = 'C'
	BEGIN	
	
		SELECT * FROM Paciente 
		WHERE Identificacion = @identificacion	
		
	END


	--Eliminar
	IF @i_accion = 'E'
	BEGIN	
			
		UPDATE Paciente 
		SET UsuarioEliminacion=@usuarioCreacion,
			FechaEliminacion=@fechaCreacion,
			Eliminado=@elimanodLogico
		WHERE Identificacion=@identificacion
		
	END

END

GO