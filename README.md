# GameStore: Comprehensive Full-Stack Project Breakdown

This document provides a highly detailed, granular breakdown of the entire Full Stack GameStore application. It covers perfectly how the ASP.NET Minimal API backend was built line-by-line, how the database structures work, and how the React frontend seamlessly fetches and renders that data.

## 1. Project Architecture & File Structure

The project directory (`/gamestore/`) is split into two major decoupled ecosystems:
- **`GameStore.API`**: The Backend Application (C# / .NET 10 / ASP.NET Minimal API).
- **`client`**: The Frontend Application (TypeScript / React 19 / Vite).

---

## 2. The Backend: ASP.NET Core (.NET 10)

### Initialization & Configuration (`Program.cs`)
This file is the engine of the backend API.
- **`var builder = WebApplication.CreateBuilder(args);`**: Bootstraps the DI (Dependency Injection) container and logging providers.
- **`DotNetEnv.Env.Load();`**: We installed the `DotNetEnv` NuGet package. This line seeks out the `.env` file at the root of the project to securely load the hidden `DATABASE_URL` string into memory.
- **`builder.Services.AddCors(...)`**: Because the React App runs on port `5173` and the ASP.NET app runs on `5270`, browsers block data sharing due to Cross-Origin Resource Sharing (CORS) security rules. This block explicitly allows `http://localhost:5173` to hit our server.
- **`builder.Services.AddDbContext<GameStoreContext>(...)`**: Connects Entity Framework Core to SQL Server using the connection string we extracted.
- **`var app = builder.Build();`**: Finalizes the configuration and builds the HTTP Request Pipeline.
- **`app.UseCors();`**: Activates the CORS middleware we configured above.
- **`app.MapGamesEndpoints();`**: We created custom extension methods to keep `Program.cs` clean; this registers all the `/games` URLs.
- **`context.Database.Migrate();`**: Inside a scoped service block, this command tells EF Core to deploy the SQL tables and seed data automatically the moment you type `dotnet run`. It creates the database if it doesn't safely exist.

### Database Logic (Entity Framework Core)

#### 1. Models (`Models/Game.cs`, `Models/Genre.cs`)
These are standard C# classes that EF Core uses as "blueprints" to build SQL Tables.
- **`Genre.cs`**: Contains `Id` (Primary Key) and `Name`.
- **`Game.cs`**: Models the game data. We establish a **One-to-Many Relationship** by defining:
  - `public int GenreId { get; set; }` (The Foreign Key mapper).
  - `public Genre? Genre { get; set; }` (The EF Core Navigation property).

#### 2. The DbContext (`Data/GameStoreContext.cs`)
We inherit from `DbContext` to declare our actual endpoints:
- `public DbSet<Game> Games => Set<Game>();` translates to the `Games` table.
- **Data Seeding**: Using `protected override void OnModelCreating(ModelBuilder modelBuilder)`, we explicitly push 5 default Genres into the system via `.HasData(...)`. When EF creates the initial migration, this generates native `INSERT INTO Genres...` SQL code.

### DTOs (Data Transfer Objects)
Located in `/DTOs/`, these are lightweight `record class` objects used strictly for client transit.
- We use DTOs like `CreateGameDto` and `UpdateGameDto` instead of native models for security. It prevents *Over-Posting* attacks (e.g. stopping a user from submitting a custom `Id` and overwriting another record).
- **Validation**: They are adorned with `[Required]`, `[StringLength(100)]`, and `[Range]`. ASP.NET Minimal APIs inherently block bad requests without us writing manual verification boilerplate.

### Mapping (`Mapping/GameMapping.cs`)
We created specific Extension Methods (`ToDto()` and `ToEntity()`). 
- **`ToDto()`**: Flattens a loaded `Game` (pulling the relational `Genre.Name` sub-property) strictly into a `GameDto` JSON string.
- **`ToEntity()`**: Accepts a newly submitted `CreateGameDto`, transcribing the parameters into a fresh `Game` model that can be dumped securely into `context.Games.Add()`.

### API Endpoints (`Endpoints/GameEndpoints.cs`)
We mapped our logic utilizing the sleek `app.MapGroup("/games")`.
- **GET (`MapGet("/")`)**: Dispatches `await context.Games.Include(g => g.Genre).AsNoTracking().ToListAsync()`. `Include()` instructs EF Core to execute a SQL `JOIN`. `AsNoTracking()` disables memory tracking making the API lightning fast for reading.
- **POST (`MapPost("/")`)**: Takes in the `.ToEntity()` transformed model, writes it to `context.Games.Add()`, and natively commits it to SQL via `await context.SaveChangesAsync()`.
- **PUT (`MapPut("/{id}")`)**: Locates the entity via `FindAsync(id)`, modifies the properties matching the user's `UpdateGameDto`, and saves it. EF automatically tracks these variables and fires standard `UPDATE` statements.
- **DELETE (`MapDelete("/{id}")`)**: Utilizes `.Where(g => g.Id == id).ExecuteDeleteAsync()`. This bypasses loading the entire game into memory and fires an immediate `DELETE FROM Games WHERE Id = @id` to SQL Server iteratively.

---

## 3. The Frontend: React, Vite, TypeScript

### Boilerplate & Setup
We used `bun create vite` to scaffold the `/client` directory. This yields a blazing fast dev server out-of-the-box leveraging TypeScript's rich validation. We heavily cleared out the default template to set up our own architecture.

### API Integration (`src/services/api.ts`)
Instead of duplicating Web Requests throughout React Components, we built an API wrapper utilizing the native Web browser `fetch()` API.
- We declared `interface CreateGameDto` to perfectly mirror our backend DTO structure in TypeScript.
- Functions like `createGame(game)` wrap the `POST` method:
  ```typescript
  export const createGame = async (game: CreateGameDto) => {
      const res = await fetch(`http://localhost:5270/games`, {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(game),
      });
      return res.json();
  };
  ```

### Routing (`src/App.tsx` & `main.tsx`)
We imported `BrowserRouter` from `react-router-dom`. 
- By wrapping the application in `<Router>`, React intercepts normal HTML page loads. 
- Using `<Routes>`, we intercept URLs natively and switch out components instantly (Single Page App logic).
- `<Route path="/" element={<GameList />} />` paints the catalog window. 
- `<Route path="/edit/:id" element={<GameForm />} />` paints the form editor natively.

### Components & State Management

**`GameList.tsx` (Read & Delete)**
- Initiates state via `const [games, setGames] = useState<Game[]>([])`.
- Inside `useEffect`, `loadGames()` fires instantly on mount, populating the component's state utilizing our `api.ts` fetcher. React immediately detects the state change and repaints the DOM tree with `<div className="card">` logic populated by `{games.map(game => ...)}`.
- For deletions, checking `confirm()` ensures intentional UI clicks. `deleteGame(id)` resolves the backend Promise, and `setGames(games.filter((g) => g.id !== id))` natively strips the game from the screen organically.

**`GameForm.tsx` (Create & Update)**
- Through `useParams()`, the component dynamically acts as *either* a new Creator, *or* an existing Editor tracking the `id`. 
- Upon mounting `useEffect()`, the file immediately retrieves `fetchGenres()` yielding our DB-seeded genres directly into an Array dynamically bound to an HTML `<select>`.
- The `formData` object is tied (via Controlled Inputs utilizing `onChange={handleChange}`) to our inputs seamlessly. 
- During `handleSubmit(e)`, it calls either `createGame` or `updateGame`. If it passes, the `useNavigate()` hook forcefully redirects the user magically back to `/`, painting the newly updated Catalog instantly.

### Stylish User Interface (`src/index.css`)
To keep the application profoundly clean, unbloated, and realistically human-authored, we scrapped heavier frameworks in favor of robust, simple standard web `.css`.
We implemented intuitive flexbox spacing (`display: flex`, `gap`), predictable standard colors, and straightforward rounded borders to deliver a classic, intuitive control panel application.