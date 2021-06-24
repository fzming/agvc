<template>
  <div class="outDiv">
    <caption-container caption="机构管理">
      <el-button type="primary" @click="newBtn">
        <i class="el-icon-plus" /> 新增机构
      </el-button>
    </caption-container>

    <el-dialog
      :title="isModify ? '修改' : '新增'"
      :visible.sync="showDialog"
      width="30%"
      center
      :show-close="showDialog"
    >
      <div style="min-height: 100px; height: auto; overflow: auto">
        <el-form
          ref="formData"
          :model="formData"
          :rules="rules"
          label-width="120px"
        >
          <div class="cap"><i class="el-icon-s-home"></i> 机构信息</div>

          <el-form-item label="机构名称" prop="Name">
            <el-input
              v-model="formData.Name"
              placeholder="请输入机构名称"
              style="width: 320px"
            />
          </el-form-item>
          <el-form-item v-if="!isModify" label="机构类型" prop="PrimaryType">
            <el-select
              v-model="formData.PrimaryType"
              placeholder="请选择机构类型"
            >
              <el-option
                v-for="item in PrimaryType"
                :key="item.value"
                :label="item.label"
                :value="item.value"
              />
            </el-select>
          </el-form-item>
          <el-form-item label="机构缩写" prop="Prefix">
            <el-input
              v-model="formData.Prefix"
              placeholder="请输入机构缩写"
              style="width: 218px"
            />
          </el-form-item>
          <el-form-item label="机构模块" prop="modules">
            <el-checkbox-group v-model="formData.modules">
              <el-checkbox
                v-for="moduleType in moduleTypes"
                :key="moduleType.value"
                :label="moduleType.value"
              >
                {{ moduleType.name }}</el-checkbox
              >
            </el-checkbox-group>
          </el-form-item>
          <template v-if="!isModify">
            <div class="cap">
              <i class="el-icon-s-custom"></i> 机构管理员信息
            </div>

            <el-form-item label="管理员账号" prop="LoginId">
              <el-input
                v-model="formData.LoginId"
                placeholder="请输入机构登录账号"
                style="width: 218px"
              />
            </el-form-item>
            <el-form-item label="管理员密码" prop="LoginPwd">
              <el-input
                v-model="formData.LoginPwd"
                placeholder="请输入机构登录密码"
                style="width: 218px"
              />
            </el-form-item>
            <el-form-item label="管理员姓名" prop="Nick">
              <el-input
                v-model="formData.Nick"
                placeholder="请输入姓名"
                style="width: 218px"
              />
            </el-form-item>
            <el-form-item label="角色分配" prop="RoleId">
              <el-select v-model="formData.RoleId" placeholder="请选择所属角色">
                <el-option
                  v-for="item in rolesList"
                  :key="item.key"
                  :label="item.name"
                  :value="item.id"
                />
              </el-select>
            </el-form-item>
          </template>
          <el-form-item>
            <template v-if="isModify">
              <el-button type="primary" @click="modSave"> 保存 </el-button>
              <el-button type="primary" @click="cancel"> 取消 </el-button>
            </template>
            <template v-else>
              <el-button type="primary" @click="addSave">保存</el-button>
              <el-button type="primary" @click="reset">重置</el-button>
            </template>
            <!-- <el-button v-if="isModify" type="primary" @click="modSave">
              保存
            </el-button>
            <el-button v-else type="primary" @click="addSave">保存</el-button>

            <el-button v-if="isModify" type="primary" @click="cancel">
              取消
            </el-button>
            <el-button v-else type="primary" @click="reset">重置</el-button> -->
          </el-form-item>
        </el-form>
      </div>
    </el-dialog>
    <el-dialog title="强制下线" :visible.sync="showKickoff" width="30%" center>
      <el-input
        type="textarea"
        :rows="2"
        placeholder="请输入内容"
        v-model="kickMessage"
      >
      </el-input>
      <div style="padding: 10px; text-align: center">
        <el-button type="danger" size="large" @click="doKick">
          确认下线操作
        </el-button>
      </div>
    </el-dialog>
    <region>
      <el-table
        :data="tableData"
        tooltip-effect="dark"
        stripe
        style="width: 100%"
      >
        <el-table-column type="selection" />
        <el-table-column label="机构名称" prop="name" width="225px" />
        <el-table-column label="名称缩写" prop="prefix" width="100px" />
        <el-table-column label="机构类型" prop="primaryType" width="100px">
          <template slot-scope="scope">
            <el-tag>
              {{
                PrimaryType.find((a) => a.value === scope.row.primaryType).label
              }}</el-tag
            >
          </template>
        </el-table-column>
        <el-table-column label="机构ID" prop="id" width="220px" />
        <el-table-column label="在线人数">
          <template slot-scope="scope">
            {{ getAgents(scope.row.id).length }}
            <el-button
              type="default"
              size="mini"
              @click="handleKickOff(scope.row.id)"
              ><i class="el-icon-edit-outline" /> 强制下线
            </el-button>
          </template>
        </el-table-column>
        <el-table-column label="操作">
          <template slot-scope="scope">
            <el-button
              v-code="comd.modify"
              type="default"
              size="mini"
              @click="handleEdit(scope.$index, scope.row)"
              ><i class="el-icon-edit-outline" /> 编辑
            </el-button>
            <el-button
              v-code="comd.delete"
              type="danger"
              size="mini"
              @click="handleDelete(scope.$index, scope.row)"
              ><i class="el-icon-delete" /> 删除
            </el-button>
            <el-button
              type="default"
              size="mini"
              @click="handleorgfeature(scope.$index, scope.row)"
              >功能差异配置</el-button
            >
          </template>
        </el-table-column>
      </el-table>
    </region>
  </div>
</template>
<script>
import { getRoles } from "@/api/role";
import account from "@/api/account";
import { MessageBox } from "element-ui";
import { MODULE_NAME_TYPE } from "@/global/const";
export default {
  data() {
    return {
      comd: {
        modify: "zl-1ffb35edf8fa4a20b01cdd85127c238c",
        delete: "zl-87b5e95f5f3847e6ac7943f10d0ef12d",
      },
      isModify: false,
      showDialog: false,
      showKickoff: false,
      kickMessage: "此登陆被服务器强制下线",
      readyKickOrgId: "",
      tableData: [],
      formData: {
        Id: "",
        Name: "",
        PrimaryType: 10,
        Prefix: "",
        LoginId: "",
        LoginPwd: "",
        Nick: "",
        RoleId: "",
        modules: [],
      },
      PrimaryType: [
        // { value: 0, label: '系统' },
        { value: 10, label: "货代" },
        { value: 20, label: "堆场" },
        // { value: "30", label: '车队' },
        // { value: "40", label: '船公司' },
        // { value: "50", label: '铁路代理' }
      ],
      rolesList: [],
      rules: {
        Name: [{ required: true, message: "请输入机构名称", trigger: "blur" }],
        PrimaryType: [
          { required: true, message: "请选择机构类型", trigger: "change" },
        ],
        Prefix: [
          { required: true, message: "请输入机构简称", trigger: "blur" },
        ],
        LoginId: [
          {
            required: true,
            message: "请输入机构管理员登录账号",
            trigger: "blur",
          },
        ],
        LoginPwd: [
          {
            required: true,
            message: "请输入机构管理员登录密码",
            trigger: "blur",
          },
        ],
        RoleId: [
          {
            required: true,
            message: "请选择机构管理员角色",
            trigger: "change",
          },
        ],
        Nick: [
          {
            required: true,
            message: "请输入机构管理员姓名",
            trigger: "blur",
          },
        ],
        modules: [
          {
            required: true,
            message: "请选择机构的功能模块",
            trigger: "blur",
          },
        ],
      },
      agents: [],
    };
  },
  computed: {
    moduleTypes() {
      return MODULE_NAME_TYPE;
    },
  },
  async created() {
    this.pgetRoles();
    await this.query();
    await this.queryAgents();
  },
  methods: {
    pgetRoles() {
      getRoles().then((l) => {
        this.rolesList = l.filter((x) => x.level <= 90);
      });
    },
    newBtn() {
      this.isModify = false;
      this.showDialog = true;
      this.reset();
      // this.$refs['formData'].resetFields()
    },
    async query() {
      this.tableData = await this.$store.dispatch("orgnization/mquery");
    },
    async queryAgents() {
      this.agents =
        (await account.user_agents({
          orgId: "",
        })) || [];
    },
    getAgents(orgId) {
      return this.agents.filter((p) => p.orgId == orgId);
    },
    handleKickOff(orgId) {
      this.showKickoff = true;
      this.readyKickOrgId = orgId;
    },
    async doKick() {
      await account.kick_agents({
        orgId: this.readyKickOrgId,
        message: this.kickMessage,
      });
      this.showKickoff = false;
      setTimeout(async () => {
        await this.queryAgents();
      }, 1000);
    },
    handleEdit(index, row) {
      this.isModify = true;
      this.showDialog = true;
      this.formData.OrgId = row.id;
      this.formData.Name = row.name;
      this.formData.Prefix = row.prefix;
      this.formData.modules = row.modules;
      // this.formData.PrimaryType = row.primaryType;
      // this.formData.LoginId = row.loginId;
      // this.formData.LoginPwd = row.loginPwd;
      // this.formData.RoleId = row.RoleId;
      console.log(this.formData);
    },
    handleDelete(index, row) {
      MessageBox.confirm("是否确认删除此机构", "提示", {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        type: "warning",
      })
        .then(() => {
          this.$store
            .dispatch("orgnization/remove", { id: row.id })
            .then((res) => {
              this.query();
              this.isModify = false;
              this.showDialog = false;
              this.reset();
            })
            .catch((res) => {
              console.log(res);
            });
        })
        .catch(() => {
          this.$message({
            type: "info",
            message: "已取消删除",
          });
        });
    },
    addSave() {
      this.$refs["formData"].validate((valid) => {
        if (valid) {
          this.$store
            .dispatch("orgnization/mcreate", this.formData)
            .then((res) => {
              console.log(res);
              this.query();
              this.showDialog = false;
            })
            .catch((res) => {
              console.log(res);
            });
        }
      });
    },
    modSave() {
      this.$refs["formData"].validate((valid) => {
        if (valid) {
          this.$store
            .dispatch("orgnization/update", this.formData)
            .then((res) => {
              MessageBox.alert("提示", "修改成功", {
                type: "success",
                confirmButtonText: "确定",
                callback: (action) => {
                  this.query();
                  this.showDialog = false;
                },
              });
            })
            .catch((res) => {
              console.log(res);
            });
        }
      });
    },
    cancel() {
      this.showDialog = false;
      setTimeout(() => {
        this.isModify = false;
        this.reset();
      }, 500);

      // console.log(this.formData)
    },
    reset() {
      this.formData.OrgId = "";
      this.formData.Name = "";
      this.formData.Prefix = "";
      this.formData.PrimaryType = 10;
      this.formData.LoginId = "";
      this.formData.LoginPwd = "";
      this.formData.RoleId = "";
    },
    //机构功能差异编辑
    handleorgfeature(index, row) {
      this.$openView(
        __dirname,
        "components/OrgFeature.vue",
        "机构功能差异设置",
        {
          orgId: row.id,
        }
      );
    },
  },
};
</script>
<style lang="scss" scoped>
.outDiv {
  margin-top: 15px;
  margin-left: 15px;
}

.btn_controlers {
  margin-top: 15px;
}

.cap {
  font-size: 16px;
  color: $primary-color;
  padding: 5px;
  border-bottom: 1px solid #eee;
  margin-bottom: 10px;
}
</style>
