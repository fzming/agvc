import AccountApi from "@/api/account";
const state = {
  salesmans: {},
};
const mutations = {
  SET_SALESMANS: (state, salesmans) => {
    state.salesmans = salesmans;
  },
};

const actions = {
  async query({ commit }, data) {
    !!commit;
    const ps = await AccountApi["QueryAccountUsersAsync"](data);
    commit("SET_SALESMANS", ps.datas);
    return ps;
  },
  // 机构用户分页数据
  async QueryAccountUsersAsync({ commit }, data) {
    !!commit;
    var res = await AccountApi["QueryAccountUsersAsync"](data);
    return res;
  },
  // // 新增机构用户
  async CreateAccountUserAsync({ commit }, data) {
    !!commit;
    var res = await AccountApi["CreateAccountUserAsync"](data);
    return res;
  },
  // 删除机构用户
  async DeleteAccountUserAsync({ commit }, data) {
    !!commit;
    var res = await AccountApi["DeleteAccountUserAsync"](data);
    return res;
  },
  // 修改机构用户
  async UpdateAccountUserAsync({ commit }, data) {
    !!commit;
    var res = await AccountApi["UpdateAccountUserAsync"](data);
    return res;
  },
  // 根据关键字查询机构用户（自动完成）
  async identity_users({ commit }, data) {
    !!commit;
    var res = await AccountApi["identity_users"](data);
    return res;
  },
  // 密码修改
  async accchange_password({ commit }, data) {
    !!commit;
    var res = await AccountApi["accchange_password"](data);
    return res;
  },
  // 基础资料修改
  async accupdate_profile({ commit }, data) {
    !!commit;
    var res = await AccountApi["accupdate_profile"](data);
    return res;
  },
  // 查询当前登录的账号
  async accprofile({ commit }, data) {
    !!commit;
    var res = await AccountApi["accprofile"](data);
    return res;
  },
  //查询当前登录账号的信息
  async unbind_wx({ commit }, data) {
    !!commit;
    var res = await AccountApi["unbind_wx"](data);
    return res;
  },
  // 获取短信验证码
  async getfetchSmsSecurity({ commit }, data) {
    !!commit;
    var res = await AccountApi["getfetchSmsSecurity"](data);
    return res;
  },
  async bind_mobile({ commit }, data) {
    !!commit;
    var res = await AccountApi["bind_mobile"](data);
    return res;
  },
  async unbind_mobile({ commit }, data) {
    !!commit;
    var res = await AccountApi["unbind_mobile"](data);
    return res;
  },
};

export default {
  namespaced: true,
  mutations,
  state,
  actions,
};
