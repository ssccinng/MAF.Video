using Anthropic;
using Microsoft.Agents.AI;

Console.WriteLine("Hello, World!");
var apiKey = Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY");
var deploymentName = Environment.GetEnvironmentVariable("ANTHROPIC_DEPLOYMENT_NAME") ?? "claude-haiku-4-5";

AnthropicClient client = new() {  ApiKey = apiKey };

AIAgent agent = client.AsAIAgent(
    model: deploymentName,
    name: "HelpfulAssistant",
    instructions: "You are a helpful assistant.");

// Invoke the agent and output the text result.
Console.WriteLine(await agent.RunAsync("Hello, how can you help me?"));