export const getters = {
  getBreadlist(state, getters, rootState) {
    return state.breadlist
  },
  nowLoadingFull(state, getters, rootState) {
    return state.fullLoadingShow
  },
  nowLoading(state, getters, rootState) {
    return state.loadingShow
  },
  nowLoadingMainte(state, getters, rootState) {
    return state.mainteLoadingShow
  },
  loadingMessage(state, getters, rootState) {
    return state.loadingMessage || "処理中です"
  }
}
