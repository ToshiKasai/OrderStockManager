// アプリケーション
import Vue from 'vue'
import { decode_base64, readJwtPayload } from '../../libraries/decodes'

export default {
  actions: {
    access_token: (context, param) => {
      if (param.hasOwnProperty('access_token')) {
        Vue.ls.set("bearer", param.access_token, param.expires_in * 1000)
        let payload = readJwtPayload(param.access_token)
        context.commit('userId', payload.nameid)
        context.commit('roles', payload.role)
        context.commit('siginId', payload.unique_name)
        context.commit('fullName', payload.name)
      } else {
        Vue.ls.remove('bearer')
        context.commit('userId', 0)
        context.commit('roles', [])
        context.commit('siginId', null)
        context.commit('fullName', null)
      }
    },
    refresh_token: (contex, param) => {
      if (param.hasOwnProperty('refresh')) {
        Vue.ls.set("refresh", param.refresh_token)
      } else {
        Vue.ls.remove('refresh')
      }
    },
    signin: ({ dispatch, commit, state }, params) => {
      if (params == null) return
      if (!params.hasOwnProperty('inputId')) return
      if (!params.hasOwnProperty('password')) return

      let fparam = new URLSearchParams()
      fparam.append('grant_type', 'password')
      fparam.append('username', params.inputId)
      fparam.append('password', params.password)
      fparam.append('client_id', Vue.ls.get("clientId", null))

      return Vue.axios.post("oauth/token", fparam)
        .then((response) => {
          var data = response.data
          dispatch('access_token', data)
          dispatch('refresh_token', data)
        })
        .catch((error) => {
          dispatch('access_token', {})
        })
    },
    signOut: (context) => {
      return new Promise((resolve, reject) => {
        setTimeout(() => {
          context.dispatch('access_token', {})
          resolve()
        }, 3000)
      })
    },
    refreshToken: () => {
      let fparam = new URLSearchParams()
      fparam.append('grant_type', 'refresh_token')
      fparam.append('refresh_token', Vue.ls.get("refresh", null))
      fparam.append('client_id', Vue.ls.get("clientId", null))

      return Vue.axios.post("oauth/token", fparam)
        .then((response) => {
          var data = response.data
          dispatch('access_token', data)
          dispatch('refresh_token', data)
        })
        .catch((error) => {
          dispatch('access_token', {})
        })
    }
  },
  mutations: {
    userId: (state, item) => {
      if (Vue.minotaka.checkType(item, "number") && !Vue.minotaka.isNaN(item)) {
        state.userId = item
      } else if (Vue.minotaka.checkType(item, "string") && /^([1-9]\d*|0)$/.test(item)) {
        state.userId = item | 0
      } else {
        state.userId = 0
      }
    },
    roles(state, items) {
      state.roles = Vue.minotaka.makeArray(items, "string")
    },
    siginId(state, item) {
      state.signinId = Vue.minotaka.makeSingleType(item, "string")
    },
    fullName(state, item) {
      state.fullName = Vue.minotaka.makeSingleType(item, "string")
    },

  },
  getters: {
    isAuthenticated: (state) => {
      return state.userId !== 0
    },
    siginId: (state, getters) => {
      if (!getters.isAuthenticated) {
        return null;
      }
      return state.signinId || '未定義';
    },
    fullName: (state, getters) => {
      if (!getters.isAuthenticated) {
        return null;
      }
      return state.fullName || '名前不明';
    },
    isAdmin: (state, getters) => {
      return getters.isAuthenticated && state.roles.indexOf("admin") >= 0
    }
  },
  state: {
    userId: 0,
    signinId: null,
    fullName: null,
    roles: []
  }
}
