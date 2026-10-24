# Docker samo za podatkovno bazo pri lokalnem razvoju

## Kontekst in opis problema

Za lokalni razvoj so potrebni PostgreSQL, backend in frontend. Bazo mora biti enostavno namestiti in ponastaviti, backend in frontend pa enostavno razhroščevati in hitro osveževati ob spremembah kode. Katere dele sistema naj med razvojem poganja Docker?

## Obravnavane možnosti

* Docker (Docker Compose) samo za PostgreSQL, backend in frontend tečeta neposredno
* Vsi trije deli v Docker zabojnikih
* Brez Dockerja, PostgreSQL nameščen neposredno na računalnik

## Izid odločitve

Izbrana možnost: "Docker (Docker Compose) samo za PostgreSQL, backend in frontend tečeta neposredno", ker je bazo v Dockerju najlažje namestiti in ponastaviti, backend in frontend pa je neposredno na računalniku preprosteje razhroščevati in hitreje osveževati ob spremembah kode. Če bi vse teklo v Dockerju, bi bilo osveževanje počasnejše, razhroščevanje pa zahtevnejše. Neposredna namestitev PostgreSQL bi otežila ponastavitev in čiščenje baze.

### Posledice

* Dobro, ker je bazo mogoče z enim ukazom ustvariti ali ponastaviti.
* Dobro, ker razhroščevanje backenda in osveževanje frontenda delujeta brez dodatnih nastavitev.
* Slabo, ker morata biti na računalniku nameščena .NET SDK in Node.js.
* Slabo, ker se lokalno okolje razlikuje od morebitnega produkcijskega okolja v zabojnikih.
