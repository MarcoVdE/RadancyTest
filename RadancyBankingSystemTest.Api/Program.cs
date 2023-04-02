using System.Reflection;
using Microsoft.OpenApi.Models;
using RadancyBankingSystemTest.Domain;
using RadancyBankingSystemTest.Domain.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AccountOptions>(builder.Configuration.GetSection("AccountOptions"));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddMvc(options => { })
    .AddJsonOptions(options => options.JsonSerializerOptions.AllowTrailingCommas = true);

builder.Services.RegisterDomainServices();
builder.Services.AddControllers();
builder.Services.AddSwaggerDocument(document =>
{
    document.Title = "Radancy Banking App Test";
    document.Version = "v1";
});
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Radancy Banking App Test", Version = "v1" });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseOpenApi();
app.UseSwaggerUi3();
app.UseStaticFiles();
app.UseRouting();
app.UseCors();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
app.Run();