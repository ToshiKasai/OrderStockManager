﻿// Vue関連
import Vue from 'vue'
// ライブラリ
import axios from 'axios'

function decode_base64(base64string) {
  let missing_padding = base64string.length % 4
  if (missing_padding != 0) {
    base64string = base64string + Array(1 + 4 - missing_padding).join('=')
  }
  return new Buffer(base64string, 'base64').toString()
}

function readJwtPayload(jwtToken) {
  var base64Url = jwtToken.split('.')[1]
  var jsonString = decode_base64(base64Url)
  return JSON.parse(jsonString)
}

var toString = Object.prototype.toString
function typeOf(obj) {
  return toString.call(obj).slice(8, -1).toLowerCase()
}

Number.isNaN = Number.isNaN || function (obj) {
  // typeof NaN === 'number' -> true と、
  // NaN !== NaN -> true を利用する
  return typeof obj === 'number' && obj !== obj
}

export const actions = {
  postSignin({ commit, state }, params) {
    if (params == null) return
    if (!params.hasOwnProperty('inputId')) return
    if (!params.hasOwnProperty('password')) return

    let fparam = new URLSearchParams()
    fparam.append('grant_type', 'password')
    fparam.append('username', params.inputId)
    fparam.append('password', params.password)
    fparam.append('client_id', Vue.ls.get("clientId", null))

    return axios.post("oauth/token", fparam).then((res) => {
      Vue.ls.set("bearer", res.data.access_token, res.data.expires_in * 1000)
      Vue.ls.set("refresh", res.data.refresh_token)
      var payload = readJwtPayload(res.data.access_token)

      commit('setNameId', payload.nameid)
      commit('setRoles', payload.role)
      commit('setUniqueName', payload.unique_name)
      commit('setName', payload.name)
    }).catch(function (error) {
      commit('setNameId', 0)
      commit('setRoles', [])
      commit('setUniqueName', null)
      commit('setName', null)
    })
  },
  postSignOut({ commit, state }) {
    return new Promise((resolve, reject) => {
      setTimeout(() => {
        commit('setNameId', 0)
        commit('setRoles', [])
        commit('setUniqueName', null)
        commit('setName', null)
        resolve()
      }, 3000)
    })
  },
  getDashboard({ commit, state }) {
    return axios.get("api/dashboards", {
      headers: { "Authorization": "bearer " + Vue.ls.get("bearer", "") }
    }).then((response) => {
      commit('setDashboard', response.data)
    }).catch(function (error) {
      commit('setDashboard', null)
    })
  }
}

export const mutations = {
  setNameId(state, item) {
    if (typeOf(item) === "number" && !Number.isNaN(item)) {
      state.nameid = item
    } else if (typeOf(item) === "string" && /^([1-9]\d*|0)$/.test(item)) {
      state.nameid = item | 0
    } else {
      state.nameid = 0
    }
  },
  setRoles(state, items) {
    if (Array.isArray(items)) {
      state.roles = items
    } else if (typeOf(items) === "string") {
      state.roles = [items]
    } else {
      state.roles = []
    }
  },
  setUniqueName(state, item) {
    if (typeOf(item) === "string") {
      state.unique_name = item
    } else {
      state.unique_name = null
    }
  },
  setName(state, item) {
    if (typeOf(item) === "string") {
      state.name = item
    } else {
      state.name = null
    }
  },
  setDashboard(state, items) {
    state.dashboard = items;
  },
  setBreadcrumb(state, items) {
    if (Array.isArray(items)) {
      state.breadlist = items
    } else if (typeOf(items) === "object") {
      state.breadlist = [items]
    } else {
      state.breadlist = []
    }
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
