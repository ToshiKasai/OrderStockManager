// Vue関連
import Vue from 'vue'
// ライブラリ
import axios from 'axios'

export const actions = {
  getDashboard({ commit, state }) {
    return Vue.axios.get("api/dashboards")
  },
  endLoading({ commit }) {
    commit('fullLoadingShow', false)
    commit('loadingShow', false)
    commit('mainteLoadingShow', false)
    commit('loadingMessage', "")
  },
  nowLoading({ commit }, message) {
    commit('loadingMessage', message)
    commit('loadingShow', true)
  },
  nowLoadingFull({ commit }, message) {
    commit('loadingMessage', message)
    commit('fullLoadingShow', true)
  },
  nowLoadingMainte({ commit }, message) {
    commit('loadingMessage', message)
    commit('mainteLoadingShow', true)
  }
}

export const mutations = {
  setBreadcrumb(state, items) {
    state.breadlist = Vue.minotaka.makeArray(items)
  },
  changeBreadcrumb(state, item) {
    var count = state.breadlist.length
    var newlist = []
    for (var i = 0; i < count; i++) {
      if (state.breadlist[i].path === item.path) {
        break
      }
      newlist.push(state.breadlist[i])
    }
    newlist.push(item)
    state.breadlist = newlist
  },
  fullLoadingShow(state, item) {
    state.fullLoadingShow = item
  },
  loadingShow(state, item) {
    state.loadingShow = item
  },
  mainteLoadingShow(state, item) {
    state.mainteLoadingShow = item
  },
  loadingMessage(state, item) {
    state.loadingMessage = item
  }
}
