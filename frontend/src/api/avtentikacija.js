import odjemalec from '@/api/odjemalec'

export function registriraj({ ime, email, geslo }) {
  return odjemalec.post('/auth/registracija', { ime, email, geslo })
}

export function prijavi({ email, geslo }) {
  return odjemalec.post('/auth/prijava', { email, geslo })
}

export function pridobiJaz() {
  return odjemalec.get('/auth/jaz')
}
