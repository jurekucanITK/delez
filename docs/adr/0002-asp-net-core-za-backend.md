# ASP.NET Core Web API za backend

## Kontekst in opis problema

Backend mora ponujati REST API, avtentikacijo in dostop do relacijske baze, jedro projekta pa je domenska logika delitve stroškov. Projekt razvija razvijalec, ki ima več izkušenj s C#, s frontend razvojem pa malo. S katerim jezikom in ogrodjem naj bo backend narejen, da bo čim več časa namenjenega logiki in ne učenju orodij?

## Obravnavane možnosti

* C# z ASP.NET Core Web API (.NET 10) in Entity Framework Core
* JavaScript/TypeScript z Node.js in Express

## Izid odločitve

Izbrana možnost: "C# z ASP.NET Core Web API (.NET 10) in Entity Framework Core", ker je C# jezik, ki ga razvijalec že dolgo uporablja, zato se lahko osredotoči na domensko logiko namesto na učenje novega jezika. ASP.NET Core je zrelo ogrodje z vgrajeno podporo za REST API, avtentikacijo z JWT in delo z bazo prek Entity Framework Core. Node.js bi sicer pomenil isti jezik na frontendu in backendu, vendar bi zahteval učenje novega ekosistema na obeh straneh hkrati.

### Posledice

* Dobro, ker je razvoj hitrejši in z manj napakami zaradi poznanega jezika in orodij.
* Dobro, ker Entity Framework Core omogoča ustvarjanje in spreminjanje sheme baze z migracijami.
* Dobro, ker je Swagger UI enostavno vključiti za pregled in testiranje API-ja.
* Slabo, ker frontend in backend uporabljata različna jezika, zato tipov in validacije ni mogoče deliti med njima.
