internal sealed class TranscriptLogger : IDisposable
{
    private readonly StreamWriter writer;
    private int entryNumber;
    private int turnNumber;
    private bool turnOpen;

    public TranscriptLogger(string workspaceRoot)
    {
        var startedAt = DateTimeOffset.Now;
        var logDirectory = Path.Combine(workspaceRoot, "logs");
        Directory.CreateDirectory(logDirectory);

        LogFilePath = Path.Combine(
            logDirectory,
            $"transcript-{startedAt:yyyyMMdd-HHmmss}.md"
        );

        writer = new StreamWriter(LogFilePath, append: true)
        {
            AutoFlush = true
        };

        writer.WriteLine("# Agent Transcript");
        writer.WriteLine();
        writer.WriteLine("## Session Summary");
        writer.WriteLine();
        writer.WriteLine($"| Field | Value |");
        writer.WriteLine($"| --- | --- |");
        writer.WriteLine($"| Started | {startedAt:O} |");
        writer.WriteLine($"| Tool Access Root | {EscapeTableCell(workspaceRoot)} |");
        writer.WriteLine($"| Transcript File | {EscapeTableCell(LogFilePath)} |");
        writer.WriteLine();
        LogSystemEvent("Transcript started.");
    }

    public string LogFilePath { get; }

    public void LogUserInput(string input)
    {
        StartTurn();
        WriteTurnTextEntry("User Prompt", input);
    }

    public void LogAssistantResponse(string response)
    {
        var timestamp = DateTimeOffset.Now;
        entryNumber++;

        EnsureTurn();

        writer.WriteLine($"### Visible Response {entryNumber}");
        writer.WriteLine();
        writer.WriteLine($"- Time: {timestamp:O}");
        writer.WriteLine();
        writer.WriteLine("> [!IMPORTANT]");
        writer.WriteLine("> This is the exact response shown to the user in the console.");
        writer.WriteLine();
        writer.WriteLine("### What The User Saw");
        writer.WriteLine();
        WriteCodeBlock("text", response);
        writer.WriteLine();
    }

    public void LogToolCall(string toolName, string arguments, string result)
    {
        var timestamp = DateTimeOffset.Now;
        entryNumber++;

        EnsureTurn();

        writer.WriteLine($"### Internal Tool Call {entryNumber}: {toolName}");
        writer.WriteLine();
        writer.WriteLine($"- Time: {timestamp:O}");
        writer.WriteLine();
        writer.WriteLine("> This data is used internally by the agent and is not shown directly to the user.");
        writer.WriteLine();
        writer.WriteLine("### Arguments");
        writer.WriteLine();
        WriteCodeBlock("json", arguments);
        writer.WriteLine();
        writer.WriteLine("### Result");
        writer.WriteLine();
        WriteCodeBlock("text", result);
        writer.WriteLine();
    }

    public void LogSystemEvent(string message)
    {
        CloseTurn();
        WriteTopLevelTextEntry("System Event", message);
    }

    public void LogError(Exception exception)
    {
        EnsureTurn();
        WriteTurnTextEntry("Error", exception.ToString());
    }

    public void Dispose()
    {
        LogSystemEvent("Transcript ended.");
        writer.Dispose();
    }

    private void StartTurn()
    {
        CloseTurn();
        turnNumber++;
        turnOpen = true;

        writer.WriteLine($"## Turn {turnNumber}");
        writer.WriteLine();
    }

    private void EnsureTurn()
    {
        if (!turnOpen)
        {
            StartTurn();
        }
    }

    private void CloseTurn()
    {
        if (!turnOpen)
            return;

        writer.WriteLine("---");
        writer.WriteLine();
        turnOpen = false;
    }

    private void WriteTopLevelTextEntry(string title, string message)
    {
        var timestamp = DateTimeOffset.Now;
        entryNumber++;

        writer.WriteLine($"## {title} {entryNumber}");
        writer.WriteLine();
        writer.WriteLine($"- Time: {timestamp:O}");
        writer.WriteLine();
        WriteCodeBlock("text", message);
        writer.WriteLine();
    }

    private void WriteTurnTextEntry(string title, string message)
    {
        var timestamp = DateTimeOffset.Now;
        entryNumber++;

        writer.WriteLine($"### {title} {entryNumber}");
        writer.WriteLine();
        writer.WriteLine($"- Time: {timestamp:O}");
        writer.WriteLine();
        WriteCodeBlock("text", message);
        writer.WriteLine();
    }

    private void WriteCodeBlock(string language, string content)
    {
        writer.WriteLine($"~~~{language}");
        writer.WriteLine(content);
        writer.WriteLine("~~~");
    }

    private static string EscapeTableCell(string value) => value.Replace("|", "\\|");
}