import $http from "./core/http";
const u = "api/hcaptcha";
export default {
  async acceptNextAsync() {
    return await $http.post(`${u}/accept-next`);
  },
  async setCodeAsync(hmSet) {
    return await $http.post(`${u}/set-code`, hmSet);
  }
}
