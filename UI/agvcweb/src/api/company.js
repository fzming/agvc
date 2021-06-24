import $http from "./core/http";
const u = 'api/company'

// 通过往来单位的Id 查找往来单位的合同
export async function contracts (companyId) {
  const d = await $http.get(`${u}/contracts?companyId=` + companyId)
  return d
}

// 通过往来单位的Id 查找往来单位的实体数据
export async function GetCompanyAsync (data) {
  const d = await $http.get(`${u}/${data}`)
  return d
}

//通过往来单位Id查找往来单位的装送货地址
export async function GetListCompanyAddressAsync (data) {
  const d = await $http.get(`${u}/address/list?companyId=` + data)
  return d
}

