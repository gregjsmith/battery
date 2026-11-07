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
| FR-1.4 | Support form-urlencoded and raw/text bodies | Should |
| FR-1.5 | Configure per-request timeout and redirect-following behaviour | Must |
| FR-1.6 | TLS options for testing (e.g. allow self-signed certificates) | Should |
| FR-1.7 | Support multipart / file-upload bodies | Phase 2 (Should) |

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

The model has two parts. The first part is the **authored definitions**. The
engine reads these from YAML. They do not change after the engine reads them.
They can hold `${var}` templates. The second part is the **result model**. A run
produces it.

Some types have more than one kind (for example an assertion). A `type:`
discriminator in the YAML names the kind. The internal Strategy registry (§8.3)
selects the matching implementation (for example `IAssertion` or
`IAuthenticationScheme`).

All entities are immutable records.

### 9.1 Authored definitions

| Entity | Fields | Requirements |
| ------ | ------ | ------------ |
| `Suite` | name, `BaseConfig?`, ordered `IReadOnlyList<Test>`, tags, `Setup?`, `Teardown?` | FR-3.1, FR-3.4, FR-3.5 |
| `BaseConfig` | base URL, common headers, default timeout, default auth | suite-level defaults |
| `Test` | name, tags, `Request`, `IReadOnlyList<Assertion>`, `IReadOnlyList<Extraction>` | FR-2.2 |
| `Request` | method, URL, query parameters, headers, `RequestBody?`, timeout, follow-redirects flag, `TlsOptions?`, `Auth?` | §5.1 |
| `RequestBody` | one kind of: `JsonInline`, `JsonFile(path)`, `Form`, `Text`; `Multipart` is Phase 2 | FR-1.3, FR-1.4, FR-1.7 |
| `Auth` | one kind of: `Bearer`, `Basic`, `ApiKey(location: header or query)` | §5.7 |
| `Assertion` | one kind of: `Status`, `Header`, `Body(jsonPath, matcher, value)`, `ResponseTime` | §5.4 |
| `Extraction` | name, source: `Body(jsonPath)`, `Header`, or `Status` | FR-6.1 |
| `Environment` | name, map of variable names to values | FR-5.2 |

A `Setup` and a `Teardown` each hold a `Request`, optional extractions, and
optional assertions. They use the same shape as a `Test`.

### 9.2 Result model

| Entity | Fields |
| ------ | ------ |
| `AssertionResult` | the assertion, pass or fail, expected value, actual value, message |
| `TestResult` | test name, outcome, `IReadOnlyList<AssertionResult>`, duration, error (if any) |
| `SuiteResult` | suite name, `IReadOnlyList<TestResult>`, counts, duration |
| `RunResult` | `IReadOnlyList<SuiteResult>`, totals, overall pass or fail |

A `TestResult` has one of three outcomes:

- `Passed` — every assertion passed.
- `Failed` — the request completed, but at least one assertion did not pass.
- `Errored` — the request did not complete (for example a transport fault).

This split keeps assertion failures apart from faults (§8.5, FR-8.2).
`RunResult` reports pass or fail for the whole run. The CLI maps a failed run to
a non-zero exit code (FR-9.2).

### 9.3 Variables and precedence

A variable gets its value from one of five sources. When two sources set the
same name, the stronger source wins. The order, from weakest to strongest, is:

1. Suite defaults (from the suite file).
2. Environment profile (for example `staging`).
3. OS environment variables (for example `ALBUMS_PASSWORD`).
4. Runtime overrides (for example `--set baseUrl=...`).
5. Extracted values (from a response, for example `token`).

An extracted value is the strongest. It wins over a runtime override. This order
lets a chained value (such as a token from a login step) flow into later
requests without a command-line change.

### 9.4 Authored request and resolved request

The engine keeps two request types:

- `Request` — the authored template. It still holds `${var}` templates.
- `ResolvedRequest` — the final request. The engine builds it after it fills
  every template with a value from the precedence order in §9.3.

The engine sends a `ResolvedRequest`. It does not send a `Request`. This split
keeps the resolution step easy to test on its own.

### 9.5 Body matchers

A body assertion uses JSONPath to select a value. It then uses a matcher to
check that value. Phase 1 supports four matchers:

- `equals` — the value equals the expected value exactly.
- `exists` — the field is present (the value does not matter).
- `contains` — the value holds the expected text or item.
- `matches` — the value fits a regular expression.

## 10. Interfaces

Phase 1 has two contracts for the user: the **YAML definition schema** (how the
user writes tests) and the **CLI command surface** (how the user runs them).
Both map to the data model in §9.

### 10.1 Project layout, the config file, and discovery

In real use the suites, data files, and environments live together in a **git
repository**. One repository can hold many suites for many APIs. A **config
file** describes a run. It points to the suites to run and holds the other run
settings. This is the normal entry point.

File types:

| File | Format | Holds |
| ---- | ------ | ----- |
| Config file | YAML | one run: the suites to run and the run settings |
| Suite file | YAML | one `Suite`: its config, setup/teardown, and tests |
| Environment file | YAML | one `Environment` profile |
| Data file | JSON | a request body referenced by `jsonFile` (FR-2.3) |

**The config file.** It declares where the suites and the environments live,
which suites to run, and the run defaults:

```yaml
# battery.yaml
suitesRoot: ./suites
environmentsRoot: ./environments
defaultEnvironment: staging

report:
  formats: [junit, json]
  output: ./out

run:                 # a list of selectors (see below)
  - albums           # all album suites, recursively
  - billing/payments # one suite
```

A CLI flag overrides any setting in this file for a single run (§10.6).

**How the tool recognises a suite.** A file is a suite only when both are true:
its name ends in `.suite.yaml`, and it has the top-level `suite:` key. A
directory scan reads only `*.suite.yaml` files and ignores the rest, so a mixed
folder is fine. A `*.suite.yaml` file that does not parse as a valid suite stops
the run at once with a located error (FR-2.5, §8.5).

**Selectors.** A selector names what to run, relative to `suitesRoot`. It uses
slashes, like the folder layout:

- A selector that names a **directory** runs every suite beneath it, recursively.
- A selector that names a **suite** (its path without the `.suite.yaml` suffix)
  runs that one suite.

| Selector | Runs (root = `suites/`) |
| -------- | ----------------------- |
| `albums` | every suite under `suites/albums/**` |
| `albums/70s` | every suite under `suites/albums/70s/**` |
| `albums/70s/steely-dan` | the suite `suites/albums/70s/steely-dan.suite.yaml` |

The `run` list in the config file is a list of selectors. The `--select` flag
overrides that list for one run (§10.6). If a selector matches no suite, the
tool errors.

**Run order.** The run is repeatable. The `run` list sets the order between
selectors, and a directory runs its suites in path-name order (FR-8.1).

### 10.2 Suite file schema

The top-level keys of a suite file map to `Suite`:

| Key | Required | Maps to |
| --- | -------- | ------- |
| `suite` | yes | the suite name |
| `baseConfig` | no | `BaseConfig` (shared defaults) |
| `setup` | no | a step that runs before the tests |
| `teardown` | no | a step that runs after the tests |
| `tests` | yes | an ordered list of `Test` |

**`baseConfig`** holds `baseUrl`, `headers` (a map), `timeout`, and `auth`. A
test inherits these and can override them.

A **`setup`** or **`teardown`** step uses the same keys as a test (`request`,
`extract`, `assert`). A setup step often logs in and extracts a token.

A **`Test`** has these keys:

| Key | Required | Maps to |
| --- | -------- | ------- |
| `name` | yes | the test name |
| `tags` | no | a list of tags (for filtering, FR-3.3) |
| `request` | yes | `Request` |
| `extract` | no | a list of `Extraction` |
| `assert` | no | a list of `Assertion` |

A **`request`** maps to `Request`:

| Key | Maps to |
| --- | ------- |
| `method` | the HTTP method (GET, POST, …) |
| `url` | a path (joined to `baseUrl`) or an absolute URL |
| `query` | a map of query parameters |
| `headers` | a map of headers |
| `body` | `RequestBody` (see below) |
| `timeout` | a duration, for example `10s` |
| `followRedirects` | `true` or `false` |
| `tls` | TLS options, for example `allowSelfSigned: true` (FR-1.6) |
| `auth` | `Auth` (see below) |

**`body`** uses one key to select the `RequestBody` kind:

| Key | Kind |
| --- | ---- |
| `jsonInline` | a JSON object written in the file |
| `jsonFile` | a path to a `.json` data file |
| `form` | a map, sent as form-urlencoded |
| `text` | a raw string |
| `multipart` | a list of parts — deferred to Phase 2 (FR-1.7) |

**`auth`** uses a `type` discriminator:

| `type` | Other keys |
| ------ | ---------- |
| `bearer` | `token` |
| `basic` | `username`, `password` |
| `apiKey` | `name`, `in` (`header` or `query`), `value` |

**`assert`** is a list. Each item uses a `type` discriminator:

| `type` | Other keys |
| ------ | ---------- |
| `status` | `equals` (a number) or `in` (a list of numbers) |
| `header` | `name`, and one of `equals`, `exists`, or `matches` |
| `body` | `jsonPath`, `matcher` (`equals`, `exists`, `contains`, `matches`), and `value` (not needed for `exists`) |
| `responseTime` | `under` (a duration, for example `500ms`) |

**`extract`** is a list. Each item sets a variable:

| Key | Meaning |
| --- | ------- |
| `name` | the variable name to set |
| `from` | `body`, `header`, or `status` |
| `jsonPath` | the path (when `from` is `body`) |
| `header` | the header name (when `from` is `header`) |

### 10.3 Variable interpolation

A string value can hold a variable in the form `${name}`. The user can use it
anywhere in a request or an assertion (FR-5.1). The engine resolves each
variable by the precedence order in §9.3. If a variable has no value, the engine
reports a located validation error (FR-2.5).

### 10.4 Environment files

Each environment profile is its own file, named `<name>.yaml`, under
`environmentsRoot`. The file maps variable names to values. For example
`environments/staging.yaml`:

```yaml
baseUrl: https://staging.albums.example.com
username: tester
```

The config file names the default environment (`defaultEnvironment`). The
`--env <name>` flag selects a different one for a single run (FR-5.2). The
config holds the name only; the tool finds the file by convention.

A secret has no value in the file. Instead the engine reads it from an OS
environment variable of the same name (FR-5.3). So `${ALBUMS_PASSWORD}` resolves
from the OS when no file sets it.

### 10.5 Validation

The loader checks the structure before the run. On bad input it reports a clear
message with the file, the location, and the problem, then stops (FR-2.5,
§8.5). It does not run a partial suite.

### 10.6 CLI command surface

The CLI follows §5.10. The main command is `run`. It has two entry points:

```
battery run [--config <path>]      # the normal path: read a config file
battery run --suite <path>         # run one suite file directly
```

By default `battery run` reads `battery.yaml` in the current folder. `--config`
points to another config file. `--suite` runs a single suite file for quick
local testing; it skips the `run` list, but still reads the config for the
environment and report defaults when a config is present (FR-10.1).

Options (a flag overrides the matching setting in the config file):

| Option | Effect | Requirement |
| ------ | ------ | ----------- |
| `--config <path>` | the config file to read | FR-10.1 |
| `--suite <path>` | run one suite file directly | FR-10.1 |
| `--select <selector>` | override the `run` list (repeatable) | FR-3.2 |
| `--env <name>` | select an environment profile | FR-5.2 |
| `--tag <tag>` / `--filter <expr>` | run only the matching tests | FR-3.3 |
| `--set <key=value>` | override a variable (repeatable) | FR-5.1 |
| `--verbosity <quiet\|normal\|verbose>` | set the output level | FR-9.5 |
| `--report <format>` | write a machine-readable report (`junit`, `json`, or both) | FR-9.4 |
| `--output <path>` | the report file or directory | FR-9.4 |
| `--stop-on-failure` | stop at the first failure | FR-8.3 |
| `--help` | show usage | FR-10.3 |

The `--report` option takes one or both of `junit` and `json`. `junit` writes
JUnit XML, which CI systems read directly. `json` writes Battery's own detailed
report (the `RunResult` from §9). The tool builds the JUnit writer first and the
JSON writer next. Each format sits behind the `IReportWriter` seam (§8.3).

The process exit code is `0` when every test passes. It is non-zero when any
test fails or errors (FR-9.2).

### 10.7 Future front-ends

A GUI, web, or desktop front-end comes in Phase 3. Each one consumes the API,
and the API consumes Core (§8.2). None of them is a Phase 1 deliverable.

## 11. Open questions

No open questions remain for the Phase 1 design. New questions will be added
here as they arise.

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
| 2026-11-07 | Defined the Core data model (§9): authored definitions and result model, all immutable records | Unblocks the engine design; ties each entity to a Phase 1 requirement |
| 2026-11-07 | Variable precedence: suite < environment < OS env < runtime override < extracted (extracted wins) | A chained value flows into later requests without a command-line change |
| 2026-11-07 | Separate `ResolvedRequest` from the authored `Request` | Keeps variable resolution a distinct, testable step |
| 2026-11-07 | Phase 1 body matchers: `equals`, `exists`, `contains`, `matches` | A small, useful set; grow later if needed |
| 2026-11-07 | Defined §10 interfaces: the YAML schema, the environment file, and the `battery run` CLI surface | Gives the user the authoring and run contracts; maps directly to §9 |
| 2026-11-07 | A secret resolves from an OS environment variable of the same name | Keeps secrets out of committed files (FR-5.3) without a second syntax |
| 2026-11-07 | Report format: both JUnit XML and custom JSON via `--report`; build JUnit first | JUnit gives CI integration at once; JSON gives full detail for our tools and a future UI; both sit behind `IReportWriter` |
| 2026-11-07 | A run is driven by a YAML config file (suitesRoot, environmentsRoot, defaultEnvironment, report, run list); CLI flags override | A repo holds many suites for many APIs; one file captures a run and keeps CI settings in the repo |
| 2026-11-07 | Convention-based discovery: a suite is a `*.suite.yaml` file with a `suite:` key; slash selectors under `suitesRoot`; a directory selector recurses | Low ceremony; mirrors the folder layout; fails fast on an unrecognised or empty match |
| 2026-11-07 | One environment file per profile, `<name>.yaml` under `environmentsRoot`; the config names it, convention locates it | Keeps each environment separate and the config short |
| 2026-11-07 | Two CLI entry points: a config file (default), or `--suite <path>` for one suite | The config file is the real-world path; a direct single-suite run helps local development |
| 2026-11-07 | Multipart / file-upload deferred entirely to Phase 2 (kept as a Should, FR-1.7) | Not needed for a usable Phase 1; form-urlencoded and raw/text bodies stay in Phase 1 |
