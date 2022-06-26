using TvScraper.Repository.Clients;
using TvScraper.Repository.MongoDb;
using TvScraper.Repository.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMemoryCache();

builder.Services.AddHttpClient<IShowProvider, ShowProvider>();
builder.Services.AddHttpClient<IShowCastProvider, ShowCastProvider>();

builder.Services.AddTransient<IShowCastCollector, ShowCastCollector>();
builder.Services.AddTransient<ITvShowDbService, TvShowDbService>();
builder.Services.AddTransient<ITvShowService, TvShowService>();

builder.Services.Configure<TvShowDbSettings>(
    builder.Configuration.GetSection("TvShowsDatabase"));
builder.Services.AddControllers();

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
app.MapControllers();

app.Run();
