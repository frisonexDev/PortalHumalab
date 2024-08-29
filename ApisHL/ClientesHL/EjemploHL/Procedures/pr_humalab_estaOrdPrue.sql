/************************************************************************
*	Stored procedure: pr_humalab_estaOrdPrue							*
*	Base de datos: DbPortalHumalab						  			    *
*	Elaborado por: José Guarnizo							            *
*----------------------------------------------------------------------	*
*					DESCRIPCION DEL PROCEDIMIENTO						*
*	En este Procedimiento realiza consulta del valor del estado orden y *
*   prueba cuando se actualiza                                          *
*	                                                                    *
*----------------------------------------------------------------------	*
*					BITACORA DE MODIFICACIONES							*
*	FECHA AUTOR RAZON													*
*						                                                *
*----------------------------------------------------------------------	*/
IF NOT EXISTS (SELECT * FROM  sys.procedures WHERE NAME = 'pr_humalab_estaOrdPrue')	
	EXEC('Create Procedure dbo.pr_humalab_estaOrdPrue As')
go

ALTER PROCEDURE [dbo].[pr_humalab_estaOrdPrue](
	@i_accion char,
	@i_idEstado int = null,
	@i_idGalileo int = null
)

as

declare @idPorRecOrd varchar(10),
		@idPorRecPrue varchar(10),
		@idCataMaestroOrd int,
		@idCataMaestroPrue int,
		@idCliente int,
		@sucursal varchar(100)

select @idCataMaestroOrd = IdCatalogoMaestro
from CatalogoMaestro
where Nombre = 'EstadoOrden'

select @idCataMaestroPrue = IdCatalogoMaestro
from CatalogoMaestro
where Nombre = 'EstadoPrueba'

if @i_accion = 'C'
begin
	select @idPorRecOrd = Valor
	from CatalogoDetalle
	where IdCatalogoMaestro = @idCataMaestroOrd
	and IdCatalogoDetalle = @i_idEstado

	select @idPorRecOrd

end

if @i_accion = 'CP'
begin
	select @idPorRecPrue = Valor
	from CatalogoDetalle
	where IdCatalogoMaestro = @idCataMaestroPrue
	and IdCatalogoDetalle = @i_idEstado

	select @idPorRecPrue
end

if @i_accion = 'L'
begin
	
	--select @idCliente = cl.IdCliente
	--from Cliente cl
	--inner join Usuario us on cl.Identificacion = us.Identificacion
	--where us.idGalileo = @i_idGalileo

	--nuevo 2024/01/24
	select @idCliente = cl.IdCliente
	from Cliente cl
	inner join Usuario us on cl.CodClienteCta = us.CodClienteCta
	where us.idGalileo = @i_idGalileo

	select @idCliente

end

GO