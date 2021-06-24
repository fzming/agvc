import $http from './http'
export default class curdBasicApi {

  constructor(api) {
    this.api = api;
  }
  async $postAsync (actionName, data) {
    return await $http.post(`${this.api}/${actionName}`, data);
  }
  async $getAsync (actionName, params) {
    return await $http.get(`${this.api}/${actionName}`, params);
  }
  async $deleteAsync (actionName, params) {
    return await $http.delete(`${this.api}/${actionName}`, params);
  }
  //基本CURD==========================================================
  async createAsync (model) {
    return await this.$postAsync(`create`, model);
  }
  async deleteAsync (id) {
    return await this.$deleteAsync(`delete/${id}`)
  }
  async getAsync (id) {
     return await this.$getAsync(`get/${id}`)
  }
  async updateAsync (updateModel) {
    return await this.$postAsync(`update`, updateModel);
  }
  async queryAsync (query) {
    return await this.$postAsync(`query`, query);
  }
  async page_queryAsync (query) {
    return await this.$postAsync(`page-query`, query);
  }
  async org_page_queryAsync (query) { 
    return await this.$postAsync(`org-page-query`,query)
  }

}
