"use strict"
// polyfills
import 'es6-promise/auto'
import 'url-search-params-polyfill';

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
import Vue2Filters from 'vue2-filters'
// ライブラリ
import axios from 'axios'
import moment from 'moment'
import momentLocale from 'moment/locale/ja'
import 'vue2-animate/dist/vue2-animate.min.css'
// アプリケーション
import App from './app.vue'
import routes from './routes'
import store from './store'
import EditNumber from './editNumber.vue'

// 設定
Vue.use(ElementUI, { locale })
Vue.use(VueRouter)
Vue.use(VueLocalStorage, { namespace: 'vuejs__' })
Vue.use(Meta)
Vue.use(VueAxios, axios)
Vue.use(DataTables)
Vue.use(Vue2Filters)
Vue.config.performance = false

// moment設定
moment.locale('ja')

// ルーター
const router = new VueRouter({
  scrollBehavior: (to, from, savedPosition) => ({ x: 0, y: 0 }),
  routes
})

router.onError((error) => {
  ElementUI.Notification.warning({ title: 'NG', message: error.message })
})

router.beforeEach((to, from, next) => {
  let result = true
  let auth = store.getters.isAuthenticated
  let notify = { title: '', message: '' }
  if (to.matched.some(record => record.meta.requiresAuth) && store.getters.isAuthenticated === false) {
    result = false
    notify = { title: 'ログインが確認できません', message: 'ログインを行ってください' }
  }
  if ((to.matched.some(record => record.meta.requiresAdmin) && store.getters.isAdminRole === false)
    || (to.matched.some(record => record.meta.requiresUser) && store.getters.isUserRole === false)
    || (to.matched.some(record => record.meta.requiresMaker) && store.getters.isMakerRole === false)
    || (to.matched.some(record => record.meta.requiresGroup) && store.getters.isGroupRole === false)
    || (to.matched.some(record => record.meta.requiresProduct) && store.getters.isProductRole === false)
    || (to.matched.some(record => record.meta.requiresLogview) && store.getters.isLogviewRole === false)) {
    result = false
    notify = { title: '機能が利用できません', message: '権限が不足しているために機能を利用することが出来ません' }
  }

  if (result === false) {
    // next(new Error("遷移できんぞ"))
    ElementUI.Notification.error(notify)
    if (auth === false) {
      next({ path: '/signin', query: { redirect: to.fullPath } })
    } else {
      next(false)
    }
  } else {
    next()
  }
})

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
  console.log(error)
  if (error.response.status === 401) {
    if (store.getters.isAuthenticated) {
      ElementUI.Notification.error({
        title: '認証情報が無効になりました',
        message: '時間の経過等により認証情報が無効になりました。再度サインインを行ってください。'
      })
    }
    router.push({ path: '/signin', query: { redirect: router.currentRoute.fullPath } })
    store.dispatch('access_token', {})
    return error
  }
  return Promise.reject(error);
})

// 独自機能設定
import minotaka from './minotakaPlugins'
Vue.use(minotaka)

// Vue.mixin({
// })

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

Vue.directive('editinput', {
  update: function (el) {
    //var target = el.children.namedItem('editinput')
    Vue.nextTick(function () {
      //target.focus()
      el.focus()
    })
  },
})

Vue.component('editNumber', EditNumber)

var app = new Vue({
  router,
  store,
  render: h => h(App)
}).$mount('#app');
