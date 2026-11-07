---
description: Resume work by reading handoff.md, replaying the context, and proposing next steps
allowed-tools: Read, Glob, Bash(git log:*), Bash(git status:*), Bash(ls:*)
---

## Context

- Repo status: !`git status --short`
- Recent commits: !`git log --oneline -10`

## Task

Pick up where the last session left off by reading the handoff and getting
re-grounded in the work.

### Steps

1. **Read `handoff.md`** at the repository root.
   - If it does not exist, say so plainly, do not invent context, and offer to
     orient from the repo instead (e.g. read `spec/design.md`, scan the tree).
     Then stop.
   - If it exists and contains multiple dated entries, the **newest entry is at
     the top** — use that as the primary handoff.

2. **Re-ground** by reading the files listed under "Relevant files" in the
   handoff (at least skim the most important ones) so your understanding
   reflects the current state of the code and docs, not just the handoff's
   summary. Reconcile anything in the handoff that the recent git history or
   current file contents contradict, and flag such drift.

3. **Play the context back to the user** as a concise briefing:
   - Where things stand (the stopping point).
   - The key decisions carried forward.
   - The files that matter and why.

4. **Propose next steps**: start from the handoff's "Next best step", refine it
   against what you now see in the repo, and present a short, prioritized set of
   options. End by asking the user how they want to proceed.

### Rules

- Treat the handoff as a starting point, not gospel — the repo is the source of
  truth. If they disagree, trust the repo and say so.
- Keep the briefing skimmable; don't dump the whole file back verbatim.
- Make no changes to the repo in this command — it is read-and-orient only.
