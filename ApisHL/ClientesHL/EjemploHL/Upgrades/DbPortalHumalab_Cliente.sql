-- Create Table [dbo].[Pedido]
Print 'Create Table [dbo].[Pedido]'
GO
IF NOT (EXISTS(SELECT * FROM sys.objects WHERE [object_id] = OBJECT_ID(N'[dbo].[Pedido]') AND [type]='U'))
BEGIN
CREATE TABLE [dbo].[Pedido](
	[IdPedido] [int] IDENTITY(1,1) NOT NULL,
	[IdOperador] [int] NULL,
	[UsuarioOperador] [varchar](50) NULL,
	[NumeroRemision] [varchar](50) NULL,
	[FechaRetiro] [datetime] NULL,
	[EstadoPedido] [int] NULL,
	[ObservacionCliente] [varchar](max) NULL,
	[ObservacionOpLogistico] [varchar](max) NULL,
	[UsuarioCreacion] [int] NULL,
	[FechaCreacion] [datetime] NULL,
	[UsuarioModificacion] [int] NULL,
	[FechaModificacion] [datetime] NULL,
	[UsuarioEliminacion] [int] NULL,
	[FechaEliminacion] [datetime] NULL,
	[Eliminado] [bit] NULL)
END
GO

-- Add Primary Key [PK_Pedido] to [dbo].[Pedido]
Print 'Add Primary Key [PK_Pedido] to [dbo].[Pedido]'
GO
IF (EXISTS(SELECT * FROM sys.objects WHERE [object_id] = OBJECT_ID(N'[dbo].[Pedido]') AND [type]='U')) 
	AND NOT (EXISTS (SELECT * FROM sys.indexes WHERE [name]=N'PK_Pedido' AND [object_id]=OBJECT_ID(N'[dbo].[Pedido]')))
	ALTER TABLE [dbo].[Pedido]
		ADD CONSTRAINT [PK_Pedido]
		PRIMARY KEY CLUSTERED ([IdPedido])

GO

-- Create Table [dbo].[Muestra]
Print 'Create Table [dbo].[Muestra]'
GO
IF NOT (EXISTS(SELECT * FROM sys.objects WHERE [object_id] = OBJECT_ID(N'[dbo].[Muestra]') AND [type]='U'))
BEGIN
CREATE TABLE [dbo].[Muestra](
	[IdMuestra] [int] IDENTITY(1,1) NOT NULL,
	[IdMuestraGalileo] [int] NOT NULL,
	[IdOrden] [int] NULL,
	[Nombre] [varchar](50) NULL,
	[Recipiente] [varchar](20) NULL,
	[MuestraAlterna] [varchar](50) NULL,
	[CodigoBarra] [varchar](50) NULL,
	[EstadoMuestra] [int] NULL,
	[UsuarioCreacion] [int] NULL,
	[FechaCreacion] [datetime] NULL,
	[UsuarioModificacion] [int] NULL,
	[FechaModificacion] [datetime] NULL,
	[UsuarioEliminacion] [int] NULL,
	[FechaEliminacion] [datetime] NULL,
	[Eliminado] [bit] NULL)
END
GO

-- Add Primary Key [PK_Muestra] to [dbo].[Muestra]
Print 'Add Primary Key [PK_Muestra] to [dbo].[Muestra]'
GO
IF (EXISTS(SELECT * FROM sys.objects WHERE [object_id] = OBJECT_ID(N'[dbo].[Muestra]') AND [type]='U')) 
	AND NOT (EXISTS (SELECT * FROM sys.indexes WHERE [name]=N'PK_Muestra' AND [object_id]=OBJECT_ID(N'[dbo].[Muestra]')))
	ALTER TABLE [dbo].[Muestra]
		ADD CONSTRAINT [PK_Muestra]
		PRIMARY KEY CLUSTERED ([IdMuestra])

GO

-- Create Table [dbo].[Observacion]
Print 'Create Table [dbo].[Observacion]'
GO
IF NOT (EXISTS(SELECT * FROM sys.objects WHERE [object_id] = OBJECT_ID(N'[dbo].[Observacion]') AND [type]='U'))
BEGIN
CREATE TABLE [dbo].[Observacion](
	[IdObservacion] [int] IDENTITY(1,1) NOT NULL,
	[IdUsuario] [int] NOT NULL,
	[Descripcion] [varchar](200) NULL,
	[UsuarioCreacion] [int] NULL,
	[FechaCreacion] [date] NULL,
	[UsuarioModificacion] [int] NULL,
	[FechaModificacion] [date] NULL,
	[UsuarioEliminacion] [int] NULL,
	[FechaEliminacion] [date] NULL,
	[Eliminado] [bit] NULL)
END
GO

-- Add Primary Key [PK_Observacion] to [dbo].[Observacion]
Print 'Add Primary Key [PK_Observacion] to [dbo].[Observacion]'
GO
IF (EXISTS(SELECT * FROM sys.objects WHERE [object_id] = OBJECT_ID(N'[dbo].[Observacion]') AND [type]='U')) 
	AND NOT (EXISTS (SELECT * FROM sys.indexes WHERE [name]=N'PK_Observacion' AND [object_id]=OBJECT_ID(N'[dbo].[Observacion]')))
	ALTER TABLE [dbo].[Observacion]
		ADD CONSTRAINT [PK_Observacion]
		PRIMARY KEY CLUSTERED ([IdObservacion])

GO

-- Create Table [dbo].[ObservacionM]
Print 'Create Table [dbo].[ObservacionM]'
GO
IF NOT (EXISTS(SELECT * FROM sys.objects WHERE [object_id] = OBJECT_ID(N'[dbo].[ObservacionM]') AND [type]='U'))
BEGIN
CREATE TABLE [dbo].[ObservacionM](
	[IdObservacionM] [int] IDENTITY(1,1) NOT NULL,
	[IdMuestra] [int] NULL,
	[Descripcion] [varchar](max) NULL,
	[NombreUsuario] [varchar](100) NULL,
	[UsuarioCreacion] [int] NULL,
	[FechaCreacion] [date] NULL,
	[UsuarioModificacion] [int] NULL,
	[FechaModificacion] [date] NULL,
	[UsuarioEliminacion] [int] NULL,
	[FechaEliminacion] [date] NULL,
	[Eliminado] [bit] NULL,
	[Operador] [bit] NULL)
END
GO

-- Add Primary Key [PK_ObservacionM] to [dbo].[ObservacionM]
Print 'Add Primary Key [PK_ObservacionM] to [dbo].[ObservacionM]'
GO
IF (EXISTS(SELECT * FROM sys.objects WHERE [object_id] = OBJECT_ID(N'[dbo].[ObservacionM]') AND [type]='U')) 
	AND NOT (EXISTS (SELECT * FROM sys.indexes WHERE [name]=N'PK_ObservacionM' AND [object_id]=OBJECT_ID(N'[dbo].[ObservacionM]')))
	ALTER TABLE [dbo].[ObservacionM]
		ADD CONSTRAINT [PK_ObservacionM]
		PRIMARY KEY CLUSTERED ([IdObservacionM])

GO

-- Create Table [dbo].[Orden]
Print 'Create Table [dbo].[Orden]'
GO
IF NOT (EXISTS(SELECT * FROM sys.objects WHERE [object_id] = OBJECT_ID(N'[dbo].[Orden]') AND [type]='U'))
BEGIN
CREATE TABLE [dbo].[Orden](
	[IdOrden] [int] IDENTITY(1,1) NOT NULL,
	[IdPedido] [int] NULL,
	[IdUsuarioGalileo] [int] NULL,
	[Identificacion] [varchar](15) NULL,
	[CodigoBarra] [varchar](50) NULL,
	[Medicamento] [varchar](200) NULL,
	[Diagnostico] [varchar](50) NULL,
	[Observacion] [varchar](max) NULL,
	[Estado] [int] NULL,
	[Resultados] [varchar](100) NULL,
	[UsuarioCreacion] [int] NULL,
	[FechaCreacion] [datetime] NULL,
	[UsuarioModificacion] [int] NULL,
	[FechaModificacion] [datetime] NULL,
	[UsuarioEliminacion] [int] NULL,
	[FechaEliminacion] [datetime] NULL,
	[Eliminado] [bit] NULL,
	[idLaboratorio] [int] NULL,
	[ResultadoFinal] [xml] NULL)
END
GO

-- Add Primary Key [PK_Orden] to [dbo].[Orden]
Print 'Add Primary Key [PK_Orden] to [dbo].[Orden]'
GO
IF (EXISTS(SELECT * FROM sys.objects WHERE [object_id] = OBJECT_ID(N'[dbo].[Orden]') AND [type]='U')) 
	AND NOT (EXISTS (SELECT * FROM sys.indexes WHERE [name]=N'PK_Orden' AND [object_id]=OBJECT_ID(N'[dbo].[Orden]')))
	ALTER TABLE [dbo].[Orden]
		ADD CONSTRAINT [PK_Orden]
		PRIMARY KEY CLUSTERED ([IdOrden])

GO

-- Create Table [dbo].[Paciente]
Print 'Create Table [dbo].[Paciente]'
GO
IF NOT (EXISTS(SELECT * FROM sys.objects WHERE [object_id] = OBJECT_ID(N'[dbo].[Paciente]') AND [type]='U'))
BEGIN
CREATE TABLE [dbo].[Paciente](
	[Identificacion] [varchar](15) NOT NULL,
	[Nombres] [varchar](50) NULL,
	[Apellidos] [varchar](50) NULL,
	[Genero] [bit] NULL,
	[FechaNacimiento] [date] NULL,
	[Edad] [int] NULL,
	[Telefono] [varchar](13) NULL,
	[Email] [varchar](70) NULL,
	[UsuarioCreacion] [int] NULL,
	[FechaCreacion] [datetime] NULL,
	[UsuarioModificacion] [int] NULL,
	[FechaModificacion] [datetime] NULL,
	[UsuarioEliminacion] [int] NULL,
	[FechaEliminacion] [datetime] NULL,
	[Eliminado] [bit] NULL,
	[TipoPaciente] [int] NULL,
	[CodLaboratorio] [varchar](50) NULL)
END
GO

-- Add Primary Key [PK_Paciente_1] to [dbo].[Paciente]
Print 'Add Primary Key [PK_Paciente_1] to [dbo].[Paciente]'
GO
IF (EXISTS(SELECT * FROM sys.objects WHERE [object_id] = OBJECT_ID(N'[dbo].[Paciente]') AND [type]='U')) 
	AND NOT (EXISTS (SELECT * FROM sys.indexes WHERE [name]=N'PK_Paciente_1' AND [object_id]=OBJECT_ID(N'[dbo].[Paciente]')))
	ALTER TABLE [dbo].[Paciente]
		ADD CONSTRAINT [PK_Paciente_1]
		PRIMARY KEY CLUSTERED ([Identificacion])

GO

-- Create Table [dbo].[PedidoObservacion]
Print 'Create Table [dbo].[PedidoObservacion]'
GO
IF NOT (EXISTS(SELECT * FROM sys.objects WHERE [object_id] = OBJECT_ID(N'[dbo].[PedidoObservacion]') AND [type]='U'))
BEGIN
CREATE TABLE [dbo].[PedidoObservacion](
	[IdPedidoObservacion] [int] IDENTITY(1,1) NOT NULL,
	[IdPedido] [int] NOT NULL,
	[IdObservacion] [int] NOT NULL,
	[UsuarioCreacion] [int] NULL,
	[FechaCreacion] [date] NULL,
	[UsuarioModificacion] [int] NULL,
	[FechaModificacion] [date] NULL,
	[UsuarioEliminacion] [int] NULL,
	[FechaEliminacion] [date] NULL,
	[Eliminado] [bit] NULL)
END
GO

-- Add Primary Key [PK_PedidoObservacion] to [dbo].[PedidoObservacion]
Print 'Add Primary Key [PK_PedidoObservacion] to [dbo].[PedidoObservacion]'
GO
IF (EXISTS(SELECT * FROM sys.objects WHERE [object_id] = OBJECT_ID(N'[dbo].[PedidoObservacion]') AND [type]='U')) 
	AND NOT (EXISTS (SELECT * FROM sys.indexes WHERE [name]=N'PK_PedidoObservacion' AND [object_id]=OBJECT_ID(N'[dbo].[PedidoObservacion]')))
	ALTER TABLE [dbo].[PedidoObservacion]
		ADD CONSTRAINT [PK_PedidoObservacion]
		PRIMARY KEY CLUSTERED ([IdPedidoObservacion])

GO

-- Create Table [dbo].[Prueba]
Print 'Create Table [dbo].[Prueba]'
GO
IF NOT (EXISTS(SELECT * FROM sys.objects WHERE [object_id] = OBJECT_ID(N'[dbo].[Prueba]') AND [type]='U'))
BEGIN
CREATE TABLE [dbo].[Prueba](
	[IdPrueba] [int] IDENTITY(1,1) NOT NULL,
	[IdOrden] [int] NULL,
	[IdPruebaGalileo] [int] NULL,
	[CodigoExamen] [varchar](10) NULL,
	[EsPerfil] [bit] NULL,
	[Nombre] [varchar](50) NULL,
	[Abreviatura] [varchar](5) NULL,
	[Metodologia] [varchar](15) NULL,
	[Precio] [money] NULL,
	[Estado] [int] NULL,
	[UsuarioCreacion] [int] NULL,
	[FechaCreacion] [datetime] NULL,
	[UsuarioModificacion] [int] NULL,
	[FechaModificacion] [datetime] NULL,
	[UsuarioEliminacion] [int] NULL,
	[FechaEliminacion] [datetime] NULL,
	[Eliminado] [bit] NULL,
	[Observacion] [varchar](200) NULL)
END
GO

-- Add Primary Key [PK_Prueba] to [dbo].[Prueba]
Print 'Add Primary Key [PK_Prueba] to [dbo].[Prueba]'
GO
IF (EXISTS(SELECT * FROM sys.objects WHERE [object_id] = OBJECT_ID(N'[dbo].[Prueba]') AND [type]='U')) 
	AND NOT (EXISTS (SELECT * FROM sys.indexes WHERE [name]=N'PK_Prueba' AND [object_id]=OBJECT_ID(N'[dbo].[Prueba]')))
	ALTER TABLE [dbo].[Prueba]
		ADD CONSTRAINT [PK_Prueba]
		PRIMARY KEY CLUSTERED ([IdPrueba])

GO

-- Create Table [dbo].[PruebaMuestra]
Print 'Create Table [dbo].[PruebaMuestra]'
GO
IF NOT (EXISTS(SELECT * FROM sys.objects WHERE [object_id] = OBJECT_ID(N'[dbo].[PruebaMuestra]') AND [type]='U'))
BEGIN
CREATE TABLE [dbo].[PruebaMuestra](
	[IdPruebaMuestra] [int] IDENTITY(1,1) NOT NULL,
	[IdPrueba] [int] NULL,
	[IdMuestra] [int] NULL,
	[UsuarioCreacion] [int] NULL,
	[FechaCreacion] [date] NULL,
	[UsuarioModificacion] [int] NULL,
	[FechaModificacion] [date] NULL,
	[UsuarioEliminacion] [int] NULL,
	[FechaEliminacion] [date] NULL,
	[Eliminado] [bit] NULL)
END
GO

--Create table [dbo].[Resultados]
Print 'Create Table [dbo].[Resultados]'
GO
IF NOT (EXISTS(SELECT * FROM sys.objects WHERE [object_id] = OBJECT_ID(N'[dbo].[Resultados]') AND [type]='U'))
BEGIN
CREATE TABLE [dbo].[Resultados](
	[IdResultados] [int] IDENTITY(1,1) NOT NULL,
	[IdLaboratorio] [int] NOT NULL,
	[IdentificacionPac] [varchar](100) NOT NULL,
	[NumeroOrden] [varchar](100) NOT NULL,
	[Informe] [xml] NOT NULL,
	[FechaRegistro] [datetime] NOT NULL,
	[Estado] [varchar](50) NOT NULL,
	[IdEstado] [int] NULL,
	[aux1] [varchar](50) NULL,
	[aux2] [varchar](50) NULL,
	[aux3] [varchar](50) NULL,
	[aux4] [varchar](50) NULL,
	[aux5] [varchar](50) NULL,
	[Genero] [varchar](20) NULL,
	[FechaNacimiento] [varchar](100) NULL,
	[FechaIngreso] [varchar](100) NULL,
	[Usuario] [varchar](50) NULL,
	[NombrePaciente] [varchar](100) NULL)
END
GO

-- Add Primary Key [PK_Resultados] to [dbo].[Resultados]
Print 'Add Primary Key [PK_Resultados] to [dbo].[Resultados]'
GO
IF (EXISTS(SELECT * FROM sys.objects WHERE [object_id] = OBJECT_ID(N'[dbo].[Resultados]') AND [type]='U')) 
	AND NOT (EXISTS (SELECT * FROM sys.indexes WHERE [name]=N'PK_Resultados' AND [object_id]=OBJECT_ID(N'[dbo].[Resultados]')))
	ALTER TABLE [dbo].[Resultados]
		ADD CONSTRAINT [PK_Resultados]
		PRIMARY KEY CLUSTERED ([IdResultados])

GO

--Add view
CREATE VIEW [dbo].[pedidos]
as

SELECT
IdPedido
,p.UsuarioCreacion as IdCliente
,u.Usuario as Cliente
,p.NumeroRemision
,p.FechaCreacion
,0 as TotalOrdenes
,0 as TotalMuestras
,0 as TotalRetiradas
,p.FechaRetiro
,p.EstadoPedido
,p.ObservacionCliente
  FROM [dbo].[Pedido] p
  INNER JOIN [dbo].[Usuario] u
  ON p.UsuarioCreacion = u.idUsuario
GO

------------------------------------
-- FK nuevos.
------------------------------------

-- Add Foreign Key [FK_ObservacionM_Muestra] to [dbo].[ObservacionM]
Print 'Add Foreign Key [FK_ObservacionM_Muestra] to [dbo].[ObservacionM]'
GO
IF (EXISTS(SELECT * FROM sys.objects WHERE [object_id] = OBJECT_ID(N'[dbo].[ObservacionM]') AND [type]='U'))
	AND (NOT EXISTS (SELECT * FROM sys.objects WHERE parent_object_id = OBJECT_ID(N'[dbo].[ObservacionM]') AND type = 'F' AND [name] = 'FK_ObservacionM_Muestra'))
    ALTER TABLE [dbo].[ObservacionM]  WITH CHECK ADD  CONSTRAINT [FK_ObservacionM_Muestra] FOREIGN KEY([IdMuestra])
    REFERENCES [dbo].[Muestra] ([IdMuestra])
GO

-- Add Foreign Key [FK_Orden_Paciente] to [dbo].[Orden]
Print 'Add Foreign Key [FK_Orden_Paciente] to [dbo].[Orden]'
GO
IF (EXISTS(SELECT * FROM sys.objects WHERE [object_id] = OBJECT_ID(N'[dbo].[Orden]') AND [type]='U'))
	AND (NOT EXISTS (SELECT * FROM sys.objects WHERE parent_object_id = OBJECT_ID(N'[dbo].[Orden]') AND type = 'F' AND [name] = 'FK_Orden_Paciente'))
    ALTER TABLE [dbo].[Orden]  WITH CHECK ADD  CONSTRAINT [FK_Orden_Paciente] FOREIGN KEY([Identificacion])
    REFERENCES [dbo].[Paciente] ([Identificacion])
GO

-- Add Foreign Key [FK_Orden_Pedido] to [dbo].[Orden]
Print 'Add Foreign Key [FK_Orden_Pedido] to [dbo].[Orden]'
GO
IF (EXISTS(SELECT * FROM sys.objects WHERE [object_id] = OBJECT_ID(N'[dbo].[Orden]') AND [type]='U'))
	AND (NOT EXISTS (SELECT * FROM sys.objects WHERE parent_object_id = OBJECT_ID(N'[dbo].[Orden]') AND type = 'F' AND [name] = 'FK_Orden_Pedido'))
    ALTER TABLE [dbo].[Orden]  WITH CHECK ADD  CONSTRAINT [FK_Orden_Pedido] FOREIGN KEY([IdPedido])
    REFERENCES [dbo].[Pedido] ([IdPedido])
GO

-- Add Foreign Key [FK_PedidoObservacion_Observacion] to [dbo].[PedidoObservacion]
Print 'Add Foreign Key [FK_PedidoObservacion_Observacion] to [dbo].[PedidoObservacion]'
GO
IF (EXISTS(SELECT * FROM sys.objects WHERE [object_id] = OBJECT_ID(N'[dbo].[PedidoObservacion]') AND [type]='U'))
	AND (NOT EXISTS (SELECT * FROM sys.objects WHERE parent_object_id = OBJECT_ID(N'[dbo].[PedidoObservacion]') AND type = 'F' AND [name] = 'FK_PedidoObservacion_Observacion'))
    ALTER TABLE [dbo].[PedidoObservacion]  WITH CHECK ADD  CONSTRAINT [FK_PedidoObservacion_Observacion] FOREIGN KEY([IdObservacion])
    REFERENCES [dbo].[Observacion] ([IdObservacion])
GO

-- Add Foreign Key [FK_PedidoObservacion_Pedido] to [dbo].[PedidoObservacion]
Print 'Add Foreign Key [FK_PedidoObservacion_Pedido] to [dbo].[PedidoObservacion]'
GO
IF (EXISTS(SELECT * FROM sys.objects WHERE [object_id] = OBJECT_ID(N'[dbo].[PedidoObservacion]') AND [type]='U'))
	AND (NOT EXISTS (SELECT * FROM sys.objects WHERE parent_object_id = OBJECT_ID(N'[dbo].[PedidoObservacion]') AND type = 'F' AND [name] = 'FK_PedidoObservacion_Pedido'))
    ALTER TABLE [dbo].[PedidoObservacion]  WITH CHECK ADD  CONSTRAINT [FK_PedidoObservacion_Pedido] FOREIGN KEY([IdPedido])
    REFERENCES [dbo].[Pedido] ([IdPedido])
GO

-- Add Foreign Key [FK_Prueba_Orden] to [dbo].[Prueba]
Print 'Add Foreign Key [FK_Prueba_Orden] to [dbo].[Prueba]'
GO
IF (EXISTS(SELECT * FROM sys.objects WHERE [object_id] = OBJECT_ID(N'[dbo].[Prueba]') AND [type]='U'))
	AND (NOT EXISTS (SELECT * FROM sys.objects WHERE parent_object_id = OBJECT_ID(N'[dbo].[Prueba]') AND type = 'F' AND [name] = 'FK_Prueba_Orden'))
    ALTER TABLE [dbo].[Prueba]  WITH CHECK ADD  CONSTRAINT [FK_Prueba_Orden] FOREIGN KEY([IdOrden])
    REFERENCES [dbo].[Orden] ([IdOrden])
GO

-- Add Foreign Key [FK_PruebaMuestra_Muestra1] to [dbo].[PruebaMuestra]
Print 'Add Foreign Key [FK_PruebaMuestra_Muestra1] to [dbo].[PruebaMuestra]'
GO
IF (EXISTS(SELECT * FROM sys.objects WHERE [object_id] = OBJECT_ID(N'[dbo].[PruebaMuestra]') AND [type]='U'))
	AND (NOT EXISTS (SELECT * FROM sys.objects WHERE parent_object_id = OBJECT_ID(N'[dbo].[PruebaMuestra]') AND type = 'F' AND [name] = 'FK_PruebaMuestra_Muestra1'))
    ALTER TABLE [dbo].[PruebaMuestra]  WITH CHECK ADD  CONSTRAINT [FK_PruebaMuestra_Muestra1] FOREIGN KEY([IdMuestra])
    REFERENCES [dbo].[Muestra] ([IdMuestra])
GO

-- Add Foreign Key [FK_PruebaMuestra_Prueba] to [dbo].[PruebaMuestra]
Print 'Add Foreign Key [FK_PruebaMuestra_Prueba] to [dbo].[PruebaMuestra]'
GO
IF (EXISTS(SELECT * FROM sys.objects WHERE [object_id] = OBJECT_ID(N'[dbo].[PruebaMuestra]') AND [type]='U'))
	AND (NOT EXISTS (SELECT * FROM sys.objects WHERE parent_object_id = OBJECT_ID(N'[dbo].[PruebaMuestra]') AND type = 'F' AND [name] = 'FK_PruebaMuestra_Prueba'))
    ALTER TABLE [dbo].[PruebaMuestra]  WITH CHECK ADD  CONSTRAINT [FK_PruebaMuestra_Prueba] FOREIGN KEY([IdPrueba])
    REFERENCES [dbo].[Prueba] ([IdPrueba])
GO

------------------------------------
-- ALTER TABLE.
------------------------------------
alter table Paciente
add CodLaboratorio varchar(50) null

------------------------------------
-- INDEX.
------------------------------------

--create Index table Muestra in column IdOrden and IdMuestraGalileo
create index idx_Muestra_IdOrden
on Muestra (IdOrden)

create index idx_Muestra_IdMuestraGalileo
on Muestra (IdMuestraGalileo)

--Create Index table Orden in column CodigoBarra y IdUsuarioGalileo
CREATE INDEX idx_Orden_CodigoBarra
on Orden (CodigoBarra)

create index idx_Orden_IdUsuarioGalileo
on Orden (IdUsuarioGalileo)

--Create Index table Pedido in column NumeroRemision
create index idx_Pedido_NumeroRemision
on Pedido (NumeroRemision)

--Create Index table Resultados in column NumeroOrden
create index idx_Resultados_NumeroOrden
on Resultados (NumeroOrden)

--Create Index table Prueba in column IdPruebaGalileo
create index idx_Prueba_IdPruebaGalileo
on Prueba (IdPruebaGalileo)

--Create Index table Usuario in column IdMuestraGalileo
create index idx_Usuario_idGalileo
on Usuario (idGalileo)

--Create Index table PruebaMuestra in columns IdPrueba and IdMuestra
create index idx_PruebaMuestra_IdPrueba
on PruebaMuestra (IdPrueba)

create index id_PruebaMuestra_IdMuestra
on PruebaMuestra (IdMuestra)

--create Index table Resultados in column IdLaboratorio and IdentificacionPac
create index idx_Resultados_IdLaboratorio
on Resultados (IdLaboratorio)

create index idx_Resultados_Identificacion
on Resultados (IdentificacionPac)


