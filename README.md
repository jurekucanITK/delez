# Delež

Delež je spletna aplikacija za beleženje in delitev skupnih stroškov v gospodinjstvih. Člani skupine vnašajo stroške in jih razdelijo med tiste, ki jih koristijo, aplikacija pa sproti izračuna, kdo je komu koliko dolžan, in predlaga čim manj nakazil za poravnavo dolgov. Uporabnik je lahko član več skupin (npr. "Stanovanje Maribor" in "Doma"), pri čemer so finance posameznih skupin ločene, pravice pa so odvisne od njegove vloge v skupini.

## Tehnologije

| Del | Tehnologije |
| --- | --- |
| Frontend | Vue 3, Vite, Vue Router, Pinia, Axios, Vuetify |
| Backend | C#, ASP.NET Core Web API (.NET 10), Entity Framework Core (Npgsql), Swagger UI |
| Podatkovna baza | PostgreSQL 18 |
| Avtentikacija | JWT, BCrypt.Net |
| Lokalni razvoj | Docker in Docker Compose, Node.js in npm, Visual Studio Code, Git in GitHub |

## Struktura repozitorija

```
├── frontend/         # Vue 3 SPA
├── backend/          # ASP.NET Core Web API
├── docs/             # projektna dokumentacija
├── docker-compose.yml
└── Delez.slnx
```

## Zagon

Potrebuješ [.NET 10 SDK](https://dotnet.microsoft.com/download), [Node.js](https://nodejs.org) 22 ali novejši in [Docker Desktop](https://www.docker.com/products/docker-desktop).

1. Kloniraj repozitorij:

   ```
   git clone https://github.com/jurekucanITK/delez.git
   cd delez
   ```

2. Zaženi bazo:

   ```
   cp .env.example .env
   docker compose up -d
   ```

3. Ustvari tabele in zaženi backend:

   ```
   dotnet tool restore
   dotnet ef database update --project backend
   dotnet run --project backend
   ```

4. V novem terminalu zaženi frontend:

   ```
   cd frontend
   npm install
   npm run dev
   ```

Aplikacija teče na http://localhost:5173, Swagger UI pa na http://localhost:5000/swagger. Nastavitve za lokalni razvoj so že v `backend/appsettings.Development.json`, zato dodatna konfiguracija ni potrebna.

Ob naslednjih zagonih zadostujejo `docker compose up -d`, `dotnet run --project backend` in `npm run dev`. Ko potegneš nove spremembe, ponovno poženi `dotnet ef database update --project backend` in `npm install`.

## Dokumentacija

1. [Zasnova rešitve](docs/01-zasnova-resitve.md)
2. [Funkcionalnosti in obseg](docs/02-funkcionalnosti-in-obseg.md)
3. [Podatkovni model](docs/03-podatkovni-model.md)
4. [Arhitektura in tehnološki sklad](docs/04-arhitektura-in-tehnoloski-sklad.md)
5. [Uporabniške zgodbe](docs/05-uporabniske-zgodbe.md)
6. [Wireframes](docs/06-wireframes.md)
7. [Arhitekturne odločitve (ADR)](docs/adr/)
