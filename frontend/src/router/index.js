import { createRouter, createWebHistory } from 'vue-router'
import ZacetnaView from '@/views/ZacetnaView.vue'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      name: 'zacetna',
      component: ZacetnaView,
    },
  ],
})

export default router
