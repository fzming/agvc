import $http from "./core/http";

export async function asyncRoutes(authMode, hasCode, authRoleId,authUserId) {
  var d = await $http.get(
    `/api/sys/menu/query?authMode=${!!authMode}&hasCode=${!!hasCode}&authRoleId=${authRoleId ||
      ''}&authUserId=${authUserId||''}`
  );
  console.log('所有菜单', d);
  return d;
}
//获取当前登录用户菜单指令列表
export async function newasyncRoutes (hasCode) { 
    var d = await $http.get(
    `/api/sys/menu/query?hasCode=${!!hasCode}`
  );
  console.log('所有菜单', d);
  return d;
}
//获取指定用户的菜单指令指令列表
export async function urseasyncRoutes (data) { 
      var d = await $http.post(
    `/api/sys/menu/user-auth-query`,data
  );
  console.log('所有菜单1', d);
  return d;
}
export async function asyncCodes() {
  // return await $http.get(`/api/sys/auth/role-codes`);
  return await $http.get(`/api/sys/auth/user-codes`);
}

export async function getRoles() {
  var d = await $http.get(`/api/sys/auth/roles`);
  return d;
}

export async function addRole(data) {
  var d = await $http.post(`/api/sys/auth/role/create`, data);
  return d;
}

export async function deleteRole(id) {
  var d = await $http.delete(`/api/sys/auth/role/delete/${id}`);
  return d;
}

export async function updateRole(data) {
  // console.log(data)
  var d = await $http.post(`/api/sys/auth/role/update`, data);
  return d;
}

export async function updateUserAuthority (data) { 
  var d = await $http.post(`/api/sys/auth/user/update`, data);
  return d;
}

// export function updateRole(id, data) {
//   return request({
//     url: `/role/${id}`,
//     method: 'put',
//     data
//   })
// }

// export function deleteRole(id) {
//   return request({
//     url: `/role/${id}`,
//     method: 'delete'
//   })
// }
