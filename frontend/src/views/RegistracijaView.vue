<script setup>
import { reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import { registriraj } from '@/api/avtentikacija'

const router = useRouter()

const obrazec = ref(null)
const podatki = reactive({ ime: '', email: '', geslo: '' })
const napake = reactive({ ime: [], email: [], geslo: [] })
const splosnaNapaka = ref('')
const posiljanje = ref(false)

const pravila = {
  ime: [(v) => !!v?.trim() || 'Ime je obvezno'],
  email: [
    (v) => !!v?.trim() || 'E-poštni naslov je obvezen',
    (v) => /^[^@\s]+@[^@\s]+$/.test(v.trim()) || 'Vnesi veljaven e-poštni naslov',
  ],
  geslo: [
    (v) => !!v || 'Geslo je obvezno',
    (v) => v.length >= 8 || 'Geslo mora imeti vsaj 8 znakov.',
  ],
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
    await registriraj(podatki)
    router.push({ name: 'prijava', query: { registracija: 'uspesna' } })
  } catch (napaka) {
    const errors = napaka.response?.status === 400 ? napaka.response.data?.errors : null
    if (errors) {
      nastaviNapakeIzApi(errors)
    } else {
      splosnaNapaka.value = 'Registracija ni uspela. Preveri povezavo in poskusi znova.'
    }
  } finally {
    posiljanje.value = false
  }
}
</script>

<template>
  <v-container class="py-12">
    <v-row justify="center">
      <v-col cols="12" sm="9" md="6" lg="5" xl="4">
        <v-card class="pa-6 pa-sm-8">
          <h1 class="text-headline-small font-weight-bold mb-2">Ustvari račun</h1>
          <p class="text-body-medium text-medium-emphasis mb-6">
            Z računom vodiš skupne stroške, drugi pa te lahko dodajo v svoje skupine.
          </p>

          <v-alert v-if="splosnaNapaka" type="error" class="mb-4">
            {{ splosnaNapaka }}
          </v-alert>

          <v-form ref="obrazec" validate-on="blur lazy" @submit.prevent="poslji">
            <v-text-field
              v-model="podatki.ime"
              label="Ime"
              autocomplete="name"
              :rules="pravila.ime"
              :error-messages="napake.ime"
              class="mb-2"
              @update:model-value="pocistiNapako('ime')"
            />
            <v-text-field
              v-model="podatki.email"
              label="E-poštni naslov"
              type="email"
              autocomplete="email"
              :rules="pravila.email"
              :error-messages="napake.email"
              class="mb-2"
              @update:model-value="pocistiNapako('email')"
            />
            <v-text-field
              v-model="podatki.geslo"
              label="Geslo"
              type="password"
              autocomplete="new-password"
              hint="Vsaj 8 znakov"
              persistent-hint
              :rules="pravila.geslo"
              :error-messages="napake.geslo"
              class="mb-4"
              @update:model-value="pocistiNapako('geslo')"
            />

            <v-btn type="submit" color="primary" size="large" block :loading="posiljanje">
              Ustvari račun
            </v-btn>
          </v-form>

          <p class="text-body-medium text-center mt-6">
            Že imaš račun?
            <router-link :to="{ name: 'prijava' }" class="text-primary font-weight-bold">
              Prijavi se
            </router-link>
          </p>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>
