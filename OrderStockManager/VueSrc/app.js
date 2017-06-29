"use strict"
// Vue関連
import Vue from 'vue'
import VueRouter from 'vue-router'
import Vuex from 'vuex'
// 追加機能
import ElementUI from 'element-ui'
import locale from 'element-ui/lib/locale/lang/ja'
import VueLocalStorage from 'vue-ls'
import Meta from 'vue-meta'
import VueAxios from 'vue-axios'
import DataTables from 'vue-data-tables'
// ライブラリ
import axios from 'axios'
import moment from 'moment'
import momentLocale from 'moment/locale/ja'
import 'vue2-animate/dist/vue2-animate.min.css'
// アプリケーション
import App from './app.vue'
import routes from './routes'
import store from './store'

// 設定
Vue.use(ElementUI, { locale })
Vue.use(VueRouter)
Vue.use(VueLocalStorage, { namespace: 'vuejs__' })
Vue.use(Meta)
Vue.use(VueAxios, axios)
Vue.use(DataTables)
Vue.config.performance = true

// moment設定
moment.locale('ja')

// axios設定
axios.defaults.baseURL = "http://192.168.6.60/OrderStockManager/"
axios.interceptors.request.use((config) => {
  if (store.getters.isAuthenticated) {
    config.headers['Authorization'] = "bearer " + Vue.ls.get("bearer", "")
  } else {
    delete config.headers['Authorization']
  }
  return config;
}, (error) => {
  return Promise.reject(error)
})
axios.interceptors.response.use((response) => {
  return response;
}, (error) => {
  return Promise.reject(error);
})

// 独自機能設定
import minotaka from './minotakaPlugins'
Vue.use(minotaka)

// フィルタ設定
Vue.filter('converetDateFormat', function (value, format) {
  try {
    var tmp = moment(value)
    var form = 'YYYY/MM/DD(ddd)'
    if (format !== null && format !== undefined) { form = format }
    return tmp.format(form)
  } catch (e) { return '' }
})
Vue.filter('deletedMessage', function (value) {
  if (value) {
    return '削除'
  } else {
    return '－'
  }
})
Vue.filter('boolMessage', function (value, trueMessage, falseMessage) {
  if (trueMessage === null || trueMessage === undefined) { trueMessage = 'true' }
  if (falseMessage === null || falseMessage === undefined) { falseMessage = 'false' }
  if (value) {
    return trueMessage
  } else {
    return falseMessage
  }
})

// ルーター
const router = new VueRouter({
  scrollBehavior: (to, from, savedPosition) => ({ x: 0, y: 0 }),
  routes
})

var app = new Vue({
  router,
  store,
  render: h => h(App)
}).$mount('#app');
