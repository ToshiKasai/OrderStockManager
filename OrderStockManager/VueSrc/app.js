"use strict"
// Vue関連
import Vue from 'vue'
import VueRouter from 'vue-router'
// 追加機能
import ElementUI from 'element-ui'
import locale from 'element-ui/lib/locale/lang/ja'
import VueLocalStorage from 'vue-ls'
import Meta from 'vue-meta'
// ライブラリ
import axios from 'axios'
// アプリケーション
import App from './app.vue'
import routes from './routes'
import store from './store'

// 設定
Vue.use(ElementUI, { locale })
Vue.use(VueRouter)
Vue.use(VueLocalStorage, { namespace: 'vuejs__' })
Vue.use(Meta)
Vue.config.debug = true
axios.defaults.baseURL = "http://localhost/OrderStockManager/"

import 'vue2-animate/dist/vue2-animate.min.css'
// import 'vue2-animate/dist/vue2-animate.css'

// ルート定義
const router = new VueRouter({
  scrollBehavior: (to, from, savedPosition) => ({ x: 0, y: 0 }),
  routes
})

var app = new Vue({
  router,
  store,
  render: h => h(App)
}).$mount('#app');
