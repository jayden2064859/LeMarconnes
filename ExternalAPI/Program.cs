using ExternalAPI;

var builder = WebApplication.CreateBuilder(args);

// http client voor externe api calls van andere groep
builder.Services.AddHttpClient<ExternalApiTestService>(client =>
{
    client.BaseAddress = new Uri("https://webappethernet-ajbvggave6aghhar.westeurope-01.azurewebsites.net");
});

builder.Services.AddControllers();
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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
