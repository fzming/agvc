<template>
  <div class="OrgFeatureBody">
    <el-table
      ref="datalist"
      :data="datalist"
      height="500px"
      style="width: 100%"
      highlight-current-row
    >
      <el-table-column label="功能名称" width="150">
        <template slot-scope="scope">
          {{ scope.row.name }}
        </template>
      </el-table-column>

      <el-table-column label="关键字" width="150">
        <template slot-scope="scope">
          {{ scope.row.key }}
        </template>
      </el-table-column>
      <el-table-column label="分组名称" width="110">
        <template slot-scope="scope">
          {{ scope.row.groupName }}
        </template>
      </el-table-column>
      <el-table-column
        label="功能描述"
        width="200"
        :show-overflow-tooltip="true"
      >
        <template slot-scope="scope">
          {{ scope.row.description }}
        </template>
      </el-table-column>
      <el-table-column label="默认值" width="110" :show-overflow-tooltip="true">
        <template slot-scope="scope">
          {{ scope.row.defaultValue }}
        </template>
      </el-table-column>
      <el-table-column label="定义值" width="110" :show-overflow-tooltip="true">
        <template slot-scope="scope">
          <span
            :style="
              scope.row.value === scope.row.defaultValue ? '' : 'color:red'
            "
          >
            {{ scope.row.value }}
          </span>
        </template>
      </el-table-column>
      <el-table-column label="操作" width="300px">
        <template slot-scope="scope">
          <el-button
            icon="el-icon-edit-outline"
            type="default"
            size="mini"
            @click="handleEdit(scope)"
          >
            设置
          </el-button>
          <el-button
            icon="el-icon-delete"
            type="danger"
            size="mini"
            @click="resetorgfeature(scope)"
          >
            重置
          </el-button>
        </template>
      </el-table-column>
    </el-table>

    <el-dialog
      width="50%"
      :visible.sync="dialogVisible"
      :modal="false"
      :title="dialogType === 'edit' ? '编辑功能差异配置' : '新增功能差异配置'"
      :before-close="handleClose"
    >
      <div>
        <el-form
          ref="datamodel"
          :model="datamodel"
          v-loading="loading"
          label-width="85px"
        >
          <el-form-item label="定义值：">
            <el-input
              v-model="datamodel.value"
              spellcheck="false"
              type="textarea"
              rows="6"
              placeholder="请输入差异配置"
            ></el-input>
          </el-form-item>
          <el-form-item label="">
            <el-button
              icon="el-icon-check"
              type="primary"
              @click="confirm('datamodel')"
            >
              确认
            </el-button>
          </el-form-item>
        </el-form>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import SystemFeatureApi from "@/api/Feature";
function initState() {
  return {
    sysFeatureId: "",
    orgId: "",
    value: "",
  };
}
export default {
  name: "OrgFeature",
  props: {
    ws: Object,
  },
  data() {
    return {
      datalist: [],
      datamodel: initState(),
      pageQuery: {
        pageIndex: 0,
        pageSize: 100,
        orgId: "",
      },
      dialogType: "",
      dialogVisible: false,
      loading: false,
    };
  },
  async created() {
    this.pageQuery.orgId = this.ws.orgId;
    await this.getfeaturelist();
  },
  methods: {
    //获取机构的功能差异配置数据列表
    async getfeaturelist() {
      const dd = await SystemFeatureApi.orgqueryAsync(this.pageQuery);
      console.log("dd", dd);
      this.datalist = dd.datas;
    },
    // 编辑框
    async handleEdit(scope) {
      this.datamodel.sysFeatureId = scope.row.id;
      this.datamodel.orgId = this.ws.orgId;
      this.datamodel.value = scope.row.value;
      this.dialogType = "edit";
      this.dialogVisible = true;
    },
    // 新增或者编辑提交
    async confirm(formName) {
      this.$refs[formName].validate(async (valid) => {
        if (valid) {
          if (!this.datamodel.value) {
            this.$message({
              type: "error",
              message: "定义值不为空!",
            });
          }
          console.log("this.datamodel", this.datamodel);
          this.loading = true;
          const dd = await SystemFeatureApi.orgset_org_feature(this.datamodel);
          console.log("res1111111", dd);
          this.loading = false;
          if (dd) {
            await this.getfeaturelist();
            this.$message({
              type: "success",
              message: "修改成功!",
            });
            this.dialogVisible = false;
          }
        }
      });
    },
    // 关闭弹框 右上角×
    handleClose() {
      this.$refs["datamodel"].resetFields();
      this.dialogVisible = false;
    },
    //重置
    async resetorgfeature(scope) {
      var aa = {
        orgId: this.ws.orgId,
        featureId: scope.row.id,
      };
      const dd = await SystemFeatureApi.reset_org_feature(aa);
      if (dd) {
        await this.getfeaturelist();
        this.$message({
          type: "success",
          message: "修改成功!",
        });
      }
    },
  },
};
</script>
<style lang="scss">
.el-tooltip__popper {
  max-width: 300px !important;
  height: auto !important;
}
</style>
