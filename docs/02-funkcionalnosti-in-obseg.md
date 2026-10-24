# 02 – Funkcionalnosti in obseg

## Uporabniške vloge

Vloge se v Deležu dodeljujejo po skupinah. Ista oseba je lahko v eni skupini skrbnik, v drugi pa navaden ali omejen član.

### Skrbnik skupine

Skrbnik je uporabnik, ki je skupino ustvaril ali mu je bila ta vloga dodeljena. Člane dodaja in odstranjuje, jim dodeljuje vloge ter lahko vnaša, ureja ali briše vse stroške v skupini, tudi v imenu drugih članov. Nima dostopa do skupin, v katerih ni član, in ne more spreminjati uporabniških računov drugih uporabnikov.

### Član

Član vnaša stroške, ki jih je plačal, vidi vse stroške in stanje vseh članov v skupini ter beleži svoja nakazila. Ureja in briše lahko samo stroške, ki jih je plačal sam. Ne more dodajati ali odstranjevati članov in ne more spreminjati vlog.

### Omejen član

Omejen član je namenjen npr. mlajšim družinskim članom ali začasnim sostanovalcem. Vidi samo stroške, v katerih ima delež, svoje stanje in nakazila, ki jih mora opraviti. Ne more vnašati stroškov in ne vidi ostalih stroškov ter stanja drugih članov.

## Predvidene funkcionalnosti

- **[MVP]** Ustvarjanje skupine in dodajanje registriranih uporabnikov prek e-poštnega naslova z dodelitvijo vloge (skrbnik, član, omejen član).
- **[MVP]** Vnos stroška z zneskom, opisom, datumom in članom, ki ga je plačal.
- **[MVP]** **[DS]** Enakomerna delitev stroška med izbrane člane, t.j. med vse ali samo med tiste, ki strošek koristijo.
- **[MVP]** **[DS]** Sprotni izračun stanja vsakega člana, torej koliko je dolžan oz. koliko mu pripada.
- **[MVP]** **[DS]** Predlog čim manjšega števila nakazil, s katerimi se poravnajo vsi dolgovi v skupini.
- **[MVP]** **[DS]** Beleženje nakazila, ki se takoj upošteva pri stanju obeh članov.
- **[DS]** Delitev stroška po deležih, na primer 60 % in 40 %.
- Skupni pregled vseh skupin uporabnika s stanjem v vsaki od njih.
- **[DS]** Prilagojen pogled za omejenega člana, ki prikazuje samo stroške z njegovim deležem.
- Zgodovina stroškov in nakazil s filtriranjem po obdobju in članu.
- Ob novem strošku ali neporavnanem dolgu uporabnik dobi sporočilo ma e-poštni naslov, prav tako dobi na e-naslov tudi mesečna poročila,
- **[DS]** Mesečni povzetek, ki prikaže, koliko je skupina in posamezen član porabil v izbranem mesecu.

## Kasnejše nadgradnje

- **Potrjevanje nakazil** - prejemnik potrdi, da je denar res prejel, preden se nakazilo upošteva,
- **Ponavljajoči stroški** - stroški, kot sta najemnina ali internet, se vsak mesec samodejno ustvarijo in razdelijo,
- **Kategorije stroškov** - razvrščanje stroškov in pregled porabe po kategorijah,
- **Grafični prikaz porabe** - prikaz porabe skozi čas v obliki grafov,
- **Izvoz obračuna** - izvoz obračunov v PDF ali CSV,
- **Podpora za več valut** - podpora za druge valute, npr. dolar, funt itd.,
- **Mobilni prikaz** - prikaz na mobilni napravi v obliki namestljive spletne aplikacije (PWA).

## Izven obsega

- Aplikacija ne izvaja dejanskih plačil in ni povezana s plačilnimi sistemi, kot sta spletna banka ali PayPal.
- Ni samodejnega uvoza transakcij iz bančnih računov.
- Ni namenjena splošnemu upravljanju osebnih financ, kot so varčevanje, investicije ali osebni proračun.
- Ne vodi računovodstva in ne pripravlja davčnih ali uradnih poročil.
- Ni javnih skupin. Skupine so zasebne in dostopne samo dodanim članom.