export const getters = {
  getUserList(state, getters, rootState) {
    return state.userList;
  },
  getRoleList(state, getters, rootState) {
    return state.roleList;
  },
  getMakerList(state, getters, rootState) {
    return state.makerList;
  }
}
