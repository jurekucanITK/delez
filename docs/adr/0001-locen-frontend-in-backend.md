# Ločen frontend in backend, povezana prek REST API-ja

## Kontekst in opis problema

Delež potrebuje spletni uporabniški vmesnik ter strežniško logiko, ki izračuna deleže, stanja članov in predlagana nakazila ter preverja pravice po skupinah. Ali naj bo aplikacija ena celota, kjer strežnik generira strani, ali dve ločeni aplikaciji, ki komunicirata prek API-ja?

## Obravnavane možnosti

* Ločena enostranska aplikacija (SPA) in REST API
* Monolitna aplikacija s strežniško generiranimi stranmi (ASP.NET Core MVC / Razor Pages)

## Izid odločitve

Izbrana možnost: "Ločena enostranska aplikacija in REST API", ker jasno loči prikaz od domenske logike. Vsa logika (izračun deležev, stanj, preverjanje pravic) je na enem mestu v backendu, frontend pa izračune samo prikazuje. API je mogoče razvijati in testirati neodvisno od vmesnika, tega pa kasneje po potrebi tudi zamenjati, ne da bi ob tem spreminjali backend. Pri strežniško generiranih straneh bi bila prikaz in logika tesno povezana, kar bi oteževalo menjavo vmesnika in samostojno testiranje logike.

### Posledice

* Dobro, ker je frontend mogoče kasneje zamenjati (npr. z Reactom) brez sprememb backenda.
* Dobro, ker je API mogoče testirati neodvisno od frontenda.
* Dobro, ker je domenska logika na enem mestu in je frontend ne podvaja.
* Slabo, ker je treba vzdrževati dva projekta z ločenima orodjema za gradnjo in zagon.
* Slabo, ker zahteva avtentikacijo prek žetonov in nastavitev CORS.
