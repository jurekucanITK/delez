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

export function pridobiStroske(id) {
  return odjemalec.get(`/skupine/${id}/stroski`)
}

export function pridobiStanja(id) {
  return odjemalec.get(`/skupine/${id}/stanja`)
}

export function pridobiPredlogNakazil(id) {
  return odjemalec.get(`/skupine/${id}/predlog-nakazil`)
}

export function pridobiNakazila(id) {
  return odjemalec.get(`/skupine/${id}/nakazila`)
}

export function dodajNakazilo(id, { placnikId, prejemnikId, znesek, datum }) {
  return odjemalec.post(`/skupine/${id}/nakazila`, { placnikId, prejemnikId, znesek, datum })
}

export function urediNakazilo(id, nakaziloId, { placnikId, prejemnikId, znesek, datum }) {
  return odjemalec.put(`/skupine/${id}/nakazila/${nakaziloId}`, {
    placnikId,
    prejemnikId,
    znesek,
    datum,
  })
}

export function izbrisiNakazilo(id, nakaziloId) {
  return odjemalec.delete(`/skupine/${id}/nakazila/${nakaziloId}`)
}

export function dodajStrosek(id, { opis, znesek, datum, placnikId, claniIds }) {
  return odjemalec.post(`/skupine/${id}/stroski`, { opis, znesek, datum, placnikId, claniIds })
}

export function pridobiStrosek(id, strosekId) {
  return odjemalec.get(`/skupine/${id}/stroski/${strosekId}`)
}

export function urediStrosek(id, strosekId, { opis, znesek, datum, placnikId, claniIds }) {
  return odjemalec.put(`/skupine/${id}/stroski/${strosekId}`, {
    opis,
    znesek,
    datum,
    placnikId,
    claniIds,
  })
}

export function izbrisiStrosek(id, strosekId) {
  return odjemalec.delete(`/skupine/${id}/stroski/${strosekId}`)
}
