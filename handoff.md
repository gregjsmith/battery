# Battery — Session Handoff

> Last saved: 2026-11-07 · Session: https://claude.ai/code/session_01MHnRgUvYi9EWy1Uxt2cr4g

## Stopping point
Battery (an **API testing toolkit** in C#/.NET — "battery of tests", not
electrical) is still in the **design/scoping** phase. The repo has a runnable
Hello World CLI and a design doc that now defines the Core data model. No engine
code exists yet. This session consolidated all work back to **`main`** and
removed the feature branches. The working history on `main` is clean and linear.
The working tree is clean; all work is pushed to `main`.

## Work completed this session
- Confirmed **JSONPath** for body assertions and extraction, with the library
  **`JsonPath.Net`** (json-everything suite — RFC 9535, native `System.Text.Json`).
- Added **`CLAUDE.md`**: the project writes every sentence in **Simplified
  Technical English (STE)**, the rules of ASD-STE100. Code identifiers are exempt.
- Defined the **Core data model (§9)** in `spec/design.md`: authored definitions
  and the result model, all immutable records. Each entity ties to a Phase 1
  requirement. Recorded the variable, request, and matcher decisions.
- Consolidated the repo: fast-forwarded `main` to this session's work, then
  deleted two parallel feature branches (`claude/adoring-cannon-3q6h1c` and
  `claude/trusting-hawking-53tvc4`). A duplicate JSONPath commit and a stale
  handoff update from the other branch were dropped on purpose.

## Key decisions
- **JSONPath** confirmed; **`JsonPath.Net`** is the library.
- **STE** is the writing standard for all prose (chat, commits, docs, comments).
- **Data model** (§9), all immutable records:
  - **Variable precedence**, weakest to strongest: suite defaults, environment,
    OS environment variables, runtime overrides, extracted values. An extracted
    value is the strongest.
  - **Two request types**: the authored `Request` keeps `${var}` templates; the
    engine builds a `ResolvedRequest` after it fills every template.
  - **Body matchers** for Phase 1: `equals`, `exists`, `contains`, `matches`.
- Earlier decisions still hold: declarative YAML + JSON; UI → API → Core layering;
  interfaces + DI for behaviour, concrete records for data; warnings-as-errors;
  xUnit under `tests/`; target .NET 10 (provisional). See §12 decision log.

## Relevant files
- `spec/design.md` — the source of truth. §5 Phase 1 requirements, §7
  architecture, §8 conventions, **§9 data model (now defined)**, §10 interfaces
  and YAML schema (**TBD — next**), §11 open questions, §12 decision log.
- `CLAUDE.md` — the STE writing rule.
- `Battery.sln`, `src/Battery.Cli/Program.cs`, `src/Battery.Cli/Battery.Cli.csproj`
  — the only code; net10.0 console app, assembly name `battery`.
- `.claude/commands/git-push-all.md`, `save-session.md`, `new-session.md` — the
  project slash commands.
- `.gitignore` — .NET.

## Next best step
**Write §10 (the YAML definition schema)** — the exact file layout that maps to
the §9 entities. The Albums API example from this session shows most of the
shape and is a good base.

Alternatives / follow-ups:
- Scaffold `Battery.Core` + `tests/` (xUnit) + `.editorconfig` /
  `Directory.Build.props` (warnings-as-errors) — turns §8 and §9 into real code.
- Refine the **integration-test approach** (parked in §8.6: in-process test
  host vs. WireMock.Net).

## Open questions / notes
- **Report format(s)**: JUnit XML, custom JSON, or both? (FR-9.4)
- **Multipart/file-upload scope** for Phase 1 (FR-1.4).
- **Definition discovery** on disk: convention vs. explicit manifest.
- There are **no feature branches now**. Future work goes on `main`, or on a new
  branch the user asks for.
- `dotnet` is not installed in the cloud session — build and run happen in the
  user's Codespaces. Confirm Codespaces has the **.NET 10** SDK.
