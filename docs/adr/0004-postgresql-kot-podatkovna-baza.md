# PostgreSQL kot podatkovna baza

## Kontekst in opis problema

Podatki so izrazito relacijski: stroški, deleži in nakazila so vezani na članstva, ta pa na uporabnike in skupine. Iz teh podatkov se računajo stanja članov, zato morajo ostati skladni (npr. nakazilo je lahko samo med člani iste skupine, vsota deležev mora ustrezati znesku stroška). Katero podatkovno bazo uporabiti?

## Obravnavane možnosti

* PostgreSQL
* Microsoft SQL Server
* MongoDB

## Izid odločitve

Izbrana možnost: "PostgreSQL", ker je popularna relacijska baza, je brezplačna in jo Entity Framework Core dobro podpira prek ponudnika Npgsql. MongoDB slabo ustreza podatkom z veliko povezavami, saj bi skladnost med dokumenti moral zagotavljati sam backend. SQL Server je sicer dobro povezan z .NET, a je brezplačna različica omejena, PostgreSQL pa za ta projekt ponuja vse potrebno brez omejitev.

### Posledice

* Dobro, ker se shema ustvari z migracijami Entity Framework Core.
* Slabo, ker je za lokalni razvoj potreben strežnik PostgreSQL.
