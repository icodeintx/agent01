using OpenAI;
using OpenAI.Chat;
using Microsoft.Extensions.Configuration;

var configuration = BuildConfiguration();
var apiKey = configuration["OpenAI:ApiKey"];

if (string.IsNullOrWhiteSpace(apiKey))
{
    Console.WriteLine("OpenAI API key not found. Set OpenAI:ApiKey in appsettings.local.json or OPENAI__APIKEY.");
    return;
}

var client = new OpenAIClient(apiKey);
var chatClient = client.GetChatClient("gpt-4.1-mini");
var workspaceRoot = ResolveWorkspaceRoot();

using var transcript = new TranscriptLogger(workspaceRoot);

var messages = new List<ChatMessage>
{
    new SystemChatMessage("""
        You are a helpful local AI agent.
        Be concise.
        Ask clarifying questions only when required.
        When the user asks to export, save, or generate a markdown report, create the file with ExportMarkdownReport and then tell the user where it was saved.
        """)
};

Console.WriteLine("Simple Agent");
Console.WriteLine("Type 'exit' to quit.");
Console.WriteLine($"Tool access root: {workspaceRoot}");
Console.WriteLine($"Transcript: {transcript.LogFilePath}");
Console.WriteLine();

var options = new ChatCompletionOptions();

foreach (var tool in AgentTools.Definitions)
{
    options.Tools.Add(tool);
}

while (true)
{
    Console.Write("You: ");
    var input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input))
        continue;

    if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
        break;

    messages.Add(new UserChatMessage(input));
    transcript.LogUserInput(input);

    try
    {
        bool requiresAction;

        do
        {
            requiresAction = false;

            var response = await chatClient.CompleteChatAsync(messages, options);
            var completion = response.Value;

            switch (completion.FinishReason)
            {
                case ChatFinishReason.Stop:
                {
                    var answer = completion.Content.Count > 0
                        ? string.Concat(completion.Content.Select(part => part.Text))
                        : "I do not have a response.";

                    Console.WriteLine();
                    Console.WriteLine($"Agent: {answer}");
                    Console.WriteLine();

                    transcript.LogAssistantResponse(answer);
                    messages.Add(new AssistantChatMessage(completion));
                    break;
                }

                case ChatFinishReason.ToolCalls:
                {
                    messages.Add(new AssistantChatMessage(completion));

                    foreach (var toolCall in completion.ToolCalls)
                    {
                        var toolResult = AgentTools.Invoke(toolCall, workspaceRoot);
                        transcript.LogToolCall(toolCall.FunctionName, toolCall.FunctionArguments.ToString(), toolResult);
                        messages.Add(new ToolChatMessage(toolCall.Id, toolResult));
                    }

                    requiresAction = true;
                    break;
                }

                default:
                {
                    Console.WriteLine();
                    Console.WriteLine($"Agent stopped with reason: {completion.FinishReason}");
                    Console.WriteLine();

                    transcript.LogSystemEvent($"Completion finished with reason: {completion.FinishReason}");
                    messages.Add(new AssistantChatMessage(completion));
                    break;
                }
            }
        }
        while (requiresAction);
    }
    catch (Exception ex)
    {
        Console.WriteLine();
        Console.WriteLine($"Agent error: {ex.Message}");
        Console.WriteLine();
        transcript.LogError(ex);
    }

    TrimConversation(messages, maxNonSystemMessages: 12);
}

static string ResolveWorkspaceRoot()
{
    var configuredWorkspace = Environment.GetEnvironmentVariable("AGENT_WORKSPACE_ROOT");
    var workspaceRoot = string.IsNullOrWhiteSpace(configuredWorkspace)
        ? Directory.GetCurrentDirectory()
        : configuredWorkspace;

    return Path.GetFullPath(workspaceRoot);
}

static IConfiguration BuildConfiguration()
{
    return new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
        .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: false)
        .AddEnvironmentVariables()
        .Build();
}

static void TrimConversation(List<ChatMessage> messages, int maxNonSystemMessages)
{
    if (messages.Count <= maxNonSystemMessages + 1)
        return;

    var systemMessage = messages[0];
    var recentMessages = messages.Skip(Math.Max(1, messages.Count - maxNonSystemMessages)).ToList();

    messages.Clear();
    messages.Add(systemMessage);
    messages.AddRange(recentMessages);
}