import odjemalec from '@/api/odjemalec'

export function registriraj({ ime, email, geslo }) {
  return odjemalec.post('/auth/registracija', { ime, email, geslo })
}
