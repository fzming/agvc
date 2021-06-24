<template>
  <div class="app-container">
    <caption-container caption="角色列表">
      <el-button type="primary" @click="handleAddRole">
        <i class="el-icon-plus" /> 新增角色
      </el-button>
    </caption-container>

    <region>
      <el-table :data="rolesList" stripe style="width: 100%;">
        <el-table-column type="selection" width="55" />
        <el-table-column label="角色名称" width="220">
          <template slot-scope="scope">
            <i class="el-icon-user" /> {{ scope.row.name }}
          </template>
        </el-table-column>
        <el-table-column align="header-center" label="描述">
          <template slot-scope="scope">
            <div style="color:gray">{{ scope.row.description }}</div>
          </template>
        </el-table-column>
        <el-table-column label="操作">
          <template slot-scope="scope">
            <!-- <el-button type="default" size="mini" :disabled="scope.row.isshow ? true : false"
              @click="handleEdit(scope)">
              <i class="el-icon-edit-outline" /> 编辑权限
            </el-button> -->
            <el-button type="default" size="mini" @click="handleEdit(scope)">
              <i class="el-icon-edit-outline" /> 编辑权限
            </el-button>

            <el-button type="danger" size="mini" :disabled="scope.row.delbtnshow ? true : false"
              @click="handleDelete(scope)">
              <i class="el-icon-delete" /> 删除
            </el-button>
          </template>
        </el-table-column>
      </el-table>
    </region>
    <el-dialog :visible.sync="dialogVisible" :title="dialogType === 'edit' ? '编辑角色' : '新增角色'">
      <el-form :model="role" label-width="80px" label-position="left">
        <el-form-item label="角色名称">
          <el-input v-model="role.name" style="width: 221px;" placeholder="角色名称" />
        </el-form-item>
        <el-form-item label="角色级别">
          <el-select v-model="role.level" placeholder="请选择">
            <el-option v-for="item in sysrolelevel" :key="item.value" :label="item.name" :value="item.value" />
          </el-select>
          * 级别数字越大，级别越高
        </el-form-item>
        <el-form-item label="角色描述">
          <el-input v-model="role.description" :autosize="{ minRows: 2, maxRows: 4 }" type="textarea" placeholder="角色描述"
            style="width: 321px;" />
        </el-form-item>
        <el-form-item label="角色授权">
          <RoleMenuTree v-model="sRoleMenus" :role-id="roleId" />
        </el-form-item>
      </el-form>
      <div style="text-align:right;">
        <el-button type="danger" @click="dialogVisible = false">
          取消
        </el-button>
        <el-button type="primary" @click="confirmRole">
          确定
        </el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script>
  import { deepClone } from "@/utils";
  import {
    asyncRoutes,
    getRoles,
    addRole,
    deleteRole,
    updateRole
  } from "@/api/role";
  import RoleMenuTree from "./components/RoleMenuTree";
  import { roleGrade } from "@/global/permission";
  const defaultRole = {
    name: "",
    description: "",
    routes: []
  };

  export default {
    components: { RoleMenuTree },
    data() {
      return {
        roleId: "",
        sRoleMenus: [],
        role: defaultRole,
        routes: [],
        rolesList: [],
        dialogVisible: false,
        dialogType: "New Role",
        checkStrictly: false,
        defaultProps: {
          children: "children",
          label: "name"
        },
        metaroutes: [],
        sysrolelevel: [],
        currentrolesId: "", // 当前登录角色Id
        currentroleslevel: 0 // 当前角色的等级
      };
    },
    computed: {
      routesData() {
        return this.routes;
      }
    },
    async created() {
      const dlroles = this.$store.getters.roles[0];
      this.currentrolesId = dlroles.id;
      this.currentroleslevel = dlroles.level;
      const a = await this.asyncRoutes(true, true);
      this.routes = a;
      await this.getallRoles(); // 获取当前的全部的角色
      const ut = this.$store.getters.token.userType; // 用户角色的等级
      if (ut === "Admin") {
        // 系统用户
        this.sysrolelevel = roleGrade.sysrolw;
      } else {
        // 机构用户
        this.sysrolelevel = roleGrade.orgrolw;
      }
    },
    methods: {
      asyncRoutes,
      getRoles,
      // 查询角色信息
      async getallRoles() {
        var a = await getRoles();
        if (a) {
          this.rolesList = [];
          this.rolesListA = a;
          this.rolesListA.forEach(item => {
            var isshow = false;
            var delbtnshow = false;
            if (item.level >= this.currentroleslevel) {
              isshow = true;
              delbtnshow = true;
            }
            if (item.level === 90) {
              delbtnshow = true;
            }
            item.isshow = isshow;
            item.delbtnshow = delbtnshow;
            this.rolesList.push(item);

          });
        }
      },
      handleAddRole() {
        this.role = Object.assign({}, defaultRole);
        this.dialogType = "new";
        this.dialogVisible = true;
        this.roleId = "";
      },
      handleEdit(scope) {
        this.role = deepClone(scope.row);
        this.roleId = this.role.id;
        // if (this.role.level >= this.currentroleslevel) {
        if (false) {
          this.$message({
            type: "error",
            message: "当前用户角色等级小于或等于该角色等级,不支持编辑!"
          });
          return;
        } else {
          this.dialogType = "edit";
          this.dialogVisible = true;
          this.checkStrictly = true;
        }
      },
      async handleDelete({ $index, row }) {
        this.role = deepClone(row);
        if (this.role.level >= this.currentroleslevel) {
          this.$message({
            type: "error",
            message: "当前用户角色等级小于或等于该角色等级,不支持编辑!"
          });
          return;
        }
        var c = await this.$confirmAsync("警告！", "确定要删除该角色吗?");
        if (c) {
          const l = await deleteRole(row.id);
          if (l) {
            this.rolesList.splice($index, 1);
            this.$message({
              type: "success",
              message: "删除成功!"
            });
          }
        }
      },
      async confirmRole() {
        const isEdit = this.dialogType === "edit";
        if (isEdit) {
          // 修改角色
          if (this.role.name === "") {
            this.$message({
              type: "error",
              message: "角色名称不为空!"
            });
            return;
          }
          this.routesData.authorizes = this.sRoleMenus;
          this.routesData.roleId = this.role.id;
          this.routesData.name = this.role.name;
          this.routesData.description = this.role.description;
          this.routesData.level = this.role.level;
          const res = await updateRole(this.routesData);
          if (res) {
            for (let index = 0; index < this.rolesList.length; index++) {
              if (this.rolesList[index].id === this.role.id) {
                this.rolesList.splice(index, 1, Object.assign({}, this.role));
                break;
              }
            }
            this.$message({
              type: "success",
              message: "更新成功!"
            });
            this.dialogVisible = false;
          } else {
            this.$message({
              type: "error",
              message: "新增失败!"
            });
          }
        } else {
          // 新增角色
          if (this.role.name === "") {
            this.$message({
              type: "error",
              message: "角色名称不为空!"
            });
            return;
          }
          var i = 0;
          while (i < this.rolesList.length) {
            if (this.rolesList[i].name === this.role.name) {
              this.$message({
                type: "error",
                message: "新增的角色名称已经存在，不可以相同!"
              });
              i++;
              return;
            }
            i++;
          }
          this.role.Authorizes = this.sRoleMenus;
          const role = await addRole(this.role);
          if (role) {
            this.rolesList.push(role);
            this.$message({
              type: "success",
              message: "新增成功!"
            });
            this.dialogVisible = false;
          } else {
            this.$message({
              type: "error",
              message: "新增失败!"
            });
          }
        }
      }
    }
  };
</script>

<style lang="scss" scoped>
  .app-container {
    .roles-table {
      margin-top: 30px;
    }

    .permission-tree {
      margin-bottom: 30px;
    }
  }
</style>