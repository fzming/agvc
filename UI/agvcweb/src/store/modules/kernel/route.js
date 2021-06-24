import { addRole, remove, modify, orderUpdate } from "@/api/route";

// import {
//   getToken
// } from '@/utils/auth';

const state = {
  token: {}, //getToken(),
  token_refreshing: false,
  name: "",
  avatar: "",
  introduction: "",
  roles: [],
};

//   PaMenuId,
//   Name,
//   Path,
//   Component,
//   Meta,
//   Redirect,
//   AlwaysShow,
//   Hidden
const mutations = {
  ["SET_TK_REFRESHING"](state, payload) {
    state.token_refreshing = payload;
  },
};
const actions = {
  async create({ commit }, data) {
    !!commit;
    return await addRole(data);
  },
  async update({ commit }, data) {
    !!commit;
    return await modify(data);
  },
  async remove({ commit }, id) {
    !!commit;
    return await remove({
      id: id.id,
    });
  },
  async upDown({ commit }, data) {
    !!commit;
    return await orderUpdate(data);
  },
  set_token_refreshing({ commit }, payload) {
    console.log("[VUEX ACTION] SET_TK_REFRESHING", payload);
    commit("SET_TK_REFRESHING", payload);
  },
};

export default {
  namespaced: true,
  state,
  mutations,
  actions,
};
