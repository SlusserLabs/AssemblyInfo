# General Instructions

- Acquire resources as late as possible after validation, and release them as early as possible
- Prefer removing unused code over commenting it out
- Correct spelling errors in instructions without changing the meaning
- Do not reformat existing code unless explicitly requested
- Always apply and follow all rules in `.editorconfig`
- Do not include or reference publicly available code unless explicitly requested
- Comments should only be used to explain the code, never to explain changes made to the code
- Add brief comments explaining non-obvious binary layouts, protocol byte offsets, bit manipulation, and byte packing; unexplained byte bashing is not acceptable
- When writing `markdown`, wrap language `keywords` and `types` in backticks
- Prefer focused, direct, concise implementations; avoid unnecessary abstractions, indirection, helper layers, and verbose boilerplate
- Keep code tight without sacrificing correctness, readability, or maintainability

# C# Code Style

## Language & Framework

- The projects target .NET 10. Use modern C# features
- Nullable reference types are enabled and should be used correctly
- Never use obsolete or deprecated APIs, methods, properties, or types; always use the recommended modern alternatives
- Never use primary constructors for classes or records; use traditional constructors instead

## File Organization

- Never have more than one type definition in a file; allow existing files with multiple definitions to remain unchanged unless explicitly asked to reformat

## Formatting

- Use file-scoped namespaces
- Place `using` directives outside the namespace; prefer them over fully qualified names
- Do not use fully qualified type names in code; add the necessary `using` directives and reference the type by its simple name
- Use implicit usings where possible
- Sort `using` directives with `System` first; remove unnecessary directives
- Use spaces for indentation; do not use tabs
- Prefer a maximum line length of 160 characters; do not wrap lines unless they exceed 160 characters
- Keep declarations, argument lists, calls, conditions, and other expressions on one line when they fit within 160 characters
- Precede a `return` statement with a blank line when more than two distinct statements come before it in the same logical block; treat a cohesive run of repeated assignments as one logical sequence and keep the `return` adjacent
- Place `#pragma warning disable` directives at the top of the file before `using` statements, separated by a blank line; do not add a corresponding `#pragma warning restore` at the end of the file; include a short justification comment on the same line:

  ```cs
  #pragma warning disable CA1062 // Public args already validated by framework

  using System;
  ```
- Always use braces with conditional statements, loops, and scope statements; never omit them
- Place braces on their own line, never on the same line as the statement:

  ```cs
  // Correct
  if (condition)
  {
      DoSomething();
  }

  // Wrong — braces on same line
  if (condition) {
      DoSomething();
  }

  // Wrong — no braces
  if (condition)
      DoSomething();
  ```

- Use `\r\n` (CRLF) for line breaks
- Do not leave whitespace characters on empty lines or at the end of lines

## Naming Conventions

- Use PascalCase for classes, methods, properties, and public fields
- Use `_camelCase` for all private fields, including `const`, `readonly`, `static`, and `static readonly` fields (e.g., `_fieldName`). Do not use PascalCase for private constants.
- Methods returning `Task` or `ValueTask` should have the `Async` suffix, except unit test methods and framework-defined signatures; unit test names follow `{MethodName}_{Scenario}_{Outcome}`

## Asynchronous Programming

- Always use `async`/`await`
- Prefer `ValueTask` / `ValueTask<T>` over `Task` / `Task<T>` whenever possible; exceptions include methods that implement framework interfaces requiring `Task` (e.g., `IHostedService.StartAsync` / `StopAsync`)
- Name `CancellationToken` parameters `cancellationToken` (never `ct`); add it as the last parameter of every async method using `= default` on public methods only
- Use `ConfigureAwait(false)` in library code; this is not necessary in test projects
- Never call async methods synchronously using `.Result` or `.GetAwaiter().GetResult()`
- Use `CancellationToken.None` instead of `new CancellationToken()`

## Error Handling & Validation

- Use modern exception helpers instead of manual checking and throwing:
  - `ArgumentNullException.ThrowIfNull`
  - `ArgumentException.ThrowIfNullOrEmpty` (prefer over `ThrowIfNullOrWhiteSpace` unless whitespace validation is specifically required)
  - `ArgumentOutOfRangeException.ThrowIfLessThan`, `ThrowIfNegativeOrZero`, `ThrowIfNegative`, `ThrowIfZero`, `ThrowIfGreaterThan`

## String Handling

- Use `string.Empty` instead of `""` for empty strings except when a constant is required
- Use `string.IsNullOrEmpty` instead of `string.IsNullOrWhiteSpace` unless whitespace validation is specifically required
- Use `default` instead of `default(T)` for default value expressions
- Use `default` instead of `null` when initializing a reference type to its default value
- Use overloads accepting `StringComparison`: `StringComparison.OrdinalIgnoreCase` for case-insensitive comparisons, `StringComparison.Ordinal` for case-sensitive comparisons
- When referencing types or members in strings, use string interpolation with `{nameof(Symbol)}` to ensure type safety during refactoring

## Other Conventions

- Prefer `Microsoft.AspNetCore.Http.StatusCodes` over `System.Net.HttpStatusCode` or integer literals
- For methods that accept an `IFormatProvider` parameter, use `CultureInfo.InvariantCulture` unless specified otherwise
- Use `TimeProvider.GetUtcNow()` in place of `DateTimeOffset.UtcNow`
- Do not place lambda expressions directly in conditional expressions (ternary operator `?:`); assign them to variables with explicit types or use `if`/`else` statements instead
- Omit the period for single-statement, single-line code comments; for comments with two or more statements, use periods
- Implement `IDisposable` for types that manage unmanaged resources; prefer `IAsyncDisposable` when the class contains members that also support `IAsyncDisposable`
- When working with I/O streams, use `System.IO.Pipelines` for high-performance I/O

# XML Documentation

- Add `/// <summary>` documentation to every public type and member; keep comments informative and concise, not generic. XML documentation is never required in test projects
- Do not generate XML documentation comments for private or internal types and members
- Place `<summary>` and `</summary>` on their own lines — never inline:

  ```cs
  /// <summary>
  /// The timer is waiting to fire.
  /// </summary>
  ```

- For `<param>` and `<typeparam>` tags, keep the start and end tags on the same line as the description. If the description is multi-line, place the start and end tags on their own lines
- For a parameter named `cancellationToken`, use `A token to monitor for cancellation requests.` as the description unless otherwise instructed
- The `<returns>` tag may be omitted on methods and `<value>` tag may be omitted on properties; never omit `<returns>` on `Try*` methods
- For methods returning `Task` or `ValueTask`, document the result type contained within the task (e.g., `A <see cref="bool" /> indicating...`), not the task itself
- Do not mention that a method is asynchronous in its documentation
- Use braces `{}` for generic type parameters in documentation (e.g., `List{T}`); never use XML escape sequences or `CDATA`
- Use `<c></c>` for inline code and keywords (e.g., `<c>true</c>`, `<c>false</c>`); never use `<code></code>`
- Use `<see cref="Symbol" />` or `<paramref name="parameterName" />` to reference types and parameters in documentation
- Do not use fully qualified type names in XML documentation; add required `using` directives and reference types by simple name
- Use `<inheritdoc />` as the sole documentation element for inherited members

# C# Unit Tests

## Framework & Tools

- Use TUnit as the test framework and Imposter as the mocking framework
- Use the built-in assertions provided by TUnit; do not add other assertion libraries like `FluentAssertions`

## TUnit Conventions

- All assertions are async: `await Assert.That(value).IsEqualTo(expected)`
- Type assertions: `await Assert.That(obj).IsTypeOf<MyClass>()`
- Exception assertions: `await Assert.ThrowsAsync<TException>(() => ...)`
- Null assertions: `await Assert.That(obj).IsNotNull()`
- Test methods use the `[Test]` attribute
- Lifecycle hooks: `[Before(HookType.Test)]`, `[After(HookType.Test)]`, `[Before(HookType.Class)]`, `[After(HookType.Class)]`
- Shared fixtures: `[ClassDataSource<TFixture>(Shared = SharedType.PerClass)]`
- Prefer parameterized tests over separate individual tests when only inputs or expected results differ: `[Arguments(...)]`, `[MethodDataSource(nameof(...))]`
- Use static helper methods for setup; do not place test logic in `[Before]`/`[After]` hooks

## Structure & Organization

- Consolidate all argument-validation exception tests (null inputs, empty strings, mutually exclusive options) into a single test method per class; do not create a separate test for each invalid argument scenario
- Follow the arrange-act-assert pattern; include `// Arrange`, `// Act`, and `// Assert` section comments unless it makes sense to omit them
- Test projects mirror the namespace and folder structure of the code under test (e.g., `Services/MyClass.cs` → `Services/MyClassTests.cs`)
- Use static helper methods for repetitive setup; test methods do not share state or mock instances between discrete tests
- Properly dispose `IDisposable` artifacts with `using` statements in both test and helper methods
- Test projects should have a file at `Properties/AssemblyInfo.cs` that excludes the project from code coverage:

  ```cs
  using System.Diagnostics.CodeAnalysis;

  [assembly: ExcludeFromCodeCoverage]
  ```

## Naming Conventions

- Name test classes `{ClassName}Tests`
- Name test methods `{MethodName}_{Scenario}_{Outcome}` (e.g., `ConnectAsync_WithNullEndPoint_ThrowsArgumentNullException`)
  - For tests that expect a successful result, the outcome should describe the expected result (e.g., `ReturnsValue`)
  - For tests that expect an exception, the outcome should be `Throws{ExceptionName}` (e.g., `ThrowsArgumentNullException`)
  - You may shorten `Constructor` to `Ctor` for constructor tests

## Coverage & Quality

- Cover happy path (valid input), edge cases (boundary values, null inputs, empty collections), and failure paths (invalid input, exceptions)
- Do not execute external dependencies, in-memory databases, or evaluate SQL queries
- Achieve 100% code coverage through public methods, including conditional logic, loops, and mocked dependencies
- Do not modify the class under test; output only test code
- Prefer a type's default interface implementation over mocking when possible

## Fakes for Services

- Do not mock `IServiceProvider`; use `ServiceCollection` and `BuildServiceProvider`; always register services with their interface
- Do not mock `ILogger` or `ILoggerFactory` unless asserting log messages; use `NullLogger.Instance`, `NullLogger<T>.Instance`, and `NullLoggerFactory.Instance` registered via their respective interfaces
- Use `Options.Create<TOptions>(optionsValue)` instead of `OptionsWrapper<TOptions>` when creating `IOptions<TOptions>` instances for testing
