/************************************************************************
*	Stored procedure: pr_humalab_operador								*
*	Base de datos: DbPortalHumalab						  			    *
*	Elaborado por: José Guarnizo							            *
*----------------------------------------------------------------------	*
*					DESCRIPCION DEL PROCEDIMIENTO						*
*	En este Procedimiento consulta el operador del cliente              *
*	                                                                    *
*----------------------------------------------------------------------	*
*					BITACORA DE MODIFICACIONES							*
*	FECHA AUTOR RAZON													*
*						                                                *
*----------------------------------------------------------------------	*/
IF NOT EXISTS (SELECT * FROM  sys.procedures WHERE NAME = 'pr_humalab_operador')	
	EXEC('Create Procedure dbo.pr_humalab_operador As')
go

ALTER PROCEDURE [dbo].[pr_humalab_operador](
	@i_accion CHAR(1),
	@idUsuario INT
)

as

declare @identiCliente varchar(20)

select @identiCliente = Identificacion
from Usuario
where idGalileo = @idUsuario

IF(@i_accion = 'C')
BEGIN

	select IdOperadorLogistico AS 'IdOperador', NombreOperadorLogistico AS 'Nombre'
	from Cliente
	where Identificacion = @identiCliente

END

GO