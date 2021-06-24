import userApi from "@/api/user";
import {
  resetRouter
} from "@/router";
// import { setUserModules, getUserModules } from "@/utils/auth";

function tokenWrapper(token, t) {
  return {
    token: {
      access_token: token.access_token,
      token_type: "bearer",
      expires_in: token.expires_in,
      refresh_token: token.refresh_token,
    },
    userType: t,
  };
}

function initState() {
  return {
    token: {},
    userType: "",
    //-----------------------
    userModules: [],
    currentModules: {},
    name: "",
    avatar: "",
    introduction: "",
    roles: [],
    orgId: "",
    orgName: "",
    id: "",
    mobile: "",
    boxOwnerIds: [],
    needSafetyValidation: false,
    needChangePassword: false,
    connectionAgents: [],
    connectionId: "",
    features: {},
  };
}

const mutations = {
  SET_TOKEN: (state, token) => {
    state.token = token;
    state.userType = token.userType;
  },
  SET_INTRODUCTION: (state, introduction) => {
    state.introduction = introduction;
  },
  SET_NAME: (state, name) => {
    state.name = name;
  },
  SET_AVATAR: (state, avatar) => {
    state.avatar = avatar;
  },
  SET_ROLES: (state, roles) => {
    state.roles = roles;
  },
  // ORGID,ORGNAME
  SET_ORGID: (state, orgId) => {
    state.orgId = orgId;
  },
  SET_ORGNAME: (state, orgName) => {
    state.orgName = orgName;
  },
  SET_ID: (state, id) => {
    state.id = id;
  },
  SET_MOBILE: (state, mobile) => {
    state.mobile = mobile;
  },
  SET_NEED_SAFETY_VALIDATION: (state, b) => {
    state.needSafetyValidation = b;
  },
  SET_NEED_CHANGE_PASSWORD: (state, b) => {
    state.needChangePassword = b;
  },
  SET_CONNECTION_AGENTS(state, agents) {
    state.connectionAgents = agents;
  },
  SET_CONNECTION_ID(state, connectionId) {
    state.connectionId = connectionId;
  },
  SET_USERMODELS(state, modules) {
    state.userModules = modules.sort();
    state.currentModules = 0;
  },
  SET_CURRENTMODULES(state, idx) {
    state.currentModules = idx;
    //setUserModules(idx);
    // const a = getUserModules();
    // console.log(a);
  },
  SET_USERBOXOWNERIDS(state, ids) {
    state.boxOwnerIds = ids;
  },
  SET_FEATURES(state, features) {
    state.features = features;
  },
};

const actions = {
  async login({
    commit
  }, userInfo) {
    const {
      username,
      password,
      grant_type,
      loginType,
      vaptchaToken,
      remember,
    } = userInfo;
    try {
      const a = await userApi.login({
        username: username.trim(),
        password: password,
        grant_type: grant_type,
        loginType: loginType,
        vaptchaToken: vaptchaToken,
        remember: remember,
      });
      if (a) {
        // console.log('===登录===', a)
        var d = tokenWrapper(a, "Admin");
        commit("SET_TOKEN", d);
        // setToken(d, remember);
        // setUserType(d.userType);
        commit("SET_CURRENTMODULES", 0);
        return a;
      }
    } catch (e) {
      // console.log(e.message)
      if (e.message === "NeedSafetyValidation") {
        commit("SET_NEED_SAFETY_VALIDATION", true);
      }
    }
  },
  setNeedSafetyValidation({
    commit
  }, bvalidation) {
    commit("SET_NEED_SAFETY_VALIDATION", bvalidation);
  },
  setConnectionAgents({
    commit
  }, agents) {
    commit("SET_CONNECTION_AGENTS", agents);
  },
  setConnectionId({
    commit
  }, connectionId) {
    commit("SET_CONNECTION_ID", connectionId);
  },

  async kickOffDeviceAsync({
    commit
  }, data) {
    !!commit;
    await userApi.kickOffDeviceAsync(data);
  },

  async orglogin({
    commit
  }, userInfo) {
    const {
      domain,
      username,
      password,
      grant_type,
      loginType,
      vaptchaToken,
      remember,
      appIndentify,
      appUserInfo,
    } = userInfo;
    try {
      let model = {
        domain: domain,
        username: username.trim(),
        password: password,
        grant_type: grant_type,
        loginType: loginType,
        vaptchaToken: vaptchaToken,
        remember: remember,
      };
      //微信扫码登录绑定
      if (appIndentify) {
        Object.assign(model, appIndentify);
      }
      if (appUserInfo) {
        Object.assign(model, {
          userInfo: JSON.stringify(appUserInfo),
        });
      }
      const a = await userApi.orglogin(model);
      if (a) {
        var d = tokenWrapper(a, "Account");
        commit("SET_TOKEN", d);
        // setToken(d, remember);
        // setUserType(d.userType);
        commit("SET_CURRENTMODULES", 0);
      }

      return a;
    } catch (e) {
      // console.log(e.message)
      if (e.message === "NeedSafetyValidation") {
        commit("SET_NEED_SAFETY_VALIDATION", true);
      }
    }
  },
  async wxlogin({
    commit
  }, model) {
    try {
      let loginParam = {
        domain: model.domain,
        username: model.appId,
        password: model.openId,
        unionId: model.unionId,
        grant_type: "password",
        loginType: "wxlogin",
      };
      const a = await userApi.orglogin(loginParam);
      if (a) {
        var d = tokenWrapper(a, "Account");
        commit("SET_TOKEN", d);
        // setToken(d, false);
        // setUserType(d.userType);
        commit("SET_CURRENTMODULES", 0);
      }

      return a;
    } catch (e) {
      // console.log(e.message)
      if (e.message === "NeedSafetyValidation") {
        commit("SET_NEED_SAFETY_VALIDATION", true);
      }
    }
  },
  updateFeatures({
    commit
  }, features) {
    commit("SET_FEATURES", features);
  },
  // get user info
  async getInfo({
    commit
  }) {
    const a = await userApi.getInfo();
    const data = a;
    if (!data) {
      // reject('Verification failed, please Login again.')
    }
    const {
      roles,
      nick,
      avatar,
      introduction,
      orgId,
      orgName,
      mobile,
      id,
      needChangePassword,
      features,
    } = data;

    if (!roles || roles.length <= 0) {
      // reject('getInfo: roles must be a non-null array!')
    }
    const modules = data.modules;
    commit("SET_ROLES", roles);
    commit("SET_NAME", nick);
    commit("SET_AVATAR", avatar);
    commit("SET_INTRODUCTION", introduction);
    commit("SET_ORGID", orgId);
    commit("SET_ORGNAME", orgName);
    commit("SET_MOBILE", mobile);
    commit("SET_ID", id);
    commit("SET_NEED_CHANGE_PASSWORD", needChangePassword);
    commit("SET_USERMODELS", modules);
    commit("SET_FEATURES", features);
    return a;
  },
  async getInfoOrg({
    commit
  }) {
    const data = await userApi.getInfoOrg();

    const roles = data.roles;
    const name = data.nick;
    const avatar = data.avatar;
    const introduction = data.introduction;
    const orgId = data.orgId;
    const orgName = data.orgName;
    const mobile = data.mobile;
    const id = data.id;
    const needChangePassword = data.needChangePassword;
    const modules = data.modules;
    const boxOwnerIds = data.boxOwnerIds;
    const features = data.features;
    if (!roles || roles.length <= 0) {
      // reject('getInfo: roles must be a non-null array!')
    }
    commit("SET_ROLES", roles);
    commit("SET_NAME", name);
    commit("SET_AVATAR", avatar);
    commit("SET_INTRODUCTION", introduction);
    commit("SET_ORGID", orgId);
    commit("SET_ORGNAME", orgName);
    commit("SET_MOBILE", mobile);
    commit("SET_ID", id);
    commit("SET_NEED_CHANGE_PASSWORD", needChangePassword);
    commit("SET_USERMODELS", modules);
    commit("SET_USERBOXOWNERIDS", boxOwnerIds);
    commit("SET_FEATURES", features);
    return data;
  },
  setNeedChangePassword({
    commit
  }, need) {
    commit("SET_NEED_CHANGE_PASSWORD", need);
  },
  // user logout
  logout({
    commit
  }) {
    commit("SET_TOKEN", {
      token: null,
      userType: null,
    });
    commit("SET_ROLES", []);
    resetRouter();
    window.sessionStorage.clear();
  },
  async refresh_ticket({
    commit,
    state
  }, data) {
    !!commit;
    var res = await userApi.refresh_ticket(data);
    console.log("refresh_ticket", res);
    if (res) {
      var d = tokenWrapper(res, state.userType);
      commit("SET_TOKEN", d);
    }   
    return res;
  },

  // remove token
  async resetToken({
    commit
  }) {
    console.log("resetToken");
    return new Promise((resolve) => {
      commit("SET_TOKEN", {
        token: null,
        userType: null,
      });
      commit("SET_ROLES", []);
      resolve();
    });
  },

  changeAvatar({
    commit
  }, avatar) {
    commit("SET_AVATAR", avatar);
  },
  changemobile({
    commit
  }, mobile) {
    commit("SET_MOBILE", mobile);
  },
  // 获取系统用户分页数据
  async GetallUsersAsync({
    commit
  }, data) {
    !!commit;
    var res = await userApi["GetallUsersAsync"](data);
    return res;
  },
  // 新增系统用户
  async addUser({
    commit
  }, data) {
    !!commit;
    var res = await userApi["addUser"](data);
    return res;
  },
  // 删除系统用户
  async delUser({
    commit
  }, data) {
    !!commit;
    var res = await userApi["delUser"](data);
    return res;
  },
  // 更新系统用户
  async updateUser({
    commit
  }, data) {
    !!commit;
    var res = await userApi["updateUser"](data);
    return res;
  },
  // 系统用户密码修改
  async change_password({
    commit
  }, data) {
    !!commit;
    var res = await userApi["change_password"](data);
    return res;
  },
  // 系统用户修改用户基本资料
  async update_profile({
    commit
  }, data) {
    !!commit;
    var res = await userApi["update_profile"](data);
    return res;
  },
  // 查询系统的用户的资料
  async sysprofile({
    commit
  }, data) {
    !!commit;
    var res = await userApi["sysprofile"](data);
    return res;
  },

  setCurrentModule({
    commit
  }, data) {
    commit("SET_CURRENTMODULES", data.idx);
  },
};

export default {
  namespaced: true,
  state: initState,
  mutations,
  actions,
};
