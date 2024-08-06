/************************************************************************
*	Stored procedure: pr_humalab_estadoCliente						    *
*	Base de datos: DbPortalHumalab						  			    *
*	Elaborado por: Jose Guarnizo						                *
*----------------------------------------------------------------------	*
*					DESCRIPCION DEL PROCEDIMIENTO						*
*	En este Procedimiento modifica el estado del cliente                *
*	                                                                    *
*----------------------------------------------------------------------	*
*					BITACORA DE MODIFICACIONES							*
*	FECHA AUTOR RAZON													*
*						                                                *
*----------------------------------------------------------------------	*/
IF NOT EXISTS (SELECT * FROM sys.procedures WHERE name = 'pr_humalab_estadoCliente')	
	EXEC('Create Procedure dbo.pr_humalab_estadoCliente As')

GO

ALTER PROCEDURE [dbo].[pr_humalab_estadoCliente](
	@i_accion		char(1),
	@i_estado		char(1) = null,
	@i_ruc			varchar(20),
	@idGalileo int = null --16/01/2024
)

as

declare @estadoCliente int, @suspendidoUser int,
	@userFechavige date

select @estadoCliente = IdCatalogoMaestro
from CatalogoMaestro
where Nombre = 'EstadoCliente'

select @suspendidoUser = IdCatalogoDetalle
from CatalogoDetalle
where IdCatalogoMaestro = @estadoCliente
and Nombre = 'Suspendido'

select @userFechavige = Fechavigencia
from Usuario
where Identificacion = @i_ruc

IF @i_accion = 'M'
BEGIN
	UPDATE	usuario
	SET		Estado				= @i_estado
	WHERE	Identificacion				= @i_ruc
	select @@ROWCOUNT actualizados
	RETURN 0
END

IF @i_accion = 'C'
BEGIN
	--modificacion 29/07/2024
	if @userFechavige is not null
	begin
		update Usuario
		set Estado = @suspendidoUser
		where identificacion = @i_ruc
		and Fechavigencia < CAST(GETDATE() AS date)
	end

	select top 1 cd.valor  as estadocliente,
		u.Fechavigencia as fechasuspension,
		u.FechaCreacion as fecharegistro,
		u.Identificacion as ruc
	from usuario u left join catalogodetalle cd on u.Estado = cd.valor 
	and IdCatalogoMaestro = @estadoCliente
	where identificacion = @i_ruc
	--and idGalileo = @idGalileo --16/01/2024
	RETURN 0
END

GO