import $http from "./core/http";

export default {
  async login(data) {
    var d = await $http.post(`/token`, data);
    return d;
  },
  async orglogin(data) {
    var d = await $http.post(`/token`, data);
    return d;
  },
  async getInfo() {
    var d = await $http.get(`/api/sys/user/profile`);
    return d;
  },
  async getInfoOrg() {
    const d = await $http.get(`/api/account/profile`);
    return d;
  },
  async isAppUser(user) {
    const d = await $http.post(`/api/account/is-app-user`, user);
    return d;
  },
  async findAppUsers(user) {
    const d = await $http.post(`/api/account/find-app-user`, user);
    return d;
  },

  async kickOffDeviceAsync(data) {
    var d = await $http.post(`/api/account/kickoff`, data);
    return d;
  },
  async logout() {
    return await $http.post(`/api/user/logout`);
  },
  async refresh_ticket(ticket) {
    var d = await $http.post(`/token`, {
      grant_type: "refresh_token",
      refresh_token: ticket,
    });
    return d;
  },
  async GetallUsersAsync(data) {
    // 获取系统用户数据
    var d = await $http.post(`/api/sys/user/query`, data);
    return d;
  },
  async addUser(data) {
    // 新增系统用户
    var d = await $http.post(`/api/sys/user/create`, data);
    return d;
  },
  async delUser(id) {
    // 删除系统用户
    var d = await $http.delete(`/api/sys/user/delete/` + id);
    return d;
  },
  async updateUser(data) {
    // 更新系统用户
    var d = await $http.post(`/api/sys/user/update`, data);
    return d;
  },
  // 系统用户密码修改
  async change_password(data) {
    var d = await $http.post(`/api/sys/user/change-password`, data);
    return d;
  },
  // 系统用户修改用户基本资料
  async update_profile(data) {
    var d = await $http.post(`/api/sys/user/update-profile`, data);
    return d;
  },
  // 查询系统的用户的资料
  async sysprofile() {
    var d = await $http.get(`/api/sys/user/profile`);
    return d;
  },
};
