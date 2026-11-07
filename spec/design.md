# Battery — Design & Requirements

> **Status:** Draft · Scoping
> **Last updated:** 2026-11-07

Battery is an **API testing toolkit** written in **C# / .NET**. This document
scopes out what it is and what it needs to do. It is a living document —
sections marked `TBD` are open and will be filled in as we iterate.

> **Note on the name:** "battery" refers to a _battery of tests_, not to
> electrical batteries.

---

## 1. Overview

Battery is a toolkit for authoring, running, and inspecting tests against HTTP
(and potentially other) APIs. It begins life as a **command-line application**,
but is designed so that the testing engine is independent of any front-end —
one or more UIs may be layered on top later.

The guiding principle is a clear separation between a reusable **core engine**
and the **interfaces** that drive it.

## 2. Goals

- Provide a solid, reusable **core library** for defining and executing API
  tests.
- Ship a **CLI** as the first consumer of that core.
- Keep the core **UI-agnostic** so a GUI/web/desktop front-end can be added
  later with no changes to the engine.
- Make tests easy to author, run, and reason about.
- TBD (more to come as we iterate)

## 3. Non-goals (for now)

- Building any graphical UI in the first phase — CLI only.
- Locking into a single transport or protocol before the core abstractions are
  proven.
- TBD

## 4. Development phases

| Phase | Focus | Deliverable |
| ----- | ----- | ----------- |
| **1** | Core functionality | A reusable core library + a CLI that drives it |
| **2+** | Front-ends | One or more UIs (GUI / web / desktop) on top of the same core |

_Phase 1 is the current focus: get the core engine and CLI right before any UI
work begins._

## 5. Functional requirements

_What the toolkit must do, from the user's point of view. To be fleshed out as
we iterate._

| ID   | Requirement | Priority | Notes |
| ---- | ----------- | -------- | ----- |
| FR-1 | TBD         | TBD      |       |

## 6. Non-functional requirements

- **Language / runtime:** C# on .NET 10 (provisional).
- **Architecture:** Core engine decoupled from all front-ends.
- **Testability:** The core must be exercisable by automated tests without a UI.
- **Performance:** TBD
- **Security / privacy:** TBD
- **Platforms:** Cross-platform (Windows / macOS / Linux) — TBD to confirm.

## 7. Architecture

High-level shape (to be refined):

- **`Battery.Core`** — the testing engine: test definitions, execution, results.
  No UI or CLI dependencies.
- **`Battery.Cli`** — the first front-end; a thin layer that drives
  `Battery.Core`.
- **`Battery.Tests`** — automated tests for the core and CLI.
- _Future_ — additional UI front-ends consuming the same `Battery.Core`.

**TBD** — concrete project/solution layout, key abstractions, extension points.

## 8. Data model

_Key entities — e.g. test definitions, requests, assertions, results. To be
defined._

**TBD**

## 9. Interfaces

- **CLI** — primary interface for phase 1. Command surface TBD.
- _Future_ — GUI / web / desktop front-ends.

## 10. Open questions

- Which .NET version to target?
- Test authoring format — code-based, declarative (YAML/JSON), or both?
- Scope of protocols — HTTP/REST first; GraphQL, gRPC, etc. later?
- TBD

## 11. Decision log

| Date       | Decision | Rationale |
| ---------- | -------- | --------- |
| 2026-11-07 | Repository initialized with `src/` and `spec/` layout | Separate source code from design/requirements documentation |
| 2026-11-07 | Language is C# / .NET | Chosen stack for the toolkit |
| 2026-11-07 | CLI-first, core-first | Prove the engine before attaching any UI; keep core UI-agnostic |
| 2026-11-07 | Target .NET 10 (provisional) | Latest .NET; revisit if a broader support target is needed |
| 2026-11-07 | Added `Battery.sln` + `src/Battery.Cli` (Hello World) | First runnable scaffold of the CLI front-end |
