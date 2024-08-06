/************************************************************************
*	Stored procedure: pr_humalab_idorden								*
*	Base de datos: DbPortalHumalab						  			    *
*	Elaborado por: Daniel Nicolalde							            *
*----------------------------------------------------------------------	*
*					DESCRIPCION DEL PROCEDIMIENTO						*
*	En este Procedimiento consulta el id de las ordenes                 *
*	                                                                    *
*----------------------------------------------------------------------	*
*					BITACORA DE MODIFICACIONES							*
*	FECHA AUTOR RAZON													*
*						                                                *
*----------------------------------------------------------------------	*/
IF NOT EXISTS (SELECT * FROM  sys.procedures WHERE NAME = 'pr_humalab_idorden')	
	EXEC('Create Procedure dbo.pr_humalab_idorden As')
go

ALTER PROCEDURE [dbo].[pr_humalab_idorden](
	@i_accion CHAR(1),
	@codigoBarra VARCHAR(50)
)

as

IF(@i_accion = 'C')
Begin
	SELECT IdOrden AS 'IdOrden' FROM Orden 
	--WHERE CodigoBarra like @codigoBarra+'%'
	WHERE CodigoBarra = @codigoBarra --01/02/2024
	and Eliminado != 1 --16/01/2024
END

GO