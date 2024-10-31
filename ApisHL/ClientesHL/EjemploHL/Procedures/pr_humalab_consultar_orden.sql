/************************************************************************
*	Stored procedure: pr_humalab_consultar_orden						*
*	Base de datos: DbPortalHumalab						  			    *
*	Elaborado por: José Guarnizo							            *
*----------------------------------------------------------------------	*
*					DESCRIPCION DEL PROCEDIMIENTO						*
*	En este Procedimiento consulta la orden                             *
*	                                                                    *
*----------------------------------------------------------------------	*
*					BITACORA DE MODIFICACIONES							*
*	FECHA AUTOR RAZON													*
*	2024/10/30 Jose Guarnizo Se modifica para que no valide las muestras*
*						     que se eliminaron.							*
*----------------------------------------------------------------------	*/
IF NOT EXISTS (SELECT * FROM  sys.procedures WHERE NAME = 'pr_humalab_consultar_orden')	
	EXEC('Create Procedure dbo.pr_humalab_consultar_orden As')
go

ALTER PROCEDURE [dbo].[pr_humalab_consultar_orden](
    @i_accion CHAR(2),
    @idOrden INT = NULL,
    @codigoBarra Varchar(20)=NULL,
    @identifiacion VARCHAR(13) =NULL,
    @idPedido INT = NULL
)

as

DECLARE @elimanodLogico AS INT=1,
	@idGene int,
	@idEstadoOrden int


--id estado Orden 15/01/2024
select @idEstadoOrden = IdCatalogoMaestro
from CatalogoMaestro
where Nombre = 'EstadoOrden'

--estado generado 15/01/2024
select @idGene = IdCatalogoDetalle
from CatalogoDetalle
where IdCatalogoMaestro = @idEstadoOrden
and Valor = 'GENE'


IF(@i_accion='C')
BEGIN
	
	select * from Orden Where IdOrden=@idOrden

END

IF(@i_accion='C1')
BEGIN
	
	Select P.IdOrden, P.IdPrueba, P.IdPruebaGalileo, P.CodigoExamen, P.EsPerfil, P.Nombre, P.Abreviatura, P.Metodologia, P.Precio, M.IdMuestraGalileo, M.IdMuestra, M.Nombre AS 'NombreMuestra', M.MuestraAlterna, M.Recipiente  
	from Prueba P 
	INNER JOIN PruebaMuestra PM ON P.IdPrueba = PM.IdPrueba 
	INNER JOIN Muestra M ON M.IdMuestra = PM.IdMuestra
	Where p.IdOrden=@idOrden 
	AND P.Eliminado <> @elimanodLogico
	--and m.Eliminado !=1
	order by P.FechaCreacion desc

	--16/01/2024
	--Select P.IdOrden, P.IdPrueba, P.IdPruebaGalileo, 
	--  P.CodigoExamen, P.EsPerfil, P.Nombre, P.Abreviatura, P.Metodologia, 
	--  P.Precio, M.IdMuestraGalileo, 
	--  M.IdMuestra, M.Nombre AS 'NombreMuestra', M.MuestraAlterna, M.Recipiente  
	--from Prueba P 
	----INNER JOIN PruebaMuestra PM ON P.IdPrueba = PM.IdPrueba 
	----INNER JOIN Muestra M ON M.IdMuestra = PM.IdMuestra
	--inner join Orden o on P.IdOrden = o.IdOrden
	--inner join Muestra m on o.IdOrden = m.IdOrden
	--Where p.IdOrden=@idOrden 
	--AND P.Eliminado<>@elimanodLogico
	--order by P.FechaCreacion desc

END

IF(@i_accion='C2')
BEGIN
	
	select *from Paciente where Identificacion=@identifiacion

END

IF(@i_accion='C3')
BEGIN	
	
	SELECT DISTINCT M.CodigoBarra, M.UsuarioCreacion, M.IdMuestra 
	FROM Orden O
	INNER JOIN Prueba P ON O.IdOrden=P.IdOrden
	INNER JOIN PruebaMuestra PM ON P.IdPrueba = PM.IdPrueba
	INNER JOIN Muestra M ON PM.IdMuestra = M.IdMuestra	
	WHERE O.IdOrden=@idOrden AND M.Eliminado<>@elimanodLogico
	
	--SELECT DISTINCT M.CodigoBarra, M.UsuarioCreacion, M.IdMuestra 
	--FROM Orden O
	--INNER JOIN Prueba P ON O.IdOrden=P.IdOrden
	----INNER JOIN PruebaMuestra PM ON P.IdPrueba = PM.IdPrueba 16/01/2024
	----INNER JOIN Muestra M ON PM.IdMuestra = M.IdMuestra 16/01/2024
	--inner join Muestra m on o.IdOrden = m.IdOrden --16/01/2024
	--WHERE O.IdOrden=@idOrden AND M.Eliminado<>@elimanodLogico

END

IF(@i_accion='C4')
BEGIN
	
	SELECT IdOrden FROM Orden 
	WHERE CodigoBarra = @codigoBarra
	AND Eliminado != 1 --15/01/2024
	AND Estado = @idGene --15/01/2024

END

IF(@i_accion='C5')
BEGIN
	
	select *from Orden where IdPedido=@idPedido

END

IF(@i_accion='C6')
BEGIN
	
	
Select  O.CodigoBarra AS 'Orden' ,O.Observacion AS'ObservacionOrden', M.CodigoBarra AS 'Muestra', OM.Descripcion AS 'ObservacionMuestra', OM.NombreUsuario AS 'Usuario', M.FechaModificacion AS 'Fecha' from Orden O 
INNER JOIN Muestra M ON O.IdOrden = M.IdOrden
INNER JOIN ObservacionM OM ON OM.IdMuestra = M.IdMuestra
where O.CodigoBarra = @codigoBarra

END

GO