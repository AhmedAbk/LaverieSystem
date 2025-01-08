using Laverie.API.Services;
using Laverie.API.Infrastructure.WebSockets;
using Microsoft.Extensions.DependencyInjection;
using Laverie.API.Infrastructure.repositories;
using Laverie.API.Infrastructure.context;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add the database context and services
builder.Services.AddScoped<AppDbContext>();

// Configs
builder.Services.AddScoped<ConfigurationRepo>();
builder.Services.AddScoped<ConfigurationService>();

// User
builder.Services.AddScoped<UserRepo>();
builder.Services.AddScoped<UserService>();

// Laundry
builder.Services.AddScoped<LaundryRepo>();
builder.Services.AddScoped<LaundryService>();

// Machine
builder.Services.AddScoped<MachineRepo>();
builder.Services.AddScoped<MachineService>();

// Add WebSocketHandler as a singleton
builder.Services.AddSingleton<WebSocketHandler>();

// Apply CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", builder =>
    {
        builder.WithOrigins("http://localhost:5173")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials()
               .SetIsOriginAllowed(_ => true);
    });
});

var app = builder.Build();
 
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}




app.UseHttpsRedirection();
 
app.UseCors("AllowReactApp");

app.UseAuthorization();

app.MapControllers();
 
app.UseWebSockets();

var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(50)
};
app.UseWebSockets(webSocketOptions);

var webSocketHandler = app.Services.GetRequiredService<WebSocketHandler>();
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/ws")
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            await webSocketHandler.HandleWebSocket(context);
        }
        else
        {
            context.Response.StatusCode = 400;
        }
    }
    else
    {
        await next();
    }
});

app.Run();