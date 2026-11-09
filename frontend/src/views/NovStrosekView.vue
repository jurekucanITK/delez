<script setup>
import { computed, reactive, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import {
  dodajStrosek,
  pridobiClane,
  pridobiSkupino,
  pridobiStrosek,
  urediStrosek,
} from '@/api/skupine'
import { pravilaZneska, preberiZnesek, vnosZneska } from '@/utils/znesek'

const route = useRoute()
const router = useRouter()

const skupina = ref(null)
const clani = ref([])
const napakaNalaganja = ref('')
const nalaganje = ref(false)

const obrazec = ref(null)
const podatki = reactive({ opis: '', znesek: '', datum: danes(), placnikId: null })
const izbrani = ref([])
const napake = reactive({ opis: [], znesek: [], datum: [], placnikid: [], claniids: [] })
const splosnaNapaka = ref('')
const posiljanje = ref(false)

const tujStrosek = ref(false)

const jeUrejanje = computed(() => route.params.strosekId !== undefined)
const jeSkrbnik = computed(() => skupina.value?.vloga === 'skrbnik')
const jeOmejen = computed(() => skupina.value?.vloga === 'omejen')

const moznostiPlacnika = computed(() =>
  clani.value
    .filter((c) => jeSkrbnik.value || c.id === skupina.value?.clanstvoId)
    .map((c) => ({ value: c.id, title: `${c.ime} (${c.email})` })),
)

const pravila = {
  opis: [
    (v) => !!v?.trim() || 'Opis je obvezen',
    (v) => v.trim().length <= 200 || 'Opis je lahko dolg največ 200 znakov',
  ],
  znesek: pravilaZneska,
  datum: [(v) => !!v || 'Datum je obvezen'],
  placnikId: [(v) => v !== null || 'Plačnik je obvezen'],
}

const oblikaZneska = new Intl.NumberFormat('sl-SI', { style: 'currency', currency: 'EUR' })

const znesekVCentih = computed(() => {
  if (!pravila.znesek.every((pravilo) => pravilo(podatki.znesek) === true)) {
    return null
  }
  return Math.round(preberiZnesek(podatki.znesek) * 100)
})

const delezi = computed(() => {
  const centi = znesekVCentih.value
  if (centi === null || izbrani.value.length === 0) {
    return {}
  }
  const ids = [...izbrani.value].sort((a, b) => a - b)
  const osnova = Math.floor(centi / ids.length)
  const ostanek = centi - osnova * ids.length
  const rezultat = Object.fromEntries(ids.map((id) => [id, osnova]))
  rezultat[ids[0]] += ostanek
  return rezultat
})

const nihceNiIzbran = computed(() => izbrani.value.length === 0)

const sporocilaDelitve = computed(() =>
  nihceNiIzbran.value ? ['Izberite vsaj enega člana'] : napake.claniids,
)

function prikaziDelez(id) {
  const centi = delezi.value[id]
  return centi === undefined ? '–' : oblikaZneska.format(centi / 100)
}

const vsotaDelezev = computed(() => {
  const vrednosti = Object.values(delezi.value)
  if (vrednosti.length === 0) {
    return '–'
  }
  return oblikaZneska.format(vrednosti.reduce((a, b) => a + b, 0) / 100)
})

function danes() {
  const d = new Date()
  const mesec = String(d.getMonth() + 1).padStart(2, '0')
  const dan = String(d.getDate()).padStart(2, '0')
  return `${d.getFullYear()}-${mesec}-${dan}`
}

async function nalozi([id, strosekId]) {
  skupina.value = null
  clani.value = []
  izbrani.value = []
  tujStrosek.value = false
  Object.assign(podatki, { opis: '', znesek: '', datum: danes(), placnikId: null })
  for (const polje of Object.keys(napake)) {
    napake[polje] = []
  }
  splosnaNapaka.value = ''
  napakaNalaganja.value = ''
  nalaganje.value = true
  try {
    const [odgovorSkupine, odgovorClanov] = await Promise.all([pridobiSkupino(id), pridobiClane(id)])
    clani.value = odgovorClanov.data
    if (strosekId === undefined) {
      izbrani.value = odgovorClanov.data.map((c) => c.id)
      podatki.placnikId = odgovorSkupine.data.clanstvoId
    } else {
      const zahtevaStroska = await naloziStrosek(id, strosekId)
      if (!zahtevaStroska) {
        return
      }
      const { data: strosek } = zahtevaStroska
      Object.assign(podatki, {
        opis: strosek.opis,
        znesek: vnosZneska(strosek.znesek),
        datum: strosek.datum,
        placnikId: strosek.placnikId,
      })
      izbrani.value = strosek.claniIds
      tujStrosek.value =
        odgovorSkupine.data.vloga === 'clan' && strosek.placnikId !== odgovorSkupine.data.clanstvoId
    }
    skupina.value = odgovorSkupine.data
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

async function naloziStrosek(id, strosekId) {
  try {
    return await pridobiStrosek(id, strosekId)
  } catch (e) {
    const status = e.response?.status
    if (status === 404) {
      napakaNalaganja.value = 'Strošek ne obstaja.'
    } else if (status !== 401) {
      napakaNalaganja.value = 'Stroška ni bilo mogoče naložiti. Preveri povezavo in poskusi znova.'
    }
    return null
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
  if (!valid || nihceNiIzbran.value) {
    return
  }

  posiljanje.value = true
  const strosek = {
    opis: podatki.opis.trim(),
    znesek: preberiZnesek(podatki.znesek),
    datum: podatki.datum,
    placnikId: podatki.placnikId,
    claniIds: izbrani.value,
  }
  try {
    if (jeUrejanje.value) {
      await urediStrosek(route.params.id, route.params.strosekId, strosek)
    } else {
      await dodajStrosek(route.params.id, strosek)
    }
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
      splosnaNapaka.value = jeUrejanje.value
        ? 'Urejaš lahko samo stroške, ki si jih plačal sam.'
        : 'Omejen član ne more vnašati stroškov.'
    } else if (status === 404 && jeUrejanje.value) {
      splosnaNapaka.value = 'Strošek ne obstaja.'
    } else {
      splosnaNapaka.value = 'Stroška ni bilo mogoče shraniti. Preveri povezavo in poskusi znova.'
    }
  } finally {
    posiljanje.value = false
  }
}

watch(() => [route.params.id, route.params.strosekId], nalozi, { immediate: true })
</script>

<template>
  <v-container class="py-12">
    <v-row justify="center">
      <v-col cols="12" lg="10" xl="9">
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
          {{ jeUrejanje ? 'Omejen član ne more urejati stroškov.' : 'Omejen član ne more vnašati stroškov.' }}
        </v-alert>

        <v-alert v-else-if="tujStrosek" type="info">
          Urejaš lahko samo stroške, ki si jih plačal sam.
        </v-alert>

        <template v-else-if="skupina">
          <h1 class="text-headline-small font-weight-bold mb-6">
            {{ jeUrejanje ? 'Uredi strošek' : 'Nov strošek' }}
          </h1>

          <v-alert v-if="splosnaNapaka" type="error" class="mb-4">
            {{ splosnaNapaka }}
          </v-alert>

          <v-form ref="obrazec" validate-on="blur lazy" @submit.prevent="poslji">
            <v-row>
              <v-col cols="12" md="6">
                <v-card class="pa-6">
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
                    @update:model-value="pocistiNapako('placnikid')"
                  />
                </v-card>
              </v-col>

              <v-col cols="12" md="6">
                <v-card class="pa-6">
                  <h2 class="text-title-large font-weight-bold mb-1">Delitev stroška</h2>
                  <p class="text-body-medium text-medium-emphasis mb-2">
                    Med koga se strošek deli
                  </p>

                  <v-list
                    v-model:selected="izbrani"
                    select-strategy="leaf"
                    class="pa-0"
                    bg-color="transparent"
                    @update:selected="pocistiNapako('claniids')"
                  >
                    <template v-for="(clan, i) in clani" :key="clan.id">
                      <v-divider v-if="i > 0" />
                      <v-list-item :value="clan.id" class="px-1 py-2" rounded="lg" :ripple="false">
                        <template #prepend="{ isSelected }">
                          <v-list-item-action start>
                            <v-checkbox-btn :model-value="isSelected" color="primary" />
                          </v-list-item-action>
                        </template>
                        <v-list-item-title class="font-weight-bold">{{ clan.ime }}</v-list-item-title>
                        <v-list-item-subtitle>{{ clan.email }}</v-list-item-subtitle>
                        <template #append>
                          <span class="text-no-wrap font-weight-bold">{{ prikaziDelez(clan.id) }}</span>
                        </template>
                      </v-list-item>
                    </template>
                  </v-list>

                  <v-messages
                    :active="sporocilaDelitve.length > 0"
                    :messages="sporocilaDelitve"
                    color="error"
                    class="px-1 mt-2"
                  />

                  <v-sheet
                    color="surface-variant"
                    rounded="lg"
                    class="d-flex justify-space-between px-4 py-3 mt-4 text-body-medium"
                  >
                    <span>Vsota deležev</span>
                    <span class="font-weight-bold">{{ vsotaDelezev }}</span>
                  </v-sheet>

                  <v-divider class="my-6" />

                  <div class="d-flex flex-wrap ga-3">
                    <v-btn
                      type="submit"
                      color="primary"
                      size="large"
                      :loading="posiljanje"
                      :disabled="nihceNiIzbran"
                    >
                      {{ jeUrejanje ? 'Shrani spremembe' : 'Shrani strošek' }}
                    </v-btn>
                    <v-btn variant="text" size="large" @click="nazaj">Prekliči</v-btn>
                  </div>
                </v-card>
              </v-col>
            </v-row>
          </v-form>
        </template>
      </v-col>
    </v-row>
  </v-container>
</template>
