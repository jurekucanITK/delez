<script setup>
import { computed, reactive, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { dodajStrosek, pridobiClane, pridobiSkupino } from '@/api/skupine'

const route = useRoute()
const router = useRouter()

const skupina = ref(null)
const clani = ref([])
const napakaNalaganja = ref('')
const nalaganje = ref(false)

const obrazec = ref(null)
const podatki = reactive({ opis: '', znesek: '', datum: danes(), placnikId: null })
const napake = reactive({ opis: [], znesek: [], datum: [], placnikid: [] })
const splosnaNapaka = ref('')
const posiljanje = ref(false)

const jeSkrbnik = computed(() => skupina.value?.vloga === 'skrbnik')
const jeOmejen = computed(() => skupina.value?.vloga === 'omejen')

const moznostiPlacnika = computed(() =>
  clani.value
    .filter((c) => jeSkrbnik.value || c.id === skupina.value?.clanstvoId)
    .map((c) => ({ value: c.id, title: `${c.ime} (${c.email})` })),
)

const VZOREC_ZNESKA = /^\d+([.,]\d+)?$/

function preberiZnesek(v) {
  return Number(v.trim().replace(',', '.'))
}

const pravila = {
  opis: [
    (v) => !!v?.trim() || 'Opis je obvezen',
    (v) => v.trim().length <= 200 || 'Opis je lahko dolg največ 200 znakov',
  ],
  znesek: [
    (v) => !!v?.trim() || 'Znesek je obvezen',
    (v) => VZOREC_ZNESKA.test(v.trim()) || 'Vnesi znesek kot število, na primer 12,50',
    (v) => preberiZnesek(v) > 0 || 'Znesek mora biti večji od 0,00 €',
    (v) => /^\d+([.,]\d{1,2})?$/.test(v.trim()) || 'Znesek ima lahko največ dve decimalki',
    (v) => preberiZnesek(v) <= 99999999.99 || 'Znesek je lahko največ 99.999.999,99 €',
  ],
  datum: [(v) => !!v || 'Datum je obvezen'],
  placnikId: [(v) => v !== null || 'Plačnik je obvezen'],
}

function danes() {
  const d = new Date()
  const mesec = String(d.getMonth() + 1).padStart(2, '0')
  const dan = String(d.getDate()).padStart(2, '0')
  return `${d.getFullYear()}-${mesec}-${dan}`
}

async function nalozi(id) {
  skupina.value = null
  clani.value = []
  napakaNalaganja.value = ''
  nalaganje.value = true
  try {
    const [odgovorSkupine, odgovorClanov] = await Promise.all([pridobiSkupino(id), pridobiClane(id)])
    skupina.value = odgovorSkupine.data
    clani.value = odgovorClanov.data
    podatki.placnikId = odgovorSkupine.data.clanstvoId
  } catch (e) {
    const status = e.response?.status
    if (status === 404) {
      napakaNalaganja.value = 'Skupina ne obstaja.'
    } else if (status === 403) {
      napakaNalaganja.value = 'Nisi član te skupine.'
    } else if (status !== 401) {
      napakaNalaganja.value = 'Skupine ni bilo mogoče naložiti. Preveri povezavo in poskusi znova.'
    }
  } finally {
    nalaganje.value = false
  }
}

function pocistiNapako(polje) {
  napake[polje] = []
}

function nastaviNapakeIzApi(errors) {
  for (const [kljuc, sporocila] of Object.entries(errors)) {
    const polje = kljuc.toLowerCase()
    if (polje in napake) {
      napake[polje] = sporocila
    }
  }
}

function nazaj() {
  router.push({ name: 'skupina', params: { id: route.params.id } })
}

async function poslji() {
  splosnaNapaka.value = ''
  const { valid } = await obrazec.value.validate()
  if (!valid) {
    return
  }

  posiljanje.value = true
  try {
    await dodajStrosek(route.params.id, {
      opis: podatki.opis.trim(),
      znesek: preberiZnesek(podatki.znesek),
      datum: podatki.datum,
      placnikId: podatki.placnikId,
    })
    nazaj()
  } catch (napaka) {
    const status = napaka.response?.status
    if (status === 401) {
      return
    }
    const errors = status === 400 ? napaka.response.data?.errors : null
    if (errors) {
      nastaviNapakeIzApi(errors)
    } else if (status === 403) {
      splosnaNapaka.value = 'Omejen član ne more vnašati stroškov.'
    } else {
      splosnaNapaka.value = 'Stroška ni bilo mogoče shraniti. Preveri povezavo in poskusi znova.'
    }
  } finally {
    posiljanje.value = false
  }
}

watch(() => route.params.id, nalozi, { immediate: true })
</script>

<template>
  <v-container class="py-12">
    <v-row justify="center">
      <v-col cols="12" sm="9" md="6" lg="5" xl="4">
        <router-link
          :to="{ name: 'skupina', params: { id: route.params.id } }"
          class="d-inline-flex align-center ga-1 text-primary font-weight-bold mb-4"
        >
          <v-icon icon="mdi-arrow-left" size="small" />
          {{ skupina?.ime ?? 'Nazaj na skupino' }}
        </router-link>

        <v-progress-linear v-if="nalaganje" indeterminate color="primary" />

        <v-alert v-else-if="napakaNalaganja" type="error">
          {{ napakaNalaganja }}
        </v-alert>

        <v-alert v-else-if="jeOmejen" type="info">
          Omejen član ne more vnašati stroškov.
        </v-alert>

        <v-card v-else-if="skupina" class="pa-6 pa-sm-8">
          <h1 class="text-h5 font-weight-bold mb-6">Nov strošek</h1>

          <v-alert v-if="splosnaNapaka" type="error" class="mb-4">
            {{ splosnaNapaka }}
          </v-alert>

          <v-form ref="obrazec" validate-on="blur lazy" @submit.prevent="poslji">
            <v-text-field
              v-model="podatki.opis"
              label="Opis"
              placeholder="npr. Elektrika oktober"
              counter="200"
              :rules="pravila.opis"
              :error-messages="napake.opis"
              class="mb-2"
              @update:model-value="pocistiNapako('opis')"
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
                  class="mb-2"
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
                  class="mb-2"
                  @update:model-value="pocistiNapako('datum')"
                />
              </v-col>
            </v-row>

            <v-select
              v-model="podatki.placnikId"
              label="Plačal"
              :items="moznostiPlacnika"
              :disabled="!jeSkrbnik"
              :hint="jeSkrbnik ? '' : 'Kot plačnika lahko izbereš samo sebe'"
              :persistent-hint="!jeSkrbnik"
              :rules="pravila.placnikId"
              :error-messages="napake.placnikid"
              class="mb-6"
              @update:model-value="pocistiNapako('placnikid')"
            />

            <div class="d-flex flex-wrap ga-3">
              <v-btn type="submit" color="primary" size="large" :loading="posiljanje">
                Shrani strošek
              </v-btn>
              <v-btn variant="text" size="large" @click="nazaj">Prekliči</v-btn>
            </div>
          </v-form>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>
