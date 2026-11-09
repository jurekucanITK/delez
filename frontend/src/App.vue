<script setup>
import { storeToRefs } from 'pinia'
import { useRoute, useRouter } from 'vue-router'
import { useAvtentikacijaStore } from '@/stores/avtentikacija'

const route = useRoute()
const router = useRouter()
const avtentikacija = useAvtentikacijaStore()
const { jePrijavljen, uporabnik } = storeToRefs(avtentikacija)

function odjava() {
  avtentikacija.odjava()
  router.push({ name: 'prijava' })
}
</script>

<template>
  <v-app>
    <v-app-bar>
      <v-container class="d-flex align-center py-0">
        <router-link
          :to="{ name: jePrijavljen ? 'skupine' : 'zacetna' }"
          class="znak d-flex align-center ga-2 text-decoration-none"
          aria-label="Delež, na začetek"
        >
          <v-icon icon="mdi-chart-pie" />
          <span class="text-title-large font-weight-bold">Delež</span>
        </router-link>

        <v-spacer />

        <template v-if="jePrijavljen">
          <div class="d-none d-sm-block">
            <v-btn variant="text" :to="{ name: 'skupine' }">Moje skupine</v-btn>
          </div>
          <span v-if="uporabnik" class="text-body-medium mx-3 d-none d-md-inline">{{ uporabnik.ime }}</span>
          <v-btn variant="text" @click="odjava">Odjava</v-btn>
        </template>

        <template v-else>
          <v-btn v-if="route.name !== 'prijava'" variant="text" :to="{ name: 'prijava' }">
            Prijavi se
          </v-btn>
          <div v-if="route.name !== 'registracija'" class="ms-2 d-none d-sm-block">
            <v-btn variant="flat" color="surface" :to="{ name: 'registracija' }">Ustvari račun</v-btn>
          </div>
        </template>
      </v-container>
    </v-app-bar>

    <v-main>
      <router-view />
    </v-main>
  </v-app>
</template>

<style scoped>
.znak {
  color: inherit;
}
</style>
