# agent01

`agent01` is a small .NET console AI agent that talks to the OpenAI API and can use a limited set of local tools against the folder where it is started.

## What it does

- Runs as an interactive console chat application.
- Uses the OpenAI .NET SDK to send chat completions.
- Supports local tools for:
  - `GetWorkspaceRoot`: returns the current tool access root.
  - `ListFiles`: lists files and directories inside the allowed folder.
  - `ReadFile`: reads text files inside the allowed folder.
- Writes a transcript log under `logs/` for each session.

## How tool access works

By default, the app uses the current working directory as its tool access root.

That means the folder you launch the app from becomes the folder that `ListFiles` and `ReadFile` can access.

If you run the app from the project folder with `dotnet run`, the tool access root will be the project folder.

If you later publish the app and make `agent01` available on your `PATH`, you can start it from some other folder and it will use that folder instead.

Example after publishing:

```powershell
Set-Location C:\work\project\agent01
```

In that example, `ListFiles` and `ReadFile` are limited to `C:\work\project-a`.

You can override the tool access root with the `AGENT_WORKSPACE_ROOT` environment variable.

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
- `ReadFile` and `ListFiles` reject paths outside that root.
- Transcript logs are stored in `logs/`, which is also ignored by git.