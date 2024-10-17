/************************************************************************
*	Stored procedure: pr_humalab_IdClientePe			                *
*	Base de datos: DbPortalHumalab						  			    *
*	Elaborado por: José Guarnizo						                *
*----------------------------------------------------------------------	*
*					DESCRIPCION DEL PROCEDIMIENTO						*
*	En este Procedimiento consulta el codigo lis de la orden            *
*	                                                                    *
*----------------------------------------------------------------------	*
*					BITACORA DE MODIFICACIONES							*
*	FECHA AUTOR RAZON													*
*						                                                *
*----------------------------------------------------------------------	*/
IF NOT EXISTS (SELECT * FROM  sys.procedures WHERE NAME = 'pr_humalab_IdClientePe')	
	EXEC('Create Procedure dbo.pr_humalab_IdClientePe As')
go

ALTER PROCEDURE [dbo].[pr_humalab_IdClientePe](
	@i_accion char,
	@i_idCliente varchar(50)
)

as

declare @ctaCuenta varchar(100)

if @i_accion = 'C'
begin

	select @ctaCuenta = CodClienteCta
	from Usuario	
	where idGalileo = @i_idCliente	

	select IdCliente
	from Cliente
	where CodClienteCta = @ctaCuenta

end

GO
