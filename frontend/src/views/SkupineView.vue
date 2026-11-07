<script setup>
import { onMounted } from 'vue'
import { pridobiJaz } from '@/api/avtentikacija'

onMounted(() => {
  pridobiJaz().catch(() => {})
})
</script>

<template>
  <v-container class="py-12">
    <h1 class="text-h5 font-weight-bold">Moje skupine</h1>
  </v-container>
</template>
