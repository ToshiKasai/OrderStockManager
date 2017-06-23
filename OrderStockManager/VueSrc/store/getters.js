export const getters = {
  getUniqName(state) {
    if (state.nameid === 0) {
      return null;
    }
    return state.unique_name || '未定義';
  },
  getName(state) {
    if (state.nameid === 0) {
      return null;
    }
    return state.name || '未定義';
  }
}
