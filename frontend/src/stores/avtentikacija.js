import { defineStore } from 'pinia'
import { computed, ref } from 'vue'

const KLJUC_ZETONA = 'delez_zeton'

export const useAvtentikacijaStore = defineStore('avtentikacija', () => {
  const zeton = ref(localStorage.getItem(KLJUC_ZETONA))

  const jePrijavljen = computed(() => !!zeton.value)

  function nastaviZeton(novZeton) {
    zeton.value = novZeton
    localStorage.setItem(KLJUC_ZETONA, novZeton)
  }

  function odjava() {
    zeton.value = null
    localStorage.removeItem(KLJUC_ZETONA)
  }

  return { zeton, jePrijavljen, nastaviZeton, odjava }
})
