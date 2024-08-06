/************************************************************************
*	Stored procedure: pr_humalab_observacion							*
*	Base de datos: DbPortalHumalab						  			    *
*	Elaborado por: José Guarnizo							            *
*----------------------------------------------------------------------	*
*					DESCRIPCION DEL PROCEDIMIENTO						*
*	En este Procedimiento consulta observaciones                        *
*	                                                                    *
*----------------------------------------------------------------------	*
*					BITACORA DE MODIFICACIONES							*
*	FECHA AUTOR RAZON													*
*						                                                *
*----------------------------------------------------------------------	*/
IF NOT EXISTS (SELECT * FROM  sys.procedures WHERE NAME = 'pr_humalab_observacion')	
	EXEC('Create Procedure dbo.pr_humalab_observacion As')
go

ALTER PROCEDURE [dbo].[pr_humalab_observacion](
	@i_accion CHAR(1),
	@descripcion Varchar(200)=NULL,
	@usuarioCreacion INT,
	@fechaCreacion Date
)

as

declare @usuarioCreacionNew int,
		@descripcionNew varchar(200),
		@fechaCreacionNew date

select @usuarioCreacionNew = @usuarioCreacion
select @descripcionNew = @descripcion
select @fechaCreacionNew =@fechaCreacion

IF(@i_accion = 'I')
BEGIN

	--INSERT Observacion 
	--Values (@usuarioCreacion, @descripcion, @usuarioCreacion,
	--		@fechaCreacion, null, null,
	--		null, null, null)
	--SELECT SCOPE_IDENTITY() as idObservacion
	INSERT Observacion 
	Values (@usuarioCreacionNew, @descripcionNew, @usuarioCreacionNew,
			@fechaCreacionNew, null, null,
			null, null, null)
	SELECT SCOPE_IDENTITY() as idObservacion

END

GO