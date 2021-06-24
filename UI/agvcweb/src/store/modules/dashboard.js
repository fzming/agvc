import dashboardApi from '@/api/dashboard'
const state = {
  month_teu_axis: [],
  user_teu_axis: [],
  company_teu_axis: [],
  month_cost_axis: []
}

const mutations = {
  UPDATE_STATE: (state, d = { name: '', count: 0 }) => {
    state[d.name] = [...d.count]
  }
}

const actions = {
  async axisFetchAsync({ commit }, ap = { api: '', param: {}}) {
    var count = await dashboardApi[ap.api](ap.param)
    // console.log(r);
    commit('UPDATE_STATE', {
      name: ap.api,
      count: count
    })
    return count;
  }
}

export default {
  namespaced: true,
  state,
  mutations,
  actions
}
