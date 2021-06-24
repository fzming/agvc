import {
  upload,
  uploadAll,
  getExpressCompanys,
  getExpressInfo,
  getExpressCompanyName
} from '@/api/common';
const mutations = {
  SET_EXPRESS: (state, express) => {
    state.express = express;
  }
}
const state = {
  express: []
}
const actions = {
  async upload({
    commit
  }, data) {
    !!commit
    var res = await upload(data);
    return res;
  },

  async getExpressCompanys({
    commit
  }, data) {
    !!commit
    const res = await getExpressCompanys(data);
    commit("SET_EXPRESS", res);
    return res;
  },

  async getExpressInfo({
    commit
  }, data) {
    !!commit
    const res = await getExpressInfo(data);

    return res;
  },

  async getExpressCompanyName({
    commit
  }, data) {
    !!commit
    const res = await getExpressCompanyName(data);
    return res;
  },

  async uploadAllAsync({
    commit
  }, data) {
    !!commit
    const res = await uploadAll(data);
    return res;
  }
};

export default {
  namespaced: true,
  state,
  mutations,
  actions
};
