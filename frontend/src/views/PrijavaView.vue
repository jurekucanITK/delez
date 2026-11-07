<script setup>
import { computed } from 'vue'
import { useRoute } from 'vue-router'

const route = useRoute()
const registracijaUspesna = computed(() => route.query.registracija === 'uspesna')
</script>

<template>
  <v-container class="py-12">
    <v-row justify="center">
      <v-col cols="12" sm="9" md="6" lg="5" xl="4">
        <v-card class="pa-6 pa-sm-8">
          <h1 class="text-h5 font-weight-bold mb-4">Prijava</h1>

          <v-alert v-if="registracijaUspesna" type="success" class="mb-4">
            Registracija je uspela. Zdaj se lahko prijaviš.
          </v-alert>

          <p class="text-body-2 text-medium-emphasis">Prijava bo na voljo kmalu.</p>

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
