import commendApi from '@/api/commend'
// create, deleted, modify, query

const actions = {
  async query({
    commit
  }, data) {
    !!commit;
    var d = await commendApi['query'](data)
    return d
  },
  async create({
    commit
  }, data) {
    !!commit;
    var d = await commendApi['create'](data)
    return d
  },
  async update({
    commit
  }, data) {
    !!commit;
    var d = await commendApi['modify'](data)
    return d
  },
  async remove({
    commit
  }, data) {
    !!commit;
    var d = await commendApi['deleted'](data)
    return d
  }
}

export default {
  namespaced: true,
  actions
}
