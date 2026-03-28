# GameStore React Front-End

A modern, simple client application crafted in React, Vite, and TypeScript. This repository handles the front-end user experience for managing the GameStore architecture.

## How It Works

The GameStore application is comprised of two discrete layers working together:

1. **The ASP.NET Core Backend (`GameStore.API`)**:
   - Built via Minimal APIs pattern, serving lightweight HTTP endpoints.
   - Connects to a SQL Server database localized using Entity Framework Core.
   - Database schemas and initial pre-seeded `Genres` (like "Fighting", "Roleplaying") are deployed automatically when the API starts via `context.Database.Migrate()`.
   - Manages connection secrets dynamically via a `.env` file wrapper using `DotNetEnv`.

2. **The React Frontend (`client`)**:
   - Engineered via `bun create vite`, resulting in an ultra-fast build process alongside TypeScript type safety.
   - Handles views using component-based structural patterns (`GameList`, `GameForm`, `Navigation`) to easily render states seamlessly via standard Vanilla React Hooks (`useState`, `useEffect`).
   - Relies on `react-router-dom` for Single Page Application navigation, ensuring fluid, reload-free transitions.
   - Communicates with the Backend entirely contained in the `src/services/api.ts` file via the native `fetch` Web API.

### Workflow
When a user visits the client:
- The React App accesses `http://localhost:5270/games` where it is allowed through ASP.NET's CORS (`Cross-Origin Resource Sharing`) mapping.
- The user can Read, Add, Edit, or Delete games using the lightweight DTOs structure bridging the client and ASP.NET.
- A minimalistic CSS architecture (`index.css`) renders the layout in a clean, human-readable UI without heavy reliance on bulky styling frameworks.

## Getting Started

1. **Launch the API Layer:**
   Ensure SQL Server is running, open a terminal in the `GameStore.API` folder, and type:
   ```bash
   dotnet run
   ```

2. **Launch the Client:**
   Open a separate terminal in the `client` directory, and start the Vite dev server with Bun:
   ```bash
   bun i
   bun run dev
   ```
   Navigate to `http://localhost:5173` to see the GameStore.
