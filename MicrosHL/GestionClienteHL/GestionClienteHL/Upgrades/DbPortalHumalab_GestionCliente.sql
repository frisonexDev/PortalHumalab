--Create DbPortalHumalab
USE [master]
GO

CREATE DATABASE [DbPortalHumalab]

ON  PRIMARY 
( NAME = N'DbPortalHumalab', FILENAME = N'E:\PortalDB\DbPortalHumalab.mdf' , SIZE = 73728KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'DbPortalHumalab_log', FILENAME = N'E:\PortalDB\DbPortalHumalab_log.ldf' , SIZE = 73728KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [DbPortalHumalab].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO

-------------------------

-- Create Table [dbo].[CatalogoMaestro]
Print 'Create Table [dbo].[CatalogoMaestro]'
GO
IF NOT (EXISTS(SELECT * FROM sys.objects WHERE [object_id] = OBJECT_ID(N'[dbo].[CatalogoMaestro]') AND [type]='U'))
BEGIN
CREATE TABLE [dbo].[CatalogoMaestro](
	[IdCatalogoMaestro] [int] NOT NULL,
	[Nombre] [varchar](100) NOT NULL,
	[Descripcion] [varchar](200) NOT NULL,
	[Eliminado] [bit] NOT NULL
)
END
GO

-- Add Primary Key [PK_CatalogoMaestro] to [dbo].[CatalogoMaestro]
Print 'Add Primary Key [PK_CatalogoMaestro] to [dbo].[CatalogoMaestro]'
GO
IF (EXISTS(SELECT * FROM sys.objects WHERE [object_id] = OBJECT_ID(N'[dbo].[CatalogoMaestro]') AND [type]='U')) 
	AND NOT (EXISTS (SELECT * FROM sys.indexes WHERE [name]=N'PK_CatalogoMaestro' AND [object_id]=OBJECT_ID(N'[dbo].[CatalogoMaestro]')))
	ALTER TABLE [dbo].[CatalogoMaestro]
		ADD CONSTRAINT [PK_CatalogoMaestro]
		PRIMARY KEY CLUSTERED ([IdCatalogoMaestro])

GO

-- Create Table [dbo].[CatalogoDetalle]
Print 'Create Table [dbo].[CatalogoDetalle]'
GO
IF NOT (EXISTS(SELECT * FROM sys.objects WHERE [object_id] = OBJECT_ID(N'[dbo].[CatalogoDetalle]') AND [type]='U'))
BEGIN
CREATE TABLE [dbo].[CatalogoDetalle](
	[IdCatalogoDetalle] [int] IDENTITY(1,1) NOT NULL,
	[IdCatalogoMaestro] [int] NOT NULL,
	[Relacion] [int] NOT NULL,
	[Nombre] [varchar](200) NOT NULL,
	[Valor] [varchar](50) NOT NULL,
	[Orden] [int] NOT NULL,
	[Editable] [bit] NOT NULL,
	[Eliminar] [bit] NOT NULL)
END
GO

-- Add Primary Key [PK_CatalogoDetalle] to [dbo].[CatalogoDetalle]
Print 'Add Primary Key [PK_CatalogoDetalle] to [dbo].[CatalogoDetalle]'
GO
IF (EXISTS(SELECT * FROM sys.objects WHERE [object_id] = OBJECT_ID(N'[dbo].[CatalogoDetalle]') AND [type]='U')) 
	AND NOT (EXISTS (SELECT * FROM sys.indexes WHERE [name]=N'PK_CatalogoDetalle' AND [object_id]=OBJECT_ID(N'[dbo].[CatalogoDetalle]')))
	ALTER TABLE [dbo].[CatalogoDetalle]
		ADD CONSTRAINT [PK_CatalogoDetalle]
		PRIMARY KEY CLUSTERED ([IdCatalogoDetalle])

GO

-- Create Table [dbo].[Usuario]
Print 'Create Table [dbo].[Usuario]'
GO
IF NOT (EXISTS(SELECT * FROM sys.objects WHERE [object_id] = OBJECT_ID(N'[dbo].[Usuario]') AND [type]='U'))
BEGIN
CREATE TABLE [dbo].[Usuario](
	[idUsuario] [int] IDENTITY(1,1) NOT NULL,
	[idGalileo] [int] NULL,
	[Usuario] [varchar](120) NULL,
	[Identificacion] [varchar](15) NULL,
	[IdRol] [int] NULL,
	[Estado] [char](1) NULL,
	[Email] [varchar](100) NULL,
	[Fechavigencia] [date] NULL,
	[Observacion] [varchar](200) NULL,
	[UsuarioCreacion] [int] NULL,
	[FechaCreacion] [datetime] NULL,
	[UsuarioModificacion] [int] NULL,
	[FechaModificacion] [datetime] NULL,
	[UsuarioEliminacion] [int] NULL,
	[FechaEliminacion] [datetime] NULL,
	[Eliminado] [bit] NULL,
	[CodClienteCta] [varchar](100) NULL)
END
GO

-- Add Primary Key [PK_Usuario] to [dbo].[Usuario]
Print 'Add Primary Key [PK_Usuario] to [dbo].[Usuario]'
GO
IF (EXISTS(SELECT * FROM sys.objects WHERE [object_id] = OBJECT_ID(N'[dbo].[Usuario]') AND [type]='U')) 
	AND NOT (EXISTS (SELECT * FROM sys.indexes WHERE [name]=N'PK_Usuario' AND [object_id]=OBJECT_ID(N'[dbo].[Usuario]')))
	ALTER TABLE [dbo].[Usuario]
		ADD CONSTRAINT [PK_Usuario]
		PRIMARY KEY CLUSTERED ([idUsuario])

GO


-- Create Table [dbo].[Cliente]
Print 'Create Table [dbo].[Cliente]'
GO
IF NOT (EXISTS(SELECT * FROM sys.objects WHERE [object_id] = OBJECT_ID(N'[dbo].[Cliente]') AND [type]='U'))
BEGIN
CREATE TABLE [dbo].[Cliente](
	[IdCliente] [int] IDENTITY(1,1) NOT NULL,
	[IdUsuario] [int] NOT NULL,
	[Identificacion] [varchar](50) NOT NULL,
	[NombreCliente] [varchar](200) NOT NULL,
	[IdOperadorLogistico] [varchar](10) NOT NULL,
	[NombreOperadorLogistico] [varchar](200) NOT NULL,
	[UsuarioCreacion] [int] NULL,
	[FechaCreacion] [datetime] NULL,
	[UsuarioModificacion] [int] NULL,
	[FechaModificacion] [datetime] NULL,
	[UsuarioEliminacion] [int] NULL,
	[FechaEliminacion] [datetime] NULL,
	[Eliminado] [bit] NULL,
	[CodClienteCta] [varchar](100) NULL,
	[IdOperadorLis] [varchar](50) NULL,
	[Telefono] [varchar](100) NULL,
	[aux1] [varchar](100) NULL,
	[aux2] [varchar](100) NULL,
	[aux3] [varchar](100) NULL,
	[aux4] [varchar](100) NULL)
END
GO

-- Add Primary Key [PK_Cliente] to [dbo].[Cliente]
Print 'Add Primary Key [PK_Cliente] to [dbo].[Cliente]'
GO
IF (EXISTS(SELECT * FROM sys.objects WHERE [object_id] = OBJECT_ID(N'[dbo].[Cliente]') AND [type]='U')) 
	AND NOT (EXISTS (SELECT * FROM sys.indexes WHERE [name]=N'PK_Cliente' AND [object_id]=OBJECT_ID(N'[dbo].[Cliente]')))
	ALTER TABLE [dbo].[Cliente]
		ADD CONSTRAINT [PK_Cliente]
		PRIMARY KEY CLUSTERED ([IdCliente])

GO

-- Create Table [dbo].[ClienteDireccion]
Print 'Create Table [dbo].[ClienteDireccion]'
GO
IF NOT (EXISTS(SELECT * FROM sys.objects WHERE [object_id] = OBJECT_ID(N'[dbo].[ClienteDireccion]') AND [type]='U'))
BEGIN
CREATE TABLE [dbo].[ClienteDireccion](
	[IdClienteDireccion] [int] IDENTITY(1,1) NOT NULL,
	[IdCliente] [int] NOT NULL,
	[Direccion] [varchar](100) NULL,
	[Provincia] [varchar](100) NULL,
	[Ciudad] [varchar](100) NULL,
	[Latitud] [varchar](100) NULL,
	[Longitud] [varchar](100) NULL,
	[FechaCreacion] [datetime] NULL,
	[UsuarioModificacion] [int] NULL,
	[FechaModificacion] [datetime] NULL,
	[UsuarioEliminacion] [int] NULL,
	[FechaEliminacion] [datetime] NULL,
	[Eliminado] [bit] NULL,
	[UsuarioCreacion] [int] NULL)
END
GO

-- Add Primary Key [PK_ClienteDireccion] to [dbo].[ClienteDireccion]
Print 'Add Primary Key [PK_ClienteDireccion] to [dbo].[ClienteDireccion]'
GO
IF (EXISTS(SELECT * FROM sys.objects WHERE [object_id] = OBJECT_ID(N'[dbo].[ClienteDireccion]') AND [type]='U')) 
	AND NOT (EXISTS (SELECT * FROM sys.indexes WHERE [name]=N'PK_ClienteDireccion' AND [object_id]=OBJECT_ID(N'[dbo].[ClienteDireccion]')))
	ALTER TABLE [dbo].[ClienteDireccion]
		ADD CONSTRAINT [PK_ClienteDireccion]
		PRIMARY KEY CLUSTERED ([IdClienteDireccion])

GO

------------------------------------
-- FK nuevos.
------------------------------------

-- Add Foreign Key [FK_CatalogoMaestro_CatalogoDetalle] to [dbo].[CatalogoDetalle]
Print 'Add Foreign Key [FK_CatalogoMaestro_CatalogoDetalle] to [dbo].[CatalogoDetalle]'
GO
IF (EXISTS(SELECT * FROM sys.objects WHERE [object_id] = OBJECT_ID(N'[dbo].[CatalogoDetalle]') AND [type]='U'))
	AND (NOT EXISTS (SELECT * FROM sys.objects WHERE parent_object_id = OBJECT_ID(N'[dbo].[CatalogoDetalle]') AND type = 'F' AND [name] = 'FK_CatalogoMaestro_CatalogoDetalle'))
    ALTER TABLE [dbo].[CatalogoDetalle]  WITH CHECK ADD  CONSTRAINT [FK_CatalogoMaestro_CatalogoDetalle] FOREIGN KEY([IdCatalogoMaestro])
    REFERENCES [dbo].[CatalogoMaestro] ([IdCatalogoMaestro])
GO

-- Add Foreign Key [FK_Cliente_Usuario] to [dbo].[Cliente]
Print 'Add Foreign Key [FK_Cliente_Usuario] to [dbo].[Cliente]'
GO
IF (EXISTS(SELECT * FROM sys.objects WHERE [object_id] = OBJECT_ID(N'[dbo].[Cliente]') AND [type]='U'))
	AND (NOT EXISTS (SELECT * FROM sys.objects WHERE parent_object_id = OBJECT_ID(N'[dbo].[Cliente]') AND type = 'F' AND [name] = 'FK_Cliente_Usuario'))
    ALTER TABLE [dbo].[Cliente]  WITH CHECK ADD  CONSTRAINT [FK_Cliente_Usuario] FOREIGN KEY([IdUsuario])
    REFERENCES [dbo].[Usuario] ([IdUsuario])
GO

-- Add Foreign Key [FK_ClienteDireccion_Cliente1] to [dbo].[ClienteDireccion] 
Print 'Add Foreign Key [FK_ClienteDireccion_Cliente1] to [dbo].[ClienteDireccion]'
GO
IF (EXISTS(SELECT * FROM sys.objects WHERE [object_id] = OBJECT_ID(N'[dbo].[ClienteDireccion]') AND [type]='U'))
	AND (NOT EXISTS (SELECT * FROM sys.objects WHERE parent_object_id = OBJECT_ID(N'[dbo].[ClienteDireccion]') AND type = 'F' AND [name] = 'FK_ClienteDireccion_Cliente1'))
    ALTER TABLE [dbo].[ClienteDireccion]  WITH CHECK ADD  CONSTRAINT [FK_ClienteDireccion_Cliente1] FOREIGN KEY([IdCliente])
    REFERENCES [dbo].[Cliente] ([IdCliente])
GO


------------------------------------
-- INDEX.
------------------------------------

--create Index table Cliente in column CodClienteCta
create index idx_Cliente_CodClienteCta
on Cliente (CodClienteCta)

--create Index table Usuario in column CodClienteCta
create index idx_Usuario_CodClienteCta
on Usuario (CodClienteCta)