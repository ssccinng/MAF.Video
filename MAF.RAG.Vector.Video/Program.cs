using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.SqliteVec;
using OpenAI.Chat;


TextSearchProviderOptions textSearchOptions = new()
{
    SearchTime = TextSearchProviderOptions.TextSearchBehavior.BeforeAIInvoke,
    // 搜索时保留最近几轮对话，便于处理依赖上下文的问题。
    RecentMessageMemoryLimit = 5
};

var endpoint = Environment.GetEnvironmentVariable("AZURE_AI_ENDPOINT") ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set.");
var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "gpt-4o";

var embeddingDeploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_EMBEDDING_DEPLOYMENT_NAME") ?? "text-embedding-3-large";

var videoMarkdownPath = Path.Combine(AppContext.BaseDirectory, "year_video.md");

AzureOpenAIClient azureOpenAIClient = new(
    new Uri(endpoint),
    new DefaultAzureCredential());


VectorStore vectorStore = new SqliteVectorStore("Data Source=videoData.db;", new()
{
    EmbeddingGenerator = azureOpenAIClient.GetEmbeddingClient(embeddingDeploymentName).AsIEmbeddingGenerator()
});

var documentationCollection = vectorStore.GetCollection<Guid, DocumentationChunk>("video_documentation");

await documentationCollection.EnsureCollectionDeletedAsync();    
await documentationCollection.EnsureCollectionExistsAsync();    

await UploadDataFromMarkdownFileAsync(videoMarkdownPath, "无聊的年视频整理", documentationCollection, 1000, 200);

Func<string, CancellationToken, Task<IEnumerable<TextSearchProvider.TextSearchResult>>> SearchAdapter = async (text, ct) =>
{
    List<TextSearchProvider.TextSearchResult> results = [];
    await foreach (var result in documentationCollection.SearchAsync(text, 5, cancellationToken: ct))
    {
        results.Add(new TextSearchProvider.TextSearchResult
        {
            SourceName = result.Record.SourceName,
            SourceLink = result.Record.SourceLink,
            Text = result.Record.Text ?? string.Empty,
            RawRepresentation = result
        });
    }
    return results;
};


AIAgent ai = new AzureOpenAIClient(
        new Uri(Environment.GetEnvironmentVariable("AZURE_AI_ENDPOINT")),
        new Azure.Identity.AzureCliCredential())
        .GetChatClient("gpt-4o")
        .AsAIAgent(new ChatClientAgentOptions
        {
            ChatOptions = new() { Instructions = "你是一个充满宇宙诗意的AI助手，会很有想象力/诗意的回答用户的问题. 当数据源可用时，请优先使用数据源中的信息来回答问题，并且在回答的末尾注明信息来源的名称." },
            AIContextProviders = [new TextSearchProvider(SearchAdapter, textSearchOptions)],
        });

AgentSession session = await ai.CreateSessionAsync();

Console.WriteLine("查询最新的视频");
Console.WriteLine(await ai.RunAsync("最新的视频是哪一个？"));

Console.WriteLine(await ai.RunAsync("播放量最高的视频是哪一个？"));
Console.WriteLine(await ai.RunAsync("Microsoft Agent Framework相关的视频呢?"));
Console.WriteLine(await ai.RunAsync(".NET相关的视频呢?"));








static async Task UploadDataFromMarkdownFileAsync(string markdownFilePath, string sourceName, VectorStoreCollection<Guid, DocumentationChunk> vectorStoreCollection, int chunkSize, int overlap)
{
    if (!File.Exists(markdownFilePath))
    {
        throw new FileNotFoundException("未找到视频数据文件。", markdownFilePath);
    }

    var markdown = await File.ReadAllTextAsync(markdownFilePath);

    var chunks = new List<DocumentationChunk>();
    for (int i = 0; i < markdown.Length; i += chunkSize)
    {
        var chunk = new DocumentationChunk
        {
            Key = Guid.NewGuid(),
            SourceLink = Path.GetFileName(markdownFilePath),
            SourceName = sourceName,
            Text = markdown.Substring(i, Math.Min(chunkSize + overlap, markdown.Length - i))
        };
        chunks.Add(chunk);
    }

    await vectorStoreCollection.UpsertAsync(chunks);
}

internal sealed class DocumentationChunk
{
    [VectorStoreKey]
    public Guid Key { get; set; }
    [VectorStoreData]
    public string SourceLink { get; set; } = string.Empty;
    [VectorStoreData]
    public string SourceName { get; set; } = string.Empty;
    [VectorStoreData]
    public string Text { get; set; } = string.Empty;
    [VectorStoreVector(Dimensions: 3072)]
    public string Embedding => this.Text;
}