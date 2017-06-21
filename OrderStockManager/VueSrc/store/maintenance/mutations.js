import Vue from 'vue';
import VueLocalStorage from 'vue-ls';
Vue.use(VueLocalStorage, { namespace: 'vuejs__' });

import axios from 'axios';

export const actions = {
    getUsers({ commit, state }) {
        axios.defaults.baseURL = "http://localhost/OrderStockManager/";
        return axios.get("api/users", {
            headers: {
                "Authorization": "bearer " + Vue.ls.get("bearer", "")
            },
            params: {
                "Deleted": true,
                "Enabled": false
            }
        }).then((res) => {
            commit('setUsers', res.data)
        }).catch(function (error) {
            commit('setUsers', [])
        });
    },
}

export const mutations = {
    setUsers(state, items) {
        if (Array.isArray(items)) {
            state.userList = items;
        } else if (typeOf(items) === "object") {
            state.userList = [items];
        } else {
            state.userList = [];
        }
    }
}
