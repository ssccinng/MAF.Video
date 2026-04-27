using Azure.AI.OpenAI;
using ModelContextProtocol.Client;
using OpenAI.Chat;

await using var mcpClient = await McpClient.CreateAsync(new StdioClientTransport(new()
{
    Name = "MCPServer",
    Command = "npx",
    Arguments = ["-y", "--verbose", "@modelcontextprotocol/server-github"],

}));
//await using var mcpClientCs = await McpClient.CreateAsync(new StdioClientTransport(new()
//{
//    Name = "MCPServer1",
//    Command = @"G:\CSharpDev\Learn\MAF.Video\McpServerLocal\bin\Debug\net10.0\win-x64\McpServerLocal.exe",

//}));


await using var mcpClientCsHttp = await McpClient.CreateAsync(new HttpClientTransport(new()
{
    Name = "MCPServerHttp",
    Endpoint = new Uri("http://localhost:6187"),

}));


Console.WriteLine(string.Join(",", (await mcpClientCsHttp.ListToolsAsync()).Select(s => s.Name)));
var tools = await mcpClientCsHttp.ListToolsAsync();

var aa = new AzureOpenAIClient(
        new Uri(Environment.GetEnvironmentVariable("AZURE_AI_ENDPOINT")),
        new Azure.Identity.AzureCliCredential());


var ai = new AzureOpenAIClient(
        new Uri(Environment.GetEnvironmentVariable("AZURE_AI_ENDPOINT")),
        new Azure.Identity.AzureCliCredential())
        .GetChatClient("gpt-4o")
        .AsAIAgent(tools: [..tools
        //, ..tools
        ]);



Console.WriteLine(await ai.RunAsync("给我一个0-10之间随机数"));