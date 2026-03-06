using Azure;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using OpenAI.Chat;

Console.WriteLine("Hello, World!");
AIAgent ai =
    new AzureOpenAIClient(
            new Uri(Environment.GetEnvironmentVariable("AZURE_AI_ENDPOINT")),
            new AzureCliCredential()).GetChatClient("gpt-4o")
            .AsAIAgent(new ChatClientAgentOptions() { ChatOptions = new() { MaxOutputTokens = 100 , Instructions = "Test"}, Name = "Test" })
            ;

AIAgent ai1 =
    new AzureOpenAIClient(
            new Uri("<your_url>"),
            new AzureKeyCredential("<your_api_key>")).GetChatClient("gpt-4o")
            .AsAIAgent(new ChatClientAgentOptions() { ChatOptions = new() { MaxOutputTokens = 100, Instructions = "Test" }, Name = "Test" })
            ;

Console.WriteLine(await ai1.RunAsync("Hello"));