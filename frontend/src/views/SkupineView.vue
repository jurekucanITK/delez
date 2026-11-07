<script setup>
import { onMounted } from 'vue'
import { pridobiJaz } from '@/api/avtentikacija'

onMounted(() => {
  pridobiJaz().catch(() => {})
})
</script>

<template>
  <v-container class="py-12">
    <div class="d-flex flex-wrap align-center justify-space-between ga-4">
      <h1 class="text-h5 font-weight-bold">Moje skupine</h1>
      <v-btn color="primary" prepend-icon="mdi-plus" :to="{ name: 'nova-skupina' }">
        Nova skupina
      </v-btn>
    </div>
  </v-container>
</template>
