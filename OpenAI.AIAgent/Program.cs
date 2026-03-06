using Microsoft.Agents.AI;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;

//dotnet add package Microsoft.Agents.AI.OpenAI --prerelease
OpenAIClient client = new OpenAIClient("<your_api_key>");
OpenAIClient customClient = new OpenAIClient(new ApiKeyCredential("<your_api_key>"), new OpenAIClientOptions()
{ Endpoint = new Uri("<your_endpoint>") });
var chatClient = client.GetChatClient("gpt-4o-mini");

AIAgent agent = chatClient.AsAIAgent(
    instructions: "You are good at telling jokes.",
    name: "Joker");

Console.WriteLine(await agent.RunAsync("Tell me a joke about a pirate."));