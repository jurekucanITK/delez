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

1. Zaženi bazo:

   ```
   cp .env.example .env
   docker compose up -d
   ```

2. Ustvari tabele in zaženi backend:

   ```
   dotnet tool restore
   dotnet ef database update --project backend
   dotnet run --project backend
   ```

3. V novem terminalu zaženi frontend:

   ```
   cd frontend
   npm install
   npm run dev
   ```

4. Odpri http://localhost:5173.

## Dokumentacija

1. [Zasnova rešitve](docs/01-zasnova-resitve.md)
2. [Funkcionalnosti in obseg](docs/02-funkcionalnosti-in-obseg.md)
3. [Podatkovni model](docs/03-podatkovni-model.md)
4. [Arhitektura in tehnološki sklad](docs/04-arhitektura-in-tehnoloski-sklad.md)
5. [Uporabniške zgodbe](docs/05-uporabniske-zgodbe.md)
6. [Wireframes](docs/06-wireframes.md)
7. [Arhitekturne odločitve (ADR)](docs/adr/)
