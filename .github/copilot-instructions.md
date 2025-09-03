## Maintenance

After any major structural change (e.g., new component, directory reorganization, significant workflow update), update this instructions file to reflect the new architecture and conventions. This ensures future AI agents remain productive and up-to-date.

# Copilot Instructions for Notist

## Overview

Notist is a multi-component solution with a WPF desktop application (`Notist`), a supporting system library (`Notist.System`), and a web frontend (`Notist.Frontend`). The architecture is modular, with clear separation between UI, connectors, models, and system logic.

## Key Components

-   **Notist/**: WPF app. Entry point is `App.xaml.cs`. UI logic in `OverlayWindow.xaml(.cs)`. Connectors in `Connectors/` handle communication between frontend, settings, and core logic.
-   **Notist.System/**: Core system logic, adapters, helpers, managers, and models. Used by the main app for backend operations.
-   **Notist.Frontend/**: Vue 3 + TypeScript + Vite SPA. Main entry is `src/main.ts`. Components in `src/components/`, styles in `src/styles/`, assets in `src/assets/`.

## Developer Workflows

-   **Build (WPF app):**
    -   Use Visual Studio or run `dotnet build Notist/Notist.csproj` from the repo root.
-   **Build (Frontend):**
    -   From `Notist.Frontend/`, run `npm install` then `npm run build`.
-   **Run Frontend Dev Server:**
    -   `npm run dev` in `Notist.Frontend/`.
-   **Debug (WPF):**
    -   Use Visual Studio debugger. Main window: `OverlayWindow.xaml.cs`.

## Patterns & Conventions

-   **Connectors** (`Notist/Connectors/`):
    -   Each connector encapsulates a boundary (e.g., frontend, settings, core logic). Use `ConnectorInvocationResult` for standardized responses.
-   **Models** (`Notist/Models/`, `Notist.System/Models/`):
    -   Data classes for app context and system state.
-   **Frontend**:
    -   Vue SFCs use `<script setup>`. TypeScript is enforced. Styles in SCSS.
-   **Global Utilities**:
    -   Prefer generic utility functions in global utility files (e.g., `src/lib/utils.ts`) rather than local inline functions. This promotes reusability and consistency across the codebase.

## Integration Points

-   **Frontend ↔ WPF Communication:**
    -   Managed via connectors. See `FrontendNegotiator.cs` and `NotistConnector.cs` for protocol.
-   **System Logic:**
    -   Shared via `Notist.System` library. Adapters and managers provide extensibility.

## External Dependencies

-   **Frontend:** Vue 3, Vite, TypeScript, SCSS.
-   **Backend:** .NET, WPF.

## Examples

-   To add a new connector, create a class in `Notist/Connectors/` and implement standardized invocation/result pattern.
-   To extend frontend, add a Vue SFC to `src/components/` and import in `src/main.ts`.

## References

-   WPF entry: `App.xaml.cs`, `OverlayWindow.xaml(.cs)`
-   Frontend entry: `src/main.ts`, `App.vue`
-   Connectors: `Notist/Connectors/`
-   System logic: `Notist.System/`

## Abstracted Project Tree

```
Notist.sln
project_structure.txt
Notist/                # WPF desktop app
  ├── App.xaml(.cs)    # Entry point
  ├── OverlayWindow.xaml(.cs) # Main UI window
  ├── Connectors/      # Frontend, settings, core logic boundaries
  ├── Models/          # App context/state models
  ├── Components/      # UI components (e.g., Tray)
  ├── Resources/       # App icons, frontend assets
  └── bin/, obj/       # Build artifacts (ignore)
Notist.System/         # Core system logic
  ├── Adapters/        # Extensible system adapters
  ├── DependencyInjection/
  ├── Helpers/
  ├── Managers/
  ├── Models/          # System-level models
  └── Settings/
Notist.Frontend/       # Vue 3 + TypeScript SPA
  ├── src/
  │   ├── main.ts      # SPA entry
  │   ├── App.vue      # Root component
  │   ├── components/  # Vue SFCs
  │   ├── styles/      # SCSS
  │   ├── assets/      # Static assets
  │   └── lib/, types/ # Shared logic/types
  ├── public/          # Static files
  ├── package.json     # NPM dependencies
  ├── vite.config.ts   # Vite config
  └── README.md
```

---

For questions or unclear conventions, ask for clarification or review related files before making changes.
