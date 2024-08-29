---------------------------------------------------
-- Registrar en la tabla CatalogoMaestro
---------------------------------------------------
if not exists (select top 1 1 from [dbo].[CatalogoMaestro] where Nombre ='Usuario')
begin
	insert into dbo.CatalogoMaestro (IdCatalogoMaestro, Nombre, Descripcion, Eliminado)
	values (1, 'Usuario', 'Rol del Usuario', 0)
end

if not exists (select top 1 1 from [dbo].[CatalogoMaestro] where Nombre ='Menu')
begin
	insert into dbo.CatalogoMaestro (IdCatalogoMaestro, Nombre, Descripcion, Eliminado)
	values (2, 'Menu', 'Menu de Opciones', 0)
end

if not exists (select top 1 1 from [dbo].[CatalogoMaestro] where Nombre ='Buscar')
begin
	insert into dbo.CatalogoMaestro (IdCatalogoMaestro, Nombre, Descripcion, Eliminado)
	values (3, 'Buscar', 'Buscar Usuarios', 0)
end

if not exists (select top 1 1 from [dbo].[CatalogoMaestro] where Nombre ='EstadoCliente')
begin
	insert into dbo.CatalogoMaestro (IdCatalogoMaestro, Nombre, Descripcion, Eliminado)
	values (4, 'EstadoCliente', 'Estados de Clientes', 0)
end

if not exists (select top 1 1 from [dbo].[CatalogoMaestro] where Nombre ='EstadoOrden')
begin
	insert into dbo.CatalogoMaestro (IdCatalogoMaestro, Nombre, Descripcion, Eliminado)
	values (5, 'EstadoOrden', 'Estados de la Orden', 0)
end

if not exists (select top 1 1 from [dbo].[CatalogoMaestro] where Nombre ='Factura')
begin
	insert into dbo.CatalogoMaestro (IdCatalogoMaestro, Nombre, Descripcion, Eliminado)
	values (6, 'Factura', 'Estado de la Factura', 0)
end

if not exists (select top 1 1 from [dbo].[CatalogoMaestro] where Nombre ='ValidaCliente')
begin
	insert into dbo.CatalogoMaestro (IdCatalogoMaestro, Nombre, Descripcion, Eliminado)
	values (7, 'ValidaCliente', 'Indica si se debe validar con la base de FlexLine', 0)
end

if not exists (select top 1 1 from [dbo].[CatalogoMaestro] where Nombre ='EstadoMuestra')
begin
	insert into dbo.CatalogoMaestro (IdCatalogoMaestro, Nombre, Descripcion, Eliminado)
	values (8, 'EstadoMuestra', 'Estado de la Muestra', 0)
end

if not exists (select top 1 1 from [dbo].[CatalogoMaestro] where Nombre ='EstadoPedido')
begin
	insert into dbo.CatalogoMaestro (IdCatalogoMaestro, Nombre, Descripcion, Eliminado)
	values (9, 'EstadoPedido', 'Estado del Pedido', 0)
end

if not exists (select top 1 1 from [dbo].[CatalogoMaestro] where Nombre ='Email')
begin
	insert into dbo.CatalogoMaestro (IdCatalogoMaestro, Nombre, Descripcion, Eliminado)
	values (10, 'Email', 'Configuracion Servidor Correo', 0)
end

if not exists (select top 1 1 from [dbo].[CatalogoMaestro] where Nombre ='Genero')
begin
	insert into dbo.CatalogoMaestro (IdCatalogoMaestro, Nombre, Descripcion, Eliminado)
	values (11, 'Genero', 'Género de pacientes', 0)
end

if not exists (select top 1 1 from [dbo].[CatalogoMaestro] where Nombre ='EstadoPrueba')
begin
	insert into dbo.CatalogoMaestro (IdCatalogoMaestro, Nombre, Descripcion, Eliminado)
	values (12, 'EstadoPrueba', 'Estado de la Prueba', 0)
end

if not exists (select top 1 1 from [dbo].[CatalogoMaestro] where Nombre ='BuscarOrden')
begin
	insert into dbo.CatalogoMaestro (IdCatalogoMaestro, Nombre, Descripcion, Eliminado)
	values (13, 'BuscarOrden', 'Parámetros de Búsqueda de una Orden', 0)
end

if not exists(select top 1 1 from [dbo].[CatalogoMaestro] where Nombre ='TipoCliente')
begin
	insert into dbo.CatalogoMaestro(IdCatalogoMaestro, Nombre, Descripcion, Eliminado)
	values (14, 'TipoCliente', 'Tipos de cliente de la orden', 0)
end

---------------------------------------------------
-- Registrar en la tabla CatalogoDetalle
---------------------------------------------------
insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (1, 0, 'Administrativo', '1', 1, 0, 1)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (1, 0, 'Cliente', '100', 2, 0, 1)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (1, 0, 'OP Logistico', '7', 3, 0, 1)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (1, 0, 'Laboratorista', '4', 4, 0, 1)

---------------------------------------------------
insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (2, 0, 'Gestión de Clientes', '5', 1, 1, 1)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (2, 0, 'Facturación', '6', 2, 1, 1)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (2, 0, 'DashBoard', '1', 1, 1, 1)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (2, 0, 'Órdenes', '2', 2, 1, 1)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (2, 0, 'Pedidos', '3', 3, 1, 1)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (2, 0, 'Catálogo de Pruebas', '4', 4, 1, 1)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (2, 0, 'Pedidos', '7', 1, 1, 1)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (2, 0, 'Órdenes', '8', 1, 1, 1)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (2, 0, 'Pedidos', '9', 2, 1, 1)

---------------------------------------------------
insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (3, 0, 'Ruc', '1', 1, 1, 1)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (3, 0, 'Cedula', '2', 2, 1, 1)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (3, 0, 'Id', '3', 3, 1, 1)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (3, 0, 'Fecha', '4', 4, 1, 1)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (3, 0, 'Estado', '5', 5, 1, 1)

---------------------------------------------------
insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (4, 0, 'Activo', 'A', 1, 1, 1)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (4, 0, 'Suspendido', 'S', 2, 1, 1)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (4, 0, 'Temporal', 'T', 3, 1, 1)

---------------------------------------------------
insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (6, 0, 'Facturado', '1', 1, 1, 1)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (6, 0, 'PreFactura', '0', 2, 1, 1)

---------------------------------------------------
insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (7, 0, 'Valida', 'True', 1, 0, 0)

---------------------------------------------------
insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (5, 0, 'Generada', 'GENE', 1, 0, 0)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (5, 0, 'Cancelada', 'CANC', 2, 0, 0)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (5, 0, 'Por Recolectar', 'PREC', 3, 0, 0)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (5, 0, 'Recolectado Total', 'RCTL', 5, 0, 0)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (5, 0, 'Rechazada', 'RCHZ', 5, 0, 0)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (5, 0, 'En Analisis', 'ANLS', 9, 0, 0)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (5, 0, 'Validado', 'VALD', 10, 0, 0)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (5, 0, 'Facturadas', 'FACT', 11, 0, 0)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (5, 0, 'Recolectado Parcial', 'RCTP', 6, 0, 0)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (5, 0, 'Recibida', 'RCBD', 7, 0, 0)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (5, 0, 'Recibida Parcial', 'RCBP', 8, 0, 0)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (5, 0, 'Enviado', 'ENV', 12, 0, 0)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values(5, 0, 'Enviado Parcial', 'ENVP', 13, 0, 0)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (5, 0, 'Resultado pendiente', 'RESTP', 14, 0, 0)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (5, 0, 'Validacion pendiente', 'VALDP', 15, 0, 0)

---------------------------------------------------
insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (9, 0, 'Por Recolectar', 'PREC', 10, 0, 0)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (9, 0, 'Recolectado', 'RCTL', 2, 0, 0)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (9, 0, 'Recolectado Parcial', 'RCPC', 3, 0, 0)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (9, 0, 'Anulado', 'ANUL', 4, 0, 0)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (9, 0, 'Enviado', 'ENV', 5, 0, 0)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (9, 0, 'Enviado Parcial', 'ENVP', 6, 0, 0)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (9, 0, 'Recolectado total/parcial', 'RCTL_RCPC', 7, 0, 0)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (9, 0, 'Enviado total/parcial', 'ENV_ENVP', 8, 0, 0)

---------------------------------------------------
insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (8, 0, 'Rechazada Operador', 'RCHO', 4, 0, 0)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (8, 0, 'Rechazada Laboratorista', 'RCHL', 5, 0, 0)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (8, 0, 'Entregada', 'ENTR', 6, 0, 0)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (8, 0, 'Cancelada', 'CANC', 7, 0, 0)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (8, 0, 'Por Recolectar', 'PREC', 1, 0, 0)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (8, 0, 'Recolectada', 'RECT', 2, 0, 0)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (8, 0, 'Recibida', 'RECB', 3, 0, 0)

---------------------------------------------------
insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (10, 0, 'Host', 'smtp-mail.outlook.com', 1, 0, 0)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (10, 0, 'EnableSsl', 'true', 2, 0, 0)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (10, 0, 'User', '', 3, 0, 0)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (10, 0, 'Password', '0', 4, 0, 0)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (10, 0, 'Port', '587', 5, 0, 0)

---------------------------------------------------
insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (11, 0, 'Femenino', 'True', 1, 0, 0)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (11, 0, 'Masculino', 'False', 2, 0, 0)

---------------------------------------------------
insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (12, 0, 'Generada', 'GENE', 1, 0, 0)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (12, 0, 'Por Recolectar', 'PREC', 2, 0, 0)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (12, 0, 'Rechazada', 'RCHZ', 3, 0, 0)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (12, 0, 'Recolectada', 'RECT', 4, 0, 0)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (12, 0, 'Por Procesar', 'PPRC', 5, 0, 0)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (12, 0, 'No Procesada', 'NPRC', 6, 0, 0)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (12, 0, 'Cancelada', 'CANC', 7, 0, 0)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (12, 0, 'Recolectada Parcial', 'RCTP', 8, 0, 0)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (12, 0, 'Validado', 'VALD', 9, 0, 0)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (12, 0, 'Facturadas', 'FACT', 10, 0, 0)

---------------------------------------------------
insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (13, 0, 'N° Orden', '1', 1, 0, 0)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (13, 0, 'Cedula', '2', 2, 0, 0)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (13, 0, 'Nombres Paciente', '3', 3, 0, 0)

---------------------------------------------------
insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (14, 0, 'PARTICULAR', 'TC_PART', 1, 0, 0)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (14, 0, 'ISSFA', 'TC_ISS', 2, 0, 0)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (14, 0, 'IESS', 'TC_IESS', 3, 0, 0)

insert into dbo.CatalogoDetalle (IdCatalogoMaestro, Relacion, Nombre, Valor, Orden, Editable, Eliminar)
values (14, 0, 'OCUPACIONAL', 'TC_OCUP', 4, 0, 0)