# AgentTools.cs - Detailed Tool Definitions Report

This report describes each tool defined in the AgentTools.cs source code, including code snippets and analysis.

---

## 1. GetWorkspaceRoot
**Description:** Returns the absolute path of the current tool access root.

```csharp
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
```
**Analysis:** This tool requires no parameters and simply returns the full path to the root of the workspace accessible by the agent.

---

## 2. ReadFile
**Description:** Reads a text file from the approved workspace using a relative path.

```csharp
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
```
**Analysis:** Takes a relative path string and returns the content of the file at that path if it exists and is within the workspace.

---

## 3. SearchText
**Description:** Searches for matching text in a file or directory inside the approved workspace.

```csharp
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
```
**Analysis:** This tool allows searching for a given text pattern within files or directories, recursively if necessary, returning matching lines with context. It limits results to 200 matches to avoid overload.

---

## 4. GetFileInfo
**Description:** Returns metadata for a file or directory inside the approved workspace.

```csharp
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
```
**Analysis:** Provides detailed info such as file size, timestamps, attribute flags for files, or directory child count for directories. Helps understand file system state.

---

## 5. GetGitStatus
**Description:** Returns the git working tree status for the repository containing the workspace root.

```csharp
public static ChatTool GetGitStatusDefinition { get; } = ChatTool.CreateFunctionTool(
    functionName: "GetGitStatus",
    functionDescription: "Returns the git working tree status for the repository containing the workspace root.",
    functionParameters: BinaryData.FromString("""
    {"type": "object", "properties": {}}
    """)
);
```
**Analysis:** Runs `git status --short --branch` to indicate modified, staged, or untracked files plus branch info.

---

## 6. GetGitDiff
**Description:** Returns the current git diff against HEAD for the repository root.

```csharp
public static ChatTool GetGitDiffDefinition { get; } = ChatTool.CreateFunctionTool(
    functionName: "GetGitDiff",
    functionDescription: "Returns the current git diff against HEAD for the repository containing the workspace root.",
    functionParameters: BinaryData.FromString("""
    { "type": "object", "properties": {} }
    """)
);
```
**Analysis:** Runs `git diff HEAD` to get all unstaged changes compared to the last commit.

---

## 7. GetRecentCommits
**Description:** Returns recent commits for the repository containing the workspace root.

```csharp
public static ChatTool GetRecentCommitsDefinition { get; } = ChatTool.CreateFunctionTool(
    functionName: "GetRecentCommits",
    functionDescription: "Returns recent commits for the repository containing the workspace root.",
    functionParameters: BinaryData.FromString("""
    { "type": "object", "properties": {} }
    """)
);
```
**Analysis:** Retrieves the last 10 commits with basic metadata (hash, date, author, subject).

---

## 8. GetCurrentBranch
**Description:** Returns the current git branch for the repository containing the workspace root.

```csharp
public static ChatTool GetCurrentBranchDefinition { get; } = ChatTool.CreateFunctionTool(
    functionName: "GetCurrentBranch",
    functionDescription: "Returns the current git branch for the repository containing the workspace root.",
    functionParameters: BinaryData.FromString("""
    { "type": "object", "properties": {} }
    """)
);
```
**Analysis:** Runs `git branch --show-current` to get the active branch name.

---

## 9. GetFileDiff
**Description:** Returns the current git diff against HEAD for a specific file inside the approved workspace.

```csharp
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
```
**Analysis:** Similar to GetGitDiff but scoped to a single file.

---

## 10. SearchCommitHistory
**Description:** Searches git commit history (messages, paths, patch content) for a query.

```csharp
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
```
**Analysis:** Executes `git log` with detailed formatting to find commits related to the query in any commit text or content diffs, returning up to 20 matches.

---

## 11. GetCommitDetails
**Description:** Returns detailed information about a specific commit hash.

```csharp
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
```
**Analysis:** Runs `git show` to get commit metadata, stats, and patch for the specified commit.

---

## 12. StageFile
**Description:** Stages a file or directory path inside the workspace for commit.

```csharp
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
```
**Analysis:** Runs `git add` for the path to stage changes.

---

## 13. UnstageFile
**Description:** Removes a file or directory path inside the workspace from the git index.

```csharp
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
```
**Analysis:** Runs `git reset HEAD` for the path to unstage changes.

---

## 14. ExportMarkdownReport
**Description:** Creates or overwrites a markdown report file inside the workspace.

```csharp
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
```
**Analysis:** Writes the given markdown content to the specified relative path inside the workspace, creating folders as needed.

---

## 15. ListFiles
**Description:** Lists files and directories inside the workspace using a relative path.

```csharp
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
```
**Analysis:** Returns up to 100 files or directories in the specified path with type information.

---

**Summary:**
The AgentTools class encapsulates a comprehensive set of tools to interact with the file system and git repository within an authorized workspace. It ensures security by verifying paths are inside the workspace and git repository roots. \n
It enables file reading, file searching, directory listing, file metadata retrieval, git status and diffs, commit history searching, staging/unstaging, and report exporting.

This modular design supports automation and intelligent agents needing to programmatically inspect and modify the workspace and repository state.

(Full source code is available in AgentTools.cs)