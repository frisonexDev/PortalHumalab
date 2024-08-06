/************************************************************************
*	Archivo Fisico: pr_cliente_humalab.sql								*
*	Stored procedure: pr_cliente_humalab								*
*	Base de datos: DbPortalHumalab						  			    *
*	Producto: Portal Clientes Humalab administracion					*
*	Elaborado por: José Guarnizo							            *
*----------------------------------------------------------------------	*
*					DESCRIPCION DEL PROCEDIMIENTO						*
*	En este Procedimiento se va a poder consultar el cliente en Humalab *
*	en base al ruc o nombre del cliente                                 *
*----------------------------------------------------------------------	*
*					BITACORA DE MODIFICACIONES							*
*	FECHA AUTOR RAZON													*
*						                                                *
*----------------------------------------------------------------------	*/
IF NOT EXISTS (SELECT * FROM  sys.procedures  WHERE NAME = 'pr_cliente_humalab')	
	EXEC('Create Procedure dbo.pr_cliente_humalab As')
go

ALTER PROCEDURE [dbo].[pr_cliente_humalab](
	@i_tipobus varchar(100) = null,
	@i_estado int = null,	
	@i_accion char(1)
)

as

declare @estadosNombre varchar(20),
	@valorEstado char,
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
		--obtiene el nombre de estado		
		select @estadosNombre = Nombre
		from  CatalogoDetalle
		where IdCatalogoDetalle = @i_estado

		--obtiene el valor del estado
		select @valorEstado = Valor
		from CatalogoDetalle
		where IdCatalogoDetalle = @i_estado

		--trae todos los clientes
		if @i_estado = 0 and @i_tipobus = ''
		begin
			
			select count(*) as total_registros
			from Usuario us
			join Cliente cl on us.idUsuario = cl.IdUsuario
			where us.IdRol = convert(int, @valorRol)
			
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
			where us.IdRol = convert(int, @valorRol)
			GROUP BY us.idUsuario
			ORDER BY MAX(us.FechaCreacion) DESC	
		end

		if @i_tipobus != null or @i_tipobus != '' 
		begin
			--obtiene al cliente en base al ruc
			if exists (select 1 from Usuario where Identificacion = @i_tipobus and Estado = @valorEstado)
			begin
				--total registros
				select count(*) as total_registros
				from Usuario us				
				where us.Identificacion = @i_tipobus
				and us.IdRol = convert(int, @valorRol)
				
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
				where us.IdRol = convert(int, @valorRol)
				and us.Identificacion = @i_tipobus
				GROUP BY us.idUsuario
				ORDER BY MAX(us.FechaCreacion) DESC	


			end
			else
			begin
				if @i_estado = 0
				begin
					if exists (select 1 from Usuario where Identificacion = @i_tipobus)
					begin
						--total registros
						select count(*) as total_registros
						from Usuario us				
						where us.Identificacion = @i_tipobus
						and us.IdRol = convert(int, @valorRol)

						--obtiene al cliente en base al ruc						
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
						where us.IdRol = convert(int, @valorRol)
						and us.Identificacion = @i_tipobus
						GROUP BY us.idUsuario
						ORDER BY MAX(us.FechaCreacion) DESC

					end
				end				
			end

			--valida nombre del cliente
			if exists (select 1 from Cliente where NombreCliente like '%' + @i_tipobus + '%')
			begin
				if @i_estado = 0
				begin
					--total registros
					select COUNT(*) as total_registros
					from Cliente cl
					join Usuario us on cl.IdUsuario = us.idUsuario
					where cl.NombreCliente like '%' + @i_tipobus + '%'
					and us.IdRol = convert(int, @valorRol)

					--obtiene clientes en base al nombre					
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
					where us.IdRol = convert(int, @valorRol)
					and cl.NombreCliente like '%' + @i_tipobus + '%'
					GROUP BY us.idUsuario
					ORDER BY MAX(us.FechaCreacion) DESC

				end
			end

			--valida nombre y estado del cliente
			if exists (select 1 from Cliente cl join Usuario us on cl.IdUsuario = us.idUsuario
								where  us.Estado = @valorEstado and cl.NombreCliente like '%' + @i_tipobus + '%')
			begin
				--toal registros
				select COUNT(*) as total_registros
				from Cliente cl
				join Usuario us on cl.IdUsuario = us.idUsuario
				where cl.NombreCliente like '%' + @i_tipobus + '%'
				and us.Estado = @valorEstado
				and us.IdRol = convert(int, @valorRol)							

				--obtiene al cliente en base al estado
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
				where us.IdRol = convert(int, @valorRol)
				and cl.NombreCliente like '%' + @i_tipobus + '%'
				and us.Estado = @valorEstado
				GROUP BY us.idUsuario
				ORDER BY MAX(us.FechaCreacion) DESC

			end
			
		end
		else
		begin
		--clientes en base al estado
			if @i_estado != 0
			begin				
				---total de registros
				select count(*) as total_registros
				from Usuario us				
				where us.Estado = @valorEstado
				and us.IdRol = convert(int, @valorRol)

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
				where us.IdRol = convert(int, @valorRol)				
				and us.Estado = @valorEstado
				GROUP BY us.idUsuario
				ORDER BY MAX(us.FechaCreacion) DESC

			end
		end
	end

end

GO