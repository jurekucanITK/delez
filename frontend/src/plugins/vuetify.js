import '@mdi/font/css/materialdesignicons.css'
import 'vuetify/styles'
import { createVuetify } from 'vuetify'

const delez = {
  dark: false,
  colors: {
    background: '#F7F3EC',
    surface: '#FFFDF9',
    'surface-variant': '#ECE6DA',
    'on-surface-variant': '#4A443B',
    'on-background': '#2E2A24',
    'on-surface': '#2E2A24',
    primary: '#4F7058',
    'on-primary': '#FFFFFF',
    secondary: '#A65A3A',
    'on-secondary': '#FFFFFF',
    error: '#B3261E',
    info: '#3F6E8C',
    success: '#3E7A4F',
    warning: '#A86B12',
  },
  variables: {
    'font-body': '"Nunito", system-ui, sans-serif',
    'font-heading': '"Nunito", system-ui, sans-serif',
    'border-color': '#3B342A',
    'border-opacity': 0.14,
    'high-emphasis-opacity': 0.9,
    'medium-emphasis-opacity': 0.68,
  },
}

export default createVuetify({
  theme: {
    defaultTheme: 'delez',
    themes: { delez },
  },
  defaults: {
    global: {
      density: 'comfortable',
    },
    VAppBar: {
      flat: true,
      color: 'primary',
    },
    VCard: {
      flat: true,
      border: true,
      rounded: 'xl',
    },
    VTextField: {
      variant: 'outlined',
      color: 'primary',
      rounded: 'lg',
    },
    VSelect: {
      variant: 'outlined',
      color: 'primary',
      rounded: 'lg',
    },
    VBtn: {
      rounded: 'lg',
      class: 'text-none font-weight-bold',
    },
    VAlert: {
      variant: 'tonal',
      rounded: 'lg',
    },
  },
})
