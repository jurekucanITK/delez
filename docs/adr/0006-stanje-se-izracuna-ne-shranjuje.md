# Stanje člana se izračuna, ne shranjuje

## Kontekst in opis problema

Za vsakega člana skupine je treba prikazati, koliko je dolžan oz. koliko mu pripada. Stanje je odvisno od stroškov, ki jih je plačal, njegovih deležev in nakazil, ki jih je poslal ali prejel. Stroške je mogoče naknadno urejati in brisati. Ali naj se stanje shranjuje v bazi ali sproti izračuna iz obstoječih podatkov?

## Obravnavane možnosti

* Sprotni izračun stanja ob vsakem zahtevku
* Shranjeno stanje, ki se posodobi ob vsaki spremembi stroškov ali nakazil

## Izid odločitve

Izbrana možnost: "Sprotni izračun stanja ob vsakem zahtevku", ker se tako stanje nikoli ne more razhajati z dejanskimi stroški in nakazili. Pri shranjenem stanju bi moral backend ob vsakem vnosu, urejanju ali brisanju pravilno popraviti stanja vseh vpletenih članov, vsaka napaka pa bi povzročila trajno napačno stanje. Gospodinjstva imajo malo članov in stroškov, zato je izračun hiter in shranjevanje zaradi zmogljivosti ni potrebno.

### Posledice

* Dobro, ker je stanje vedno skladno s podatki.
* Dobro, ker urejanje in brisanje stroškov ne zahteva dodatne logike za popravljanje stanj.
* Slabo, ker je pri vsakem zahtevku treba prebrati vse stroške, deleže in nakazila skupine.
* Slabo, ker bi bilo treba pri zelo velikih skupinah ali dolgi zgodovini odločitev ponovno pretehtati.
