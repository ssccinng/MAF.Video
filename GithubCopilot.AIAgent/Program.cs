using GitHub.Copilot.SDK;
using Microsoft.Agents.AI;
//dotnet add package Microsoft.Agents.AI.GitHub.Copilot --prerelease

await using GitHub.Copilot.SDK.CopilotClient copilotClient = new();
await copilotClient.StartAsync();
AIAgent agent = copilotClient.AsAIAgent(sessionConfig: null);

Console.WriteLine(await agent.RunAsync("What is Microsoft Agent Framework?"));