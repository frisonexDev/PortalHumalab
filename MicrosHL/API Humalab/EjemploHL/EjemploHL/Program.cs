using ClienteHL.Datos;
using EjemploHL.Datos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Configuración de implementaciones de interfaces
builder.Services.AddTransient<IMapeoDatosCatalogoMaestro, MapeoDatosCatalogoMaestro>();
builder.Services.AddTransient<IMapeoDatosOrden, MapeoDatosOrden>();
builder.Services.AddTransient<IMapeoDatosPaciente, MapeoDatosPaciente>();
builder.Services.AddTransient<IMapeoDatosPedido, MapeoDatosPedido>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
    //app.UseSwaggerUI();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ClienteHL"));
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ClienteHL");
        c.RoutePrefix = string.Empty;
    });
}

app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.MapControllers();

app.Run();
