using Microsoft.EntityFrameworkCore;
using Midas.Configuration;
using Midas.Data;
using Midas.Services;
using Midas.Services.AlgorithmServices;
using Midas.Services.HostedServices;
using Midas.Services.Workbook;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
      options.UseSqlServer(builder.Configuration.GetSection("SqlServerSettings")["ConnectionString"])
      .UseLazyLoadingProxies());

builder.Services.AddControllers().AddJsonOptions(
    opts =>
        {
            opts.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals;
            //opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            // opts.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            //opts.JsonSerializerOptions.Converters.Add(new PolymorphicContentJsonConverter());
        }
    );

var allowAllOrigins = "AllowAllOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        allowAllOrigins,
        builder =>
        {
            builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMapster();

//builder.Services.AddSignalR();
builder.Services.AddHttpClient();
builder.Services.AddHostedService<IntraDailyStockRequestor>();

builder.Services.AddScoped<IReadExcelData, ReadExcelData>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<ITwelveDataCandleService, TwelveDataCandleService>();
builder.Services.AddScoped<ICandleService, CandleService>();
//builder.Services.AddScoped<IAddyAlgorithm, AddyAlgorithm>();
builder.Services.AddScoped<IAddyAlgorithm, AddyAlgorithm3>();
builder.Services.AddScoped<IParameterizedAddy, ParameterizedAddy>();
builder.Services.AddScoped<IMetricsWorkbookService, MetricsWorkbookService>();
builder.Services.AddScoped<CondensedMetricsWorkbookService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(20)
};


//app.UseWebSockets(webSocketOptions);
//app.UseMiddleware<SocketWare>();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<ApplicationDbContext>();

    context.Database.Migrate();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
