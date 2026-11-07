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
| FR-4.3 | Assert on response body values via **JSONPath** | Must |
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
- **`Battery.Tests`** — automated tests for the core and CLI (to live under a
  top-level `tests/` directory).
- _Future_ — `Battery.Api` (an HTTP projection of Core) and one or more UIs; the
  UI consumes the API, the API consumes Core. Included in the design purely to
  validate that Core supports multiple independent consumers — the API is **not**
  a Phase 1 deliverable.

**TBD** — concrete project/solution layout, key abstractions, extension points.

## 8. Conventions & principles

How we build Battery. These are working rules, applied pragmatically.

### 8.1 Working agreements

- **Pragmatism over purity.** Follow established C# best practice and use design
  patterns where they earn their place; prefer the simplest thing that meets the
  requirement. Add structure on real need (a second implementation, a genuine
  seam, a testability problem), not speculatively (**YAGNI**).
- **Ambiguity triggers dialogue.** Where it's unclear whether a pattern or
  abstraction applies, pause and confirm before committing to it.
- **The spec is the source of truth.** Requirements and notable decisions live
  here; the decision log (§12) is kept current.

### 8.2 Architecture & layering

- **Core is the single home of functionality.** `Battery.Core` holds all domain
  logic — definition parsing, execution, assertions, variables, chaining,
  results.
- **Front-ends are thin adapters.** Each consumer (CLI now; API and UI later)
  dips into Core and owns only its own concerns — the CLI owns argument parsing
  and console rendering; the API owns endpoints, routing, and wire DTOs. No
  domain logic lives in a front-end.
- **Layering:** UI → API → Core. The UI talks to the API; the API talks to Core.
- **Dependencies point inward.** Core depends on nothing app-specific and knows
  nothing of Console, ASP.NET, or any UI. Front-ends depend on Core; never the
  reverse.
- **Core is library-first.** Its public surface is a deliberate, documented API;
  internals stay `internal`.
- **Front-ends translate.** A front-end maps to/from its own models rather than
  exposing Core's internal types on its interface (e.g. the API doesn't
  serialize Core internals onto the wire).

### 8.3 Design & dependency injection

- **Interfaces + DI for all behaviour; concrete records for data.** Services and
  anything that *does* something or could vary/be faked are defined as
  interfaces and composed via `Microsoft.Extensions.DependencyInjection`.
  Value/data types (e.g. `TestDefinition`, `AssertionResult`, `RunResult`) stay
  concrete records.
- **When to introduce a seam:** either (a) multiple implementations are known
  today, or (b) substitution is needed for tests. Otherwise stay concrete and
  revisit via the ambiguity → dialogue rule.
- **Known Phase 1 seams (provisional names):** `IAssertion`,
  `IAuthenticationScheme`, `IReportWriter`, `IHttpExecutor` (testability),
  `IClock` (deterministic timing), plus definition loading and variable
  resolution. These follow the Strategy pattern where several kinds exist.
- **Resolution** of a strategy from its YAML discriminator (e.g.
  `type: status`) uses a simple internal registry/factory — **not** external
  plugin/assembly loading, which is out of scope for Phase 1.
- **Favour composition over inheritance; prefer immutability** for domain data.

### 8.4 Coding standards

- Follow **Microsoft's C# conventions** and the **.NET library design
  guidelines**.
- **Nullable reference types on**; **warnings-as-errors** (enforced in CI).
- **Async all the way** for I/O, with `CancellationToken` threaded end-to-end.
- Use **modern C# where it clarifies** — records, pattern matching,
  expression-bodied members — not reflexively.
- **Clarity over cleverness**; intention-revealing names; one primary type per
  file.
- **XML doc comments on Core's public API** (it is a real contract).
- Conventions enforced via **`.editorconfig` + Roslyn analyzers + `dotnet
  format`** in CI.

### 8.5 Error handling

- **Expected failures are outcome objects, not exceptions.** A failing assertion
  or a non-2xx response under test flows back as a result (ties to FR-8.2).
- **Exceptions are for genuine faults** — I/O failures, bugs, misconfiguration.
- **Fail fast on bad definitions** with clear, located, actionable messages.

### 8.6 Testing

- **xUnit**, with tests under a top-level **`tests/`** directory.
- **Core is fully unit-testable headless** (via `IHttpExecutor` etc.).
- **Integration tests** drive the CLI/API against an in-process mock HTTP
  server. _The exact integration-test approach is still to be refined (its own
  topic)._
- Tests are **deterministic and isolated**.

### 8.7 Dependencies

- **Lean on the BCL; justify every third-party package.**
- Expected minimal set (provisional): `System.Text.Json` (data), a YAML library
  (YamlDotNet likely), **`JsonPath.Net`** (json-everything suite — RFC 9535,
  built on `System.Text.Json`) for body assertions/extraction, the DI/Hosting
  extensions, and xUnit (test-only).
- **Core stays lightweight** — no heavyweight frameworks pulled into it.

## 9. Data model

_Key entities — e.g. Suite, Test, Request, Assertion, Extraction, Environment,
Variable, RunResult. To be defined._

**TBD**

## 10. Interfaces

- **CLI** — primary interface for Phase 1. Command surface TBD (see §5.10).
- **YAML definition schema** — the authoring contract for suites/tests. TBD.
- _Future_ — GUI / web / desktop front-ends.

## 11. Open questions

- **Report format(s)** — JUnit XML, custom JSON, or both?
- **Variable precedence** — confirm the exact scoping/override order.
- **Multipart/file-upload scope** — how much to support in Phase 1 (FR-1.4).
- **Definition discovery** — how suites are found on disk (convention vs
  explicit manifest).

## 12. Decision log

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
| 2026-11-07 | Core-central layering: UI → API → Core; front-ends are thin adapters | Keep one home for functionality; support multiple consumers |
| 2026-11-07 | `Battery.Api` is illustrative, deferred (not Phase 1) | Shows multi-consumer design without expanding Phase 1 scope |
| 2026-11-07 | Interfaces + DI for behaviour; concrete records for data | Testability and pluggability where it matters, no ceremony for plain data |
| 2026-11-07 | Seam only on known-plural or testability need (Strategy via internal registry) | Pragmatic abstraction; no speculative plugin system |
| 2026-11-07 | Warnings-as-errors; nullable on; analyzers + `.editorconfig` in CI | Enforce quality automatically |
| 2026-11-07 | Expected failures as outcome objects; exceptions for faults | Clean separation of test results from errors (FR-8.2) |
| 2026-11-07 | xUnit; tests under top-level `tests/` | Standard .NET testing; clear separation from `src/` |
| 2026-11-07 | JSONPath confirmed for body assertions/extraction; `JsonPath.Net` (json-everything) as the library | RFC 9535-compliant, native `System.Text.Json`, fits the lean-dependency rule (vs Newtonsoft `SelectToken`) |
