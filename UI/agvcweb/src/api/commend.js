import $http from './core/http'

const u = 'api/sys/auth'

export default {
  async create(data) {
    var d = await $http.post(`${u}/code/create`, data)
    return d
  },
  async deleted(data) {
    var d = await $http.delete(`${u}/code/delete/` + data.codeId)
    return d
  },
  async modify(data) {
    var d = await $http.post(`${u}/code/update`, data)
    return d
  },
  async query(data) {
    var d = await $http.get(`${u}/codes`, data)
    return d
  }
}
