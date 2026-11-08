<script setup>
import { ref, watch } from 'vue'
import { pridobiPredlogNakazil } from '@/api/skupine'

const props = defineProps({
  skupinaId: { type: [Number, String], required: true },
  mojeClanstvoId: { type: Number, required: true },
  mojaVloga: { type: String, required: true },
  osvezitev: { type: Number, default: 0 },
})

const emit = defineEmits(['zabelezi'])

const predlog = ref([])
const napakaNalaganja = ref('')
const nalaganje = ref(false)

const evri = new Intl.NumberFormat('sl-SI', { style: 'currency', currency: 'EUR' })

function lahkoZabelezi(nakazilo) {
  if (props.mojaVloga === 'skrbnik') return true
  if (props.mojaVloga === 'clan') return nakazilo.placnikId === props.mojeClanstvoId
  return false
}

function zabelezi(nakazilo) {
  emit('zabelezi', {
    placnikId: nakazilo.placnikId,
    prejemnikId: nakazilo.prejemnikId,
    znesek: nakazilo.znesek,
  })
}

async function nalozi([id], prejsnje) {
  if (prejsnje?.[0] !== id) {
    predlog.value = []
  }
  napakaNalaganja.value = ''
  nalaganje.value = true
  try {
    const { data } = await pridobiPredlogNakazil(id)
    predlog.value = data
  } catch (e) {
    if (e.response?.status !== 401) {
      napakaNalaganja.value =
        'Predlaganih nakazil ni bilo mogoče naložiti. Preveri povezavo in poskusi znova.'
    }
  } finally {
    nalaganje.value = false
  }
}

watch(() => [props.skupinaId, props.osvezitev], nalozi, { immediate: true })
</script>

<template>
  <v-card class="pa-6">
    <h2 class="text-h6 font-weight-bold mb-2">Predlagana nakazila</h2>

    <v-progress-linear
      :active="nalaganje"
      indeterminate
      color="primary"
      class="mb-2"
    />

    <v-alert v-if="napakaNalaganja" type="error" class="mt-2">
      {{ napakaNalaganja }}
    </v-alert>

    <div
      v-else-if="!nalaganje && predlog.length === 0"
      class="text-body-2 text-medium-emphasis py-2"
    >
      Vse je poravnano
    </div>

    <v-list v-else class="pa-0" bg-color="transparent">
      <template
        v-for="(nakazilo, i) in predlog"
        :key="`${nakazilo.placnikId}-${nakazilo.prejemnikId}`"
      >
        <v-divider v-if="i > 0" />
        <v-list-item class="px-0 py-3">
          <v-list-item-title class="font-weight-bold text-wrap">
            {{ nakazilo.placnikIme }} → {{ nakazilo.prejemnikIme }}
          </v-list-item-title>
          <v-list-item-subtitle class="text-wrap">
            {{ nakazilo.placnikEmail }} → {{ nakazilo.prejemnikEmail }}
          </v-list-item-subtitle>
          <template #append>
            <div class="text-no-wrap font-weight-bold ps-3">
              {{ evri.format(nakazilo.znesek) }}
            </div>
          </template>
          <v-btn
            v-if="lahkoZabelezi(nakazilo)"
            variant="outlined"
            size="small"
            class="mt-3"
            @click="zabelezi(nakazilo)"
          >
            Zabeleži nakazilo
          </v-btn>
        </v-list-item>
      </template>
    </v-list>
  </v-card>
</template>
