using MDWatch.Model;
using Microsoft.Extensions.Azure;
using RESTApi.Mapper;
using RESTApi.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//AutoMapper configuration
builder.Services.AddAutoMapper(typeof(CandidateMapperProfile));
builder.Services.AddAutoMapper(typeof(FinanceTotalsMapperProfile));
builder.Services.AddAutoMapper(typeof(ScheduleBDetailMapperProfile));
//Repository configuration
builder.Services.AddSingleton<ICandidateRepository<Candidate>, CandidateRepository>();
builder.Services.AddSingleton<IFinanceTotalsRepository<CandidateHistoryTotal>, FinanceTotalsRepository>();
builder.Services.AddSingleton<IScheduleBDetailRepository<ScheduleBByRecipientID>, ScheduleBDetailRepository>();
builder.Services.AddSingleton<IScheduleBOverviewRepository<ScheduleBCandidateOverview>, ScheduleBOverviewRepository>();
builder.Services.AddSingleton<IUINavRepository, UINavRepository>();
//cors configuration
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        {
            policy.AllowAnyOrigin().AllowAnyHeader();
        });
});
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddBlobServiceClient(builder.Configuration["default:blob"], preferMsi: true);
    clientBuilder.AddQueueServiceClient(builder.Configuration["default:queue"], preferMsi: true);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();
app.UseAuthorization();

app.MapControllers();

app.Run();