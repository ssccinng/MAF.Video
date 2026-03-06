//dotnet add package Azure.AI.OpenAI --prerelease
//dotnet add package Azure.Identity
//dotnet add package Microsoft.Agents.AI.OpenAI --prerelease

// Microsoft Agent Framework

using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI.Chat;

AIAgent ai = new AzureOpenAIClient(
        new Uri(Environment.GetEnvironmentVariable("AZURE_AI_ENDPOINT")),
        new Azure.Identity.AzureCliCredential())
        .GetChatClient("gpt-4o")
        .AsAIAgent("你是一个充满宇宙诗意的AI助手，会很有想象力/诗意的回答用户的问题", "Cosmos AI");
//.AsAIAgent(new ChatClientAgentOptions() { ChatOptions = new() { MaxOutputTokens = 100, Instructions = "Test" }, Name = "Test" })
;

var result = await ai.RunAsync("What is Microsoft Agent Framework?");

Console.WriteLine(result.Text);

var stream =  ai.RunStreamingAsync("What is Microsoft Agent Framework?");


Console.WriteLine();
await foreach (var item in stream)
{
    Console.Write(item.Text);
}

Console.WriteLine();
Console.WriteLine();

var messages = new Microsoft.Extensions.AI
    .ChatMessage(ChatRole.User, [
        new TextContent("这张图片是什么"),
        new UriContent(new Uri("https://gips1.baidu.com/it/u=2460156916,875951288&fm=3030&app=3030&size=re3,2&q=75&n=0&g=4n&f=JPEG&fmt=auto&maxorilen2heic=2000000?s=1C94CC12CFA059011C55C0D60300D0B3"),  "image/jpeg")
        ]);

result = await ai.RunAsync([messages]);


Console.WriteLine(result.Text);

var session = await ai.CreateSessionAsync();

Console.WriteLine(await ai.RunAsync("791 + 197 = ?", session));
Console.WriteLine(await ai.RunAsync("then add 555 = ?", session));

var save = await ai.SerializeSessionAsync(session);

Console.WriteLine(save);

AIAgent ai2 = new AzureOpenAIClient(
        new Uri(Environment.GetEnvironmentVariable("AZURE_AI_ENDPOINT")),
        new Azure.Identity.AzureCliCredential())
        .GetChatClient("gpt-4o")
        .AsAIAgent("你是一个充满宇宙诗意的AI助手，会很有想象力/诗意的回答用户的问题", "Cosmos AI");

var session2 = await ai2.DeserializeSessionAsync(save);






Console.WriteLine(await ai.RunAsync("then add 777 = ?", session2));

