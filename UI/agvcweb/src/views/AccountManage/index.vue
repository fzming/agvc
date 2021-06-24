<template>
      <div class="grid-content bg-purple">
        <div class="app-container">
          <caption-container caption="机构用户管理">
            <el-button type="primary" @click="handleAddUser">
              <i class="el-icon-plus" /> 新增用户
            </el-button>
          </caption-container>
          <region>
            <el-table
              :data="userList"
              style="width: 100%"
              tooltip-effect="dark"
              stripe
            >
              <el-table-column type="index" label="序号" width="55" />
              <el-table-column label="姓名" width="150">
                <template slot-scope="scope">
                  <img
                    v-if="scope.row.avatar"
                    v-src="scope.row.avatar"
                    default-img="http://www.yy5156.com/assets/images/avatar.png"
                    class="user-avatar"
                  />
                  <span v-else class="tag">
                    {{ scope.row.nick.substr(-1, 1) }}
                  </span>
                  {{ scope.row.nick }}
                </template>
              </el-table-column>
              <el-table-column label="账号" width="100">
                <template slot-scope="scope">
                  {{
                    !scope.row.forbiddenLogin ? scope.row.loginId : "禁止登陆"
                  }}
                </template>
              </el-table-column>
              <el-table-column label="分公司">
                <template slot-scope="scope">
                  {{ scope.row.branchCompany ? scope.row.branchCompany : "" }}
                </template>
              </el-table-column>
              <el-table-column label="部门">
                <template slot-scope="scope">
                  {{ scope.row.department ? scope.row.department : "" }}
                </template>
              </el-table-column>
              <el-table-column label="角色" width="100">
                <template slot-scope="scope">
                  {{ scope.row.roleName }}
                </template>
              </el-table-column>
              <el-table-column label="操作">
                <template slot-scope="scope">
                  <el-button
                    v-code="comd.modify"
                    icon="el-icon-edit-outline"
                    type="default"
                    size="mini"
                    @click="handleEdit(scope)"
                  >
                    修改
                  </el-button>
                  <el-button
                    v-code="comd.delete"
                    icon="el-icon-delete"
                    :disabled="scope.row.isshow ? true : false"
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
          <pagination
            :total="total"
            :page.sync="PageQuery.pageIndex"
            :page-sizes="[10, 20, 30, 40]"
            :limit.sync="PageQuery.pageSize"
            @pagination="GetPageDatasAsync"
          />
        </div>
      </div>
</template>

<script>
import { deepClone } from "@/utils";
import Pagination from "@/components/Pagination";
import { getRoles, updateUserAuthority } from "@/api/role";
import { mapAccountActions } from "@/store/namespaced/Account";

function initState() {
  return {
    id: "",
    loginId: "",
    loginPwd: "",
    roleId: "",
    roleName: "",
    nick: "",
    needChangePassword: true,
    forbiddenLogin: false, // 禁止登陆 /// 单纯创建用户，不设置用户名和密码
    branchCompany: "",
    dDepartment: "",
    Departmentdata: "",
    branchCompanydata: "",
  };
}

export default {
  components: {
    Pagination,
  },
  data() {
    return {
      activeName: "first",
      comd: {
        modify: "zl-66c7522e539543f9a78814207c5ccda7",
        delete: "zl-f2d08563718d4925bb2e53e5603d38ca",
      },
      sysuser: initState(),
      userList: [],
      total: 0,
      listQuery: {
        page: 0, // dang
        limit: 1, // 每页显示的个数
      },
      PageQuery: {
        branchCompanyId: "",
        departmentId: "",
        pageIndex: 1,
        pageSize: 10,
      },
      dialogVisible: false,
      dialogType: "new",
      checkStrictly: false,
      rolelist: [],
      // rowroleId: '',
      IsEiderole: true, // 是否可以编辑自己的角色
      currentuserId: "", // 当前登录角色Id
      currentroleslevel: 0, // 当前角色的等级
      Isallow: false,
    };
  },
  async created() {
    this.currentuserId = this.$store.getters.id; // 当前登录账号的Id
    this.currentroleslevel = this.$store.getters.roles[0].level; // 当前登录的账号角色的等级

    // this.pgetuserList()// 获取系统用户的分页数据
    await this.GetPageDatasAsync();
  },
  methods: {
    updateUserAuthority,
    ...mapAccountActions([
      "QueryAccountUsersAsync",
      "CreateAccountUserAsync",
      "UpdateAccountUserAsync",
      "DeleteAccountUserAsync",
    ]),
    getTagStyle(charCode) {
      // 97 - 122
      const colors = ["#FA6E86", "#19D4AE", "#0080FF", "#5AB1EF"];
      const rate = (122 - 97 + 1) / colors.length;
      console.log(rate);
      let index = -1;
      colors.forEach((color, i) => {
        if (index > -1) return;
        if (charCode <= 97 + (i + 1) * rate) {
          index = i;
        }
      });
      return {
        background: colors[index < 0 ? 0 : index],
      };
    },
    async GetPageDatasAsync(xid) {
      if (xid) {
        this.PageQuery.pageIndex = xid.page;
        this.PageQuery.pageSize = xid.limit;
      }
      const ps = await this.QueryAccountUsersAsync(this.PageQuery);
      console.log("chax", ps);
      var userListA = ps.datas;
      this.userList = [];
      userListA.forEach((item) => {
        var isshow = false;
        if (item.id === this.currentuserId) {
          isshow = true;
        }
        if (item.roleLevel >= this.currentroleslevel) {
          isshow = true;
        }
        item.isshow = isshow;
        this.userList.push(item);
      });
      this.total = ps.total; // 总条数
    },

    //点击查询查询
    async Querybtnclick(arg, id) {
      if (arg === 0) {
        this.PageQuery.branchCompanyId = "";
        this.PageQuery.departmentId = "";
      }
      if (arg === 1) {
        this.PageQuery.branchCompanyId = id;
        this.PageQuery.departmentId = "";
      }
      if (arg === 3) {
        this.PageQuery.branchCompanyId = "";
        this.PageQuery.departmentId = id;
      }
      await this.GetPageDatasAsync();
    },

    // 新增用户
    async handleAddUser() {
      this.dialogType = "new";
      this.sysuser = initState();
      // // 获取全部的角色数据
      this.rolelist = await getRoles();
      this.rolelist = this.rolelist.filter((item) => {
        return item.level <= this.currentroleslevel;
      });
      this.$openView(__dirname, "components/accountedit.vue", "新增用户", {
        modeldata: this.sysuser,
        rolelist: this.rolelist,
        dialogType: this.dialogType,
        callback: this.GetPageDatasAsync,
      });
    },
    // 编辑用户
    async handleEdit(scope) {
      this.sysuser = deepClone(scope.row);
      this.dialogType = "edit";
      // this.dialogVisible = true;
      // this.checkStrictly = true;

      // this.rowroleId = this.sysuser.roleId
      // 获取全部的角色数据
      this.rolelist = await getRoles();
      this.$openView(__dirname, "components/accountedit.vue", "编辑用户", {
        modeldata: this.sysuser,
        rolelist: this.rolelist,
        dialogType: this.dialogType,
        callback: this.GetPageDatasAsync,
      });
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

    async handleDelete({ $index, row }) {
      this.sysuser = deepClone(row);
      var c = await this.$confirmAsync("警告！", "确定要删除系统账号吗?");
      if (c) {
        const l = await this.DeleteAccountUserAsync(row.id);
        if (l) {
          this.userList.splice($index, 1);
          this.$message({
            type: "success",
            message: "删除成功!",
          });
        }
      }
    },

    // 关闭弹框按钮关闭
    closedialog() {
      // this.$refs["sysuser"].resetFields();
      this.dialogVisible = false;
      this.sysuser = initState();
    },
  },
};
</script>
<style lang="scss" scoped>
span.tag {
  display: inline-block;
  width: 30px;
  height: 30px;
  font-size: 18px;
  background: $primary-color;
  color: white;
  text-align: center;
  line-height: 30px;
  border-radius: 50%;
  margin-right: 10px;
}

.user-avatar {
  cursor: pointer;
  width: 30px;
  height: 30px;
  border-radius: 50%;
  margin-right: 10px;
  vertical-align: middle;
}

.window-container {
  z-index: 2010 !important;
}
</style>
