import curdApi from './core/curdApi'
// 功能差异配置 Api
class SystemFeatureApi extends curdApi {
    constructor() {
        super('/api/sys/feature');
    }
    //机构功能差异配置列表
    async orgqueryAsync (query) {
        return this.$postAsync(`org-features`, query)
    }
    //设置机构值定义
    async orgset_org_feature (model) {
        return this.$postAsync(`set-org-feature`, model)
    }
    //恢复初始化机构值初始定义
    async reset_org_feature (model) {
        return this.$postAsync(`reset-org-feature`, model)
    }
}
export default new SystemFeatureApi;
