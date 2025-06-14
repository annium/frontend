---
_layout: landing
---

# Annium.Frontend Documentation

Annium.Frontend is a comprehensive Blazor component library framework targeting .NET 9.0, providing specialized UI components, state management, and advanced styling capabilities for building modern web applications.

## Blazor Components

### Core Components

- **[Annium.Blazor.Core](api/web/Annium.Blazor.Core/Annium.Blazor.Core.yml)** - Base component infrastructure, utilities, and common patterns
- **[Annium.Blazor.Css](api/web/Annium.Blazor.Css/Annium.Blazor.Css.yml)** - CSS-in-C# styling system with fluent API for dynamic styling
- **[Annium.Blazor.Interop](api/web/Annium.Blazor.Interop/Annium.Blazor.Interop.yml)** - DOM manipulation and JavaScript interop utilities with type-safe wrappers

### UI Component Libraries

- **[Annium.Blazor.Ant](api/web/Annium.Blazor.Ant/Annium.Blazor.Ant.yml)** - Ant Design component wrappers and integrations
- **[Annium.Blazor.MatBlazor](api/web/Annium.Blazor.MatBlazor/Annium.Blazor.MatBlazor.yml)** - Material Design component wrappers and theming

### Advanced Components

- **[Annium.Blazor.Charts](api/web/Annium.Blazor.Charts/Annium.Blazor.Charts.yml)** - Advanced charting components including candlestick charts, line charts, and multi-series visualizations
- **[Annium.Blazor.Routing](api/web/Annium.Blazor.Routing/Annium.Blazor.Routing.yml)** - Advanced routing system with path matching and navigation utilities
- **[Annium.Blazor.State](api/web/Annium.Blazor.State/Annium.Blazor.State.yml)** - Component state management with local/session storage integration
- **[Annium.Blazor.Net](api/web/Annium.Blazor.Net/Annium.Blazor.Net.yml)** - Network utilities and HTTP client extensions for Blazor applications

## State Management

Observable state management system for complex application state:

- **[Annium.Components.State.Core](api/shared/Annium.Components.State.Core/Annium.Components.State.Core.yml)** - Core observable state management with `IObservableState<T>` interface and state containers
- **[Annium.Components.State.Forms](api/shared/Annium.Components.State.Forms/Annium.Components.State.Forms.yml)** - Form state containers including atomic, array, map, and object containers with validation support
- **[Annium.Components.State.Operations](api/shared/Annium.Components.State.Operations/Annium.Components.State.Operations.yml)** - Operation state tracking with `IOperationState<T>` for managing async operations and their lifecycle

## Key Features

### Component Architecture
- All components inherit from `BaseComponent` providing common functionality
- CSS styling via fluent API using `Annium.Blazor.Css`
- Dependency injection through `ServiceContainer` abstraction
- Component-scoped stylesheets and dynamic styling support

### CSS-in-C# System
- Fluent API for building CSS rules: `Rule.Create().Width(100).Height(50)`
- Support for responsive design and media queries
- Component-scoped styling with automatic class generation
- Integration with popular CSS frameworks

### State Management
- Observable state with automatic change notifications
- Form state containers with validation integration
- Operation state tracking with `IOperationState<T>`
- Local and session storage persistence

### JavaScript Interop
- Type-safe JavaScript interop via `InteropContext`
- Element manipulation through strongly-typed wrappers
- Event handling for DOM events (mouse, keyboard, resize, wheel)
- Canvas and advanced DOM manipulation support

### Advanced Charting
- Candlestick charts for financial data visualization
- Multi-series line and area charts
- Responsive and interactive chart components
- Customizable styling and theming

## Getting Started

Each component library is designed to work independently while providing seamless integration when used together. The framework follows modern Blazor patterns and conventions:

- Components target .NET 9.0 with latest C# language features
- Nullable reference types enabled with warnings as errors
- Central package management for consistent versioning
- Comprehensive testing framework with xunit.v3

## Architecture Overview

The framework is organized into two main categories:

**Web Components** (`web/src/`):
- Blazor-specific UI components and libraries
- Browser-specific functionality and interop
- Component demos and examples

**Shared Components** (`shared/src/`):
- Platform-agnostic state management
- Core abstractions and utilities
- Reusable across different UI frameworks

## Contributing

The framework uses modern development practices including:
- Component-based architecture with inheritance from `BaseComponent`
- CSS-in-C# for maintainable styling
- Observable state patterns for reactive UIs
- Type-safe JavaScript interop
- Comprehensive unit testing

Explore the API documentation for detailed information about each component library and discover how to build modern, reactive web applications with Annium.Frontend.