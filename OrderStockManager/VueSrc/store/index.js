// Vue関連
import Vue from 'vue'
import Vuex from 'vuex'
// ライブラリ
import createPersistedState from 'vuex-persistedstate'
// import * as Cookies from 'js-cookie'
// const Cookies = require('js-cookie')

// アプリケーション
import { actions, mutations } from './mutations'
import { getters } from './getters'
import maintenance from './maintenance'

// 設定
Vue.use(Vuex)

export default new Vuex.Store({
  modules: {
    maintenance: maintenance
  },
  actions: actions,
  mutations,
  getters: getters,
  state: {
    nameid: 0,
    roles: [],
    unique_name: null,
    name: null,
    breadlist: [],
    dashboard: []
  },
  plugins: [
    /*
    createPersistedState({
      getState: (key) => Cookies.getJSON(key),
      setState: (key, state) => Cookies.set(key, state, { secure: true })
      // setState: (key, state) => Cookies.set(key, state, { expires: 3, secure: true })
    })
    */
    createPersistedState({ storage: window.sessionStorage })
  ]
})
