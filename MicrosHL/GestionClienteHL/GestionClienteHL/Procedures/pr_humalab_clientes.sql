/************************************************************************
*	Archivo Fisico: pr_humalab_clientes.sql								*
*	Stored procedure: pr_humalab_clientes								*
*	Base de datos: DbPortalHumalab						  			    *
*	Producto: Portal Clientes Humalab administracion					*
*	Elaborado por: José Guarnizo							            *
*----------------------------------------------------------------------	*
*					DESCRIPCION DEL PROCEDIMIENTO						*
*En este Procedimiento se va a poder consultar los clientes en Humalab  *
*----------------------------------------------------------------------	*
*					BITACORA DE MODIFICACIONES							*
*	FECHA AUTOR RAZON													*
*						                                                *
*----------------------------------------------------------------------	*/
IF NOT EXISTS (SELECT * FROM  sys.procedures WHERE NAME = 'pr_humalab_clientes')	
	EXEC('Create Procedure dbo.pr_humalab_clientes As')
go

ALTER PROCEDURE [dbo].[pr_humalab_clientes](
	@i_accion char(1)
)

as

declare @estadosNombre varchar(20),
	@idRol int,
	@idMaestro int,
	@valorRol varchar(10)

select @idMaestro = IdCatalogoMaestro
from CatalogoMaestro
where Nombre = 'Usuario'

select @valorRol = Valor
from CatalogoDetalle
where IdCatalogoMaestro = @idMaestro 
and Nombre = 'Cliente'

begin
	--consultar clientes en base a estados
	if @i_accion = 'C'
	begin	
		if exists (SELECT top 1 idUsuario from Usuario)
		begin			
			---total de registros
			select count(*) as total_registros
			from Usuario us
			join Cliente cl on us.idUsuario = cl.IdUsuario
			where IdRol = convert(int, @valorRol)			

			--obtiene al cliente en base al estado y nombre o ruc			
			SELECT us.idUsuario as IdCliente, 
				MAX(cl.NombreCliente) AS Cliente,
				MAX(us.Identificacion) AS Ruc,
				MAX(us.Usuario) AS Usuario,
				MAX(us.FechaCreacion) AS FechaRegistro,
				MAX(cl.NombreOperadorLogistico) AS OperadorLogistico,
				MAX(cd.IdCatalogoDetalle) as EstadoCodigo,
				MAX(cd.Nombre) as Estado,
				MAX(us.Fechavigencia) as FechaTemporal
			FROM Usuario us	
			INNER JOIN Cliente cl ON cl.Identificacion = us.Identificacion
			join CatalogoDetalle cd on us.Estado = cd.Valor
			where us.IdRol = convert(int, 100)
			GROUP BY us.idUsuario
			ORDER BY MAX(us.FechaCreacion) DESC

		end

	end
end

GO