<script setup>
import { ref, watch } from 'vue'
import { useRoute } from 'vue-router'
import { pridobiSkupino } from '@/api/skupine'
import ClaniSkupine from '@/components/ClaniSkupine.vue'
import StroskiSkupine from '@/components/StroskiSkupine.vue'

const route = useRoute()

const zavihek = ref('pregled')
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
    <router-link
      :to="{ name: 'skupine' }"
      class="d-inline-flex align-center ga-1 text-primary font-weight-bold mb-4"
    >
      <v-icon icon="mdi-arrow-left" size="small" />
      Moje skupine
    </router-link>

    <v-progress-linear v-if="nalaganje" indeterminate color="primary" />

    <v-alert v-else-if="napaka" type="error">
      {{ napaka }}
    </v-alert>

    <template v-else-if="skupina">
      <h1 class="text-h5 font-weight-bold mb-6">{{ skupina.ime }}</h1>

      <v-tabs v-model="zavihek" color="primary">
        <v-tab value="pregled">Pregled</v-tab>
        <v-tab value="clani">Člani</v-tab>
      </v-tabs>
      <v-divider class="mb-6" />

      <v-window v-model="zavihek">
        <v-window-item value="pregled">
          <StroskiSkupine :skupina-id="skupina.id" :moja-vloga="skupina.vloga" />
        </v-window-item>
        <v-window-item value="clani">
          <ClaniSkupine :skupina-id="skupina.id" :moja-vloga="skupina.vloga" />
        </v-window-item>
      </v-window>
    </template>
  </v-container>
</template>
