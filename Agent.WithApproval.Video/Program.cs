using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI.Chat;
using System.ComponentModel;
using ChatMessage = Microsoft.Extensions.AI.ChatMessage;

[Description("Get the weather for a given location.")]
static string GetWeather([Description("The location to get the weather for.")] string location)
    => $"The weather in {location} is cloudy with a high of 15°C.";


AIAgent ai = new AzureOpenAIClient(
        new Uri(Environment.GetEnvironmentVariable("AZURE_AI_ENDPOINT")),
        new Azure.Identity.AzureCliCredential())
        .GetChatClient("gpt-4o")
        .AsAIAgent("你是一个充满宇宙诗意的AI助手，会很有想象力/诗意的回答用户的问题", "Cosmos AI"
        , tools: [
            new ApprovalRequiredAIFunction(
                AIFunctionFactory.Create(GetWeather, "getweather")
            )
            ] 
        
        );

AgentSession session = await ai.CreateSessionAsync();

var responses = await ai.RunAsync("杭州天气如何？", session);

var approvalRequests = responses
    .Messages
    .SelectMany(m => m.Contents)
    .OfType<ToolApprovalRequestContent>()
    .ToList();

while (approvalRequests.Count > 0)
{
    List<ChatMessage> userInputResponses = approvalRequests
        .ConvertAll(functionApprovalRequest =>
        {
            Console.WriteLine($"agent 想要调用工具，输入Y允许: Name {((FunctionCallContent)functionApprovalRequest.ToolCall).Name}");
            return new ChatMessage(ChatRole.User, [functionApprovalRequest.CreateResponse(Console.ReadLine()?.Equals("Y", StringComparison.OrdinalIgnoreCase) ?? false)]);
        });

    responses = await ai.RunAsync(userInputResponses, session);
    approvalRequests = responses
        .Messages
        .SelectMany(m => m.Contents)
        .OfType<ToolApprovalRequestContent>()
        .ToList();
}


Console.WriteLine(responses.Text);