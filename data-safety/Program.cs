using System.Numerics;
using data_safety.MD5;
using data_safety.PseudoRandomNumbers;
using data_safety.RC5;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAny",
            builder =>
            {
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
                    var result = await new LinearCongruentialGenerator(A,
                                    X0,
                                    C,
                                    M,
                                    SequenceLength)
                            .GenerateRandomNumbersAndWriteToFile();
                    return Results.Ok(result);
                })
        .WithName("GetPseudoRandomNumbers")
        .Produces(StatusCodes.Status200OK, typeof(LinearCongruentialGeneratorResult))
        .WithOpenApi();


app.MapGet("/hash/string",
                ([FromQuery] string input) =>
                {
                    var md5 = new Md5Util();
                    md5.ComputeHash(input);
                    return Results.Ok(md5.HashAsString);
                })
        .WithName("GetHashFromString")
        .Produces(StatusCodes.Status200OK, typeof(string))
        .WithOpenApi();


app.MapGet("/hash/file",
                async ([FromQuery] string filePath) =>
                {
                    Console.WriteLine(filePath);
                    Console.WriteLine(File.Exists(filePath));
                    var md5 = new Md5Util();
                    try
                    {
                        await md5.ComputeFileHashAsync(filePath);
                    }
                    catch (Exception e)
                    {
                        return Results.Problem(detail: e.Message, statusCode: StatusCodes.Status500InternalServerError);
                    }

                    var filename = $"{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.md5";
                    File.WriteAllText(filename, $"{md5.HashAsString} *{filePath}");

                    return Results.Ok(md5.HashAsString);
                })
        .WithName("GetHashFromFile")
        .Produces(StatusCodes.Status200OK, typeof(string))
        .WithOpenApi();


app.MapPost("/rc5/encrypt",
                async ([FromQuery] string key, [FromQuery] string fileName,
                        [FromBody] RC5Settings rc5Settings) =>
                {
                    if (!File.Exists(fileName))
                        return Results.NotFound("File not found.");
                    
                    var rc5 = new RC5Util(rc5Settings, key);

                    var fileBytes = await File.ReadAllBytesAsync(fileName);
                    var encodedFileContent = rc5.EncryptCBCPAD(fileBytes);
                    var outputFileName =
                            $"{Path.GetFileNameWithoutExtension(fileName)}-enc{Path.GetExtension(fileName)}";

                    await File.WriteAllBytesAsync(outputFileName, encodedFileContent);
                    return Results.Ok(outputFileName);
                })
        .WithName("EncryptFileWithRC5_CBC-PAD")
        .Produces(StatusCodes.Status200OK, typeof(string))
        .WithOpenApi();

app.MapPost("/rc5/decrypt",
                async ([FromQuery] string key, [FromQuery] string fileName,
                        [FromBody] RC5Settings rc5Settings) =>
                {
                    if (!File.Exists(fileName))
                        return Results.NotFound("File not found.");
                    
                    var rc5 = new RC5Util(rc5Settings, key);
                    var encryptedFileBytes = await File.ReadAllBytesAsync(fileName);
                    var decodedFileContent = rc5.DecryptCBCPAD(encryptedFileBytes);
                    var outputFileName = $"{Path.GetFileNameWithoutExtension(fileName)}-dec{Path.GetExtension(fileName)}";
                    await File.WriteAllBytesAsync(outputFileName, decodedFileContent);
                    return Results.Ok(outputFileName);
                })
        .WithName("DecryptFileWithRC5_CBC-PAD")
        .Produces(StatusCodes.Status200OK, typeof(string))
        .WithOpenApi();

app.Run();