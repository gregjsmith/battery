---
description: Capture the current session's context, decisions, and next steps into handoff.md
argument-hint: "[--overwrite (default) | --append]"
allowed-tools: Read, Write, Edit, Bash(git log:*), Bash(git status:*), Bash(git diff:*), Bash(ls:*)
---

## Context

- Repo status: !`git status --short`
- Recent commits: !`git log --oneline -15`

## Task

Write a session handoff to **`handoff.md`** at the repository root, capturing
everything a future session (or a future you, starting cold) needs to resume
this work smoothly.

### Mode (from the argument "$ARGUMENTS")

- If the argument contains `--append` → **APPEND** mode.
- Otherwise (empty, `--overwrite`, or anything else) → **OVERWRITE** mode (the
  default).

- **OVERWRITE**: replace the entire contents of `handoff.md` with a single,
  fresh snapshot using the template below.
- **APPEND**: read the existing `handoff.md` and insert a new dated session
  entry (the template below, minus the top-level `# …` title) directly beneath
  the title, above any earlier entries, separated by a `---` rule, so the newest
  handoff is always first and earlier handoffs are preserved. If `handoff.md`
  does not exist yet, behave exactly like OVERWRITE.

### What to capture

Draw on the actual conversation and the git context above — be concrete and
specific, not generic. Use this structure:

```
# Battery — Session Handoff

> Last saved: <YYYY-MM-DD> · Session: <session link if known, else "n/a">

## Stopping point
<One short paragraph: the state the work is in right now and what was just
finished.>

## Work completed this session
- <Concrete things done, with commit hashes where relevant.>

## Key decisions
- <Decisions made this session and the reasoning, one line each.>

## Relevant files
- `<path>` — <why it matters / what's in it>

## Next best step
<The single most sensible next action, stated clearly. Then any alternatives or
follow-ups as a short list.>

## Open questions / notes
- <Anything unresolved, parked, or worth warning the next session about.>
```

### Rules

- Fill the date from today. Include the session link if you know it; otherwise
  write `n/a`.
- Keep it tight and skimmable — this is a handoff, not a transcript.
- Only describe what actually happened in this session; do not invent progress.
- After writing, briefly confirm to the user which mode was used and the path.
- Do **not** commit unless the user's standing preference is to commit on every
  change (or they ask); follow the session's git workflow.
