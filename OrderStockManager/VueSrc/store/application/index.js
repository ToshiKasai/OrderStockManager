// アプリケーション
import { actions, mutations } from './mutations';
import { getters } from './getters';

export default {
  namespaced: false,
  actions: actions,
  mutations: mutations,
  getters: getters,
  state: {
    selectMyMaker: true,
    selectMaker: null,
    selectGroup: null
  }
}
