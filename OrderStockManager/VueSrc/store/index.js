// Vue関連
import Vue from 'vue'
import Vuex from 'vuex'
// ライブラリ
import createPersistedState from 'vuex-persistedstate'

// アプリケーション
import { actions, mutations } from './mutations'
import { getters } from './getters'
import maintenance from './maintenance'
import auth from './auth'
import app from './application'

// 設定
Vue.use(Vuex)

export default new Vuex.Store({
  modules: {
    maintenance: maintenance,
    authentication: auth,
    application: app
  },
  actions: actions,
  mutations,
  getters: getters,
  state: {
    breadlist: [],
    fullLoadingShow: false,
    loadingShow: false,
    mainteLoadingShow: false,
    loadingMessage: null,
    activeIndex: ""
  },
  plugins: [
    createPersistedState({ storage: window.sessionStorage })
  ]
})
