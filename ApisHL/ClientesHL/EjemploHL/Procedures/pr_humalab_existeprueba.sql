/************************************************************************
*	Stored procedure: pr_humalab_existeprueba							*
*	Base de datos: DbPortalHumalab						  			    *
*	Elaborado por: José Guarnizo							            *
*----------------------------------------------------------------------	*
*					DESCRIPCION DEL PROCEDIMIENTO						*
*	En este Procedimiento verifica si existe una prueba                 *
*	                                                                    *
*----------------------------------------------------------------------	*
*					BITACORA DE MODIFICACIONES							*
*	FECHA AUTOR RAZON													*
*	2024/10/30 Jose Guarnizo Se cambia que valida por el eliminado      *
*							 logico en la consulta C y C1.              *
*----------------------------------------------------------------------	*/
IF NOT EXISTS (SELECT * FROM  sys.procedures WHERE NAME = 'pr_humalab_existeprueba')	
	EXEC('Create Procedure dbo.pr_humalab_existeprueba As')
go

ALTER PROCEDURE [dbo].[pr_humalab_existeprueba](
	@i_accion CHAR(2),
	@idOrden INT=NULL,
	@idPruebaGalileo INT=NULL,
	@idPrueba INT = NULL,
	@idPruebaMuestra INT =NULL,
	@idMuestra INT=NULL
)

as

DECLARE @elimanodLogico AS INT=1

IF(@i_accion = 'C')
Begin
	SELECT COUNT(IdPrueba)AS 'Existe' 
	FROM PRUEBA 
	WHERE IdOrden=@idOrden 
	AND IdPruebaGalileo=@idPruebaGalileo 
	AND Eliminado<>@elimanodLogico
END

IF(@i_accion = 'C1')
Begin
	select IdPrueba 
	from Prueba 
	where IdPruebaGalileo=@idPruebaGalileo 
	AND IdOrden=@idOrden
	AND Eliminado<>@elimanodLogico
END

IF(@i_accion = 'C2')
Begin
	select IdPruebaMuestra from PruebaMuestra where IdPrueba=@idPrueba
END

IF(@i_accion = 'C3')
Begin
	select IdMuestra from PruebaMuestra where IdPruebaMuestra=@idPruebaMuestra
END

IF(@i_accion = 'C4')
Begin
	select count(IdMuestra)AS'Cantidad' from PruebaMuestra where IdMuestra = @idMuestra AND Eliminado<>@elimanodLogico
END


GO