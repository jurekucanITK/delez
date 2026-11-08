import odjemalec from '@/api/odjemalec'

export function pridobiSkupine() {
  return odjemalec.get('/skupine')
}

export function ustvariSkupino({ ime, opis }) {
  return odjemalec.post('/skupine', { ime, opis })
}

export function pridobiSkupino(id) {
  return odjemalec.get(`/skupine/${id}`)
}

export function pridobiClane(id) {
  return odjemalec.get(`/skupine/${id}/clani`)
}

export function dodajClana(id, { email, vloga }) {
  return odjemalec.post(`/skupine/${id}/clani`, { email, vloga })
}
