<script setup>
import { storeToRefs } from 'pinia'
import { useRouter } from 'vue-router'
import { useAvtentikacijaStore } from '@/stores/avtentikacija'

const router = useRouter()
const avtentikacija = useAvtentikacijaStore()
const { jePrijavljen } = storeToRefs(avtentikacija)

function odjava() {
  avtentikacija.odjava()
  router.push({ name: 'prijava' })
}
</script>

<template>
  <v-app>
    <v-app-bar>
      <v-app-bar-title>Delež</v-app-bar-title>
      <v-spacer />
      <v-btn v-if="jePrijavljen" variant="text" class="me-2" @click="odjava">Odjava</v-btn>
    </v-app-bar>

    <v-main>
      <router-view />
    </v-main>
  </v-app>
</template>
