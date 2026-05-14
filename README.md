# agent01

`agent01` is a small .NET console AI agent that talks to the OpenAI API and can use a limited set of local tools against the folder where it is started.

## What it does

- Runs as an interactive console chat application.
- Uses the OpenAI .NET SDK to send chat completions.
- Supports local tools for:
  - `GetWorkspaceRoot`: returns the current tool access root.
  - `GetFileInfo`: returns metadata for a file or directory inside the allowed folder.
  - `GetGitStatus`: returns the current git working tree status.
  - `GetGitDiff`: returns the current git diff against `HEAD`.
  - `GetRecentCommits`: returns recent commits from the current repository.
  - `GetCurrentBranch`: returns the current git branch.
  - `GetFileDiff`: returns the current git diff for a single file.
  - `GetCommitDetails`: returns detailed information for a specific commit hash.
  - `ListFiles`: lists files and directories inside the allowed folder.
  - `ReadFile`: reads text files inside the allowed folder.
  - `ExportMarkdownReport`: creates or overwrites a markdown report file inside the allowed folder.
  - `SearchCommitHistory`: searches commit messages, changed paths, and patch text.
  - `SearchText`: searches for case-insensitive text matches in a file or directory inside the allowed folder.
  - `StageFile`: stages a file or directory path for commit.
  - `UnstageFile`: removes a file or directory path from the git index.
- Writes a Markdown transcript under `logs/` for each session, with a session summary table and turn-by-turn sections that group each user prompt, internal tool activity, visible console response, timestamps, and fenced blocks together.

## How tool access works

By default, the app uses the current working directory as its tool access root.

That means the folder you launch the app from becomes the folder that `GetFileInfo`, `ListFiles`, `ReadFile`, `SearchText`, and `ExportMarkdownReport` can access.

If you run the app from the project folder with `dotnet run`, the tool access root will be the project folder.

If you later publish the app and make `agent01` available on your `PATH`, you can start it from some other folder and it will use that folder instead.

Example after publishing:

```powershell
Set-Location C:\work\project\agent01
```

In that example, `GetFileInfo`, `ListFiles`, `ReadFile`, `SearchText`, and `ExportMarkdownReport` are limited to `C:\work\project\agent01`.

You can override the tool access root with the `AGENT_WORKSPACE_ROOT` environment variable.

## Exporting reports

If you ask the agent to export a summary or report, it can write a markdown file inside the workspace root by using `ExportMarkdownReport`.

Example prompt:

```text
Summarize this project and export the report to reports/project-summary.md
```

The exported file is always written as markdown and is restricted to the configured workspace root.

## Requirements

- .NET 10 SDK
- An OpenAI API key

## Configuration

The app reads configuration in this order:

1. `appsettings.json`
2. `appsettings.local.json`
3. Environment variables

The API key is read from:

- `OpenAI:ApiKey`
- or environment variable `OPENAI__APIKEY`

`appsettings.local.json` is ignored by git so you can keep your local secret there.

### appsettings.json

Checked into the repo as a template:

```json
{
  "OpenAI": {
    "ApiKey": ""
  }
}
```

### appsettings.local.json

Create or update this locally with your real key:

```json
{
  "OpenAI": {
    "ApiKey": "your-api-key-here"
  }
}
```

## Setup

1. Install the .NET 10 SDK.
2. Restore dependencies:

```powershell
dotnet restore
```

3. Put your API key in `appsettings.local.json` or set `OPENAI__APIKEY`.
4. Build the project:

```powershell
dotnet build
```

5. Run it:

```powershell
dotnet run
```

## Optional: run from anywhere

If you want to run the app from any folder, publish it and add the output directory to your `PATH`.

Example:

```powershell
dotnet publish -c Release -o .\publish
```

Then add the `publish` folder to your `PATH`, or wrap it with a small shell script/alias.

## Project files

- `Program.cs`: startup, configuration, chat loop, and transcript wiring.
- `AgentTools.cs`: tool definitions and tool execution.
- `TranscriptLogger.cs`: per-session logging.
- `appsettings.json`: checked-in config template.
- `appsettings.local.json`: local secrets file, ignored by git.

## Notes

- Tool access is intentionally restricted to the configured workspace root.
- `GetFileInfo`, `ReadFile`, `ListFiles`, `SearchText`, and `ExportMarkdownReport` are restricted to that root.
- `ExportMarkdownReport` can only write markdown files inside that root.
- Git tools operate on the repository that contains the workspace root and return an error if the workspace is not inside a git repository.
- `StageFile` and `UnstageFile` change the repository index, so they should be used deliberately.
- Transcript Markdown files are stored in `logs/`, which is also ignored by git.