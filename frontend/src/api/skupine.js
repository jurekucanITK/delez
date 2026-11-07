import odjemalec from '@/api/odjemalec'

export function ustvariSkupino({ ime, opis }) {
  return odjemalec.post('/skupine', { ime, opis })
}

export function pridobiSkupino(id) {
  return odjemalec.get(`/skupine/${id}`)
}
