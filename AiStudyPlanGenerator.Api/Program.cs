using AiStudyPlanGenerator.Api.Options;
using AiStudyPlanGenerator.Api.Services;
using AiStudyPlanGenerator.Api.Skills;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddOptions<ClaudeOptions>()
    .Bind(builder.Configuration.GetSection(ClaudeOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddHttpClient<StudyPlanService>();

builder.Services.AddScoped<IPromptSkill, BeginnerLearningSkill>();
builder.Services.AddScoped<IPromptSkill, IntermediateLearningSkill>();
builder.Services.AddScoped<IPromptSkill, AdvancedLearningSkill>();

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

await app.RunAsync();
