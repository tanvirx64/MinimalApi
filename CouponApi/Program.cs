var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/hello/{id:int}",(int id) => {
    return Results.Ok("Id = " + id);
});
app.MapPost("/hello_post", () => {
    return Results.Ok("Hello World!");
});


app.UseHttpsRedirection();

app.Run();

