<script setup>
import { computed, reactive, ref, watch } from 'vue'
import { dodajClana, pridobiClane } from '@/api/skupine'

const props = defineProps({
  skupinaId: { type: [Number, String], required: true },
  mojaVloga: { type: String, required: true },
})

const imenaVlog = {
  skrbnik: 'Skrbnik',
  clan: 'Član',
  omejen: 'Omejen član',
}

const moznostiVlog = Object.entries(imenaVlog).map(([value, title]) => ({ value, title }))

const jeSkrbnik = computed(() => props.mojaVloga === 'skrbnik')

const clani = ref([])
const napakaNalaganja = ref('')
const nalaganje = ref(false)

const obrazec = ref(null)
const podatki = reactive({ email: '', vloga: 'clan' })
const napake = reactive({ email: [], vloga: [] })
const splosnaNapaka = ref('')
const posiljanje = ref(false)

const pravila = {
  email: [
    (v) => !!v?.trim() || 'E-poštni naslov je obvezen',
    (v) => /^\S+@\S+\.\S+$/.test(v?.trim() ?? '') || 'Vnesi veljaven e-poštni naslov',
  ],
}

function primerjaj(a, b) {
  return a.ime.localeCompare(b.ime, 'sl') || a.email.localeCompare(b.email, 'sl')
}

async function nalozi(id) {
  clani.value = []
  napakaNalaganja.value = ''
  nalaganje.value = true
  try {
    const { data } = await pridobiClane(id)
    clani.value = data
  } catch (e) {
    if (e.response?.status !== 401) {
      napakaNalaganja.value = 'Članov ni bilo mogoče naložiti. Preveri povezavo in poskusi znova.'
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

async function poslji() {
  splosnaNapaka.value = ''
  const { valid } = await obrazec.value.validate()
  if (!valid) {
    return
  }

  posiljanje.value = true
  try {
    const { data } = await dodajClana(props.skupinaId, {
      email: podatki.email.trim(),
      vloga: podatki.vloga,
    })
    clani.value = [...clani.value, data].sort(primerjaj)
    podatki.email = ''
    podatki.vloga = 'clan'
    obrazec.value.resetValidation()
  } catch (napaka) {
    const status = napaka.response?.status
    if (status === 401) {
      return
    }
    const errors = status === 400 ? napaka.response.data?.errors : null
    if (errors) {
      nastaviNapakeIzApi(errors)
    } else if (status === 403) {
      splosnaNapaka.value = 'Člane lahko dodaja samo skrbnik skupine.'
    } else {
      splosnaNapaka.value = 'Člana ni bilo mogoče dodati. Preveri povezavo in poskusi znova.'
    }
  } finally {
    posiljanje.value = false
  }
}

watch(() => props.skupinaId, nalozi, { immediate: true })
</script>

<template>
  <v-row>
    <v-col cols="12" :md="jeSkrbnik ? 7 : 8">
      <v-card class="pa-6">
        <h2 class="text-title-large font-weight-bold mb-2">Člani</h2>

        <v-progress-linear v-if="nalaganje" indeterminate color="primary" class="my-4" />

        <v-alert v-else-if="napakaNalaganja" type="error" class="mt-4">
          {{ napakaNalaganja }}
        </v-alert>

        <v-list v-else class="pa-0" bg-color="transparent">
          <template v-for="(clan, i) in clani" :key="clan.id">
            <v-divider v-if="i > 0" />
            <v-list-item class="px-0 py-3">
              <v-list-item-title class="font-weight-bold">{{ clan.ime }}</v-list-item-title>
              <v-list-item-subtitle>{{ clan.email }}</v-list-item-subtitle>
              <template #append>
                <span class="text-body-medium text-medium-emphasis">{{ imenaVlog[clan.vloga] }}</span>
              </template>
            </v-list-item>
          </template>
        </v-list>
      </v-card>
    </v-col>

    <v-col v-if="jeSkrbnik" cols="12" md="5">
      <v-card class="pa-6">
        <h2 class="text-title-large font-weight-bold mb-2">Dodaj člana</h2>
        <p class="text-body-medium text-medium-emphasis mb-6">
          Vpiši e-poštni naslov, s katerim je oseba registrirana v Delež.
        </p>

        <v-alert v-if="splosnaNapaka" type="error" class="mb-4">
          {{ splosnaNapaka }}
        </v-alert>

        <v-form ref="obrazec" validate-on="submit lazy" @submit.prevent="poslji">
          <v-text-field
            v-model="podatki.email"
            label="E-poštni naslov"
            type="email"
            autocomplete="off"
            :rules="pravila.email"
            :error-messages="napake.email"
            class="mb-2"
            @update:model-value="pocistiNapako('email')"
          />
          <v-select
            v-model="podatki.vloga"
            label="Vloga"
            :items="moznostiVlog"
            :error-messages="napake.vloga"
            class="mb-4"
            @update:model-value="pocistiNapako('vloga')"
          />

          <v-btn type="submit" color="primary" size="large" block :loading="posiljanje">
            Dodaj člana
          </v-btn>
        </v-form>
      </v-card>
    </v-col>
  </v-row>
</template>
