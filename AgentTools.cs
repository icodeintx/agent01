using System.Diagnostics;
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

    public static ChatTool SearchTextDefinition { get; } = ChatTool.CreateFunctionTool(
        functionName: "SearchText",
        functionDescription: "Searches for matching text in a file or directory inside the approved workspace.",
        functionParameters: BinaryData.FromString("""
        {
          "type": "object",
          "properties": {
            "relativePath": {
              "type": "string",
              "description": "Relative file or directory path inside the workspace. Use . for the workspace root."
            },
            "searchPattern": {
              "type": "string",
              "description": "Text to search for inside matching files."
            }
          },
          "required": [ "relativePath", "searchPattern" ]
        }
        """)
    );

        public static ChatTool GetFileInfoDefinition { get; } = ChatTool.CreateFunctionTool(
                functionName: "GetFileInfo",
                functionDescription: "Returns metadata for a file or directory inside the approved workspace using a relative path.",
                functionParameters: BinaryData.FromString("""
                {
                    "type": "object",
                    "properties": {
                        "relativePath": {
                            "type": "string",
                            "description": "Relative file or directory path inside the workspace."
                        }
                    },
                    "required": [ "relativePath" ]
                }
                """)
        );

        public static ChatTool GetGitStatusDefinition { get; } = ChatTool.CreateFunctionTool(
                functionName: "GetGitStatus",
                functionDescription: "Returns the git working tree status for the repository containing the workspace root.",
                functionParameters: BinaryData.FromString("""
                {
                    "type": "object",
                    "properties": {}
                }
                """)
        );

        public static ChatTool GetGitDiffDefinition { get; } = ChatTool.CreateFunctionTool(
                functionName: "GetGitDiff",
                functionDescription: "Returns the current git diff against HEAD for the repository containing the workspace root.",
                functionParameters: BinaryData.FromString("""
                {
                    "type": "object",
                    "properties": {}
                }
                """)
        );

        public static ChatTool GetRecentCommitsDefinition { get; } = ChatTool.CreateFunctionTool(
                functionName: "GetRecentCommits",
                functionDescription: "Returns recent commits for the repository containing the workspace root.",
                functionParameters: BinaryData.FromString("""
                {
                    "type": "object",
                    "properties": {}
                }
                """)
        );

        public static ChatTool GetCurrentBranchDefinition { get; } = ChatTool.CreateFunctionTool(
                functionName: "GetCurrentBranch",
                functionDescription: "Returns the current git branch for the repository containing the workspace root.",
                functionParameters: BinaryData.FromString("""
                {
                    "type": "object",
                    "properties": {}
                }
                """)
        );

        public static ChatTool GetFileDiffDefinition { get; } = ChatTool.CreateFunctionTool(
                functionName: "GetFileDiff",
                functionDescription: "Returns the current git diff against HEAD for a file inside the approved workspace.",
                functionParameters: BinaryData.FromString("""
                {
                    "type": "object",
                    "properties": {
                        "path": {
                            "type": "string",
                            "description": "Relative file path inside the workspace."
                        }
                    },
                    "required": [ "path" ]
                }
                """)
        );

        public static ChatTool SearchCommitHistoryDefinition { get; } = ChatTool.CreateFunctionTool(
                functionName: "SearchCommitHistory",
                functionDescription: "Searches git commit messages, changed paths, and patch text for a query in the repository containing the workspace root.",
                functionParameters: BinaryData.FromString("""
                {
                    "type": "object",
                    "properties": {
                        "query": {
                            "type": "string",
                            "description": "Text to search for in commit history."
                        }
                    },
                    "required": [ "query" ]
                }
                """)
        );

        public static ChatTool GetCommitDetailsDefinition { get; } = ChatTool.CreateFunctionTool(
                functionName: "GetCommitDetails",
                functionDescription: "Returns details for a specific commit hash in the repository containing the workspace root.",
                functionParameters: BinaryData.FromString("""
                {
                    "type": "object",
                    "properties": {
                        "commitHash": {
                            "type": "string",
                            "description": "Commit hash or revision expression to inspect."
                        }
                    },
                    "required": [ "commitHash" ]
                }
                """)
        );

        public static ChatTool StageFileDefinition { get; } = ChatTool.CreateFunctionTool(
                functionName: "StageFile",
                functionDescription: "Stages a file or directory path inside the approved workspace for commit.",
                functionParameters: BinaryData.FromString("""
                {
                    "type": "object",
                    "properties": {
                        "path": {
                            "type": "string",
                            "description": "Relative file or directory path inside the workspace."
                        }
                    },
                    "required": [ "path" ]
                }
                """)
        );

        public static ChatTool UnstageFileDefinition { get; } = ChatTool.CreateFunctionTool(
                functionName: "UnstageFile",
                functionDescription: "Removes a file or directory path inside the approved workspace from the git index.",
                functionParameters: BinaryData.FromString("""
                {
                    "type": "object",
                    "properties": {
                        "path": {
                            "type": "string",
                            "description": "Relative file or directory path inside the workspace."
                        }
                    },
                    "required": [ "path" ]
                }
                """)
        );

        public static ChatTool ExportMarkdownReportDefinition { get; } = ChatTool.CreateFunctionTool(
                functionName: "ExportMarkdownReport",
                functionDescription: "Creates or overwrites a markdown report file inside the approved workspace.",
                functionParameters: BinaryData.FromString("""
                {
                    "type": "object",
                    "properties": {
                        "relativePath": {
                            "type": "string",
                            "description": "Relative markdown file path inside the workspace, for example reports/project-summary.md"
                        },
                        "markdownContent": {
                            "type": "string",
                            "description": "Full markdown content to write to the report file."
                        }
                    },
                    "required": [ "relativePath", "markdownContent" ]
                }
                """)
        );

    public static IReadOnlyList<ChatTool> Definitions =>
    [
        GetWorkspaceRootDefinition,
        ReadFileDefinition,
        SearchTextDefinition,
                GetFileInfoDefinition,
                GetGitStatusDefinition,
                GetGitDiffDefinition,
                GetRecentCommitsDefinition,
                GetCurrentBranchDefinition,
                GetFileDiffDefinition,
                SearchCommitHistoryDefinition,
                GetCommitDetailsDefinition,
                StageFileDefinition,
                UnstageFileDefinition,
                ExportMarkdownReportDefinition,
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
                "SearchText" => SearchText(toolCall.FunctionArguments, workspaceRoot),
                "GetFileInfo" => GetFileInfo(toolCall.FunctionArguments, workspaceRoot),
                "GetGitStatus" => GetGitStatus(workspaceRoot),
                "GetGitDiff" => GetGitDiff(workspaceRoot),
                "GetRecentCommits" => GetRecentCommits(workspaceRoot),
                "GetCurrentBranch" => GetCurrentBranch(workspaceRoot),
                "GetFileDiff" => GetFileDiff(toolCall.FunctionArguments, workspaceRoot),
                "SearchCommitHistory" => SearchCommitHistory(toolCall.FunctionArguments, workspaceRoot),
                "GetCommitDetails" => GetCommitDetails(toolCall.FunctionArguments, workspaceRoot),
                "StageFile" => StageFile(toolCall.FunctionArguments, workspaceRoot),
                "UnstageFile" => UnstageFile(toolCall.FunctionArguments, workspaceRoot),
                "ExportMarkdownReport" => ExportMarkdownReport(toolCall.FunctionArguments, workspaceRoot),
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

    private static string SearchText(BinaryData functionArguments, string workspaceRoot)
    {
        using var arguments = JsonDocument.Parse(functionArguments);

        if (!arguments.RootElement.TryGetProperty("relativePath", out var relativePathElement))
            return "Missing required argument: relativePath.";

        if (!arguments.RootElement.TryGetProperty("searchPattern", out var searchPatternElement))
            return "Missing required argument: searchPattern.";

        var relativePath = relativePathElement.GetString();
        var searchPattern = searchPatternElement.GetString();

        if (string.IsNullOrWhiteSpace(relativePath))
            return "relativePath must not be empty.";

        if (string.IsNullOrWhiteSpace(searchPattern))
            return "searchPattern must not be empty.";

        if (!TryResolveWorkspacePath(workspaceRoot, relativePath, out var fullPath))
            return "Access denied.";

        IEnumerable<string> files;

        if (File.Exists(fullPath))
        {
            files = [fullPath];
        }
        else if (Directory.Exists(fullPath))
        {
            files = Directory.EnumerateFiles(fullPath, "*", SearchOption.AllDirectories);
        }
        else
        {
            return "Path not found.";
        }

        var fullWorkspacePath = Path.GetFullPath(workspaceRoot);
        var matches = new List<string>();

        foreach (var file in files)
        {
            try
            {
                var lineNumber = 0;

                foreach (var line in File.ReadLines(file))
                {
                    lineNumber++;

                    if (!line.Contains(searchPattern, StringComparison.OrdinalIgnoreCase))
                        continue;

                    var displayPath = Path.GetRelativePath(fullWorkspacePath, file);
                    matches.Add($"{displayPath}:{lineNumber}: {line.Trim()}");

                    if (matches.Count >= 200)
                        return string.Join(Environment.NewLine, matches) + Environment.NewLine + "Results truncated at 200 matches.";
                }
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
        }

        return matches.Count == 0 ? "No matches found." : string.Join(Environment.NewLine, matches);
    }

    private static string GetFileInfo(BinaryData functionArguments, string workspaceRoot)
    {
        using var arguments = JsonDocument.Parse(functionArguments);

        if (!arguments.RootElement.TryGetProperty("relativePath", out var relativePathElement))
            return "Missing required argument: relativePath.";

        var relativePath = relativePathElement.GetString();

        if (string.IsNullOrWhiteSpace(relativePath))
            return "relativePath must not be empty.";

        if (!TryResolveWorkspacePath(workspaceRoot, relativePath, out var fullPath))
            return "Access denied.";

        var info = File.Exists(fullPath)
            ? CreateFileInfoPayload(workspaceRoot, fullPath)
            : Directory.Exists(fullPath)
                ? CreateDirectoryInfoPayload(workspaceRoot, fullPath)
                : null;

        return info is null
            ? "Path not found."
            : JsonSerializer.Serialize(info, new JsonSerializerOptions { WriteIndented = true });
    }

    private static string GetGitStatus(string workspaceRoot)
    {
        if (!TryGetGitRepositoryRoot(workspaceRoot, out var repositoryRoot, out var error))
            return error;

        return RunGitCommand(
            repositoryRoot,
            ["status", "--short", "--branch"],
            emptyOutputMessage: "Working tree clean.");
    }

    private static string GetGitDiff(string workspaceRoot)
    {
        if (!TryGetGitRepositoryRoot(workspaceRoot, out var repositoryRoot, out var error))
            return error;

        return RunGitCommand(
            repositoryRoot,
            ["diff", "--no-ext-diff", "HEAD", "--"],
            emptyOutputMessage: "No differences from HEAD.");
    }

    private static string GetRecentCommits(string workspaceRoot)
    {
        if (!TryGetGitRepositoryRoot(workspaceRoot, out var repositoryRoot, out var error))
            return error;

        return RunGitCommand(
            repositoryRoot,
            ["log", "-n", "10", "--date=short", "--pretty=format:%h %ad %an %s"],
            emptyOutputMessage: "No commits found.");
    }

    private static string GetCurrentBranch(string workspaceRoot)
    {
        if (!TryGetGitRepositoryRoot(workspaceRoot, out var repositoryRoot, out var error))
            return error;

        return RunGitCommand(
            repositoryRoot,
            ["branch", "--show-current"],
            emptyOutputMessage: "Detached HEAD.");
    }

    private static string GetFileDiff(BinaryData functionArguments, string workspaceRoot)
    {
        using var arguments = JsonDocument.Parse(functionArguments);

        if (!arguments.RootElement.TryGetProperty("path", out var pathElement))
            return "Missing required argument: path.";

        var relativePath = pathElement.GetString();

        if (string.IsNullOrWhiteSpace(relativePath))
            return "path must not be empty.";

        if (!TryResolveWorkspacePath(workspaceRoot, relativePath, out var fullPath))
            return "Access denied.";

        if (!File.Exists(fullPath))
            return "File not found.";

        if (!TryGetGitRepositoryRoot(workspaceRoot, out var repositoryRoot, out var error))
            return error;

        var fullRepositoryPath = Path.GetFullPath(repositoryRoot);
        var gitRelativePath = Path.GetRelativePath(fullRepositoryPath, fullPath);

        if (gitRelativePath.StartsWith("..", StringComparison.OrdinalIgnoreCase) || Path.IsPathRooted(gitRelativePath))
            return "File is outside the git repository.";

        return RunGitCommand(
            repositoryRoot,
            ["diff", "--no-ext-diff", "HEAD", "--", gitRelativePath],
            emptyOutputMessage: "No differences from HEAD for that file.");
    }

    private static string SearchCommitHistory(BinaryData functionArguments, string workspaceRoot)
    {
        using var arguments = JsonDocument.Parse(functionArguments);

        if (!arguments.RootElement.TryGetProperty("query", out var queryElement))
            return "Missing required argument: query.";

        var query = queryElement.GetString();

        if (string.IsNullOrWhiteSpace(query))
            return "query must not be empty.";

        if (!TryGetGitRepositoryRoot(workspaceRoot, out var repositoryRoot, out var error))
            return error;

        const string commitBoundary = "__COMMIT_BOUNDARY__";

        if (!TryRunGitCommand(
                repositoryRoot,
                [
                    "log",
                    "--all",
                    "-n",
                    "100",
                    "--date=short",
                    "--no-ext-diff",
                    "--name-only",
                    "-p",
                    "--unified=0",
                    $"--pretty=format:{commitBoundary}%n%H%x1f%ad%x1f%an%x1f%s%n%b"
                ],
                out var output,
                out error))
        {
            return error;
        }

        var matches = new List<string>();
        var blocks = output.Split([commitBoundary], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var block in blocks)
        {
            var lines = SplitLines(block.Trim());

            if (lines.Length == 0)
                continue;

            var metadata = lines[0].Split('\u001f', 4);

            if (metadata.Length < 4)
                continue;

            var matchKinds = GetCommitMatchKinds(block, query);

            if (matchKinds.Count == 0)
                continue;

            var hash = metadata[0];
            var date = metadata[1];
            var author = metadata[2];
            var subject = metadata[3];
            var snippet = CreateSearchSnippet(block, query);
            matches.Add($"{hash[..Math.Min(7, hash.Length)]} {date} {author} {subject} [{string.Join(", ", matchKinds)}]{Environment.NewLine}  {snippet}");

            if (matches.Count >= 20)
                break;
        }

        if (matches.Count == 0)
            return "No matching commits found.";

        return TruncateOutput(string.Join(Environment.NewLine, matches), maxLines: 120, maxCharacters: 12000);
    }

    private static string GetCommitDetails(BinaryData functionArguments, string workspaceRoot)
    {
        using var arguments = JsonDocument.Parse(functionArguments);

        if (!arguments.RootElement.TryGetProperty("commitHash", out var commitHashElement))
            return "Missing required argument: commitHash.";

        var commitHash = commitHashElement.GetString();

        if (string.IsNullOrWhiteSpace(commitHash))
            return "commitHash must not be empty.";

        if (!TryGetGitRepositoryRoot(workspaceRoot, out var repositoryRoot, out var error))
            return error;

        return RunGitCommand(
            repositoryRoot,
            ["show", "--stat", "--summary", "--date=short", "--no-ext-diff", "--format=commit %H%nAuthor: %an <%ae>%nDate: %ad%n%n%s%n%n%b", commitHash],
            emptyOutputMessage: "Commit not found.");
    }

    private static string StageFile(BinaryData functionArguments, string workspaceRoot)
    {
        if (!TryResolveGitPath(functionArguments, workspaceRoot, out var repositoryRoot, out var gitRelativePath, out var error))
            return error;

        return RunGitCommand(
            repositoryRoot,
            ["add", "--", gitRelativePath],
            emptyOutputMessage: $"Staged: {gitRelativePath}");
    }

    private static string UnstageFile(BinaryData functionArguments, string workspaceRoot)
    {
        if (!TryResolveGitPath(functionArguments, workspaceRoot, out var repositoryRoot, out var gitRelativePath, out var error))
            return error;

        return RunGitCommand(
            repositoryRoot,
            ["reset", "HEAD", "--", gitRelativePath],
            emptyOutputMessage: $"Unstaged: {gitRelativePath}");
    }

    private static string ExportMarkdownReport(BinaryData functionArguments, string workspaceRoot)
    {
        using var arguments = JsonDocument.Parse(functionArguments);

        if (!arguments.RootElement.TryGetProperty("relativePath", out var relativePathElement))
            return "Missing required argument: relativePath.";

        if (!arguments.RootElement.TryGetProperty("markdownContent", out var markdownContentElement))
            return "Missing required argument: markdownContent.";

        var relativePath = relativePathElement.GetString();
        var markdownContent = markdownContentElement.GetString();

        if (string.IsNullOrWhiteSpace(relativePath))
            return "relativePath must not be empty.";

        if (string.IsNullOrWhiteSpace(markdownContent))
            return "markdownContent must not be empty.";

        if (!relativePath.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
            relativePath += ".md";

        if (!TryResolveWorkspacePath(workspaceRoot, relativePath, out var fullPath))
            return "Access denied.";

        var directoryPath = Path.GetDirectoryName(fullPath);

        if (!string.IsNullOrWhiteSpace(directoryPath))
            Directory.CreateDirectory(directoryPath);

        File.WriteAllText(fullPath, markdownContent);

        var savedRelativePath = Path.GetRelativePath(Path.GetFullPath(workspaceRoot), fullPath);
        return $"Markdown report exported to {savedRelativePath}";
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

    private static object CreateFileInfoPayload(string workspaceRoot, string fullPath)
    {
        var fileInfo = new FileInfo(fullPath);

        return new
        {
            relativePath = Path.GetRelativePath(Path.GetFullPath(workspaceRoot), fileInfo.FullName),
            fullPath = fileInfo.FullName,
            type = "file",
            sizeBytes = fileInfo.Length,
            createdUtc = fileInfo.CreationTimeUtc,
            lastModifiedUtc = fileInfo.LastWriteTimeUtc,
            attributes = fileInfo.Attributes.ToString()
        };
    }

    private static object CreateDirectoryInfoPayload(string workspaceRoot, string fullPath)
    {
        var directoryInfo = new DirectoryInfo(fullPath);

        return new
        {
            relativePath = Path.GetRelativePath(Path.GetFullPath(workspaceRoot), directoryInfo.FullName),
            fullPath = directoryInfo.FullName,
            type = "directory",
            childCount = directoryInfo.EnumerateFileSystemInfos().Count(),
            createdUtc = directoryInfo.CreationTimeUtc,
            lastModifiedUtc = directoryInfo.LastWriteTimeUtc,
            attributes = directoryInfo.Attributes.ToString()
        };
    }

    private static bool TryResolveGitPath(BinaryData functionArguments, string workspaceRoot, out string repositoryRoot, out string gitRelativePath, out string error)
    {
        repositoryRoot = string.Empty;
        gitRelativePath = string.Empty;
        error = string.Empty;

        using var arguments = JsonDocument.Parse(functionArguments);

        if (!arguments.RootElement.TryGetProperty("path", out var pathElement))
        {
            error = "Missing required argument: path.";
            return false;
        }

        var relativePath = pathElement.GetString();

        if (string.IsNullOrWhiteSpace(relativePath))
        {
            error = "path must not be empty.";
            return false;
        }

        if (!TryResolveWorkspacePath(workspaceRoot, relativePath, out var fullPath))
        {
            error = "Access denied.";
            return false;
        }

        if (!TryGetGitRepositoryRoot(workspaceRoot, out repositoryRoot, out error))
            return false;

        var fullRepositoryPath = Path.GetFullPath(repositoryRoot);
        gitRelativePath = Path.GetRelativePath(fullRepositoryPath, fullPath);

        if (gitRelativePath.StartsWith("..", StringComparison.OrdinalIgnoreCase) || Path.IsPathRooted(gitRelativePath))
        {
            error = "Path is outside the git repository.";
            return false;
        }

        return true;
    }

    private static string RunGitCommand(string workingDirectory, IReadOnlyList<string> arguments, string emptyOutputMessage)
    {
        if (!TryRunGitCommand(workingDirectory, arguments, out var output, out var error))
            return error;

        var result = string.IsNullOrWhiteSpace(output) ? emptyOutputMessage : output.TrimEnd();
        return TruncateOutput(result, maxLines: 300, maxCharacters: 16000);
    }

    private static bool TryRunGitCommand(string workingDirectory, IReadOnlyList<string> arguments, out string output, out string error)
    {
        using var process = new Process();

        process.StartInfo = new ProcessStartInfo
        {
            FileName = "git",
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        foreach (var argument in arguments)
        {
            process.StartInfo.ArgumentList.Add(argument);
        }

        try
        {
            process.Start();
        }
        catch (Exception ex)
        {
            output = string.Empty;
            error = $"Unable to run git: {ex.Message}";
            return false;
        }

        output = process.StandardOutput.ReadToEnd();
        var stderr = process.StandardError.ReadToEnd();
        process.WaitForExit();

        if (process.ExitCode == 0)
        {
            error = string.Empty;
            return true;
        }

        var message = string.IsNullOrWhiteSpace(stderr) ? output : stderr;
        error = string.IsNullOrWhiteSpace(message)
            ? $"git exited with code {process.ExitCode}."
            : $"git error: {message.Trim()}";
        output = string.Empty;
        return false;
    }

    private static string TruncateOutput(string text, int maxLines, int maxCharacters)
    {
        if (text.Length <= maxCharacters)
        {
            var lineCount = text.Count(ch => ch == '\n') + 1;

            if (lineCount <= maxLines)
                return text;
        }

        var lines = SplitLines(text);
        var truncatedLines = lines.Take(maxLines).ToList();
        var truncatedText = string.Join(Environment.NewLine, truncatedLines);

        if (truncatedText.Length > maxCharacters)
            truncatedText = truncatedText[..maxCharacters];

        return truncatedText + Environment.NewLine + "Output truncated.";
    }

    private static List<string> GetCommitMatchKinds(string block, string query)
    {
        var matchKinds = new List<string>();
        var queryComparison = StringComparison.OrdinalIgnoreCase;
        var lines = SplitLines(block);

        if (lines.Length == 0)
            return matchKinds;

        var messageLines = new List<string>();
        var pathLines = new List<string>();
        var patchLines = new List<string>();
        var inPatch = false;

        foreach (var line in lines.Skip(1))
        {
            if (!inPatch && line.StartsWith("diff --git ", StringComparison.Ordinal))
            {
                inPatch = true;
            }

            if (inPatch)
            {
                patchLines.Add(line);
                continue;
            }

            if (line.StartsWith("commit ", StringComparison.Ordinal))
                continue;

            if (string.IsNullOrWhiteSpace(line))
            {
                messageLines.Add(line);
                continue;
            }

            if (!line.Contains(' ') && (line.Contains('/') || line.Contains('\\') || line.Contains('.')))
            {
                pathLines.Add(line);
                continue;
            }

            messageLines.Add(line);
        }

        if (messageLines.Any(line => line.Contains(query, queryComparison)) || lines[0].Contains(query, queryComparison))
            matchKinds.Add("message");

        if (pathLines.Any(line => line.Contains(query, queryComparison)))
            matchKinds.Add("path");

        if (patchLines.Any(line => line.Contains(query, queryComparison)))
            matchKinds.Add("content");

        return matchKinds;
    }

    private static string CreateSearchSnippet(string block, string query)
    {
        var lines = SplitLines(block.Trim());

        foreach (var line in lines.Skip(1))
        {
            if (line.Contains(query, StringComparison.OrdinalIgnoreCase))
                return line.Trim();
        }

        return lines.Length > 0 ? lines[0].Trim() : "Match found.";
    }

    private static string[] SplitLines(string text)
        => text.Split(["\r\n", "\n"], StringSplitOptions.None);

    private static bool TryGetGitRepositoryRoot(string workspaceRoot, out string repositoryRoot, out string error)
    {
        using var process = new Process();

        process.StartInfo = new ProcessStartInfo
        {
            FileName = "git",
            WorkingDirectory = workspaceRoot,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        process.StartInfo.ArgumentList.Add("rev-parse");
        process.StartInfo.ArgumentList.Add("--show-toplevel");

        try
        {
            process.Start();
        }
        catch (Exception ex)
        {
            repositoryRoot = string.Empty;
            error = $"Unable to run git: {ex.Message}";
            return false;
        }

        var output = process.StandardOutput.ReadToEnd();
        var stderr = process.StandardError.ReadToEnd();
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            repositoryRoot = string.Empty;
            error = string.IsNullOrWhiteSpace(stderr)
                ? "Not a git repository."
                : $"git error: {stderr.Trim()}";
            return false;
        }

        repositoryRoot = output.Trim();
        error = string.Empty;
        return !string.IsNullOrWhiteSpace(repositoryRoot);
    }

    private static bool TryResolveWorkspacePath(string workspaceRoot, string relativePath, out string fullPath)
    {
        var fullWorkspacePath = Path.GetFullPath(workspaceRoot);
        fullPath = Path.GetFullPath(Path.Combine(fullWorkspacePath, relativePath));
        var relativeResolvedPath = Path.GetRelativePath(fullWorkspacePath, fullPath);

        return !relativeResolvedPath.StartsWith("..", StringComparison.OrdinalIgnoreCase) &&
               !Path.IsPathRooted(relativeResolvedPath);
    }
}