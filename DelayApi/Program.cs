var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Logging.AddConsole();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/delay", async () =>
{
    await Task.Delay(TimeSpan.FromSeconds(1));
    return Results.Ok(new { Result = "DONE" });
})
.WithName("delay")
.WithOpenApi();


app.MapPost("/delayMessage", async (string message, ILogger<Program> logger) =>
{
    logger.LogInformation($"==== >> Time: {DateTime.Now} || Received message: {message}");
    await Task.Delay(TimeSpan.FromSeconds(1));
    return Results.Ok(new { Result = "DONE" });
})
.WithName("delayMessage")
.WithOpenApi();

app.Run();
