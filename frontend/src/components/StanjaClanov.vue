<script setup>
import { computed, ref, watch } from 'vue'
import { pridobiStanja } from '@/api/skupine'
import { barvaStanja, oblikujStanje } from '@/utils/stanje'

const props = defineProps({
  skupinaId: { type: [Number, String], required: true },
  mojeClanstvoId: { type: Number, required: true },
  osvezitev: { type: Number, default: 0 },
})

const stanja = ref([])
const napakaNalaganja = ref('')
const nalaganje = ref(false)

const razvrscena = computed(() => [
  ...stanja.value.filter((s) => s.clanstvoId === props.mojeClanstvoId),
  ...stanja.value.filter((s) => s.clanstvoId !== props.mojeClanstvoId),
])

function opisStanja(stanje) {
  if (stanje > 0) return 'mu pripada'
  if (stanje < 0) return 'je dolžan'
  return 'poravnano'
}

async function nalozi([id], prejsnje) {
  if (prejsnje?.[0] !== id) {
    stanja.value = []
  }
  napakaNalaganja.value = ''
  nalaganje.value = true
  try {
    const { data } = await pridobiStanja(id)
    stanja.value = data
  } catch (e) {
    if (e.response?.status !== 401) {
      napakaNalaganja.value = 'Stanj ni bilo mogoče naložiti. Preveri povezavo in poskusi znova.'
    }
  } finally {
    nalaganje.value = false
  }
}

watch(() => [props.skupinaId, props.osvezitev], nalozi, { immediate: true })
</script>

<template>
  <v-card class="pa-6">
    <h2 class="text-title-large font-weight-bold mb-2">Stanja članov</h2>

    <v-progress-linear
      :active="nalaganje"
      indeterminate
      color="primary"
      class="mb-2"
    />

    <v-alert v-if="napakaNalaganja" type="error" class="mt-2">
      {{ napakaNalaganja }}
    </v-alert>

    <v-list v-else class="pa-0" bg-color="transparent">
      <template v-for="(clan, i) in razvrscena" :key="clan.clanstvoId">
        <v-divider v-if="i > 0" />
        <v-list-item class="px-0 py-3">
          <v-list-item-title class="font-weight-bold">
            {{ clan.ime }}<template v-if="clan.clanstvoId === mojeClanstvoId"> (jaz)</template>
          </v-list-item-title>
          <v-list-item-subtitle>{{ clan.email }}</v-list-item-subtitle>
          <template #append>
            <div class="text-end ps-3">
              <div class="text-no-wrap font-weight-bold" :class="barvaStanja(clan.stanje)">
                {{ oblikujStanje(clan.stanje) }}
              </div>
              <div class="text-body-small text-medium-emphasis text-no-wrap">
                {{ opisStanja(clan.stanje) }}
              </div>
            </div>
          </template>
        </v-list-item>
      </template>
    </v-list>
  </v-card>
</template>
