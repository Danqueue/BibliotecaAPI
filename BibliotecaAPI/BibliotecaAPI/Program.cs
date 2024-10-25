using BibliotecaAPI.Repositories;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();


builder.Services.AddSingleton<BibliotecaRepository>(
    provider =>
    new BibliotecaRepository(builder.Configuration
    .GetConnectionString("DefaultConnection")));

// Swagger Parte 1
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
});

var app = builder.Build();

// Swagger Parte 2
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Crud Biblioteca V1");
        c.RoutePrefix = string.Empty;
    });
}


app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
