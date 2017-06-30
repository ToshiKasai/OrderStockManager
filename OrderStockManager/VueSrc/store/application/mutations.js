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
  getProducts({ commit, products }) {
    return Vue.axios.get("api/products")
  }
}

export const mutations = {
  selectMaker(state, item){
    state.selectMaker = item
  },
  selectMyMaker(state, item){
    state.selectMyMaker = item
  }
}
