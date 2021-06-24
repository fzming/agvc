import $http from './core/http'
const u = 'api/discuss' // 费用统计

export default {
  async send(data) {
    const a = $http.post(`${u}/send`, data)
    return a
  },
  async send_attachment(data) {
    const a = $http.upload(`${u}/send-attachment`, data)
    return a
  },
  async query(data) {
    const a = $http.post(`${u}/query`, data)
    return a
  },
  async readed(data) {
    const a = $http.get(`${u}/read/${data.id}`)
    return a
  },
  async unreadcount(data) {
    const a = $http.get(`${u}/unread-count?group=${data}`)
    return a
  },
  async delMessage(data) {
    const a = $http.delete(`${u}/delete/${data.id}`)
    return a
  }
}
