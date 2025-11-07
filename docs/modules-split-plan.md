# Core2D Modules Split Plan

## Current Modules and Dependencies

| Module | Core2D dependencies | External packages | Depends on other modules |
| --- | --- | --- | --- |
| FileSystem | Model | – | – |
| FileWriter | Model, ViewModels | SkiaSharp | Renderer, SvgExporter, XamlExporter |
| Log | Model | – | – |
| Renderer | Model, ViewModels, Spatial | Avalonia, SkiaSharp (and NativeAssets), PdfSharp, Svg, netDxf, ACadSharp (with CSMath/CSUtilities), UglyToad.PdfPig.Rendering.Skia | – |
| ScriptRunner | Model, ViewModels | Microsoft.CodeAnalysis.CSharp.Scripting | – |
| Serializer | Model | Autofac, Newtonsoft.Json | – |
| ServiceProvider | – | Autofac | – |
| SvgExporter | Model, ViewModels | – | – |
| TextFieldReader | Model, ViewModels | CsvHelper, DocumentFormat.OpenXml | – |
| TextFieldWriter | Model, ViewModels | CsvHelper, DocumentFormat.OpenXml | – |
| XamlExporter | Model, ViewModels | – | – |

## Split Strategy

1. Create shared class libraries so modules can reference their current dependencies without circular references.  
   - `Core2D.Spatial` includes `src/Core2D/Spatial/**/*`.  
   - `Core2D.ViewModels` bundles both `src/Core2D/Model/**/*` and `src/Core2D/ViewModels/**/*` (the two are tightly coupled) and references `Core2D.Spatial`.
2. For every directory in `src/Core2D/Modules/*`, create a dedicated `Core2D.Modules.<Name>.csproj` class library. Each project will include its folder, reference the shared projects above, and specify required package dependencies. Cross-module references (e.g., `FileWriter` → `Renderer`, `SvgExporter`, `XamlExporter`) will be expressed via `ProjectReference`.
3. Update `Core2D/Core2D.csproj` to remove the module folders from its compile items and instead reference the newly created projects. Move package references that are now module-specific into the appropriate project files.
4. Add all new projects to `Core2D.sln` and ensure `Core2D` references them so the build graph matches the legacy monolith behavior.
5. Run `dotnet build Core2D.sln` to confirm that the refactoring preserves functionality.

## Numbered Tasks

1. Generate the new shared projects (`Core2D.Spatial`, combined `Core2D.ViewModels` + Model), include the correct sources, and add initial project references.
2. For each folder in `src/Core2D/Modules`, author `Core2D.Modules.<Folder>.csproj` with folder-specific compile includes, package references, and inter-module `ProjectReference`s.
3. Remove module sources from `Core2D/Core2D.csproj`, add references to every new module project, and relocate package references that now live in shared/module projects.
4. Register all new projects inside `Core2D.sln`.
5. Build the solution to verify the modular split and fix any compilation fallout.
