
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI.Chat;
using System.ComponentModel;


AIAgent ai = new AzureOpenAIClient(
        new Uri(Environment.GetEnvironmentVariable("AZURE_AI_ENDPOINT")),
        new Azure.Identity.AzureCliCredential())
        .GetChatClient("gpt-4o")
        .AsAIAgent("你是一个充满宇宙诗意的AI助手，会很有想象力/诗意的回答用户的问题", "Cosmos AI",
        tools: [AIFunctionFactory.Create(GetWeather)]
        );



var session = await ai.CreateSessionAsync();

Console.WriteLine(await ai.RunAsync("杭州的天气，如何？", session));


var save = await ai.SerializeSessionAsync(session);

Console.WriteLine(save);

AIAgent ai2 = new AzureOpenAIClient(
        new Uri(Environment.GetEnvironmentVariable("AZURE_AI_ENDPOINT")),
        new Azure.Identity.AzureCliCredential())
        .GetChatClient("gpt-4o")
        .AsAIAgent("你是一个充满宇宙诗意的AI助手，会很有想象力/诗意的回答用户的问题", "Cosmos AI",
        tools: [AIFunctionFactory.Create(GetWeather)]
        );


var session2 = await ai2.DeserializeSessionAsync(save);
Console.WriteLine(await ai2.RunAsync("杭州的天气，如何？", session2));
//person.Result

[Description("获取指定城市的天气")]
string GetWeather([Description("指定的城市")] string city) => $"The weather in {city} is sunny with a high of 25°C and a low of 15°C.";


public record Person(string Name, string Age);