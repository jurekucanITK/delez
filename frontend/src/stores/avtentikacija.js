import { defineStore } from 'pinia'
import { computed, ref } from 'vue'

const KLJUC_ZETONA = 'delez_zeton'
const KLJUC_UPORABNIKA = 'delez_uporabnik'

function preberiUporabnika() {
  try {
    return JSON.parse(localStorage.getItem(KLJUC_UPORABNIKA))
  } catch {
    return null
  }
}

export const useAvtentikacijaStore = defineStore('avtentikacija', () => {
  const zeton = ref(localStorage.getItem(KLJUC_ZETONA))
  const uporabnik = ref(preberiUporabnika())

  const jePrijavljen = computed(() => !!zeton.value)

  function prijava(novZeton, novUporabnik) {
    zeton.value = novZeton
    uporabnik.value = novUporabnik
    localStorage.setItem(KLJUC_ZETONA, novZeton)
    localStorage.setItem(KLJUC_UPORABNIKA, JSON.stringify(novUporabnik))
  }

  function odjava() {
    zeton.value = null
    uporabnik.value = null
    localStorage.removeItem(KLJUC_ZETONA)
    localStorage.removeItem(KLJUC_UPORABNIKA)
  }

  return { zeton, uporabnik, jePrijavljen, prijava, odjava }
})
