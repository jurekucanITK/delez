<script setup>
import { computed, ref, watch } from 'vue'
import { izbrisiStrosek, pridobiStroske } from '@/api/skupine'

const props = defineProps({
  skupinaId: { type: [Number, String], required: true },
  mojeClanstvoId: { type: Number, required: true },
  mojaVloga: { type: String, required: true },
})

const emit = defineEmits(['nakazilo', 'spremenjeno'])

const lahkoVnasa = computed(() => props.mojaVloga !== 'omejen')
const stroski = ref([])
const imaUpravljanje = computed(() => stroski.value.some(lahkoUreja))
const napakaNalaganja = ref('')
const nalaganje = ref(false)

const zaBrisanje = ref(null)
const potrditevOdprta = ref(false)
const brisanje = ref(false)
const napakaBrisanja = ref('')

const oblikaZneska = new Intl.NumberFormat('sl-SI', { style: 'currency', currency: 'EUR' })
const oblikaDatuma = new Intl.DateTimeFormat('sl-SI', { day: 'numeric', month: 'numeric' })

function prikaziZnesek(znesek) {
  return oblikaZneska.format(znesek)
}

function prikaziDatum(datum) {
  const [leto, mesec, dan] = datum.split('-').map(Number)
  return oblikaDatuma.format(new Date(leto, mesec - 1, dan))
}

function lahkoUreja(strosek) {
  return (
    props.mojaVloga === 'skrbnik' ||
    (props.mojaVloga === 'clan' && strosek.placnikId === props.mojeClanstvoId)
  )
}

async function nalozi(id) {
  napakaNalaganja.value = ''
  nalaganje.value = true
  try {
    const { data } = await pridobiStroske(id)
    stroski.value = data
  } catch (e) {
    stroski.value = []
    if (e.response?.status !== 401) {
      napakaNalaganja.value = 'Stroškov ni bilo mogoče naložiti. Preveri povezavo in poskusi znova.'
    }
  } finally {
    nalaganje.value = false
  }
}

function odpriPotrditev(strosek) {
  zaBrisanje.value = strosek
  napakaBrisanja.value = ''
  potrditevOdprta.value = true
}

async function potrdiBrisanje() {
  napakaBrisanja.value = ''
  brisanje.value = true
  try {
    await izbrisiStrosek(props.skupinaId, zaBrisanje.value.id)
    potrditevOdprta.value = false
    emit('spremenjeno')
    await nalozi(props.skupinaId)
  } catch (e) {
    const status = e.response?.status
    if (status === 401) {
      return
    }
    if (status === 403) {
      napakaBrisanja.value = 'Brišeš lahko samo stroške, ki si jih plačal sam.'
    } else if (status === 404) {
      napakaBrisanja.value = 'Strošek ne obstaja več.'
    } else {
      napakaBrisanja.value = 'Stroška ni bilo mogoče izbrisati. Preveri povezavo in poskusi znova.'
    }
  } finally {
    brisanje.value = false
  }
}

watch(
  () => props.skupinaId,
  (id) => {
    stroski.value = []
    nalozi(id)
  },
  { immediate: true },
)
</script>

<template>
  <div class="d-flex align-center justify-space-between flex-wrap ga-3 mb-4">
    <h2 class="text-h6 font-weight-bold">Stroški</h2>
    <div v-if="lahkoVnasa" class="d-flex flex-wrap ga-2">
      <v-btn
        color="primary"
        prepend-icon="mdi-plus"
        :to="{ name: 'nov-strosek', params: { id: skupinaId } }"
      >
        Nov strošek
      </v-btn>
      <v-btn variant="outlined" @click="emit('nakazilo')">Nakazilo</v-btn>
    </div>
  </div>

  <v-card>
    <v-progress-linear v-if="nalaganje && stroski.length === 0" indeterminate color="primary" />

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
          <th v-if="imaUpravljanje"><span class="d-sr-only">Dejanja</span></th>
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
          <td v-if="imaUpravljanje" class="text-end text-no-wrap pe-2">
            <template v-if="lahkoUreja(strosek)">
              <v-btn
                icon="mdi-pencil-outline"
                variant="text"
                size="small"
                aria-label="Uredi"
                v-tooltip:top="'Uredi'"
                :to="{ name: 'uredi-strosek', params: { id: skupinaId, strosekId: strosek.id } }"
              />
              <v-btn
                icon="mdi-close"
                variant="text"
                size="small"
                aria-label="Izbriši"
                v-tooltip:top="'Izbriši'"
                @click="odpriPotrditev(strosek)"
              />
            </template>
          </td>
        </tr>
      </tbody>
    </v-table>
  </v-card>

  <v-dialog v-model="potrditevOdprta" max-width="420" :persistent="brisanje">
    <v-card v-if="zaBrisanje" class="pa-6">
      <h2 class="text-h6 font-weight-bold mb-2">Izbriši strošek?</h2>
      <p class="text-body-2 mb-4">
        Strošek „{{ zaBrisanje.opis }}“ v znesku {{ prikaziZnesek(zaBrisanje.znesek) }} bo izbrisan
        skupaj z deleži vseh članov. Stanja se bodo preračunala.
      </p>

      <v-alert v-if="napakaBrisanja" type="error" class="mb-4">
        {{ napakaBrisanja }}
      </v-alert>

      <div class="d-flex flex-wrap justify-end ga-3">
        <v-btn variant="text" :disabled="brisanje" @click="potrditevOdprta = false">Prekliči</v-btn>
        <v-btn color="error" :loading="brisanje" @click="potrdiBrisanje">Izbriši</v-btn>
      </div>
    </v-card>
  </v-dialog>
</template>
