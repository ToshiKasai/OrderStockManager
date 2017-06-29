// Vue関連
import Vue from 'vue'

export const actions = {
  getUsers({ commit, state }) {
    return Vue.axios.get("api/users", {
      params: { "Deleted": true, "Enabled": false }
    })
  },
  getUser({ commit, state }, id) {
    return Vue.axios.get("api/users/" + id)
  },
  setUser({ commit, state }, user) {
    return Vue.axios.put("api/users/" + user.id, user)
  },
  addUser({ commit, state }, user) {
    return Vue.axios.post("api/users", user)
  },
  getUserRoles({ commit, state }, id) {
    return Vue.axios.get("api/users/" + id + "/roles")
  },
  setUserRoles({ commit, state }, params) {
    return Vue.axios.post("api/users/" + params.id + "/roles", params.roles)
  },
  getUserMakers({ commit, state }, id) {
    return Vue.axios.get("api/users/" + id + "/makers")
  },
  setUserMakers({ commit, state }, params) {
    return Vue.axios.post("api/users/" + params.id + "/makers", params.makers)
  },
  getRoles({ commit, state }) {
    return Vue.axios.get("api/roles", {
      params: { "Deleted": true, "Enabled": false }
    })
  },
  getRoleList({ commit, state }) {
    return Vue.axios.get("api/roles")
  },
  getMakers({ commit, state }) {
    return Vue.axios.get("api/makers", {
      params: { "Deleted": true, "Enabled": false }
    })
  },
  getMakerList(){
    return Vue.axios.get("api/makers")
  },
  setMakers({ commit, state }, maker) {
    return Vue.axios.put("api/makers/" + maker.id, maker)
  },
  getProducts({ commit, products }) {
    return Vue.axios.get("api/products", {
      params: { "Deleted": true, "Enabled": false }
    })
  },
  getProduct({ commit, products }, id) {
    return Vue.axios.get("api/products/" + id)
  },
  setProduct({ commit, state }, product) {
    return Vue.axios.put("api/products/" + product.id, product)
  }
}

export const mutations = {
}
