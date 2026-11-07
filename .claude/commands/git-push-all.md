---
description: Stage every change (new, modified, deleted), commit, and push to the current branch in one step
argument-hint: [optional commit message]
allowed-tools: Bash(git add:*), Bash(git status:*), Bash(git commit:*), Bash(git push:*), Bash(git branch:*), Bash(git rev-parse:*), Bash(git diff:*), Bash(git log:*)
---

## Context

- Current branch: !`git rev-parse --abbrev-ref HEAD`
- Working tree status: !`git status --short`
- Change summary: !`git diff HEAD --stat`

## Task

Stage **all** changes in the working tree — new, modified, and deleted files — then commit and push to the current branch on `origin`, in one go.

Commit message (may be empty): "$ARGUMENTS"

Steps:

1. Run `git add -A` to stage everything, including untracked (new) files and deletions.
2. If the tree is clean (nothing staged after step 1), stop and report that there is nothing to commit. Do **not** create an empty commit.
3. Commit the staged changes:
   - If a commit message was provided above, use it verbatim as the subject line.
   - If it is empty, write a concise one-line message summarizing what changed.
   - End the commit message with this session's standard attribution trailers.
4. Push with `git push -u origin <current-branch>`. This sets the upstream if the branch doesn't have one yet. On a transient network failure, retry a few times with short backoff.
5. Report back: the branch, the commit hash and subject, and confirmation that the push succeeded (or the exact error if it didn't).

Guardrails:

- Do **not** open a pull request.
- Do **not** switch, create, or delete branches — commit and push on whatever branch is currently checked out.
- Only operate on this repository's working tree.
