# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Development Commands

**Essential Commands:**

- `make setup` - Restore dotnet tools (CSharpier, xs.cli)
- `make format` - Format code using CSharpier and xs format
- `make build` - Build solution in Release configuration
- `make test` - Run all tests with TRX logging
- `make clean` - Clean solution and remove packages

**Documentation Commands:**

- `make docs-build` - Build documentation using DocFX
- `make docs-serve` - Build and serve documentation locally at http://localhost:8080
- `make docs-clean` - Clean documentation artifacts (_site, api, obj)
- `make docs-watch` - Build and serve documentation with file watching

**Demo Applications:**

- `make demo-blazor-ant` - Run Ant Design demo in development mode
- `make demo-blazor-interop` - Run DOM interop demo in development mode
- `make demo-blazor-charts` - Run charts demo in development mode
- Production versions available with `-prod` suffix

**Single Test Execution:**

```bash
dotnet test path/to/TestProject.csproj --filter "TestMethodName"
dotnet test --filter "ClassName.TestMethodName"
```

**Package Management:**

- `make update` - Update all packages using xs tool
- `make pack` - Create NuGet packages
- Uses central package management via `Directory.Packages.props`

## Architecture Overview

This is a Blazor component library framework targeting .NET 9.0, organized into specialized UI and state management
components:

**Core Structure:**

- `shared/src/` - Shared state management components
- `web/src/` - Blazor web components and UI libraries
- `web/demo/` - Demo applications showcasing component usage
- `web/test/` - Component unit tests

**Key Component Libraries:**

- `Annium.Blazor.Core` - Base component infrastructure and utilities
- `Annium.Blazor.Css` - CSS-in-C# styling system with fluent API
- `Annium.Blazor.Charts` - Advanced charting components (candlestick, line, multi-series)
- `Annium.Blazor.Interop` - DOM manipulation and JavaScript interop utilities
- `Annium.Blazor.Routing` - Advanced routing system with path matching
- `Annium.Blazor.State` - Component state management with local/session storage
- `Annium.Blazor.Ant` - Ant Design component wrappers
- `Annium.Blazor.MatBlazor` - Material Design component wrappers
- `Annium.Components.State.*` - Observable state management system

## Key Patterns

**Component Architecture:**

- All components inherit from `BaseComponent` which provides common functionality
- CSS styling via fluent API using `Annium.Blazor.Css`
- State management through observable state containers
- Dependency injection through `ServiceContainer` abstraction

**State Management System:**

- Observable state with `IObservableState<T>` interface
- Form state containers: `IAtomicContainer`, `IArrayContainer`, `IMapContainer`, `IObjectContainer`
- Operation state tracking with `IOperationState<T>`
- Tracked state changes with validation integration

**Testing Framework:**

- Custom `TestBase` class with DI container setup
- Uses xunit.v3 as test framework
- Fluent assertions via Annium.Testing framework
- Test naming convention: `MethodName_Scenario_ExpectedResult()`

**CSS-in-C# System:**

- Fluent API for building CSS rules: `Rule.Create().Width(100).Height(50)`
- Component-scoped stylesheets via `StyleSheet` component
- Support for responsive design and dynamic styling

**Interop System:**

- Type-safe JavaScript interop via `InteropContext`
- Element manipulation through strongly-typed wrappers (`Canvas`, `Div`, `Input`)
- Event handling for DOM events (mouse, keyboard, resize, wheel)

## Project Conventions

- All projects target .NET 9.0 with latest C# language version
- Nullable reference types enabled with warnings as errors
- Central package management via `Directory.Packages.props`
- Component projects use `Annium.Blazor.{Module}` naming convention
- State projects use `Annium.Components.State.{Module}` naming convention
- Test projects named `{Module}.Tests`
- Documentation generated via DocFX with API documentation
- TypeScript compilation for JavaScript interop files