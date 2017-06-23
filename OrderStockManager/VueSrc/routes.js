// Components
import Wellcome from './wellcome.vue'
import SignIn from './components/signin.vue'
import Menu from './components/mainmenu.vue'
// ApplicationComponents
// MaintenanceComponents
import Maintenance from './components/maintenance.vue'
import MainteMenu from './components/maintenance/maintemenu.vue'
import Users from './components/maintenance/users.vue'
import Roles from './components/maintenance/roles.vue'
import Makers from './components/maintenance/makers.vue'

export default [
  { path: '/', component: Wellcome },
  { path: '/signin', component: SignIn },
  { path: '/menu', component: Menu },
  {
    path: '/mainte', component: Maintenance,
    children: [
      { path: '/', redirect: '/maintemenu' },
      { path: '/maintemenu', component: MainteMenu },
      { path: '/users', component: Users },
      { path: '/roles', component: Roles },
      { path: '/makers', component: Makers }
    ]
  },
  { path: '*', redirect: '/' }
]
