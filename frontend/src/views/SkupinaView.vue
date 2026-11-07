<script setup>
import { ref, watch } from 'vue'
import { useRoute } from 'vue-router'
import { pridobiSkupino } from '@/api/skupine'

const route = useRoute()

const skupina = ref(null)
const napaka = ref('')
const nalaganje = ref(false)

async function nalozi(id) {
  skupina.value = null
  napaka.value = ''
  nalaganje.value = true
  try {
    const { data } = await pridobiSkupino(id)
    skupina.value = data
  } catch (e) {
    const status = e.response?.status
    if (status === 404) {
      napaka.value = 'Skupina ne obstaja.'
    } else if (status === 403) {
      napaka.value = 'Nisi član te skupine.'
    } else if (status !== 401) {
      napaka.value = 'Skupine ni bilo mogoče naložiti. Preveri povezavo in poskusi znova.'
    }
  } finally {
    nalaganje.value = false
  }
}

watch(() => route.params.id, nalozi, { immediate: true })
</script>

<template>
  <v-container class="py-12">
    <v-progress-linear v-if="nalaganje" indeterminate color="primary" />

    <v-alert v-else-if="napaka" type="error">
      {{ napaka }}
    </v-alert>

    <h1 v-else-if="skupina" class="text-h5 font-weight-bold">{{ skupina.ime }}</h1>

    <router-link :to="{ name: 'skupine' }" class="d-inline-block text-primary font-weight-bold mt-6">
      Nazaj na skupine
    </router-link>
  </v-container>
</template>
