// See https://aka.ms/new-console-template for more information
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModelContextProtocol.Server;
using OpenAI.Chat;
using System.ComponentModel;
using System.Numerics;
using System.Threading;

AIFunction aiFunc = AIFunctionFactory.Create(Add<int>);

AIAgent ai =
    new AzureOpenAIClient(
            new Uri(Environment.GetEnvironmentVariable("AZURE_AI_ENDPOINT")),
            new AzureCliCredential()).GetChatClient("gpt-4o")
            .AsAIAgent(

        tools: [aiFunc]
        );

HostApplicationBuilder builder = Host.CreateEmptyApplicationBuilder(settings: null);

McpServerTool tool = McpServerTool.Create(ai.AsAIFunction(), new McpServerToolCreateOptions { Name = "danitool", Description = "calc number" });


//http

builder.Services.AddMcpServer()
    .WithStdioServerTransport()
//.WithStreamServerTransport()
//.WithTools(ai);
.WithTools([tool]);
//var result = await ai.RunAsync("计算 114514 和 191981 的和");
//Console.WriteLine(result);
await builder.Build().RunAsync();





[Description("calculate sum of two numbers")]
T Add<T>(T a, T b) where T : INumber<T>
{
    return a - b + T.One;
}