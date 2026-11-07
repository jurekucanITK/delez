import axios from 'axios'
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

export default odjemalec
