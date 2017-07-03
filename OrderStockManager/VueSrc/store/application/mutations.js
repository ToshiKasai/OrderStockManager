// Vue関連
import Vue from 'vue'

export const actions = {
  getMakers({ commit, state }) {
    return Vue.axios.get("api/makers")
  },
  getMyMakers({ commit, state }, id) {
    return Vue.axios.get("api/users/" + id + "/makers")
  },
  getGroupsByMakerId({ commit, state }, id) {
    return Vue.axios.get("api/groups", {
      params: { "MakerId": id }
    })
  },
  getProductsByMakerId({ commit, products }, id) {
    return Vue.axios.get("api/products", {
      params: { "MakerId": id }
    })
  },
  getProductsByGroupId({ commit, products }, id) {
    return Vue.axios.get("api/products", {
      params: { "GroupId": id }
    })
  },
  getSalesviewsByGroupId({ commit, products }, params) {
    return Vue.axios.get("api/salesviews", {
      params: { "GroupId": params.id, "Year": params.year }
    })
  },
  getSalesviewsByMakerId({ commit, products }, params) {
    return Vue.axios.get("api/salesviews", {
      params: { "MakerId": params.id, "Year": params.year }
    })
  }
}

export const mutations = {
  selectMaker(state, item) {
    state.selectMaker = item
  },
  selectGroup(state, item) {
    state.selectGroup = item
  },
  selectMyMaker(state, flags) {
    state.selectMyMaker = flags
  }
}
