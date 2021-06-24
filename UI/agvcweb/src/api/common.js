import $http from "./core/http";

const v = 'api/com';

export async function upload(data) {
  var d = await $http.upload(`${v}/upload`, data);
  return d;
}

export async function uploadAll(data) {
  var d = await $http.upload(`${v}/multiple-upload`, data);
  return d;
}

// 按城市名称模糊查询(自动完成)
export async function identity_citys(data) {
  var d = await $http.get(`${v}/identity-citys?kw=${data.kw}`);
  return d;
}
// 查询省市级联数据
export async function cascader_areas(data) {
  var d = await $http.get(`${v}/cascader-areas?level=${data.kw}`);
  return d;
}

export async function getExpressCompanys() {
  const d = await $http.get(`${v}/express/companys`);
  return d;
}

export async function getExpressCompanyName(data) {
  const d = await $http.get(`${v}/express/auto-ident/${data.no}`);
  return d;
}

export async function getExpressInfo(data) {
  const d = await $http.post(`${v}/express/query`, data);
  return d;
}

// 快递服务
// 查询快递信息
export async function e_query(data) {
  var d = await $http.post(`${v}/express/query`, data);
  return d;
}
