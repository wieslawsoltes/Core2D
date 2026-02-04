# Engineering Guide (AGENTS)

This document defines the architectural and coding rules for the project. It is
authoritative for all new code and refactors.

## 1) Core principles (non-negotiable)

### SOLID (strict)
- Single Responsibility: every class has exactly one reason to change.
- Open/Closed: extend behavior via composition and interfaces; avoid modifying
  stable code paths when adding features.
- Liskov Substitution: derived types must be safely substitutable without
  altering expected behavior or contract.
- Interface Segregation: prefer small, focused interfaces; avoid "god" interfaces.
- Dependency Inversion: depend on abstractions; wire concrete types in the
  composition root only.

### MVVM (strict)
- Views are passive. No UI logic in code-behind beyond `InitializeComponent()`.
- All inputs are routed to ViewModels via bindings, commands, and behaviors.
- ViewModels are UI-framework agnostic and unit-testable.
- Models and services contain business logic and data access; ViewModels orchestrate
  them via DI.
- Prefer composition in ViewModels/services/code over inheritance wherever possible,
  except where framework base types are required (e.g., `ReactiveObject` for ViewModels).

## 2) Architecture

### Layering
- UI (Avalonia Views + XAML): visual composition only.
- Presentation (ViewModels): state, commands, reactive composition.
- Domain/Services: business logic, parsing, validation, domain rules.
- Infrastructure: file system, persistence, external integrations.

### Boundaries
- UI depends on Presentation; Presentation depends on Domain; Infrastructure is
  depended on by Domain or Presentation via interfaces.
- No reference from Domain to UI or Avalonia types.

## 3) Avalonia UI best practices (aligned with Avalonia codebase)

Reference: https://github.com/AvaloniaUI/Avalonia

### Views and styling
- Use XAML for layout and visuals; avoid creating controls in code.
- Define styles and resources in dedicated resource dictionaries and merge them
  in `App.axaml` to keep styling consistent and maintainable.
- Prefer `StaticResource` for immutable resources and `DynamicResource` when
  runtime updates are required.

### Data binding
- Use compiled bindings only (no reflection bindings) with explicit `x:DataType` on
  all binding scopes (views, DataTemplates, control themes, and resources).
- Keep bindings one-way unless user input must update the ViewModel.
- Use `DataTemplates` or a `ViewLocator` (custom, non-reflection) for view lookup.

### Custom controls
- Use `StyledProperty` only for values that must participate in styling.
- Prefer `DirectProperty` for non-styled properties to avoid extra overhead.
- For best UI/UX, prefer custom control creation or re-templating using control themes
  instead of CRUD-style UI.

## 4) ReactiveUI (required)

Reference: https://github.com/reactiveui/ReactiveUI

### ViewModel base
- All ViewModels inherit from `ReactiveObject`.
- Use `ReactiveCommand` for commands; never use event handlers in code-behind.
- Use `WhenAnyValue`, `ObservableAsPropertyHelper`, and `Interaction<TIn,TOut>`
  to model state, derived values, and dialogs.
- Use `ReactiveUI.SourceGenerators` for INPC/ReactiveObject boilerplate where applicable.
  https://github.com/reactiveui/ReactiveUI.SourceGenerators

### Navigation (ReactiveUI routing)
- Use `IScreen` with a single `RoutingState` as the navigation root.
- All navigable ViewModels implement `IRoutableViewModel`.
- Views host navigation via `RoutedViewHost`.
- Use route segments that are stable, explicit, and testable.

### Avalonia integration
- Use `ReactiveUI.Avalonia` (latest) and do not use `Avalonia.ReactiveUI` directly.
- If a third-party dependency requires `Avalonia.ReactiveUI` (e.g., Dock integration),
  isolate it to the docking layer and do not reference it from app UI code.
  https://github.com/reactiveui/ReactiveUI.Avalonia

## 5) Input and interaction via Xaml.Behaviors (required)

Reference: https://github.com/wieslawsoltes/Xaml.Behaviors

- All UI input and events are handled via behaviors/triggers.
- Prefer source-generator-based behaviors/actions (no reflection) wherever available.
- Use trigger behaviors (property, data, loaded/unloaded, routed event) for
  lifecycles and state transitions.
- Code-behind must not contain event handlers or direct ViewModel calls.

## 6) Docking layout with Dock for Avalonia (required)

Reference: https://github.com/wieslawsoltes/Dock

- Use Dock.Model.* to represent the docking layout state.
- Use Dock.Avalonia for the view layer and Dock.Model.ReactiveUI for MVVM
  integration.
- Persist layout state to user settings and restore on startup.
- Keep layout logic in ViewModels; Views only render the Dock model.

## 7) Text editing with AvaloniaEdit (required)

Reference: https://github.com/AvaloniaUI/AvaloniaEdit

- Use AvaloniaEdit `TextEditor` for all code/text editing surfaces.
- Enable syntax highlighting using TextMate grammars/themes.
- Keep editor configuration in ViewModels (options, text, selection) and bind to
  the view.

## 8) Data presentation with ProDataGrid (required)

Reference: https://github.com/wieslawsoltes/ProDataGrid

- Use ProDataGrid `DataGrid` for all tabular data, tree views, and list displays.
- Always use the ProDataGrid model approach with code-based column bindings and
  fast paths.
- Always enable full filtering, searching, and sorting support.

## 9) Dependency Injection (Microsoft.Extensions.DependencyInjection)

- Configure services in a single composition root (App startup).
- Use `AddSingleton`, `AddScoped`, `AddTransient` correctly:
  - Singleton: thread-safe, shared, expensive-to-create services.
  - Scoped: per-document or per-operation services created within explicit scopes.
  - Transient: stateless lightweight services.
- Never resolve scoped services from singletons without creating a scope.
- Do not dispose services resolved from the container manually.

## 10) Performance (required)

- Prefer allocation-free APIs: `Span<T>`, `ReadOnlySpan<T>`, `Memory<T>`,
  `ValueTask`, `ArrayPool<T>`, and `System.Buffers`.
- Use SIMD (`System.Numerics.Vector<T>` or hardware intrinsics) where it provides
  measurable wins and keeps code maintainable.
- Avoid LINQ in hot paths; use loops and pre-sized collections.
- Minimize boxing, virtual dispatch in tight loops, and avoid unnecessary
  allocations in render/update loops.
- Profile before and after optimizations; document expected gains.

## 11) Reflection and source generation (required)

- Avoid reflection whenever possible.
- Prefer source generators (incremental generators required) before any reflection-based
  approach.
- If reflection is the only viable option, ask the user explicitly before introducing it.

## 12) Testing and validation

References:
- https://github.com/AvaloniaUI/Avalonia
- https://docs.avaloniaui.net/docs/concepts/headless/

- All production code must be covered by unit tests; xUnit is required for unit testing.
- UI tests must use Avalonia Headless (xUnit integration) and follow the headless testing
  guidance and helpers for input simulation.
- Unit-test ViewModels and Domain services.
- Use integration tests for parsing, IO, and docking layout persistence.
- UI tests should validate navigation flows, docking, and editor behaviors.

## 13) Code conventions

- No code-behind event handlers.
- Avoid static state (except truly immutable constants).
- Prefer explicit types where clarity is improved; avoid `var` in public APIs.
- All public APIs must be documented and unit-tested.
