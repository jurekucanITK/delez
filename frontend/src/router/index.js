import { createRouter, createWebHistory } from 'vue-router'
import { useAvtentikacijaStore } from '@/stores/avtentikacija'
import ZacetnaView from '@/views/ZacetnaView.vue'
import RegistracijaView from '@/views/RegistracijaView.vue'
import PrijavaView from '@/views/PrijavaView.vue'
import SkupineView from '@/views/SkupineView.vue'
import NovaSkupinaView from '@/views/NovaSkupinaView.vue'
import SkupinaView from '@/views/SkupinaView.vue'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      name: 'zacetna',
      component: ZacetnaView,
    },
    {
      path: '/registracija',
      name: 'registracija',
      component: RegistracijaView,
    },
    {
      path: '/prijava',
      name: 'prijava',
      component: PrijavaView,
    },
    {
      path: '/skupine',
      name: 'skupine',
      component: SkupineView,
      meta: { zahtevaPrijavo: true },
    },
    {
      path: '/skupine/nova',
      name: 'nova-skupina',
      component: NovaSkupinaView,
      meta: { zahtevaPrijavo: true },
    },
    {
      path: '/skupine/:id(\\d+)',
      name: 'skupina',
      component: SkupinaView,
      meta: { zahtevaPrijavo: true },
    },
  ],
})

router.beforeEach((to) => {
  const avtentikacija = useAvtentikacijaStore()
  if (to.meta.zahtevaPrijavo && !avtentikacija.jePrijavljen) {
    return { name: 'prijava' }
  }
})

export default router
