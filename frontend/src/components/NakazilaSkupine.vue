<script setup>
import { computed, ref, watch } from 'vue'
import { izbrisiNakazilo, pridobiNakazila } from '@/api/skupine'

const props = defineProps({
  skupinaId: { type: [Number, String], required: true },
  mojeClanstvoId: { type: Number, required: true },
  mojaVloga: { type: String, required: true },
  osvezitev: { type: Number, default: 0 },
})

const emit = defineEmits(['uredi', 'spremenjeno'])

const nakazila = ref([])
const imaUpravljanje = computed(() => nakazila.value.some(lahkoUreja))
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

function lahkoUreja(nakazilo) {
  return (
    props.mojaVloga === 'skrbnik' ||
    (props.mojaVloga === 'clan' && nakazilo.placnikId === props.mojeClanstvoId)
  )
}

async function nalozi([id], prejsnje) {
  if (prejsnje?.[0] !== id) {
    nakazila.value = []
  }
  napakaNalaganja.value = ''
  nalaganje.value = true
  try {
    const { data } = await pridobiNakazila(id)
    nakazila.value = data
  } catch (e) {
    nakazila.value = []
    if (e.response?.status !== 401) {
      napakaNalaganja.value = 'Nakazil ni bilo mogoče naložiti. Preveri povezavo in poskusi znova.'
    }
  } finally {
    nalaganje.value = false
  }
}

function odpriPotrditev(nakazilo) {
  zaBrisanje.value = nakazilo
  napakaBrisanja.value = ''
  potrditevOdprta.value = true
}

async function potrdiBrisanje() {
  napakaBrisanja.value = ''
  brisanje.value = true
  try {
    await izbrisiNakazilo(props.skupinaId, zaBrisanje.value.id)
    potrditevOdprta.value = false
    emit('spremenjeno')
  } catch (e) {
    const status = e.response?.status
    if (status === 401) {
      return
    }
    if (status === 403) {
      napakaBrisanja.value = 'Brišeš lahko samo nakazila, ki si jih plačal sam.'
    } else if (status === 404) {
      napakaBrisanja.value = 'Nakazilo ne obstaja več.'
    } else {
      napakaBrisanja.value = 'Nakazila ni bilo mogoče izbrisati. Preveri povezavo in poskusi znova.'
    }
  } finally {
    brisanje.value = false
  }
}

watch(() => [props.skupinaId, props.osvezitev], nalozi, { immediate: true })
</script>

<template>
  <h2 class="text-h6 font-weight-bold mb-4">Nakazila</h2>

  <v-card>
    <v-progress-linear v-if="nalaganje && nakazila.length === 0" indeterminate color="primary" />

    <v-alert v-else-if="napakaNalaganja" type="error" class="ma-4">
      {{ napakaNalaganja }}
    </v-alert>

    <div v-else-if="nakazila.length === 0" class="pa-6 text-body-2 text-medium-emphasis">
      Ni še nakazil.
    </div>

    <v-table v-else class="bg-transparent">
      <thead>
        <tr>
          <th>Datum</th>
          <th>Plačal</th>
          <th>Prejel</th>
          <th class="text-end">Znesek</th>
          <th v-if="imaUpravljanje"><span class="d-sr-only">Dejanja</span></th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="nakazilo in nakazila" :key="nakazilo.id">
          <td class="text-no-wrap">{{ prikaziDatum(nakazilo.datum) }}</td>
          <td class="text-no-wrap">{{ nakazilo.placnikIme }}</td>
          <td class="text-no-wrap">{{ nakazilo.prejemnikIme }}</td>
          <td class="text-end text-no-wrap font-weight-bold">{{ prikaziZnesek(nakazilo.znesek) }}</td>
          <td v-if="imaUpravljanje" class="text-end text-no-wrap pe-2">
            <template v-if="lahkoUreja(nakazilo)">
              <v-btn
                icon="mdi-pencil-outline"
                variant="text"
                size="small"
                aria-label="Uredi"
                v-tooltip:top="'Uredi'"
                @click="emit('uredi', nakazilo)"
              />
              <v-btn
                icon="mdi-close"
                variant="text"
                size="small"
                aria-label="Izbriši"
                v-tooltip:top="'Izbriši'"
                @click="odpriPotrditev(nakazilo)"
              />
            </template>
          </td>
        </tr>
      </tbody>
    </v-table>
  </v-card>

  <v-dialog v-model="potrditevOdprta" max-width="420" :persistent="brisanje">
    <v-card v-if="zaBrisanje" class="pa-6">
      <h2 class="text-h6 font-weight-bold mb-2">Izbriši nakazilo?</h2>
      <p class="text-body-2 mb-4">
        Nakazilo od {{ zaBrisanje.placnikIme }} za {{ zaBrisanje.prejemnikIme }} v znesku
        {{ prikaziZnesek(zaBrisanje.znesek) }} bo izbrisano. Stanja se bodo preračunala.
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
