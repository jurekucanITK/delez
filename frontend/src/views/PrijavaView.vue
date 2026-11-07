<script setup>
import { computed, reactive, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { prijavi } from '@/api/avtentikacija'
import { useAvtentikacijaStore } from '@/stores/avtentikacija'

const route = useRoute()
const router = useRouter()
const avtentikacija = useAvtentikacijaStore()

const registracijaUspesna = computed(() => route.query.registracija === 'uspesna')

const obrazec = ref(null)
const podatki = reactive({ email: '', geslo: '' })
const napake = reactive({ email: [], geslo: [] })
const splosnaNapaka = ref('')
const posiljanje = ref(false)

const pravila = {
  email: [
    (v) => !!v?.trim() || 'E-poštni naslov je obvezen',
    (v) => /^[^@\s]+@[^@\s]+$/.test(v.trim()) || 'Vnesi veljaven e-poštni naslov',
  ],
  geslo: [(v) => !!v || 'Geslo je obvezno'],
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
    const { data } = await prijavi(podatki)
    avtentikacija.prijava(data.zeton, data.uporabnik)
    router.push({ name: 'skupine' })
  } catch (napaka) {
    const status = napaka.response?.status
    if (status === 401) {
      splosnaNapaka.value = 'Napačen e-poštni naslov ali geslo'
    } else if (status === 400 && napaka.response.data?.errors) {
      nastaviNapakeIzApi(napaka.response.data.errors)
    } else {
      splosnaNapaka.value = 'Prijava ni uspela. Preveri povezavo in poskusi znova.'
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
          <h1 class="text-h5 font-weight-bold mb-2">Prijava</h1>
          <p class="text-body-2 text-medium-emphasis mb-6">
            Prijavi se in poglej stroške v svojih skupinah.
          </p>

          <v-alert v-if="registracijaUspesna && !splosnaNapaka" type="success" class="mb-4">
            Registracija je uspela. Zdaj se lahko prijaviš.
          </v-alert>

          <v-alert v-if="splosnaNapaka" type="error" class="mb-4">
            {{ splosnaNapaka }}
          </v-alert>

          <v-form ref="obrazec" validate-on="blur lazy" @submit.prevent="poslji">
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
              autocomplete="current-password"
              :rules="pravila.geslo"
              :error-messages="napake.geslo"
              class="mb-4"
              @update:model-value="pocistiNapako('geslo')"
            />

            <v-btn type="submit" color="primary" size="large" block :loading="posiljanje">
              Prijavi se
            </v-btn>
          </v-form>

          <p class="text-body-2 text-center mt-6">
            Še nimaš računa?
            <router-link :to="{ name: 'registracija' }" class="text-primary font-weight-bold">
              Ustvari račun
            </router-link>
          </p>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>
