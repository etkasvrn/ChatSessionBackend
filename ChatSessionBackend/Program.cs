using ChatSessionBackend.Models;
using ChatSessionBackend.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Initialize teams
var teams = new List<Team>
{
    new Team
    {
        Name = "Team A",
        Agents = new List<Agent>
        {
            new Agent { Name = "Team Lead", Seniority = SeniorityLevel.TeamLead },
            new Agent { Name = "Mid-Level 1", Seniority = SeniorityLevel.MidLevel },
            new Agent { Name = "Mid-Level 2", Seniority = SeniorityLevel.MidLevel },
            new Agent { Name = "Junior", Seniority = SeniorityLevel.Junior }
        }
    },
    // Add more teams as necessary...
};

// Register services
builder.Services.AddSingleton(teams);  // Register the list of teams as a singleton
builder.Services.AddSingleton<IAgentService, AgentService>();
builder.Services.AddSingleton<IQueueService>(new QueueService(24)); // Example queue length
builder.Services.AddSingleton<ChatAssignmentService>();

// Add services to the container for Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Chat Session API",
        Description = "API for managing chat sessions with agents",
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline for Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Chat Session API v1");
        options.RoutePrefix = "swagger"; // Swagger UI at the root
    });
}

// Define minimal API endpoints
app.MapPost("/api/chat/create", (IQueueService queueService, ChatAssignmentService chatAssignmentService) =>
{
    var chatSession = new ChatSession();
    var result = queueService.Enqueue(chatSession);

    if (result.IsSuccess)
    {
        chatAssignmentService.AssignChats();
        return Results.Ok(chatSession.Id);
    }

    return Results.BadRequest(result.Message);
});

app.MapGet("/api/chat/poll/{chatId}", (Guid chatId, IQueueService queueService) =>
{
    var result = queueService.Poll(chatId);

    if (result.IsSuccess)
    {
        return Results.Ok("Chat session active");
    }

    return Results.BadRequest("Chat session inactive");
});

app.Run();
