/************************************************************************
*	Stored procedure: pr_estado_ordenHumalab					        *
*	Base de datos: DbPortalHumalab						  			    *
*	Elaborado por: José Guarnizo						                *
*----------------------------------------------------------------------	*
*					DESCRIPCION DEL PROCEDIMIENTO						*
*	En este Procedimiento consulta el nombre del estado                 *
*	                                                                    *
*----------------------------------------------------------------------	*
*					BITACORA DE MODIFICACIONES							*
*	FECHA AUTOR RAZON													*
*						                                                *
*----------------------------------------------------------------------	*/
IF NOT EXISTS (SELECT * FROM  sys.procedures WHERE NAME = 'pr_estado_ordenHumalab')	
	EXEC('Create Procedure dbo.pr_estado_ordenHumalab As')
go

ALTER PROCEDURE [dbo].[pr_estado_ordenHumalab](
	@i_accion char,
	@i_idEstado int

)

as

declare @i_nombreEstado varchar(50)

if @i_accion = 'C'
BEGIN
	
	SELECT @i_nombreEstado = Nombre
	from CatalogoDetalle
	where IdCatalogoDetalle = @i_idEstado

	select @i_nombreEstado

END

GO