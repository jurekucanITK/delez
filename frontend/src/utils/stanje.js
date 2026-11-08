const evri = new Intl.NumberFormat('sl-SI', { style: 'currency', currency: 'EUR' })

export function oblikujStanje(stanje) {
  const znesek = evri.format(Math.abs(stanje))
  if (stanje > 0) return `+ ${znesek}`
  if (stanje < 0) return `− ${znesek}`
  return znesek
}

export function barvaStanja(stanje) {
  if (stanje > 0) return 'text-success'
  if (stanje < 0) return 'text-secondary'
  return ''
}
