import { createRouter, createWebHistory } from 'vue-router'
import ZacetnaView from '@/views/ZacetnaView.vue'
import RegistracijaView from '@/views/RegistracijaView.vue'
import PrijavaView from '@/views/PrijavaView.vue'

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
  ],
})

export default router
