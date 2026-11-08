<script setup>
import { onMounted, ref } from 'vue'
import { pridobiJaz } from '@/api/avtentikacija'
import { pridobiSkupine } from '@/api/skupine'
import { barvaStanja, oblikujStanje } from '@/utils/stanje'

const imenaVlog = {
  skrbnik: 'Skrbnik',
  clan: 'Član',
  omejen: 'Omejen član',
}

const skupine = ref([])
const napaka = ref('')
const nalaganje = ref(true)

function opisStanja(stanje) {
  if (stanje > 0) return 'vam pripada'
  if (stanje < 0) return 'ste dolžni'
  return 'poravnano'
}

async function nalozi() {
  napaka.value = ''
  nalaganje.value = true
  try {
    const { data } = await pridobiSkupine()
    skupine.value = data
  } catch (e) {
    if (e.response?.status !== 401) {
      napaka.value = 'Skupin ni bilo mogoče naložiti. Preveri povezavo in poskusi znova.'
    }
  } finally {
    nalaganje.value = false
  }
}

onMounted(() => {
  pridobiJaz().catch(() => {})
  nalozi()
})
</script>

<template>
  <v-container class="py-12">
    <div class="d-flex flex-wrap align-center justify-space-between ga-4 mb-8">
      <h1 class="text-h5 font-weight-bold">Moje skupine</h1>
      <v-btn color="primary" prepend-icon="mdi-plus" :to="{ name: 'nova-skupina' }">
        Nova skupina
      </v-btn>
    </div>

    <v-progress-linear v-if="nalaganje" indeterminate color="primary" />

    <v-alert v-else-if="napaka" type="error">
      {{ napaka }}
    </v-alert>

    <v-card v-else-if="skupine.length === 0" class="mx-auto pa-8 text-center" max-width="440">
      <p class="text-body-1 mb-6">Še niste član nobene skupine</p>
      <v-btn variant="tonal" color="primary" :to="{ name: 'nova-skupina' }">
        Ustvari skupino
      </v-btn>
    </v-card>

    <v-row v-else>
      <v-col v-for="skupina in skupine" :key="skupina.id" cols="12" md="6">
        <v-card
          class="pa-6 h-100"
          min-height="200"
          :to="{ name: 'skupina', params: { id: skupina.id } }"
        >
          <div class="d-flex align-start justify-space-between ga-4">
            <h2 class="text-h6 font-weight-bold">{{ skupina.ime }}</h2>
            <span class="text-body-2 text-medium-emphasis">{{ imenaVlog[skupina.vloga] }}</span>
          </div>
          <p class="text-body-2 text-medium-emphasis mt-8">Moje stanje:</p>
          <p class="text-h4 font-weight-bold" :class="barvaStanja(skupina.stanje)">
            {{ oblikujStanje(skupina.stanje) }}
          </p>
          <p class="text-body-2 text-medium-emphasis mt-1">{{ opisStanja(skupina.stanje) }}</p>
        </v-card>
      </v-col>

      <v-col cols="12" md="6">
        <v-card
          class="nova-kartica d-flex align-center justify-center h-100 text-primary font-weight-bold"
          color="background"
          min-height="200"
          :to="{ name: 'nova-skupina' }"
        >
          + Ustvari novo skupino
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<style scoped>
.nova-kartica {
  border-style: dashed !important;
}
</style>
