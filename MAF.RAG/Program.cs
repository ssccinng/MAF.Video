using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using OpenAI.Chat;

TextSearchProviderOptions providerOptions = new TextSearchProviderOptions()
{
    SearchTime = TextSearchProviderOptions.TextSearchBehavior.BeforeAIInvoke
};

AIAgent ai = new AzureOpenAIClient(
        new Uri(Environment.GetEnvironmentVariable("AZURE_AI_ENDPOINT")),
        new Azure.Identity.AzureCliCredential())
        .GetChatClient("gpt-4o")
        .AsAIAgent(new ChatClientAgentOptions
        {
            ChatOptions = new() { Instructions = "你是一个充满宇宙诗意的AI助手，会很有想象力/诗意的回答用户的问题. 当数据源可用时，请优先使用数据源中的信息来回答问题，并且在回答的末尾注明信息来源的名称." },
            AIContextProviders = [new TextSearchProvider(SearchAdapter, providerOptions)],
        });

Console.WriteLine(await ai.RunAsync("4月21日发生了什么？"));


static Task<IEnumerable<TextSearchProvider.TextSearchResult>> SearchAdapter(string query, CancellationToken cancellationToken)
{
    // The mock search inspects the user's question and returns pre-defined snippets
    // that resemble documents stored in an external knowledge source.
    List<TextSearchProvider.TextSearchResult> results = new();

    if (query.Contains("4月21日"))
    {
        results.Add(new()
        {
            SourceName = "4月21号事件",
            SourceLink = "none",
            Text = "无聊的年即将发布MAF的RAG相关的视频."
        });
    }

    return Task.FromResult<IEnumerable<TextSearchProvider.TextSearchResult>>(results);
}