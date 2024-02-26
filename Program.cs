var builder = WebApplication.CreateBuilder(args);
var misReglasCors = "ReglasCors";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: misReglasCors,

    builder =>
    {
        builder.AllowAnyOrigin()
    .AllowAnyHeader()
    .AllowAnyMethod();

    });

});

builder.Services.AddControllers();

var app = builder.Build();

app.UseCors(misReglasCors);

app.UseAuthorization();

app.MapControllers();

app.Run();
