# Battery — YAML Loader Implementation Spec

> **Status:** Locked · Ready to implement
> **Last updated:** 2026-11-08
> **Related:** `spec/design.md` (the master design). This file expands §10 into a
> build contract. Where the two disagree, `design.md` wins; raise the conflict
> and fix it there.

This spec is self-contained. A later session can build the loader from this file
alone, without re-deriving the decisions. Write all prose in Simplified Technical
English (STE), per `CLAUDE.md`.

---

## 1. Purpose

The loader turns YAML **text** into the §9 data-model records. On bad input it
reports **located errors** (the file, the line, the column, and the problem),
then stops. It never runs a partial suite (FR-2.5, §8.5, §10.5).

This spec covers the **suite loader** — the loader for a suite file. The suite
file is the hardest file to parse, because it holds the discriminated kinds. The
environment loader and the config loader come in later slices (see §12).

## 2. Context (what the implementer must know)

- **Where it lives:** `Battery.Core`, in a new namespace `Battery.Core.Loading`.
  §7 puts "definition parsing" in Core.
- **What it produces:** the authored records from `spec/design.md` §9.1, which
  already exist in `src/Battery.Core/Definitions/`: `Suite`, `BaseConfig`,
  `Test`, `Step`, `Request`, the `RequestBody` kinds, the `Auth` kinds, the
  `Assertion` kinds, `Extraction`, and the `ExtractionSource` kinds. These are
  immutable init-only records with `required` members; collections default to
  empty. The loader builds them; it does not change them.
- **Requirements it meets:** FR-2.1 (YAML definitions), FR-2.2 (a test is a
  request plus assertions and extractions), FR-2.4 (readable, diff-friendly),
  FR-2.5 (validate and report located errors), and the §10.2 suite-file schema.
- **Rules it follows:** §8.5 (expected failures are outcome objects, not
  exceptions; fail fast on bad definitions); §8.4 (nullable on,
  warnings-as-errors, async for I/O, XML docs on the public API); §8.7 (lean on
  the BCL; justify each package).
- **The writing style tells it nothing about data types.** The YAML holds
  `${var}` templates in string values. The loader keeps them as literal strings.
  The resolver fills them later (§9.4). The loader does not look inside a
  template.

## 3. Locked decisions

| # | Decision | Rationale |
| - | -------- | --------- |
| D1 | Parse to the YamlDotNet **DOM** (`YamlStream` → `YamlMappingNode` / `YamlScalarNode` / `YamlSequenceNode`), then hand-map to records. Do **not** deserialize into POCOs. | Every DOM node carries a `Start` mark (line and column), so every error is located (FR-2.5). The DOM gives full control of the `type:` discriminators. The §9 records stay free of serialization attributes. |
| D2 | Return a **`LoadResult<T>`** that holds either the value or a list of located errors. Collect **many** schema errors in one pass. Throw only for genuine faults. | The user fixes a file's problems all at once. Bad user input is an outcome, not an exception (§8.5). |
| D3 | **Reject unknown keys** with a located error (strict). | Catches typos such as `methd:` early. High value in a declarative tool. |
| D4 | For a `jsonFile` body, **store the path only**. Do not read or parse the data file at load time. | Keeps the loader about YAML structure. Execution reads the data file later, relative to the run. |
| D5 | Convert a `jsonInline` body to a `JsonNode` with **minimal scalar type inference** (bool, number, null, else string). A quoted scalar is always a string. A `${var}` template stays a string. | The body is sent as JSON, so it needs JSON types. Templates never match a number or bool pattern, so they stay strings. |
| D6 | Build the **suite loader first**. The environment loader and the config loader plus discovery come after. | The suite file is the hardest and builds the shared machinery (the reader, the error model, the duration parser) that the others reuse. |
| D7 | Add **`YamlDotNet`** to `Battery.Core`. | It is Core's first third-party package. §8.7 already names it as the likely YAML library. |
| D8 | The disk entry point is **async** (`LoadAsync`, with a `CancellationToken`). The in-memory `Parse` is **sync**. | Async for I/O (§8.4). `Parse` is CPU-only and is the deterministic unit under test (§8.6). |

## 4. Public surface

Namespace `Battery.Core.Loading`.

```csharp
public interface ISuiteLoader
{
    // Deterministic, in-memory. The unit under test.
    LoadResult<Suite> Parse(string yaml, string sourceName);

    // Reads the file, then calls Parse with the path as the source name.
    Task<LoadResult<Suite>> LoadAsync(string path, CancellationToken cancellationToken);
}
```

- `sourceName` is the label that appears in each error's `Source` field. For
  `LoadAsync` it is the file path.
- `LoadAsync` reads the text with `File.ReadAllTextAsync`. A read fault (for
  example the file is missing, or access is denied) returns a `Failure` with one
  `LoadError`, not an exception. The loader treats a missing file as bad input.
- The concrete `SuiteLoader` stays `internal`. The test project already has
  `InternalsVisibleTo`.

### 4.1 The result and the error

```csharp
public sealed record LoadError
{
    public required string Source { get; init; }   // the file path or in-memory label
    public required int Line { get; init; }         // 1-based; 0 when unknown
    public required int Column { get; init; }       // 1-based; 0 when unknown
    public required string Path { get; init; }      // the location in the document, e.g. tests[2].request.method
    public required string Message { get; init; }   // the problem, in plain STE
}
```

`LoadResult<T>` is a small discriminated result. Model it as a sealed record with
a private constructor and two factory methods, or as an abstract base with
`Success` and `Failure` derived records. Either way it exposes:

- `bool IsSuccess`
- `T? Value` (set only on success)
- `IReadOnlyList<LoadError> Errors` (empty on success)

Put `LoadResult<T>` and `LoadError` in `Battery.Core.Loading`. They are a loading
concern, not domain data, so they do not belong in `Definitions` or `Results`.

### 4.2 Dependency injection

Add a light registration so the CLI can resolve the loader later:

```csharp
public static IServiceCollection AddBatteryCore(this IServiceCollection services);
```

It registers `ISuiteLoader` → `SuiteLoader`. This adds
`Microsoft.Extensions.DependencyInjection.Abstractions` to Core, which §8.3 and
§8.7 anticipate. If we decide to defer DI until the CLI wires services, drop this
method; it is the one soft point in the plan.

## 5. The reader substrate

A small internal `YamlReader` wraps a `YamlMappingNode` plus the source name, the
current document path, and a shared error sink (a `List<LoadError>`). It makes the
mapping code short and uniformly located. Each method that fails **logs a located
error to the sink and returns a default**, so the mapping continues and gathers
more than one error per pass.

Methods (names are a guide, not fixed):

| Method | Behaviour |
| ------ | --------- |
| `RequireString(key)` | The key must be a scalar. Missing or wrong type logs an error, returns `null`. |
| `OptionalString(key)` | Returns the scalar, or `null` when absent. |
| `RequireEnum<TEnum>(key)` | Maps a scalar to an enum value (case-insensitive). An unknown value logs an error that lists the valid values. |
| `OptionalBool(key)` | Reads `true`/`false`. A non-bool scalar logs an error. |
| `OptionalDuration(key)` | Reads a duration scalar (see §7). A bad format logs an error. |
| `OptionalMap(key)` | Returns the child `YamlMappingNode`, or `null`. A non-map logs an error. |
| `OptionalStringMap(key)` | Returns an `IReadOnlyDictionary<string,string>` from a mapping of scalars. Defaults to empty. |
| `OptionalList(key)` | Returns the child `YamlSequenceNode`, or `null`. A non-sequence logs an error. |
| `RequireExactlyOneOf(keys…)` | Returns the single present key. Zero or more than one logs an error. Used to pick the body kind. |
| `RejectUnknownKeys(known…)` | Logs an error for each key not in the known set (D3). |

A located error reads its `Line`/`Column` from the offending node's `Start` mark.
For a **missing** required key, the location is the parent mapping's mark. Track
the document `Path` as the reader descends (for example `tests[2].request`), so
each error names where it is.

At the end of a parse: if the sink is empty, return `Success(value)`; otherwise
return `Failure(sink)` and discard the partial value.

## 6. Discriminator dispatch

Four dispatch styles, matching §10.2. Each unknown discriminator is a located
error that lists the valid values.

| Family | Selector | Values → kind |
| ------ | -------- | ------------- |
| `Assertion` | the `type` key | `status`→`StatusAssertion`, `header`→`HeaderAssertion`, `body`→`BodyAssertion`, `responseTime`→`ResponseTimeAssertion` |
| `Auth` | the `type` key | `bearer`→`BearerAuth`, `basic`→`BasicAuth`, `apiKey`→`ApiKeyAuth` |
| `ExtractionSource` | the `from` key | `body`→`BodyExtractionSource`, `header`→`HeaderExtractionSource`, `status`→`StatusExtractionSource` |
| `RequestBody` | **which key is present** | `jsonInline`→`JsonInlineBody`, `jsonFile`→`JsonFileBody`, `form`→`FormBody`, `text`→`TextBody` |

Use a `switch` on the discriminator string. §8.3 asks for "a simple internal
registry/factory — not external plugin loading"; a `switch` meets that for a
fixed, small set. If the behaviour side (the `IAssertion` evaluators) later wants
to share the same table, promote it to a `Dictionary<string, …>` registry then,
not now.

## 7. The duration parser

Used by `request.timeout` and by the `responseTime` assertion's `under` key. It
produces a `TimeSpan`.

- Grammar: a number, then a unit. Pattern: `^\s*(\d+(\.\d+)?)\s*(ms|s|m|h)\s*$`.
- Units: `ms` = milliseconds, `s` = seconds, `m` = minutes, `h` = hours.
- The number can be an integer or a decimal (for example `1.5s`).
- A value with no unit, an unknown unit, or a non-number is a located error. The
  message gives one valid example, for example `10s` or `500ms`.

Put it in an internal `DurationParser` with a `bool TryParse(string, out TimeSpan)`
shape, so the reader and the tests can both call it.

## 8. The `jsonInline` converter

An internal `YamlToJsonConverter` walks a YAML node into a `System.Text.Json.Nodes.JsonNode`:

- A YAML mapping → `JsonObject`.
- A YAML sequence → `JsonArray`.
- A YAML scalar:
  - A **quoted** scalar (the node `Style` is single- or double-quoted) is always a
    string.
  - A **plain** scalar is inferred: `null`/`~`/empty → JSON null; `true`/`false`
    (case-insensitive) → bool; an integer pattern → a JSON number (long); a
    decimal pattern → a JSON number (double); otherwise a string.
- A `${var}` template never matches a number or bool pattern, so it stays a
  string. The resolver walks the string leaves later.

## 9. The schema (the full key-to-record map)

This restates §10.2 so the spec is self-contained. Every entity below rejects
unknown keys (D3).

### 9.1 Suite file (top level) → `Suite`

| Key | Required | Maps to | Notes |
| --- | -------- | ------- | ----- |
| `suite` | yes | `Name` | A non-empty string. |
| `tags` | no | `Tags` | A list of strings. Fills a small gap: §9.1 gives `Suite` a `tags` field, but the §10.2 table omits the key. FR-3.3 tags suites too, so support it. Defaults to empty. |
| `baseConfig` | no | `BaseConfig` | See §9.2. |
| `setup` | no | `Setup` | A step (§9.3). |
| `teardown` | no | `Teardown` | A step (§9.3). |
| `tests` | yes | `Tests` | A non-empty list of tests (§9.4). Run order is the list order (FR-8.1). |

### 9.2 `baseConfig` → `BaseConfig`

| Key | Required | Maps to |
| --- | -------- | ------- |
| `baseUrl` | no | `BaseUrl` |
| `headers` | no | `Headers` (string map) |
| `timeout` | no | `Timeout` (duration) |
| `auth` | no | `Auth` (§9.6) |

### 9.3 `setup` / `teardown` → `Step`

| Key | Required | Maps to |
| --- | -------- | ------- |
| `request` | yes | `Request` (§9.5) |
| `extract` | no | `Extractions` (§9.8) |
| `assert` | no | `Assertions` (§9.7) |

### 9.4 A test → `Test`

| Key | Required | Maps to |
| --- | -------- | ------- |
| `name` | yes | `Name` |
| `tags` | no | `Tags` (string list) |
| `request` | yes | `Request` (§9.5) |
| `extract` | no | `Extractions` (§9.8) |
| `assert` | no | `Assertions` (§9.7) |

### 9.5 `request` → `Request`

| Key | Required | Maps to | Notes |
| --- | -------- | ------- | ----- |
| `method` | yes | `Method` | One of GET, POST, PUT, PATCH, DELETE, HEAD, OPTIONS (case-insensitive). |
| `url` | yes | `Url` | A path or an absolute URL. Kept as text (may hold templates). |
| `query` | no | `Query` (string map) | |
| `headers` | no | `Headers` (string map) | |
| `body` | no | `Body` | Exactly one body kind (§9.9). |
| `timeout` | no | `Timeout` (duration) | |
| `followRedirects` | no | `FollowRedirects` (bool) | |
| `tls` | no | `Tls` | A map with `allowSelfSigned` (bool) → `TlsOptions`. |
| `auth` | no | `Auth` | §9.6. |

### 9.6 `auth` → `Auth` (discriminator `type`)

| `type` | Keys | Maps to |
| ------ | ---- | ------- |
| `bearer` | `token` | `BearerAuth { Token }` |
| `basic` | `username`, `password` | `BasicAuth { Username, Password }` |
| `apiKey` | `name`, `in` (`header` or `query`), `value` | `ApiKeyAuth { Name, Location, Value }` |

### 9.7 `assert` list → `Assertion` (discriminator `type`)

| `type` | Keys | Maps to |
| ------ | ---- | ------- |
| `status` | `equals` (a number) **or** `in` (a list of numbers) — exactly one | `StatusAssertion { ExpectedStatus }` or `{ AllowedStatuses }` |
| `header` | `name`, and **one of** `equals` / `exists` / `matches` (the present key sets the matcher and its value; `exists` takes no value) | `HeaderAssertion { Name, Matcher, Value }` |
| `body` | `jsonPath`, `matcher` (`equals` / `exists` / `contains` / `matches`), `value` (not needed for `exists`) | `BodyAssertion { JsonPath, Matcher, Value }` |
| `responseTime` | `under` (a duration) | `ResponseTimeAssertion { Under }` |

> Note the two matcher styles: `header` selects its matcher by **which key is
> present**; `body` selects its matcher by an explicit **`matcher` field**. This
> matches §10.2. Keep both as written.

### 9.8 `extract` list → `Extraction`

| Key | Required | Maps to |
| --- | -------- | ------- |
| `name` | yes | `Name` |
| `from` | yes | the source kind: `body` / `header` / `status` |
| `jsonPath` | when `from: body` | `BodyExtractionSource { JsonPath }` |
| `header` | when `from: header` | `HeaderExtractionSource { HeaderName }` |
| (none) | when `from: status` | `StatusExtractionSource` |

### 9.9 `body` → `RequestBody` (exactly one key)

| Key | Maps to | Notes |
| --- | ------- | ----- |
| `jsonInline` | `JsonInlineBody { Json }` | Convert the sub-tree to a `JsonNode` (§8). |
| `jsonFile` | `JsonFileBody { Path }` | Store the path only (D4). |
| `form` | `FormBody { Fields }` | A string map. |
| `text` | `TextBody { Text }` | A raw string. |
| `multipart` | — | A located error: multipart is deferred to Phase 2 (FR-1.7). |

## 10. Error semantics

- **YAML syntax error** (the DOM will not build): catch YamlDotNet's
  `YamlException`, read its `Start` mark, and return a `Failure` with one
  `LoadError`. The parse stops here, because there is no tree to walk.
- **Schema errors** (missing or wrong keys, unknown keys, bad discriminators, bad
  durations): collect every one found in a single pass, then return `Failure`.
- The loader **never** returns a partial `Suite`. A `Success` means the whole file
  mapped cleanly.
- An empty document, or a top level that is not a mapping, is a single located
  error.

## 11. Testing

- Feed YAML **strings** to `Parse(yaml, sourceName)`. No disk. Deterministic and
  isolated (§8.6).
- For a valid file, assert the produced `Suite` equals the expected record.
- For a bad file, assert the `Failure` holds the expected errors: check the
  count, the `Path`, and the `Message` substring. Check the `Line` on a few, to
  prove locations work.
- Use table-driven tests for the many schema rules (one bad field per case).
- Cover: a missing required key; an unknown key; an unknown discriminator; a bad
  duration; two body kinds at once; `multipart` present; a `status` assertion
  with neither `equals` nor `in`; `from: header` without `header`; the `jsonInline`
  type inference (a quoted number stays a string); templates survive untouched.
- Add one `LoadAsync` test against a temp file for the happy path and the
  missing-file path.

## 12. Out of scope (future slices)

- **Variable resolution.** The loader keeps `${var}` as text. The resolver (its
  own slice) fills templates and reports a located error for an unknown variable
  (§10.3). `ResolvedRequest` (§9.4) belongs to that slice.
- **The `jsonFile` data file.** Read at execution, not at load (D4).
- **The environment loader.** A later slice. It maps `<name>.yaml` (a flat
  `name: value` map) to `EnvironmentProfile`. It reuses this spec's reader and
  error model.
- **The config loader and discovery.** A later slice. It maps `battery.yaml` to a
  new `RunConfig` record (and a `ReportConfig`), then walks `suitesRoot` for
  `*.suite.yaml` files and applies the selectors (§10.1). §9 does not define
  `RunConfig`; add it, and record the addition in `design.md` §12 then.

## 13. New types this slice introduces

- `Battery.Core.Loading.ISuiteLoader` (public)
- `Battery.Core.Loading.SuiteLoader` (internal)
- `Battery.Core.Loading.LoadResult<T>` (public)
- `Battery.Core.Loading.LoadError` (public record)
- `Battery.Core.Loading.YamlReader` (internal)
- `Battery.Core.Loading.DurationParser` (internal)
- `Battery.Core.Loading.YamlToJsonConverter` (internal)
- `AddBatteryCore` DI extension (public), if we keep §4.2
- Package: `YamlDotNet` on `Battery.Core`. Pin a current stable version at build
  time; mark it provisional, like the test packages.

## 14. Build increments (commit and push each)

1. `LoadResult` / `LoadError` + the `YamlReader` substrate + `DurationParser`,
   with unit tests. Add the `YamlDotNet` package.
2. Request mapping: `method`, `url`, `query`, `headers`, `body` kinds (plus the
   `jsonInline` converter), `auth`, `tls`, `timeout`, with tests.
3. Assertion and extraction mapping, with tests.
4. Suite / Test / Step / BaseConfig assembly: the full `Parse`, then `LoadAsync`
   and the DI registration, with tests.

Each increment compiles and tests on its own, given the ones before it. The
build is verified in Codespaces on the .NET 10 SDK, because this cloud session
has no `dotnet`.

## 15. Worked example

Input (`albums.suite.yaml`):

```yaml
suite: Albums API
baseConfig:
  baseUrl: https://staging.albums.example.com
  headers:
    Accept: application/json
  timeout: 10s
  auth:
    type: bearer
    token: ${token}

setup:
  request:
    method: POST
    url: /login
    body:
      jsonInline:
        user: ${username}
        password: ${ALBUMS_PASSWORD}
  extract:
    - name: token
      from: body
      jsonPath: $.access_token

tests:
  - name: get an album
    tags: [smoke]
    request:
      method: GET
      url: /albums/1
    assert:
      - type: status
        equals: 200
      - type: body
        jsonPath: $.title
        matcher: equals
        value: Aja
      - type: responseTime
        under: 500ms
```

Maps to a `Suite` with: `Name = "Albums API"`; a `BaseConfig` with the base URL,
one header, a 10-second timeout, and a `BearerAuth` whose token is the literal
`${token}`; a `Setup` step that posts to `/login` with a `JsonInlineBody` and
extracts `token` from the body; and one `Test` named `get an album`, tagged
`smoke`, with a GET request and three assertions (status equals 200, body
`$.title` equals `Aja`, response time under 500 ms).

A bad input:

```yaml
suite: Albums API
tests:
  - name: broken
    request:
      methd: GET
      url: /albums/1
```

Returns a `Failure` with two located errors at `tests[0].request`: one for the
unknown key `methd`, and one for the missing required key `method`. No `Suite` is
produced.
