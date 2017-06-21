import Wellcome from './wellcome.vue'
import SignIn from './components/signin.vue'
import Menu from './components/mainmenu.vue'
import Maintenance from './components/maintenance.vue'

import MainteMenu from './components/maintenance/maintemenu.vue'
import Users from './components/maintenance/users.vue'

export default [
    {
        path: '/',
        component: Wellcome
    },
    {
        path: '/signin',
        component: SignIn
    },
    {
        path: '/menu',
        component: Menu
    },
    {
        path: '/mainte',
        component: Maintenance,
        children: [
            {
                path: '/',
                component: MainteMenu
            },
            {
                path: '/users',
                component: Users
            },
            {
                path: '/roles',
                component: MainteMenu
            }
        ]
    },
    {
        path: '*',
        redirect: '/'
    }
]
