// Vue関連
import Vue from 'vue'
// ライブラリ
import axios from 'axios'

export class myRestSupport {
  constructor(){  }
  get(url, params){
    return axios.get("api/users", {
      headers: { "Authorization": "bearer " + Vue.ls.get("bearer", "") },
      params: params
    }).then((response) => {
      commit('setUsers', response.data)
    }).catch(function (error) {
      commit('setUsers', [])
      if(error.response.status === 401){
      }
    })
  }
}
