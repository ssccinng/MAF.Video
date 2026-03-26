using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI.Chat;
using System.ComponentModel;


AIAgent ai = new AzureOpenAIClient(
        new Uri(Environment.GetEnvironmentVariable("AZURE_AI_ENDPOINT")),
        new Azure.Identity.AzureCliCredential())
        .GetChatClient("o3")
        .AsAIAgent("你是一个充满宇宙诗意的AI助手，会很有想象力/诗意的回答用户的问题", "Cosmos AI",
        tools: [AIFunctionFactory.Create(GetPerson)]
        );

var image = File.ReadAllBytes(@"C:\Users\scixing\Pictures\@O@Y$G({G$86NJRP0](0%SP.png");

var file = File.ReadAllBytes(@$"C:\Users\scixing\Downloads\bili_1773883776128.bcc");
var messages = new Microsoft.Extensions.AI
    .ChatMessage(ChatRole.User, [
        new TextContent("这张图片是什么"),
        new DataContent(image, "image/png")
        ]);




Console.WriteLine( await ai.RunAsync(messages));
//Console.WriteLine( await ai.RunAsync("杭州的天气，如何？"));
Console.WriteLine( await ai.RunAsync<Person>("随便给我一个人的信息", serializerOptions: new(System.Text.Json.JsonSerializerDefaults.General)));
//var person = await ai.RunAsync<Person>("随便给我一个人的信息", serializerOptions: new(System.Text.Json.JsonSerializerDefaults.General));
//person.Result

[Description("获取指定城市的天气")]
string GetWeather([Description("指定的城市")]string city) => $"The weather in {city} is sunny with a high of 25°C and a low of 15°C.";

[Description("获取随机人物信息")]

Person GetPerson() => new Person("Veyle", "1000");

public record Person(string Name, string Age);