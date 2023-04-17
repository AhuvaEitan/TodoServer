using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ToDoDbContext>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>{
     options.AddPolicy("MyCorsPolicy", builder => 
                        builder.WithOrigins("*")
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

app.UseSwagger(options =>
{
    options.SerializeAsV2 = true;
});

app.UseCors("MyCorsPolicy");



app.MapGet("/items", async (ToDoDbContext db) =>
    await db.Items.ToListAsync());

app.MapPost("/items", async (Item todo, ToDoDbContext db) =>
{
    db.Items.Add(todo);
    await db.SaveChangesAsync();
    return Results.Created("/todoitems/${todo.Id}", todo);
});



app.MapPut("/items/{id}", async ( int id,Item inputTodo, ToDoDbContext db) =>
{
    var todo = await db.Items.FindAsync(inputTodo.Id);

    if (todo is null) return Results.NotFound();

    todo.Name = inputTodo.Name;
    todo.IsComplete = inputTodo.IsComplete;
    db.Items.Update(todo);

    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/items/{id}", async (int id, ToDoDbContext db) =>
{
    if (await db.Items.FindAsync(id) is Item todo)
    {
        db.Items.Remove(todo);
        await db.SaveChangesAsync();
        return Results.Ok(todo);
    }
    return Results.NotFound();
});

app.Run();
