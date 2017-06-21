import Vue from 'vue';
import VueRouter from 'vue-router';

import App from './app.vue';
import routes from './routes'

import ElementUI from 'element-ui';
import locale from 'element-ui/lib/locale/lang/ja';

import store from './store';

import VueLocalStorage from 'vue-ls';

Vue.use(ElementUI, { locale });
Vue.use(VueRouter);
Vue.use(VueLocalStorage, { namespace: 'vuejs__' });

Vue.config.debug = true

const router = new VueRouter({
    scrollBehavior: () => ({ y: 0 }),
    routes
})

var app = new Vue({
    router,
    store,
    render: h => h(App)
}).$mount('#app');
