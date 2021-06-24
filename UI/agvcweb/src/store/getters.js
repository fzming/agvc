 const getters = {
  sidebar: (state) => state.app.sidebar,
   size: (state) => state.app.size,
    device: (state) => state.app.device,
  token: (state) => state.user.token,
  avatar: (state) => state.user.avatar,
  name: (state) => state.user.name, // 用户昵称
  orgId: (state) => state.user.orgId,
  orgName: (state) => state.user.orgName,
  mobile: (state) => state.user.mobile,
  id: (state) => state.user.id,
  features: (state) => state.user.features,
  needChangePassword: (state) => state.user.needChangePassword,
  introduction: (state) => state.user.introduction,
  roles: (state) => state.user.roles, // array[object]
  module: (state) => state.user.userModules[state.user.currentModules],
  modules: (state) => state.user.userModules,
  // permission
  role_codes: (state) => state.permission.codes,
  permission_routes: (state) => state.permission.routes,
  flat_routes: (state) => state.permission.flat_routes,
  client: (state) => state.client.client,
  
  navurdcount: (state) => state.discussMessage.navurdcount,
  navurdcountList: (state) => state.discussMessage.navurdcountList,
  messagelist: (state) => state.discussMessage.messagelist,
};
export default getters;
