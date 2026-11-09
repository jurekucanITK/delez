<script setup>
import { reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import { ustvariSkupino } from '@/api/skupine'

const router = useRouter()

const obrazec = ref(null)
const podatki = reactive({ ime: '', opis: '' })
const napake = reactive({ ime: [], opis: [] })
const splosnaNapaka = ref('')
const posiljanje = ref(false)

const pravila = {
  ime: [
    (v) => !!v?.trim() || 'Ime skupine je obvezno',
    (v) => v.trim().length <= 100 || 'Ime skupine je lahko dolgo največ 100 znakov',
  ],
  opis: [(v) => !v || v.trim().length <= 500 || 'Opis je lahko dolg največ 500 znakov'],
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
    const { data } = await ustvariSkupino(podatki)
    router.push({ name: 'skupina', params: { id: data.id } })
  } catch (napaka) {
    const status = napaka.response?.status
    if (status === 401) {
      return
    }
    const errors = status === 400 ? napaka.response.data?.errors : null
    if (errors) {
      nastaviNapakeIzApi(errors)
    } else {
      splosnaNapaka.value = 'Skupine ni bilo mogoče ustvariti. Preveri povezavo in poskusi znova.'
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
          <h1 class="text-headline-small font-weight-bold mb-2">Nova skupina</h1>
          <p class="text-body-medium text-medium-emphasis mb-6">
            V skupini z drugimi člani gospodinjstva beležite skupne stroške.
          </p>

          <v-alert v-if="splosnaNapaka" type="error" class="mb-4">
            {{ splosnaNapaka }}
          </v-alert>

          <v-form ref="obrazec" validate-on="blur lazy" @submit.prevent="poslji">
            <v-text-field
              v-model="podatki.ime"
              label="Ime skupine"
              counter="100"
              :rules="pravila.ime"
              :error-messages="napake.ime"
              class="mb-2"
              @update:model-value="pocistiNapako('ime')"
            />
            <v-textarea
              v-model="podatki.opis"
              label="Opis"
              hint="Neobvezno"
              persistent-hint
              counter="500"
              rows="3"
              auto-grow
              :rules="pravila.opis"
              :error-messages="napake.opis"
              class="mb-4"
              @update:model-value="pocistiNapako('opis')"
            />

            <v-btn type="submit" color="primary" size="large" block :loading="posiljanje">
              Ustvari skupino
            </v-btn>
          </v-form>

          <p class="text-body-medium text-center mt-6">
            <router-link :to="{ name: 'skupine' }" class="text-primary font-weight-bold">
              Nazaj na skupine
            </router-link>
          </p>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>
