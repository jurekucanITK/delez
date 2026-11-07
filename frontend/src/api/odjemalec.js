import axios from 'axios'
import router from '@/router'
import { useAvtentikacijaStore } from '@/stores/avtentikacija'

const odjemalec = axios.create({
  baseURL: import.meta.env.VITE_API_URL ?? 'http://localhost:5000/api',
})

odjemalec.interceptors.request.use((config) => {
  const avtentikacija = useAvtentikacijaStore()
  if (avtentikacija.zeton) {
    config.headers.Authorization = `Bearer ${avtentikacija.zeton}`
  }
  return config
})

odjemalec.interceptors.response.use(
  (odgovor) => odgovor,
  (napaka) => {
    const jePrijava = napaka.config?.url === '/auth/prijava'
    if (napaka.response?.status === 401 && !jePrijava) {
      useAvtentikacijaStore().odjava()
      router.push({ name: 'prijava' })
    }
    return Promise.reject(napaka)
  },
)

export default odjemalec
