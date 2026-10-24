# Lastna avtentikacija z JWT in BCrypt

## Kontekst in opis problema

Uporabniki se registrirajo z imenom, e-poštnim naslovom in geslom. Ker sta frontend in backend ločena, mora backend uporabnika prepoznati pri vsakem zahtevku. Vloge niso vezane na uporabnika, ampak na članstvo v skupini, podatkovni model pa ima preprosto tabelo "uporabnik". Kako izvesti avtentikacijo, da bo varna in se bo ujemala s podatkovnim modelom?

## Obravnavane možnosti

* Lastna tabela "uporabnik" z BCrypt zgoščenimi gesli in JWT žetoni
* ASP.NET Core Identity
* Zunanji ponudnik identitete (npr. Auth0, Keycloak)

## Izid odločitve

Izbrana možnost: "Lastna tabela "uporabnik" z BCrypt zgoščenimi gesli in JWT žetoni", ker se ujema s preprostim podatkovnim modelom in je za potrebe projekta dovolj varna. ASP.NET Core Identity ustvari vrsto lastnih tabel, ki se ne ujemajo s podatkovnim modelom, njegov sistem vlog pa je vezan na uporabnika in ne na članstvo v skupini, zato bi ga bilo treba vseeno obiti. Zunanji ponudnik bi prinesel dodatno storitev in nastavitve, ki bi projekt te velikosti po nepotrebi zakomplicirale. JWT žetoni so primerni za ločen frontend, saj strežniku ni treba hraniti sej.

### Posledice

* Dobro, ker podatkovni model ostane preprost in pod popolnim nadzorom.
* Dobro, ker je avtorizacijo po vlogah v skupini mogoče izvesti neposredno nad tabelo "clanstvo".
* Dobro, ker so gesla zaradi BCrypt zgoščevanja varno shranjena.
* Slabo, ker je treba funkcionalnosti, kot sta ponastavitev gesla ali potrditev e-pošte, po potrebi implementirati sami.
* Slabo, ker izdanega JWT žetona ni mogoče preklicati pred iztekom njegove veljavnosti.
