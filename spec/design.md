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

Tests are authored **declaratively**: suites and tests are described in **YAML**
files, and request **data payloads** live in **JSON** files. The toolkit reads
those definitions, executes the requests, evaluates assertions, and reports
pass/fail results.

The guiding principle is a clear separation between a reusable **core engine**
and the **interfaces** that drive it.

## 2. Goals

- Provide a solid, reusable **core library** for defining and executing API
  tests.
- Ship a **CLI** as the first consumer of that core.
- Keep the core **UI-agnostic** so a GUI/web/desktop front-end can be added
  later with no changes to the engine.
- Make tests easy to author, run, and reason about — **declarative, readable,
  diff-friendly**.
- Be usable in **CI**: a run either passes or fails, with a meaningful process
  exit code and a machine-readable report.

## 3. Non-goals (for now)

- Building any graphical UI in the first phase — CLI only.
- Load / performance / stress testing (Battery is about correctness, not
  throughput).
- Payload/data generation — deferred to Phase 2 (see §4).
- Locking into protocols beyond HTTP before the core abstractions are proven
  (GraphQL, gRPC, etc. come later).

## 4. Development phases

| Phase | Focus | Deliverable |
| ----- | ----- | ----------- |
| **1** | Core functionality | A reusable core engine + a CLI that runs declarative YAML test suites with assertions, variables/environments, auth, chaining, and CI-friendly reporting |
| **2** | Enhancements | Payload/data generation, schema validation, OAuth2 flows, parallel execution, import from OpenAPI/Postman collections, additional transports |
| **3+** | Front-ends | One or more UIs (GUI / web / desktop) on top of the same core |

_Phase 1 is the current focus: get the core engine and CLI right before any UI
work begins._

## 5. Phase 1 functional requirements

The feature set that makes Battery a genuinely usable API testing tool.
Priority uses **Must** (required for Phase 1 to be done) and **Should**
(important; may slip to late Phase 1 / early Phase 2 if needed).

### 5.1 Request execution

| ID | Requirement | Priority |
| -- | ----------- | -------- |
| FR-1.1 | Issue HTTP requests with any method (GET, POST, PUT, PATCH, DELETE, HEAD, OPTIONS) | Must |
| FR-1.2 | Configure URL, path, query parameters, and headers per request | Must |
| FR-1.3 | Send JSON request bodies, sourced inline or from a referenced JSON data file | Must |
| FR-1.4 | Support form-urlencoded, raw/text, and multipart/file-upload bodies | Should |
| FR-1.5 | Configure per-request timeout and redirect-following behaviour | Must |
| FR-1.6 | TLS options for testing (e.g. allow self-signed certificates) | Should |

### 5.2 Declarative test definitions

| ID | Requirement | Priority |
| -- | ----------- | -------- |
| FR-2.1 | Define tests and suites in **YAML** files | Must |
| FR-2.2 | A test is a request + its assertions (+ optional extractions) | Must |
| FR-2.3 | Reference external **JSON** files for request bodies / data payloads | Must |
| FR-2.4 | Definitions are human-readable, comment-friendly, and diff-friendly | Must |
| FR-2.5 | Validate definition files and report clear, located errors on malformed input | Must |

### 5.3 Suites & organization

| ID | Requirement | Priority |
| -- | ----------- | -------- |
| FR-3.1 | Group tests into a suite (an ordered collection of tests) | Must |
| FR-3.2 | Run a single test, a single suite, or all discovered suites | Must |
| FR-3.3 | Name and/or tag tests and suites for selective execution (filtering) | Should |
| FR-3.4 | Suite-level shared configuration (base URL, common headers, defaults) | Should |
| FR-3.5 | Suite-level setup / teardown steps (e.g. acquire a token, clean up) | Should |

### 5.4 Assertions

| ID | Requirement | Priority |
| -- | ----------- | -------- |
| FR-4.1 | Assert on HTTP status code | Must |
| FR-4.2 | Assert on response headers (presence and value) | Must |
| FR-4.3 | Assert on response body values (matching mechanism provisional — JSONPath is the leading candidate) | Must |
| FR-4.4 | Assert on response time against a threshold | Should |
| FR-4.5 | Allow multiple assertions per test; report each assertion's pass/fail individually | Must |
| FR-4.6 | Validate a response body against a JSON Schema | Phase 2 |

### 5.5 Variables, environments & parameterization

| ID | Requirement | Priority |
| -- | ----------- | -------- |
| FR-5.1 | Define variables and interpolate them anywhere in a request or assertion | Must |
| FR-5.2 | Switchable **environment** profiles (e.g. dev / staging / prod) selected at run time | Must |
| FR-5.3 | Source values from files **and** from OS environment variables, keeping secrets out of committed files | Must |
| FR-5.4 | Defined variable precedence / scoping (e.g. suite < environment < runtime override) | Should |
| FR-5.5 | Data-driven tests: run one test over multiple rows of input data | Should |

### 5.6 Request chaining / data flow

| ID | Requirement | Priority |
| -- | ----------- | -------- |
| FR-6.1 | Extract values from a response (e.g. a token, an id) into variables | Must |
| FR-6.2 | Use extracted variables in subsequent requests within a suite | Must |

### 5.7 Authentication

| ID | Requirement | Priority |
| -- | ----------- | -------- |
| FR-7.1 | Bearer / JWT token auth — static, or obtained via a chained login request (FR-6) | Must |
| FR-7.2 | HTTP Basic auth | Should |
| FR-7.3 | API key auth (in header or query parameter) | Should |
| FR-7.4 | OAuth2 client-credentials flow as a first-class feature | Phase 2 |

### 5.8 Execution & error handling

| ID | Requirement | Priority |
| -- | ----------- | -------- |
| FR-8.1 | Execute tests within a suite sequentially, in a defined order | Must |
| FR-8.2 | Clearly distinguish an assertion failure from a transport/network error | Must |
| FR-8.3 | Stop-on-first-failure vs continue-through options | Should |
| FR-8.4 | Configurable retry of a request on transient/network failure | Should |
| FR-8.5 | Parallel suite/test execution | Phase 2 |

### 5.9 Response inspection & results

| ID | Requirement | Priority |
| -- | ----------- | -------- |
| FR-9.1 | Human-readable console output: per-test pass/fail and a run summary | Must |
| FR-9.2 | Return a non-zero process exit code when any test fails (CI gate) | Must |
| FR-9.3 | Present responses in both friendly (formatted) and raw form | Must |
| FR-9.4 | Emit a machine-readable report (e.g. JUnit XML and/or JSON) to a file | Should |
| FR-9.5 | Verbosity levels (quiet / normal / verbose) — verbose shows full request & response | Should |

### 5.10 CLI

| ID | Requirement | Priority |
| -- | ----------- | -------- |
| FR-10.1 | A run command that takes a path to a file or directory of YAML definitions | Must |
| FR-10.2 | Options/flags to select environment, filter tests, set verbosity, and choose report format/output path | Must |
| FR-10.3 | Clear `--help` / usage output | Must |

## 6. Non-functional requirements

- **Language / runtime:** C# on .NET 10 (provisional).
- **Architecture:** Core engine (`Battery.Core`) fully decoupled from all
  front-ends; the CLI is a thin consumer.
- **Testability:** The core must be exercisable by automated tests without a UI.
- **Determinism / isolation:** A test's result depends only on its request and
  the inputs it declares; tests should not implicitly depend on each other
  except via explicit chaining (FR-6).
- **Security / privacy:** Secrets (tokens, passwords, keys) must never need to
  be stored in committed definition files; they are supplied via environment
  variables or uncommitted local files.
- **Platforms:** Cross-platform (Windows / macOS / Linux).
- **Performance:** Correctness-focused; no specific throughput targets in
  Phase 1.

## 7. Architecture

High-level shape (to be refined):

- **`Battery.Core`** — the testing engine: definition parsing, request
  execution, assertion evaluation, variable resolution, chaining, results. No
  UI or CLI dependencies.
- **`Battery.Cli`** — the first front-end; a thin layer that drives
  `Battery.Core`.
- **`Battery.Tests`** — automated tests for the core and CLI.
- _Future_ — additional UI front-ends consuming the same `Battery.Core`.

**TBD** — concrete project/solution layout, key abstractions, extension points.

## 8. Data model

_Key entities — e.g. Suite, Test, Request, Assertion, Extraction, Environment,
Variable, RunResult. To be defined._

**TBD**

## 9. Interfaces

- **CLI** — primary interface for Phase 1. Command surface TBD (see §5.10).
- **YAML definition schema** — the authoring contract for suites/tests. TBD.
- _Future_ — GUI / web / desktop front-ends.

## 10. Open questions

- **Assertion/extraction mechanism** — confirm JSONPath (vs a fuller expression
  language) for body matching. _Pending evaluation._
- **Report format(s)** — JUnit XML, custom JSON, or both?
- **Variable precedence** — confirm the exact scoping/override order.
- **Multipart/file-upload scope** — how much to support in Phase 1 (FR-1.4).
- **Definition discovery** — how suites are found on disk (convention vs
  explicit manifest).

## 11. Decision log

| Date       | Decision | Rationale |
| ---------- | -------- | --------- |
| 2026-11-07 | Repository initialized with `src/` and `spec/` layout | Separate source code from design/requirements documentation |
| 2026-11-07 | Language is C# / .NET | Chosen stack for the toolkit |
| 2026-11-07 | CLI-first, core-first | Prove the engine before attaching any UI; keep core UI-agnostic |
| 2026-11-07 | Target .NET 10 (provisional) | Latest .NET; revisit if a broader support target is needed |
| 2026-11-07 | Added `Battery.sln` + `src/Battery.Cli` (Hello World) | First runnable scaffold of the CLI front-end |
| 2026-11-07 | Tests declarative; YAML for suite/test definitions, JSON for data payloads | Readable, diff-friendly authoring; JSON stays for structured data |
| 2026-11-07 | Payload/data generation deferred to Phase 2 | Keep Phase 1 focused on a usable core |
| 2026-11-07 | Defined Phase 1 functional requirement set (§5) | Scope the minimal usable API testing tool |
