<script setup>
import { computed, reactive, ref } from 'vue'
import { dodajNakazilo, pridobiClane, urediNakazilo } from '@/api/skupine'
import { pravilaZneska, preberiZnesek, vnosZneska } from '@/utils/znesek'

const props = defineProps({
  skupinaId: { type: [Number, String], required: true },
  mojeClanstvoId: { type: Number, required: true },
  mojaVloga: { type: String, required: true },
})

const emit = defineEmits(['shranjeno'])

const odprt = ref(false)
const urejanoId = ref(null)
const jeUrejanje = computed(() => urejanoId.value !== null)
const clani = ref([])
const nalozeniZa = ref(null)
const nalaganje = ref(false)
const napakaNalaganja = ref('')

const obrazec = ref(null)
const podatki = reactive({ placnikId: null, prejemnikId: null, znesek: '', datum: '' })
const napake = reactive({ placnikid: [], prejemnikid: [], znesek: [], datum: [] })
const splosnaNapaka = ref('')
const posiljanje = ref(false)

const jeSkrbnik = computed(() => props.mojaVloga === 'skrbnik')

function oznaka(clan) {
  return { value: clan.id, title: `${clan.ime} (${clan.email})` }
}

const moznostiPlacnika = computed(() =>
  clani.value.filter((c) => jeSkrbnik.value || c.id === props.mojeClanstvoId).map(oznaka),
)

const moznostiPrejemnika = computed(() =>
  clani.value.filter((c) => c.id !== podatki.placnikId).map(oznaka),
)

const pravila = {
  placnikId: [(v) => v !== null || 'Plačnik je obvezen'],
  prejemnikId: [
    (v) => v !== null || 'Prejemnik je obvezen',
    (v) => v !== podatki.placnikId || 'Plačnik in prejemnik ne smeta biti ista oseba',
  ],
  znesek: pravilaZneska,
  datum: [(v) => !!v || 'Datum je obvezen'],
}

function danes() {
  const d = new Date()
  const mesec = String(d.getMonth() + 1).padStart(2, '0')
  const dan = String(d.getDate()).padStart(2, '0')
  return `${d.getFullYear()}-${mesec}-${dan}`
}

async function naloziClane() {
  if (nalozeniZa.value === props.skupinaId) {
    return
  }
  napakaNalaganja.value = ''
  nalaganje.value = true
  try {
    const { data } = await pridobiClane(props.skupinaId)
    clani.value = data
    nalozeniZa.value = props.skupinaId
  } catch (e) {
    if (e.response?.status !== 401) {
      napakaNalaganja.value = 'Članov ni bilo mogoče naložiti. Preveri povezavo in poskusi znova.'
    }
  } finally {
    nalaganje.value = false
  }
}

function odpri({ id, placnikId, prejemnikId, znesek, datum } = {}) {
  urejanoId.value = id ?? null
  podatki.placnikId = placnikId ?? props.mojeClanstvoId
  podatki.prejemnikId = prejemnikId ?? null
  podatki.znesek = znesek === undefined || znesek === null ? '' : vnosZneska(znesek)
  podatki.datum = datum ?? danes()
  for (const polje of Object.keys(napake)) {
    napake[polje] = []
  }
  splosnaNapaka.value = ''
  obrazec.value?.resetValidation()
  odprt.value = true
  naloziClane()
}

function zapri() {
  odprt.value = false
}

function pocistiNapako(polje) {
  napake[polje] = []
}

function spremeniPlacnika() {
  pocistiNapako('placnikid')
  if (podatki.prejemnikId === podatki.placnikId) {
    podatki.prejemnikId = null
  }
}

function nastaviNapakeIzApi(errors) {
  for (const [kljuc, sporocila] of Object.entries(errors)) {
    const polje = kljuc.toLowerCase()
    if (polje in napake) {
      napake[polje] = sporocila
    }
  }
}

async function poslji() {
  splosnaNapaka.value = ''
  const { valid } = await obrazec.value.validate()
  if (!valid) {
    return
  }

  posiljanje.value = true
  try {
    const nakazilo = {
      placnikId: podatki.placnikId,
      prejemnikId: podatki.prejemnikId,
      znesek: preberiZnesek(podatki.znesek),
      datum: podatki.datum,
    }
    if (jeUrejanje.value) {
      await urediNakazilo(props.skupinaId, urejanoId.value, nakazilo)
    } else {
      await dodajNakazilo(props.skupinaId, nakazilo)
    }
    odprt.value = false
    emit('shranjeno')
  } catch (napaka) {
    const status = napaka.response?.status
    if (status === 401) {
      return
    }
    const errors = status === 400 ? napaka.response.data?.errors : null
    if (errors) {
      nastaviNapakeIzApi(errors)
    } else if (status === 403) {
      splosnaNapaka.value = jeUrejanje.value
        ? 'Urejaš lahko samo nakazila, ki si jih plačal sam.'
        : 'Omejen član ne more beležiti nakazil.'
    } else if (status === 404 && jeUrejanje.value) {
      splosnaNapaka.value = 'Nakazilo ne obstaja več.'
    } else {
      splosnaNapaka.value = 'Nakazila ni bilo mogoče shraniti. Preveri povezavo in poskusi znova.'
    }
  } finally {
    posiljanje.value = false
  }
}

defineExpose({ odpri })
</script>

<template>
  <v-dialog v-model="odprt" max-width="480" :persistent="posiljanje">
    <v-card class="pa-6">
      <h2 class="text-h6 font-weight-bold mb-4">{{ jeUrejanje ? 'Uredi nakazilo' : 'Nakazilo' }}</h2>

      <v-progress-linear v-if="nalaganje" indeterminate color="primary" />

      <v-alert v-else-if="napakaNalaganja" type="error">
        {{ napakaNalaganja }}
      </v-alert>

      <v-form v-else ref="obrazec" validate-on="blur lazy" @submit.prevent="poslji">
        <v-alert v-if="splosnaNapaka" type="error" class="mb-4">
          {{ splosnaNapaka }}
        </v-alert>

        <v-select
          v-model="podatki.placnikId"
          label="Plačnik"
          :items="moznostiPlacnika"
          :disabled="!jeSkrbnik"
          :hint="jeSkrbnik ? '' : 'Kot plačnika lahko izbereš samo sebe'"
          :persistent-hint="!jeSkrbnik"
          :rules="pravila.placnikId"
          :error-messages="napake.placnikid"
          class="mb-2"
          @update:model-value="spremeniPlacnika"
        />

        <v-select
          v-model="podatki.prejemnikId"
          label="Prejemnik"
          :items="moznostiPrejemnika"
          :rules="pravila.prejemnikId"
          :error-messages="napake.prejemnikid"
          class="mb-2"
          @update:model-value="pocistiNapako('prejemnikid')"
        />

        <v-row dense>
          <v-col cols="12" sm="6">
            <v-text-field
              v-model="podatki.znesek"
              label="Znesek (€)"
              placeholder="0,00"
              inputmode="decimal"
              autocomplete="off"
              :rules="pravila.znesek"
              :error-messages="napake.znesek"
              @update:model-value="pocistiNapako('znesek')"
            />
          </v-col>
          <v-col cols="12" sm="6">
            <v-text-field
              v-model="podatki.datum"
              label="Datum"
              type="date"
              :rules="pravila.datum"
              :error-messages="napake.datum"
              @update:model-value="pocistiNapako('datum')"
            />
          </v-col>
        </v-row>

        <div class="d-flex flex-wrap ga-3 mt-4">
          <v-btn type="submit" color="primary" size="large" :loading="posiljanje">
            {{ jeUrejanje ? 'Shrani spremembe' : 'Shrani nakazilo' }}
          </v-btn>
          <v-btn variant="text" size="large" :disabled="posiljanje" @click="zapri">Prekliči</v-btn>
        </div>
      </v-form>
    </v-card>
  </v-dialog>
</template>
