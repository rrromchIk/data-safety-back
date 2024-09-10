using System.Numerics;
using data_safety.Utils;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options => {
    options.AddPolicy("AllowAny", builder => {
        builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAny");
app.UseHttpsRedirection();

app.MapGet("/pseudo-random-numbers",
                async ([FromQuery] int A,
                        [FromQuery] int X0,
                        [FromQuery] int C,
                        [FromQuery] BigInteger M,
                        [FromQuery] int SequenceLength) =>
                {
                    var result = await new LinearCongruentialGenerator()
                            .GenerateRandomNumbersAndWriteToFile(
                                    A,
                                    X0,
                                    C,
                                    M,
                                    SequenceLength
                            );
                    
                    return Results.Ok(result);
                })
        .WithName("GetPseudoRandomNumbers")
        .Produces(StatusCodes.Status200OK, typeof(LinearCongruentialGeneratorResult))
        .WithOpenApi();

app.Run();

public record LinearCongruentialGeneratorResult(List<string> PseudoRandomNumbers, int Period);
