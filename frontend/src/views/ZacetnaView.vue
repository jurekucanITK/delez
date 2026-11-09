<script setup>
const zmoznosti = [
  {
    ikona: 'mdi-receipt-text-outline',
    naslov: 'Deliš stroške',
    opis: 'Vneseš račun in izbereš, kdo ga koristi. Delež ga razdeli na cent natančno.',
  },
  {
    ikona: 'mdi-scale-balance',
    naslov: 'Vidiš stanje',
    opis: 'Za vsakega člana je sproti jasno, koliko je dolžan ali koliko mu pripada.',
  },
  {
    ikona: 'mdi-swap-horizontal',
    naslov: 'Poravnaš dolgove',
    opis: 'Delež predlaga čim manj nakazil, s katerimi se vsi dolgovi poravnajo.',
  },
]
</script>

<template>
  <v-container class="py-12 py-md-16">
    <section class="uvod">
      <h1 class="text-headline-large text-md-display-small font-weight-bold mb-6">
        Skupni stroški gospodinjstva, pošteno razdeljeni
      </h1>
      <p class="text-body-large text-medium-emphasis mb-8">
        Delež je za vse, ki si delijo stanovanje ali dom. Beležite skupne račune, aplikacija pa
        izračuna, kdo je komu dolžan.
      </p>
      <div class="d-flex flex-wrap align-center ga-4">
        <v-btn color="primary" size="large" :to="{ name: 'registracija' }">Ustvari račun</v-btn>
        <span class="text-body-medium">
          Že imaš račun?
          <router-link :to="{ name: 'prijava' }" class="text-primary font-weight-bold">
            Prijavi se
          </router-link>
        </span>
      </div>
    </section>

    <v-divider class="my-12 my-md-16" />

    <v-row tag="ul" class="zmoznosti">
      <v-col v-for="zmoznost in zmoznosti" :key="zmoznost.naslov" tag="li" cols="12" md="4">
        <div class="mb-4">
          <v-avatar color="primary" variant="tonal" size="48">
            <v-icon :icon="zmoznost.ikona" />
          </v-avatar>
        </div>
        <h2 class="text-title-large font-weight-bold mb-2">{{ zmoznost.naslov }}</h2>
        <p class="text-body-large text-medium-emphasis">{{ zmoznost.opis }}</p>
      </v-col>
    </v-row>
  </v-container>
</template>

<style scoped>
.uvod {
  max-width: 40rem;
}

.zmoznosti {
  list-style: none;
  padding-left: 0;
}
</style>
