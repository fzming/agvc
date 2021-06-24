<template>
  <div class="app-container">
    <caption-container caption="系统用户管理">
      <el-button type="primary" @click="handleAddUser">
        <i class="el-icon-plus" /> 新增用户
      </el-button>
    </caption-container>

    <region>
      <el-table :data="userList" stripe style="width: 100%">
        <el-table-column type="selection" width="55" />
        <el-table-column align="left" sortable label="用户账号" width="220">
          <template slot-scope="scope">
            {{ scope.row.loginId }}
          </template>
        </el-table-column>
        <el-table-column sortable align="left" label="角色" width="220">
          <template slot-scope="scope">
            {{ scope.row.roleName }}
          </template>
        </el-table-column>

        <el-table-column sortable align="left" label="姓名" width="220">
          <template slot-scope="scope">
            {{ scope.row.nick }}
          </template>
        </el-table-column>
        <el-table-column sortable align="left" label="手机号" width="220">
          <template slot-scope="scope">
            {{ scope.row.mobile }}
          </template>
        </el-table-column>

        <el-table-column align="left" label="操作">
          <template slot-scope="scope">
            <el-button
              type="default"
              icon="el-icon-edit-outline"
              :disabled="scope.row.isshow ? true : false"
              size="mini"
              @click="handleEdit(scope)"
            >
              修改
            </el-button>
            <el-button
              type="danger"
              icon="el-icon-delete"
              :disabled="scope.row.isshow ? true : false"
              size="mini"
              @click="handleDelete(scope)"
            >
              删除
            </el-button>
          </template>
        </el-table-column>
      </el-table>
    </region>

    <pagination
      :total="total"
      :page.sync="PageQuery.pageIndex"
      :page-sizes="[10, 20, 30, 40]"
      :limit.sync="PageQuery.pageSize"
      @pagination="GetPageDatasAsync"
    />
    <el-dialog
      :visible.sync="dialogVisible"
      width="440px"
      :title="dialogType === 'edit' ? '编辑用户' : '新增用户'"
    >
      <div style="padding: 20px">
        <el-form
          ref="sysuser"
          :model="sysuser"
          :rules="rules"
          label-width="80px"
          label-position="left"
        >
          <el-form-item label="用户账号" prop="loginId">
            <el-input v-model="sysuser.loginId" placeholder="" />
          </el-form-item>
          <el-form-item label="账户密码" prop="loginPwd">
            <!-- <PasswordStrengthMeter v-model="sysuser.loginPwd"></PasswordStrengthMeter> -->
            <PasswordStrengthMeter
              :pwd="sysuser.loginPwd"
              @changevlue="changevlue"
            >
            </PasswordStrengthMeter>
          </el-form-item>
          <!-- <el-form-item v-else label="账户密码">
            <el-input v-model="sysuser.loginPwd" placeholder="密码为空,将使用旧密码！" :type="'password'" />

          </el-form-item> -->
          <el-form-item
            v-if="dialogType !== 'edit'"
            label="首登"
            prop="needChangePassword"
          >
            <el-switch
              v-model="sysuser.needChangePassword"
              active-text="强制修改密码"
            />
          </el-form-item>
          <el-form-item label="手机号" prop="mobile">
            <el-input v-model="sysuser.mobile" placeholder="" />
          </el-form-item>
          <el-form-item label="姓名" prop="nick">
            <el-input v-model="sysuser.nick" placeholder="" />
          </el-form-item>
          <el-form-item label="所属角色" prop="roleId">
            <el-select
              v-model="sysuser.roleId"
              placeholder="请选择"
              value-key="id"
            >
              <el-option
                v-for="role in rolelist"
                :key="role.id"
                :value="role.id"
                :label="role.name"
              />
            </el-select>
          </el-form-item>
        </el-form>
        <div style="text-align: right">
          <el-button
            icon="el-icon-circle-close"
            type="danger"
            @click="closedialog('sysuser')"
          >
            取消
          </el-button>
          <el-button
            icon="el-icon-check"
            type="primary"
            @click="confirmUser('sysuser')"
          >
            确认
          </el-button>
        </div>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import { deepClone } from "@/utils";
import { mapUserActions } from "@/store/namespaced/user";
import Pagination from "@/components/Pagination";
import { getRoles } from "@/api/role";
import PasswordStrengthMeter from "@/components/PasswordStrengthMeter";
function initState() {
  return {
    Id: "",
    loginId: "",
    loginPwd: "",
    roleId: "",
    roleName: "",
    nick: "",
    mobile: "",
    needChangePassword: true,
  };
}

export default {
  components: { Pagination, PasswordStrengthMeter },
  data() {
    return {
      sysuser: initState(),
      userList: [],
      total: 0,
      listQuery: {
        page: 0, // dang
        limit: 1, // 每页显示的个数
      },
      PageQuery: {
        pageIndex: 1,
        pageSize: 10,
      },
      dialogVisible: false,
      dialogType: "新增用户",
      checkStrictly: false,
      rolelist: [],
      // rowroleId: '',
      currentuserId: "", // 当前登录角色Id
      currentroleslevel: 0, // 当前角色的等级
      rules: {
        loginId: [
          {
            required: true,
            message: "请输入账号",
            trigger: "blur",
          },
        ],
        loginPwd: [
          {
            required: true,
            message: "请输入密码",
            trigger: "blur",
          },
        ],
        needChangePassword: [
          {
            required: true,
            message: "请选择是否首次登陆强制修改密码",
            trigger: "blur",
          },
        ],
        nick: [{ required: true, message: "请输入姓名", trigger: "blur" }],
        roleId: [
          {
            required: true,
            message: "请选择角色",
            trigger: "change",
          },
        ],
        mobile: [
          {
            required: true,
            message: "请输入手机号",
            trigger: "blur",
          },
          {
            pattern: /^1[\d]{10}$/,
            message: "手机号格式不对",
            trigger: "blur",
          },
        ],
      },
      Isallow: false,
    };
  },
  async created() {
    this.currentuserId = this.$store.getters.id; // 当前登录账号的Id
    this.currentroleslevel = this.$store.getters.roles[0].level; // 当前登录的账号角色的等级
    await this.GetPageDatasAsync(); // 分页数据
  },
  methods: {
    // 查询全部角色的数据
    getRoles,
    // 获取用户分页数据
    ...mapUserActions(["GetallUsersAsync", "addUser", "delUser", "updateUser"]),
    async GetPageDatasAsync() {
      const ps = await this.GetallUsersAsync(this.PageQuery);
      var userListA = ps.datas;

      this.userList = [];
      userListA.forEach((item) => {
        var isshow = false;
        if (item.id === this.currentuserId) {
          // 当前登录的账号不可以修改自己的
          isshow = true;
        }
        if (item.roleLevel >= this.currentroleslevel) {
          // 不可以编辑角色等级大于或者等于的账号数据
          isshow = true;
        }
        item.isshow = isshow;
        this.userList.push(item);
      });
      this.total = ps.total; // 总条数
    },
    // 新增用户
    async handleAddUser() {
      this.role = Object.assign({}, initState());
      this.dialogType = "new";
      this.dialogVisible = true;
      this.sysuser = initState();
      // this.rowroleId = ''
      // 获取全部的角色数据
      this.rolelist = await getRoles();
    },
    // 编辑用户
    async handleEdit(scope) {
      this.sysuser = deepClone(scope.row);
      this.dialogType = "edit";
      this.dialogVisible = true;
      this.checkStrictly = true;

      // 获取全部的角色数据
      this.rolelist = await getRoles();
    },

    // 通过角色的Id 返回角色名称
    GetroleName(roleId) {
      let b = "";
      if (this.rolelist) {
        this.rolelist.forEach((a) => {
          if (a.id === roleId) {
            b = a.name;
          }
        });
      }
      return b;
    },
    // 新增或者编辑系统用户提交
    async confirmUser(formName) {
      this.$refs[formName].validate(async (valid) => {
        if (valid) {
          const isEdit = this.dialogType === "edit";
          if (isEdit) {
            // 修改系统用户
            if (this.sysuser.loginPwd) {
              if (!this.Isallow) {
                this.$message({
                  type: "error",
                  message: "密码不合规则!",
                });
                return;
              }
            }
            this.sysuser.userId = this.sysuser.id;
            const user = await this.updateUser(this.sysuser);
            if (user) {
              for (let index = 0; index < this.userList.length; index++) {
                if (this.userList[index].id === this.sysuser.id) {
                  this.userList.splice(
                    index,
                    1,
                    Object.assign({}, this.sysuser)
                  );
                  break;
                }
              }
              this.$message({
                type: "success",
                message: "修改成功!",
              });
              this.dialogVisible = false;
              await this.GetPageDatasAsync();
            }
          } else {
            if (!this.Isallow) {
              this.$message({
                type: "error",
                message: "密码不合规则!",
              });
              return;
            }
            // 新增系统用户
            const user = await this.addUser(this.sysuser);
            if (user) {
              this.sysuser = user;
              this.userList.push(this.sysuser);
              this.$message({
                type: "success",
                message: "新增成功!",
              });
              this.dialogVisible = false;
              await this.GetPageDatasAsync();
            }
          }
        }
      });
    },

    async handleDelete({ $index, row }) {
      this.sysuser = row;
      var c = await this.$confirmAsync("警告！", "确定要删除系统账号吗?");
      if (c) {
        const l = await this.delUser(row.id);
        if (l) {
          this.userList.splice($index, 1);
          this.$message({
            type: "success",
            message: "删除成功!",
          });
          await this.GetPageDatasAsync();
        }
      }
    },
    // 关闭弹框同时刷新表单的按钮
    closedialog() {
      // this.$refs[formName].resetFields();
      this.dialogVisible = false;
    },
    changevlue(arg) {
      this.sysuser.loginPwd = arg.pwd;
      this.Isallow = arg.Isallow;
    },
  },
};
</script>
