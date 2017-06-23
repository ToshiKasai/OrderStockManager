// Vue関連
import Vue from 'vue'
// ライブラリ
import axios from 'axios'

export const actions = {
  getUsers({ commit, state }) {
    return axios.get("api/users", {
      headers: { "Authorization": "bearer " + Vue.ls.get("bearer", "") },
      params: { "Deleted": true, "Enabled": false }
    }).then((res) => {
      commit('setUsers', res.data)
    }).catch(function (error) {
      commit('setUsers', [])
    })
  },
  getRoles({ commit, state }) {
    return axios.get("api/roles", {
      headers: { "Authorization": "bearer " + Vue.ls.get("bearer", "") },
      params: { "Deleted": true, "Enabled": false }
    }).then((res) => {
      commit('setRoles', res.data)
    }).catch(function (error) {
      commit('setRoles', [])
    })
  },
  getMakers({ commit, state }) {
    return axios.get("api/makers", {
      headers: { "Authorization": "bearer " + Vue.ls.get("bearer", "") },
      params: { "Deleted": true, "Enabled": false }
    }).then((res) => {
      commit('setMakers', res.data)
    }).catch(function (error) {
      commit('setMakers', [])
    })
  },
  setMakers({ commit, state }, maker) {
    return axios.put("api/makers/" + maker.id, maker, {
      headers: { "Authorization": "bearer " + Vue.ls.get("bearer", "") }
    }).then((res) => {
    }).catch(function (error) {
    })
  },
  clearMakers({ commit, state }) {
    return new Promise((resolve, reject) => {
      commit('setMakers', null)
      resolve()
    })

  }
}

export const mutations = {
  setUsers(state, items) {
    if (Array.isArray(items)) {
      state.userList = items
    } else if (typeOf(items) === "object") {
      state.userList = [items]
    } else {
      state.userList = []
    }
  },
  setRoles(state, items) {
    if (Array.isArray(items)) {
      state.roleList = items
    } else if (typeOf(items) === "object") {
      state.roleList = [items]
    } else {
      state.roleList = []
    }
  },
  setMakers(state, items) {
    if (items === null) {
      state.makerList.length = 0
    } else if (Array.isArray(items)) {
      state.makerList = items
    } else if (typeOf(items) === "object") {
      state.makerList = [items]
    } else {
      state.makerList = []
    }
  }
}
