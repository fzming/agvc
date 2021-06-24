<template>
  <div class="app-container">
    <caption-container caption="功能差异配置">
      <el-button
        type="success"
        round
        icon="el-icon-circle-plus"
        style="margin: right"
        @click="handleAddFeature"
      >
        新增
      </el-button>
    </caption-container>
    <region>
      <el-table
        ref="datalist"
        :data="datalist"
        height="500px"
        highlight-current-row
      >
        <el-table-column label="功能名称">
          <template slot-scope="scope">
            {{ scope.row.name }}
          </template>
        </el-table-column>
        <el-table-column label="关键字">
          <template slot-scope="scope">
            {{ scope.row.key }}
          </template>
        </el-table-column>
        <el-table-column label="分组名称">
          <template slot-scope="scope">
            {{ scope.row.groupName }}
          </template>
        </el-table-column>
        <el-table-column label="功能描述">
          <template slot-scope="scope">
            {{ scope.row.description }}
          </template>
        </el-table-column>
        <el-table-column label="定义值">
          <template slot-scope="scope">
            {{ scope.row.value }}
          </template>
        </el-table-column>
        <el-table-column label="操作">
          <template slot-scope="scope">
            <el-button
              icon="el-icon-edit-outline"
              type="default"
              size="mini"
              @click="handleEdit(scope)"
            >
              修改
            </el-button>
            <el-button
              icon="el-icon-delete"
              type="danger"
              size="mini"
              @click="handleDelete(scope)"
            >
              删除
            </el-button>
          </template>
        </el-table-column>
      </el-table>
    </region>
    <el-dialog
      :visible.sync="dialogVisible"
      :modal="false"
      width="500px"
      :title="dialogType === 'edit' ? '编辑功能差异配置' : '新增功能差异配置'"
      :before-close="handleClose"
    >
      <div style="max-width: 320px; margin: 0 auto">
        <el-form
          ref="featuremodel"
          :model="featuremodel"
          label-width="85px"
          label-position="left"
          v-loading="loading"
        >
          <el-form-item label="功能名称：">
            <el-input v-model="featuremodel.name" placeholder=""></el-input>
          </el-form-item>
          <el-form-item label="关键字：">
            <el-input v-model="featuremodel.key" placeholder=""></el-input>
          </el-form-item>
          <el-form-item label="分组名称：">
            <el-input
              v-model="featuremodel.groupName"
              placeholder=""
            ></el-input>
          </el-form-item>
          <el-form-item label="功能描述：">
            <el-input
              type="textarea"
              v-model="featuremodel.description"
              placeholder=""
            ></el-input>
          </el-form-item>
          <el-form-item label="定义值：">
            <el-input v-model="featuremodel.value" placeholder=""></el-input>
          </el-form-item>
          <el-form-item label="安全选项">
            <el-switch
              v-model="featuremodel.safety"
              active-color="#13ce66"
              inactive-color="#ff4949"
            >
            </el-switch>
          </el-form-item>
          <el-form-item label="">
            <el-button
              icon="el-icon-check"
              type="primary"
              @click="confirm('featuremodel')"
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
import _ from "lodash";
function initState() {
  return {
    name: "",
    key: "",
    groupName: "",
    description: "",
    value: "",
  };
}
export default {
  name: "list",
  data() {
    return {
      datalist: [],
      featuremodel: initState(),
      pageQuery: {
        pageIndex: 0,
        pageSize: 0,
      },
      dialogType: "",
      dialogVisible: false,
      loading: false,
      safety: false,
    };
  },
  async created() {
    await this.getfeaturelist();
  },
  methods: {
    //获取功能差异配置数据列表
    async getfeaturelist() {
      const dd = await SystemFeatureApi.page_queryAsync(this.pageQuery);
      console.log("dd", dd);
      this.datalist = dd.datas;
    },
    //新增配置
    handleAddFeature() {
      this.dialogType = "new";
      this.dialogVisible = true;
      this.featuremodel = initState();
    },
    // 编辑装拆箱费用弹框
    async handleEdit(scope) {
      this.featuremodel = _.cloneDeep(scope.row);
      this.dialogType = "edit";
      this.dialogVisible = true;
    },
    // 新增或者编辑提交
    async confirm(formName) {
      this.$refs[formName].validate(async (valid) => {
        if (valid) {
          const isEdit = this.dialogType === "edit";
          if (isEdit) {
            this.loading = true;
            const res = await SystemFeatureApi.updateAsync(this.featuremodel);
            this.loading = false;
            if (res) {
              await this.getfeaturelist();
              this.$message({
                type: "success",
                message: "修改成功!",
              });
              this.dialogVisible = false;
            }
          } else {
            this.loading = true;
            const res = await SystemFeatureApi.createAsync(this.featuremodel);
            this.loading = false;
            if (res) {
              await this.getfeaturelist();
              this.$message({
                type: "success",
                message: "新增成功!",
              });
              this.dialogVisible = false;
            }
          }
        }
      });
    },
    // 关闭弹框 右上角×
    handleClose() {
      this.$refs["featuremodel"].resetFields();
      this.dialogVisible = false;
    },
    // 删除
    async handleDelete({ row }) {
      var c = await this.$confirmAsync("警告！", "确定要删除功能配置数据吗?");
      if (c) {
        const l = await SystemFeatureApi.deleteAsync(row.id);
        if (l) {
          await this.getfeaturelist();
          this.$message({
            type: "success",
            message: "删除成功!",
          });
        }
      }
    },
  },
};
</script>
