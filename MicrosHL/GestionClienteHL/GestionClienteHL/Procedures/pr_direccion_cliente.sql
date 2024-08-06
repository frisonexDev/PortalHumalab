/************************************************************************
*	Archivo Fisico: pr_direccion_cliente.sql							*
*	Stored procedure: pr_direccion_cliente								*
*	Base de datos: DbPortalHumalab						  			    *
*	Producto: Portal Clientes Humalab administracion					*
*	Elaborado por: José Guarnizo							            *
*----------------------------------------------------------------------	*
*					DESCRIPCION DEL PROCEDIMIENTO						*
*	En este Procedimiento se va a poder consultar la direccion del      *
*	humalab                                                             *
*----------------------------------------------------------------------	*
*					BITACORA DE MODIFICACIONES							*
*	FECHA AUTOR RAZON													*
*						                                                *
*----------------------------------------------------------------------	*/
IF NOT EXISTS (SELECT * FROM  sys.procedures  WHERE NAME = 'pr_direccion_cliente')	
	EXEC('Create Procedure dbo.pr_direccion_cliente As')
go

ALTER PROCEDURE [dbo].[pr_direccion_cliente](
	@i_accion char,
	@i_ruc varchar(50)
)

as

declare @rucValidado varchar(50),
	@codigo varchar(10)

if @i_accion = 'C'
begin
	
	select @rucValidado = Identificacion
	from Cliente
	where Identificacion = @i_ruc

	if @rucValidado != null or @rucValidado != ''
	begin
		
		select Direccion, Provincia, Ciudad, Latitud, Longitud
		from ClienteDireccion cd
		join Cliente c on cd.IdCliente = c.IdCliente
		where c.Identificacion = @i_ruc

		select @codigo = '00'
		select @codigo as Codigo
	end
	else
	begin

		select '' as Direccion, '' as Provincia, 
			   '' as Ciudad, '' as Latitud, '' as Longitud

		select @codigo = '01'
		select @codigo as Codigo

	end

end

GO