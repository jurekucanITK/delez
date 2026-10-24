# 05 – Uporabniške zgodbe

## US-01: Registracija

**Funkcionalnost:** Ustvarjanje skupine in dodajanje registriranih uporabnikov
**Vloga:** Uporabnik

**Zgodba:** Kot obiskovalec želim ustvariti uporabniški račun, da lahko začnem uporabljati aplikacijo in me drugi lahko dodajo v svoje skupine.

**Acceptance criteria:**

1. Obrazec za registracijo vsebuje polja za ime, e-poštni naslov in geslo. Vsa tri so obvezna.
2. Če e-poštni naslov že obstaja v sistemu, registracija ni uspešna in se prikaže sporočilo "Ta e-poštni naslov je že registriran".
3. Geslo mora imeti vsaj 8 znakov. Krajše geslo sistem zavrne s sporočilom, ki navede minimalno dolžino.
4. Po uspešni registraciji je uporabnik preusmerjen na prijavo s sporočilom o uspešni registraciji.

## US-02: Prijava

**Funkcionalnost:** Ustvarjanje skupine in dodajanje registriranih uporabnikov

**Vloga:** Uporabnik

**Zgodba:** Kot registriran uporabnik se želim prijaviti z e-poštnim naslovom in geslom, da lahko dostopam do svojih skupin in podatkov.

**Acceptance criteria:**

1. Ob pravilnem e-poštnem naslovu in geslu sistem vrne JWT žeton in uporabnika preusmeri na pregled skupin.
2. Ob napačnem e-poštnem naslovu ali geslu se prikaže enako sporočilo "Napačen e-poštni naslov ali geslo", ki ne razkrije, kateri podatek je napačen.
3. Zahtevek brez veljavnega žetona backend zavrne s statusom 401, uporabnik pa je preusmerjen na prijavo.
4. Po kliku na "Odjava" se žeton izbriše iz brskalnika in uporabnik je preusmerjen na prijavo.

## US-03: Ustvarjanje skupine

**Funkcionalnost:** Ustvarjanje skupine
**Vloga:** Uporabnik

**Zgodba:** Kot prijavljen uporabnik želim ustvariti novo skupino za svoje gospodinjstvo, da lahko z drugimi člani začnemo beležiti skupne stroške.

**Acceptance criteria:**

1. Obrazec za novo skupino vsebuje obvezno ime (največ 100 znakov) in neobvezen opis (največ 500 znakov).
2. Ustvarjalec skupine samodejno dobi članstvo z vlogo skrbnik.
3. Po uspešnem ustvarjanju se nova skupina takoj prikaže na pregledu skupin in uporabnik je preusmerjen na njeno stran.
4. Če je ime prazno, sistem skupine ne ustvari in ob polju prikaže napako "Ime skupine je obvezno".
5. Uporabnik lahko ustvari poljubno število skupin, imena skupin pa so lahko enaka, saj so skupine med seboj ločene.

## US-04: Dodajanje člana v skupino

**Funkcionalnost:** Dodajanje registriranih uporabnikov prek e-poštnega naslova z dodelitvijo vloge
**Vloga:** Skrbnik skupine

**Zgodba:** Kot skrbnik skupine želim dodati registriranega uporabnika prek njegovega e-poštnega naslova in mu dodeliti vlogo, da lahko sodeluje pri deljenju stroškov.

**Acceptance criteria:**

1. Obrazec za dodajanje člana vsebuje polje za e-poštni naslov in izbiro vloge (skrbnik, član, omejen član). Privzeta vloga je član.
2. Če uporabnik z vpisanim e-poštnim naslovom ni registriran, sistem člana ne doda in prikaže sporočilo "Uporabnik s tem naslovom ni registriran".
3. Če je uporabnik že član skupine, sistem ne ustvari drugega članstva in prikaže sporočilo "Uporabnik je že član skupine".
4. Po uspešnem dodajanju se novi član takoj pojavi na seznamu članov skupine z izbrano vlogo.
5. Gumb za dodajanje članov je viden samo skrbniku. Backend zahtevek člana ali omejenega člana zavrne s statusom 403.


## US-05: Vnos stroška

**Funkcionalnost:** Vnos stroška z zneskom, opisom, datumom in članom, ki ga je plačal
**Vloga:** Član

**Zgodba:** Kot član skupine želim vnesti strošek, ki sem ga plačal, da ostali člani vidijo, koliko mi dolgujejo.

**Acceptance criteria:**

1. Obrazec za strošek vsebuje opis (obvezen, največ 200 znakov), znesek (obvezen, večji od 0,00 €, največ dve decimalki), datum (obvezen, privzeto današnji) in plačnika.
2. Član lahko kot plačnika izbere samo sebe. Skrbnik lahko izbere katerega koli člana skupine.
3. Če znesek ni veljavno pozitivno število, sistem strošek zavrne in ob polju prikaže napako.
4. Po uspešnem shranjevanju se strošek pojavi na seznamu stroškov skupine, stanja članov pa se posodobijo brez ročnega osveževanja strani.
5. Omejen član gumba za vnos stroška ne vidi, backend pa njegov zahtevek za ustvarjanje stroška zavrne s statusom 403.

## US-06: Enakomerna delitev stroška

**Funkcionalnost:** Enakomerna delitev stroška med izbrane člane
**Vloga:** Član

**Zgodba:** Kot član skupine želim strošek enakomerno razdeliti med člane, ki ga koristijo, da vsak plača pošten del in tisti, ki stroška ne koristijo, ne nosijo ničesar.

**Acceptance criteria:**

1. Pri vnosu stroška so na voljo potrditvena polja za vse člane skupine. Privzeto so izbrani vsi.
2. Če izberem npr. 3 člane in je znesek 90,00 €, sistem vsakemu izbranemu članu določi delež 30,00 €.
3. Kadar se znesek ne deli brez ostanka (npr. 10,00 € med 3 člane), sistem ostanek centov prišteje enemu članu tako, da je vsota deležev natančno enaka znesku stroška (3,34 € + 3,33 € + 3,33 €).
4. Če ni izbran noben član, sistem strošek zavrne s sporočilom "Izberite vsaj enega člana".
5. Člani, ki niso izbrani, v bazi nimajo deleža za ta strošek.

## US-07: Delitev stroška po deležih

**Funkcionalnost:** Delitev stroška po deležih, na primer 60 % in 40 %
**Vloga:** Član

**Zgodba:** Kot član skupine želim strošek razdeliti po odstotkih, da lahko upoštevam, da nekateri člani strošek koristijo več kot drugi.

**Acceptance criteria:**

1. Pri vnosu stroška lahko preklopim med načinoma "Enakomerno" in "Po deležih". V načinu "Po deležih" se ob vsakem izbranem članu prikaže polje za odstotek.
2. Sistem sproti prikazuje vsoto vnesenih odstotkov in gumb za shranjevanje je onemogočen, dokler vsota ni natanko 100 %.
3. Sistem na podlagi odstotkov izračuna deleže v evrih (npr. 60 % od 50,00 € = 30,00 €), pri čemer je vsota deležev natančno enaka znesku stroška.
4. Odstotek mora biti večji od 0 in največ 100. Neveljaven vnos sistem zavrne z napako ob polju.
5. Pri urejanju stroška se način delitve in odstotki prikažejo tako, kot so bili shranjeni.

## US-08: Urejanje in brisanje stroška

**Funkcionalnost:** Vnos stroška (upravljanje stroškov)
**Vloga:** Član, skrbnik skupine

**Zgodba:** Kot član skupine želim popraviti ali izbrisati strošek, ki sem ga vnesel, da odpravim napake v vnosu.

**Acceptance criteria:**

1. Član vidi gumba "Uredi" in "Izbriši" samo pri stroških, ki jih je plačal sam. Skrbnik ju vidi pri vseh stroških v skupini.
2. Če član s klicem API-ja poskuša urediti ali izbrisati tuj strošek, backend vrne status 403 in strošek ostane nespremenjen.
3. Po spremembi zneska ali udeležencev sistem znova izračuna deleže, tako da je njihova vsota enaka novemu znesku.
4. Pred brisanjem sistem zahteva potrditev v pogovornem oknu. Po potrditvi se strošek in njegovi deleži izbrišejo.
5. Po urejanju ali brisanju se stanja članov in predlagana nakazila takoj posodobijo.

## US-09: Pregled stanja članov

**Funkcionalnost:** Sprotni izračun stanja vsakega člana
**Vloga:** Član

**Zgodba:** Kot član skupine želim videti, koliko je kdo dolžan oz. koliko mu pripada, da imamo vsi pregled nad dolgovi v gospodinjstvu.

**Acceptance criteria:**

1. Na strani skupine je prikazan seznam vseh članov s stanjem v evrih. Pozitivno stanje je označeno kot "mu pripada", negativno kot "je dolžan".
2. Stanje člana se izračuna po naslednji enačbi: vsota plačanih stroškov − vsota njegovih deležev + vsota poslanih nakazil − vsota prejetih nakazil.
3. Vsota stanj vseh članov v skupini je vedno enaka 0,00 €.
4. Po vnosu, urejanju ali brisanju stroška ali nakazila se stanja posodobijo brez ročnega osveževanja strani.
5. Član s poravnanim stanjem je prikazan z 0,00 € in oznako "poravnano".

## US-10: Predlog poravnave dolgov

**Funkcionalnost:** Predlog čim manjšega števila nakazil, s katerimi se poravnajo vsi dolgovi v skupini
**Vloga:** Član

**Zgodba:** Kot član skupine želim videti predlog nakazil, s katerimi se poravnajo vsi dolgovi, da dolgove poravnamo z najmanj plačili.

**Acceptance criteria:**

1. Na strani skupine je prikazan seznam predlaganih nakazil v obliki "A → B: znesek", urejen po znesku padajoče.
2. Če bi vsa predlagana nakazila bila izvedena, bi bilo stanje vseh članov 0,00 €.
3. Število predlaganih nakazil je največ n − 1, kjer je n število članov z neničelnim stanjem (npr. pri treh članih z neničelnim stanjem največ 2 nakazili).
4. Če so vsa stanja 0,00 €, se namesto seznama prikaže sporočilo "Vse je poravnano".
5. Ob vsakem predlaganem nakazilu je gumb "Zabeleži nakazilo", ki odpre obrazec za beleženje z že izpolnjenim plačnikom, prejemnikom in zneskom.

## US-11: Beleženje nakazila

**Funkcionalnost:** Beleženje nakazila, ki se takoj upošteva pri stanju obeh članov
**Vloga:** Član

**Zgodba:** Kot član skupine želim zabeležiti nakazilo, ki sem ga opravil drugemu članu, da se moj dolg zmanjša in drugi člani vidijo, da sem ga poravnal.

**Acceptance criteria:**

1. Obrazec za nakazilo vsebuje plačnika, prejemnika, znesek (večji od 0,00 €) in datum (privzeto današnji).
2. Član lahko kot plačnika izbere samo sebe, skrbnik pa katerega koli člana. Plačnik in prejemnik ne smeta biti ista oseba.
3. Po shranjevanju se nakazilo takoj upošteva pri stanju plačnika, ki se mu stanje poveča in prejemnika, ki se mu stanje zmanjša in se prikaže v zgodovini skupine.
4. Če je plačnik ali prejemnik izven skupine, backend zahtevek zavrne s statusom 400.
5. Omejen član nakazil ne more beležiti, vidi pa nakazila, ki jih mora opraviti.

## US-12: Pregled vseh skupin

**Funkcionalnost:** Skupni pregled vseh skupin uporabnika s stanjem v vsaki od njih
**Vloga:** Uporabnik

**Zgodba:** Kot uporabnik, ki je član več skupin, želim na enem mestu videti vse svoje skupine in svoje stanje v vsaki, da hitro vidim, kje sem dolžan oziroma kje mi dolgujejo.

**Acceptance criteria:**

1. Po prijavi se prikaže seznam vseh skupin, v katerih je uporabnik član. Za vsako skupino so prikazani ime, uporabnikova vloga in njegovo stanje v evrih.
2. Stanja različnih skupin se med seboj ne seštevajo, vsaka skupina ima svoje stanje.
3. Klik na skupino odpre njeno stran.
4. Uporabnik brez skupin vidi sporočilo "Še niste član nobene skupine" in gumb "Ustvari skupino".
5. Seznam ne vsebuje skupin, v katerih uporabnik ni član.

## US-13: Pogled omejenega člana

**Funkcionalnost:** Prilagojen pogled za omejenega člana, ki prikazuje samo stroške z njegovim deležem
**Vloga:** Omejen član

**Zgodba:** Kot omejen član želim videti samo stroške, pri katerih imam delež, in svoje stanje, da vem, koliko moram prispevati, ne da bi videl finance ostalih.

**Acceptance criteria:**

1. Na strani skupine omejen član vidi samo stroške, v katerih ima delež, in pri vsakem svoj delež v evrih.
2. Omejen član vidi samo svoje stanje, stanj drugih članov in stroškov brez njegovega deleža ne vidi.
3. Omejen član vidi seznam nakazil, ki jih mora opraviti, z zneskom in prejemnikom.
4. Gumbov za vnos stroška in beleženje nakazila omejen član ne vidi.
5. Če omejen član s klicem API-ja zahteva podatke o strošku, v katerem nima deleža, backend vrne status 403 ali 404.

## US-14: Zgodovina s filtriranjem

**Funkcionalnost:** Zgodovina stroškov in nakazil s filtriranjem po obdobju in članu
**Vloga:** Član

**Zgodba:** Kot član skupine želim pregledati zgodovino stroškov in nakazil ter jo filtrirati po obdobju in članu, da lahko preverim, kaj se je zgodilo v določenem času ali s posameznim članom.

**Acceptance criteria:**

1. Zgodovina prikazuje stroške in nakazila skupine v enem seznamu, urejenem po datumu padajoče. Vsak vnos vsebuje datum, opis ali vrsto, znesek in udeležene člane.
2. Filter po obdobju sprejme začetni in končni datum. Prikazani so samo vnosi, katerih datum je znotraj izbranega obdobja (vključno z mejnima dnevoma).
3. Filter po članu prikaže samo vnose, kjer je izbrani član plačnik, prejemnik ali ima delež.
4. Filtra je mogoče kombinirati. gumb "Počisti filtre" ponastavi prikaz na vse vnose.
5. Če noben vnos ne ustreza filtrom, se prikaže sporočilo "Ni zadetkov za izbrane filtre".

## US-15: Mesečni povzetek

**Funkcionalnost:** Mesečni povzetek, ki prikaže, koliko je skupina in posamezen član porabil v izbranem mesecu
**Vloga:** Član

**Zgodba:** Kot član skupine želim videti mesečni povzetek porabe, da vem, koliko smo kot skupina in koliko sem jaz porabil v izbranem mesecu.

**Acceptance criteria:**

1. Uporabnik izbere mesec in leto. Privzeto je izbran tekoči mesec.
2. Povzetek prikaže skupno porabo skupine v mesecu, to je vsoto zneskov vseh stroškov z datumom v izbranem mesecu.
3. Povzetek za vsakega člana prikaže vsoto plačanih stroškov in vsoto njegovih deležev v izbranem mesecu. Omejen član vidi samo svoje deleže.
4. Nakazila se v porabo ne štejejo, ker so poravnava dolgov in ne izdatek.
5. Če v izbranem mesecu ni stroškov, se prikaže sporočilo "V tem mesecu ni zabeleženih stroškov" in vsi zneski so 0,00 €.