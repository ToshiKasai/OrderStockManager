// Vue関連
import Vue from 'vue'
// ライブラリ
import axios from 'axios'

export const actions = {
  getDashboard({ commit, state }) {
    return Vue.axios.get("api/dashboards")
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
  }
}
