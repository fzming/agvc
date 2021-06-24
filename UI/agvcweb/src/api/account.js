import $http from "./core/http";
const u = "api/account";

export default {
  // 获取机构用户的分页数据
  async QueryAccountUsersAsync(data) {
    var d = await $http.post(`${u}/query`, data);
    return d;
  },
  // 新增机构用户
  async CreateAccountUserAsync(data) {
    var d = await $http.post(`${u}/create`, data);
    return d;
  },
  // 修改机构用户
  async UpdateAccountUserAsync(data) {
    var d = await $http.post(`${u}/update`, data);
    return d;
  },
  // 删除机构用户
  async DeleteAccountUserAsync(data) {
    var d = await $http.delete(`${u}/delete/${data}`);
    return d;
  },
  // 根据关键字查询机构用户（自动完成）
  async identity_users(data) {
    console.log(data);
    const a = await $http.get(`${u}/identity-users?kw=${data.kw}`);
    return a;
  },
  async user_agents(model) {
    const a = await $http.post(`${u}/user-agents`, model);
    return a;
  },
  async kick_agents(model) {
    const a = await $http.post(`${u}/kick-agents`, model);
    return a;
  },
  // 密码修改
  async accchange_password(data) {
    const a = await $http.post(`${u}/change-password`, data);
    return a;
  },
  async find_password(data) {
    const a = await $http.post(`${u}/find-password`, data);
    return a;
  },
  async reset_password(data) {
    const a = await $http.post(`${u}/reset-password`, data);
    return a;
  },
  // 基础资料修改
  async accupdate_profile(data) {
    const a = await $http.post(`${u}/update-profile`, data);
    return a;
  },
  // 查询当前登录账号的信息
  async accprofile() {
    const a = await $http.get(`${u}/profile`);
    return a;
  },
  //解除微信绑定
  async unbind_wx(data) {
    const a = await $http.delete(`${u}/unbind-wx?appId=${data}`);
    return a;
  },
  // 获取短信验证码
  async getfetchSmsSecurity(data) {
    const a = await $http.get(
      `${u}/fetchSmsSecurity?mobile=` +
      data.mobile +
      `&key=` +
      data.key +
      "&securityKey=" +
      data.securityKey
    );
    return a;
  },
  // 绑定手机号
  async bind_mobile(data) {
    const a = await $http.post(`${u}/bind-mobile`, data);
    return a;
  },
  // 解绑手机号
  async unbind_mobile(data) {
    const a = await $http.post(`${u}/unbind-mobile`, data);
    return a;
  },
};
