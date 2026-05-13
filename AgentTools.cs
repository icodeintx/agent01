using OpenAI.Chat;
using System.Text.Json;

internal static class AgentTools
{
    public static ChatTool GetWorkspaceRootDefinition { get; } = ChatTool.CreateFunctionTool(
        functionName: "GetWorkspaceRoot",
        functionDescription: "Returns the absolute path of the current tool access root.",
        functionParameters: BinaryData.FromString("""
        {
          "type": "object",
          "properties": {}
        }
        """)
    );

    public static ChatTool ReadFileDefinition { get; } = ChatTool.CreateFunctionTool(
        functionName: "ReadFile",
        functionDescription: "Reads a text file from the approved workspace using a relative path.",
        functionParameters: BinaryData.FromString("""
        {
          "type": "object",
          "properties": {
            "relativePath": {
              "type": "string",
              "description": "Relative path inside the workspace, for example Program.cs"
            }
          },
          "required": [ "relativePath" ]
        }
        """)
    );

    public static IReadOnlyList<ChatTool> Definitions =>
    [
                GetWorkspaceRootDefinition,
        ReadFileDefinition,
        ListFilesDefinition
    ];

        public static ChatTool ListFilesDefinition { get; } = ChatTool.CreateFunctionTool(
                functionName: "ListFiles",
                functionDescription: "Lists files and directories inside the approved workspace using a relative path.",
                functionParameters: BinaryData.FromString("""
                {
                    "type": "object",
                    "properties": {
                        "relativePath": {
                            "type": "string",
                            "description": "Relative directory path inside the workspace. Use . for the workspace root.",
                            "default": "."
                        }
                    }
                }
                """)
        );

    public static string Invoke(ChatToolCall toolCall, string workspaceRoot)
    {
        try
        {
            return toolCall.FunctionName switch
            {
                "GetWorkspaceRoot" => GetWorkspaceRoot(workspaceRoot),
                "ReadFile" => ReadFile(toolCall.FunctionArguments, workspaceRoot),
                "ListFiles" => ListFiles(toolCall.FunctionArguments, workspaceRoot),
                _ => $"Unknown tool: {toolCall.FunctionName}"
            };
        }
        catch (JsonException)
        {
            return "Invalid tool arguments.";
        }
        catch (Exception ex)
        {
            return $"Tool error: {ex.Message}";
        }
    }

    private static string ReadFile(BinaryData functionArguments, string workspaceRoot)
    {
        using var arguments = JsonDocument.Parse(functionArguments);

        if (!arguments.RootElement.TryGetProperty("relativePath", out var relativePathElement))
            return "Missing required argument: relativePath.";

        var relativePath = relativePathElement.GetString();

        if (string.IsNullOrWhiteSpace(relativePath))
            return "relativePath must not be empty.";

        if (!TryResolveWorkspacePath(workspaceRoot, relativePath, out var fullPath))
            return "Access denied.";

        if (!File.Exists(fullPath))
            return "File not found.";

        return File.ReadAllText(fullPath);
    }

    private static string ListFiles(BinaryData functionArguments, string workspaceRoot)
    {
        using var arguments = JsonDocument.Parse(functionArguments);

        var relativePath = ".";

        if (arguments.RootElement.TryGetProperty("relativePath", out var relativePathElement) &&
            relativePathElement.ValueKind == JsonValueKind.String)
        {
            relativePath = relativePathElement.GetString() ?? ".";
        }

        if (!TryResolveWorkspacePath(workspaceRoot, relativePath, out var fullPath))
            return "Access denied.";

        if (!Directory.Exists(fullPath))
            return "Directory not found.";

        var entries = Directory
            .EnumerateFileSystemEntries(fullPath)
            .OrderBy(Path.GetFileName)
            .Take(100)
            .Select(path =>
            {
                var name = Path.GetFileName(path);
                var kind = Directory.Exists(path) ? "dir" : "file";
                return $"[{kind}] {name}";
            });

        var listing = string.Join(Environment.NewLine, entries);
        return string.IsNullOrWhiteSpace(listing) ? "Directory is empty." : listing;
    }

    private static string GetWorkspaceRoot(string workspaceRoot) => Path.GetFullPath(workspaceRoot);

    private static bool TryResolveWorkspacePath(string workspaceRoot, string relativePath, out string fullPath)
    {
        var fullWorkspacePath = Path.GetFullPath(workspaceRoot);
        fullPath = Path.GetFullPath(Path.Combine(fullWorkspacePath, relativePath));
        var relativeResolvedPath = Path.GetRelativePath(fullWorkspacePath, fullPath);

        return !relativeResolvedPath.StartsWith("..", StringComparison.OrdinalIgnoreCase) &&
               !Path.IsPathRooted(relativeResolvedPath);
    }
}