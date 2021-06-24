import $http from './core/http'

const u = '/api/sys/menu'

export async function addRole(data) {
  var d = await $http.post(`${u}/create`, data)
  return d
}

export async function modify(data) {
  var d = await $http.post(`${u}/update`, data)
  return d
}

export async function remove(data) {
  var d = await $http.delete(`${u}/delete/` + data.id)
  return d
}

export async function orderUpdate(data) {
  var d = await $http.post(`${u}/update-order`, data)
  return d
}
