const VZOREC_ZNESKA = /^\d+([.,]\d+)?$/

export function preberiZnesek(v) {
  return Number(v.trim().replace(',', '.'))
}

export function vnosZneska(znesek) {
  return Number(znesek).toFixed(2).replace('.', ',')
}

export const pravilaZneska = [
  (v) => !!v?.trim() || 'Znesek je obvezen',
  (v) => VZOREC_ZNESKA.test(v.trim()) || 'Vnesi znesek kot število, na primer 12,50',
  (v) => preberiZnesek(v) > 0 || 'Znesek mora biti večji od 0,00 €',
  (v) => /^\d+([.,]\d{1,2})?$/.test(v.trim()) || 'Znesek ima lahko največ dve decimalki',
  (v) => preberiZnesek(v) <= 99999999.99 || 'Znesek je lahko največ 99.999.999,99 €',
]
