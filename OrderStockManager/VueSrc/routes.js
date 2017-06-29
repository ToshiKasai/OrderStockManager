// Components
import Wellcome from './wellcome.vue'
import SignIn from './components/signin.vue'
import Menu from './components/mainmenu.vue'
// ApplicationComponents
// MaintenanceComponents
import Maintenance from './components/maintenance.vue'
import MainteMenu from './components/maintenance/maintemenu.vue'
import Users from './components/maintenance/users.vue'
import UserEdit from './components/maintenance/useredit.vue'
import UserAdd from './components/maintenance/useradd.vue'
import UserRoles from './components/maintenance/userroles.vue'
import UserMakers from './components/maintenance/usermakers.vue'
import Roles from './components/maintenance/roles.vue'
import Makers from './components/maintenance/makers.vue'
import Products from './components/maintenance/products.vue'

export default [
  { path: '/', component: Wellcome },
  { path: '/signin', component: SignIn, props: (route) => ({ redirect: route.query.redirect }) },
  { path: '/menu', component: Menu, meta: { requiresAuth: true } },
  {
    path: '/mainte', component: Maintenance,
    children: [
      { path: '', redirect: 'maintemenu' },
      { path: 'maintemenu', component: MainteMenu },
      { path: 'users', component: Users },
      { path: 'users/:id/edit', component: UserEdit, name: 'useredit', props: true },
      { path: 'users/add', component: UserAdd, name: 'useradd' },
      { path: 'users/:id/roles', component: UserRoles, name: 'userroles', props: true },
      { path: 'users/:id/makers', component: UserMakers, name: 'usermakers', props: true },
      { path: 'roles', component: Roles },
      { path: 'makers', component: Makers },
      { path: 'products', component: Products }
    ],
    meta: { requiresAuth: true }
  },
  { path: '*', redirect: '/' }
]
