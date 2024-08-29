/************************************************************************
*	Archivo Fisico: pr_muestras_humalab.sql							    *
*	Stored procedure: pr_muestras_humalab								*
*	Base de datos: DbPortalHumalab						  			    *
*	Producto: Portal Clientes Humalab administracion					*
*	Elaborado por: José Guarnizo							            *
*----------------------------------------------------------------------	*
*					DESCRIPCION DEL PROCEDIMIENTO						*
*	En este Procedimiento consulta todos las muestras del mes actual y  *
*    mes anterior                                                       *
*	                                                                    *
*----------------------------------------------------------------------	*
*					BITACORA DE MODIFICACIONES							*
*	FECHA AUTOR RAZON													*
*						                                                *
*----------------------------------------------------------------------	*/
IF NOT EXISTS (SELECT * FROM  sys.procedures WHERE NAME = 'pr_muestras_humalab')	
	EXEC('Create Procedure dbo.pr_muestras_humalab As')
go

ALTER PROCEDURE [dbo].[pr_muestras_humalab](
	@i_accion char(1),
	@idUsuario int = null
)

as

begin
	
	if @i_accion = 'C'
	begin
		if exists (select 1 from Muestra where UsuarioCreacion = @idUsuario)
		begin
			--TOTAL DE MUESTRAS DEL MES ACTUAL
			SELECT DATEPART(WEEKDAY, FechaCreacion) AS dia_semana,
				COUNT(*) AS total_muestras into #muestrasMesActual
			FROM Muestra
			WHERE  MONTH(FechaCreacion) = MONTH(GETDATE()) -- Filtrar por el mes actual
			AND DATEPART(WEEKDAY, FechaCreacion) IN (1, 2, 3, 4, 5, 6, 7, 8) -- Filtrar por lunes(2), martes(3), 
																			  --miércoles(4), jueves(5), viernes(6), 
																			  --sabado(7) y domingo(8)
			and UsuarioCreacion = @idUsuario
			AND Eliminado = 0
			GROUP BY DATEPART(WEEKDAY, FechaCreacion)
			ORDER BY DATEPART(WEEKDAY, FechaCreacion)

			select dia_semana, total_muestras from #muestrasMesActual

			--TOTAL DE MUESTRAS MES ANTERIOR
			SELECT DATEPART(WEEKDAY, FechaCreacion) AS dia_semana,
					COUNT(*) AS total_muestras into #muestrasMesAnterior
			FROM Muestra
			WHERE  MONTH(FechaCreacion) = MONTH(DATEADD(MONTH, -1, GETDATE())) -- Filtrar por el mes anterior
			AND DATEPART(WEEKDAY, FechaCreacion) IN (1, 2, 3, 4, 5, 6, 7, 8) -- Filtrar por lunes(2), martes(3), 
																			  --miércoles(4), jueves(5), viernes(6), 
																			  --sabado(7) y domingo(8)
			and UsuarioCreacion = @idUsuario
			AND Eliminado = 0
			GROUP BY DATEPART(WEEKDAY, FechaCreacion)
			ORDER BY DATEPART(WEEKDAY, FechaCreacion)

			select dia_semana, total_muestras from #muestrasMesAnterior
		end
	end
end

GO