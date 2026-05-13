internal sealed class TranscriptLogger : IDisposable
{
    private readonly StreamWriter writer;

    public TranscriptLogger(string workspaceRoot)
    {
        var logDirectory = Path.Combine(workspaceRoot, "logs");
        Directory.CreateDirectory(logDirectory);

        LogFilePath = Path.Combine(
            logDirectory,
            $"transcript-{DateTimeOffset.Now:yyyyMMdd-HHmmss}.log"
        );

        writer = new StreamWriter(LogFilePath, append: true)
        {
            AutoFlush = true
        };

        LogSystemEvent("Transcript started.");
    }

    public string LogFilePath { get; }

    public void LogUserInput(string input) => WriteEntry("USER", input);

    public void LogAssistantResponse(string response) => WriteEntry("AGENT", response);

    public void LogToolCall(string toolName, string arguments, string result)
    {
        WriteEntry("TOOL", $"{toolName} args: {arguments}");
        WriteEntry("TOOL_RESULT", result);
    }

    public void LogSystemEvent(string message) => WriteEntry("SYSTEM", message);

    public void LogError(Exception exception) => WriteEntry("ERROR", exception.ToString());

    public void Dispose()
    {
        LogSystemEvent("Transcript ended.");
        writer.Dispose();
    }

    private void WriteEntry(string category, string message)
    {
        writer.WriteLine($"[{DateTimeOffset.Now:O}] {category}");
        writer.WriteLine(message);
        writer.WriteLine();
    }
}