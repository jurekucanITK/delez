<script setup>
import { computed, ref, watch } from 'vue'
import { pridobiStroske } from '@/api/skupine'

const props = defineProps({
  skupinaId: { type: [Number, String], required: true },
  mojaVloga: { type: String, required: true },
})

const lahkoVnasa = computed(() => props.mojaVloga !== 'omejen')

const stroski = ref([])
const napakaNalaganja = ref('')
const nalaganje = ref(false)

const oblikaZneska = new Intl.NumberFormat('sl-SI', { style: 'currency', currency: 'EUR' })
const oblikaDatuma = new Intl.DateTimeFormat('sl-SI', { day: 'numeric', month: 'numeric' })

function prikaziZnesek(znesek) {
  return oblikaZneska.format(znesek)
}

function prikaziDatum(datum) {
  const [leto, mesec, dan] = datum.split('-').map(Number)
  return oblikaDatuma.format(new Date(leto, mesec - 1, dan))
}

async function nalozi(id) {
  stroski.value = []
  napakaNalaganja.value = ''
  nalaganje.value = true
  try {
    const { data } = await pridobiStroske(id)
    stroski.value = data
  } catch (e) {
    if (e.response?.status !== 401) {
      napakaNalaganja.value = 'Stroškov ni bilo mogoče naložiti. Preveri povezavo in poskusi znova.'
    }
  } finally {
    nalaganje.value = false
  }
}

watch(() => props.skupinaId, nalozi, { immediate: true })
</script>

<template>
  <div class="d-flex align-center justify-space-between flex-wrap ga-3 mb-4">
    <h2 class="text-h6 font-weight-bold">Stroški</h2>
    <v-btn
      v-if="lahkoVnasa"
      color="primary"
      prepend-icon="mdi-plus"
      :to="{ name: 'nov-strosek', params: { id: skupinaId } }"
    >
      Nov strošek
    </v-btn>
  </div>

  <v-card>
    <v-progress-linear v-if="nalaganje" indeterminate color="primary" />

    <v-alert v-else-if="napakaNalaganja" type="error" class="ma-4">
      {{ napakaNalaganja }}
    </v-alert>

    <div v-else-if="stroski.length === 0" class="pa-6 text-body-2 text-medium-emphasis">
      Ni še stroškov.
      <template v-if="lahkoVnasa">Prvega dodaj z gumbom Nov strošek.</template>
    </div>

    <v-table v-else class="bg-transparent">
      <thead>
        <tr>
          <th>Datum</th>
          <th>Opis</th>
          <th>Plačal</th>
          <th class="text-end">Znesek</th>
          <th class="text-end">Moj delež</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="strosek in stroski" :key="strosek.id">
          <td class="text-no-wrap">{{ prikaziDatum(strosek.datum) }}</td>
          <td>{{ strosek.opis }}</td>
          <td class="text-no-wrap">{{ strosek.placnikIme }}</td>
          <td class="text-end text-no-wrap font-weight-bold">{{ prikaziZnesek(strosek.znesek) }}</td>
          <td class="text-end text-no-wrap">
            {{ strosek.mojDelez === null ? '' : prikaziZnesek(strosek.mojDelez) }}
          </td>
        </tr>
      </tbody>
    </v-table>
  </v-card>
</template>
