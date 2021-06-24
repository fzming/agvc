import $http from "./core/http";

const u = "api/sys/org";
export async function create(data) {
  var d = await $http.post(`${u}/create`, data);
  return d;
}

export async function deleted(data) {
  var d = await $http.delete(`${u}/delete/` + data.codeId);
  return d;
}

export async function modify(data) {
  var d = await $http.post(`${u}/update`, data);
  return d;
}
export async function byDomain(data) {
  var d = await $http.post(`${u}/byDomain`, data);
  return d;
}
export async function query() {
  var d = await $http.get(`${u}/query`);
  return d;
}
//增加机构开票资料
export async function AccountCreate(data) {
  var d = await $http.post(`${u}/invoices/add`, data);
  return d;
}
//更新机构开票资料
export async function AccountUpdate(data) {
  var d = await $http.post(`${u}/invoices/update`, data);
  return d;
}
//删除开票资料
export async function AccountDelete(id) {
  var d = await $http.delete(`${u}/invoices/remove/${id}`);
  return d;
}
//获取机构的所有开票资料
export async function AccountQuery() {
  var d = await $http.post(`${u}/invoices`);
  return d;
}
export async function GetInvoiceByIdAsync(id) {
  var d = await $http.get(`${u}/invoices/get/${id}`);
  return d;
}
