-- Create Table [dbo].[Facturacion]
Print 'Create Table [dbo].[Facturacion]'
GO
IF NOT (EXISTS(SELECT * FROM sys.objects WHERE [object_id] = OBJECT_ID(N'[dbo].[Facturacion]') AND [type]='U'))
BEGIN
CREATE TABLE [dbo].[Facturacion](
	[IdFactura] [int] IDENTITY(1,1) NOT NULL,
	[NumeroFactura] [varchar](50) NULL,
	[IdentificacionUsuario] [varchar](13) NOT NULL,
	[TotalFactura] [money] NULL,
	[TotalMuestras] [int] NULL,
	[Estado] [int] NULL,
	[UsuarioCreacion] [int] NULL,
	[FechaCreacion] [datetime] NULL,
	[UsuarioModificacion] [int] NULL,
	[FechaModificacion] [datetime] NULL,
	[UsuarioEliminarion] [int] NULL,
	[FechaEliminacion] [datetime] NULL,
	[Elimanado] [bit] NULL)
END
GO

-- Add Primary Key [PK_Facturacion] to [dbo].[Facturacion]
Print 'Add Primary Key [PK_Facturacion] to [dbo].[Facturacion]'
GO
IF (EXISTS(SELECT * FROM sys.objects WHERE [object_id] = OBJECT_ID(N'[dbo].[Facturacion]') AND [type]='U')) 
	AND NOT (EXISTS (SELECT * FROM sys.indexes WHERE [name]=N'PK_Facturacion' AND [object_id]=OBJECT_ID(N'[dbo].[Facturacion]')))
	ALTER TABLE [dbo].[Facturacion]
		ADD CONSTRAINT [PK_Facturacion]
		PRIMARY KEY CLUSTERED ([IdFactura])

GO

-- Create Table [dbo].[FacturaOrden]
Print 'Create Table [dbo].[FacturaOrden]'
GO
IF NOT (EXISTS(SELECT * FROM sys.objects WHERE [object_id] = OBJECT_ID(N'[dbo].[FacturaOrden]') AND [type]='U'))
BEGIN
CREATE TABLE [dbo].[FacturaOrden](
	[IdFactura] [int] NULL,
	[IdOrden] [int] NULL)
END
GO

------------------------------------
-- FK nuevos.
------------------------------------

-- Add Foreign Key [FK_FacturaOrden_Facturacion] to [dbo].[FacturaOrden]
Print 'Add Foreign Key [FK_FacturaOrden_Facturacion] to [dbo].[FacturaOrden]'
GO
IF (EXISTS(SELECT * FROM sys.objects WHERE [object_id] = OBJECT_ID(N'[dbo].[FacturaOrden]') AND [type]='U'))
	AND (NOT EXISTS (SELECT * FROM sys.objects WHERE parent_object_id = OBJECT_ID(N'[dbo].[FacturaOrden]') AND type = 'F' AND [name] = 'FK_FacturaOrden_Facturacion'))
    ALTER TABLE [dbo].[FacturaOrden]  WITH CHECK ADD  CONSTRAINT [FK_FacturaOrden_Facturacion] FOREIGN KEY([IdFactura])
    REFERENCES [dbo].[Facturacion] ([IdFactura])
GO

-- Add Foreign Key [FK_FacturaOrden_Orden] to [dbo].[FacturaOrden]
Print 'Add Foreign Key [FK_FacturaOrden_Orden] to [dbo].[FacturaOrden]'
GO
IF (EXISTS(SELECT * FROM sys.objects WHERE [object_id] = OBJECT_ID(N'[dbo].[FacturaOrden]') AND [type]='U'))
	AND (NOT EXISTS (SELECT * FROM sys.objects WHERE parent_object_id = OBJECT_ID(N'[dbo].[FacturaOrden]') AND type = 'F' AND [name] = 'FK_FacturaOrden_Orden'))
    ALTER TABLE [dbo].[FacturaOrden]  WITH CHECK ADD  CONSTRAINT [FK_FacturaOrden_Orden] FOREIGN KEY([IdOrden])
    REFERENCES [dbo].[Orden] ([IdOrden])
GO