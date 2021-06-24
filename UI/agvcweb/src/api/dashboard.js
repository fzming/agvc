import $http from "./core/http";
const u = 'api/dashboard';
// 统计每月订单箱量合计
export default {
  async month_teu_axis(data) {
    return await $http.post(`${u}/month-teu-axis`, data);
  },
  // 统计一段时间内所有业务员箱量排名
  async user_teu_axis(data) {
    const d = await $http.post(`${u}/user-teu-axis`, data);
    return d;
  },
  // 统计一段时间内所有客户的箱量排名
  async company_teu_axis(data) {
    const d = await $http.post(`${u}/company-teu-axis`, data);
    return d;
  },
  // 统计每个自然月的应收款合计以及未销帐合计
  async month_cost_axis(data) {
    const d = await $http.post(`${u}/month-cost-axis`, data);
    return d;
  },
  async counter() {
    const d = await $http.get(`${u}/counter`);
    return d;
  }
};
