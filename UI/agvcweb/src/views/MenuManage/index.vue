<template>
  <el-row :gutter="20">
    <el-col :span="16">
      <el-table
        row-key="id"
        ref="dataTable"
        :data="routesData"
        :default-expand-all="false"
        :tree-props="{ children: 'children', hasChildren: 'row.hasChildren' }"
        :height="tableHeight"
        @row-click="handleRowClick"
      >
        <el-table-column label="名称" prop="metaTitle"></el-table-column>
        <el-table-column label="是否显示" width="80" align="center">
          <template slot-scope="{ row }">
            <a v-if="row.hidden" style="color: #f56c6c">隐藏</a>
            <a v-else style="color: #67c23a">显示</a>
          </template>
        </el-table-column>
        <el-table-column label="导航显示" width="80" align="center">
          <template slot-scope="{ row }">
            <a v-if="!row.meta.breadCrumb" style="color: #f56c6c">隐藏</a>
            <a v-else style="color: #67c23a">显示</a>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="200">
          <template slot-scope="{ node, row }">
            <template v-if="!row.hasChildren">
              <el-button
                circle
                type="danger"
                icon="el-icon-delete"
                @click="remove(node, row)"
                size="mini"
              >
              </el-button>
            </template>
            <el-button
              circle
              type="primary"
              icon="el-icon-edit-outline"
              @click="btnmodify(node, row)"
              size="mini"
            >
            </el-button>
            <el-button
              circle
              icon="el-icon-top"
              size="mini"
              @click="orderUp(node, row)"
            >
            </el-button>
            <el-button
              circle
              icon="el-icon-bottom"
              size="mini"
              @click="orderDown(node, row)"
            >
            </el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-col>
    <el-col :span="8">
      <el-form
        ref="ruleForm"
        :model="ruleForm"
        :rules="rules"
        label-width="120px"
        class="demo-ruleForm"
      >
        <el-form-item label="路由地址" prop="Path">
          <el-input v-model="ruleForm.Path" />
        </el-form-item>
        <el-form-item label="组件视图路径" prop="Component">
          <el-input v-model="ruleForm.Component" />
        </el-form-item>
        <el-form-item label="菜单名称" prop="Name">
          <el-input v-model="ruleForm.Name" />
        </el-form-item>
        <el-form-item label="跳转地址" prop="Redirect">
          <el-input v-model="ruleForm.Redirect" />
        </el-form-item>
        <el-form-item label="标题" prop="Meta.Title">
          <el-input v-model="ruleForm.Meta.Title" />
        </el-form-item>
        <el-form-item label="上级菜单">
          <el-cascader
            v-model="curentRouteId"
            :options="routesData"
            :props="defaultDroup"
            :change-on-select="true"
            clearable
            @change="handleChange"
          >
            <template slot-scope="{ node, data }">
              <span>{{ data.meta.title }}</span>
            </template>
          </el-cascader>
        </el-form-item>
        <el-form-item label="根目录展示" prop="AlwaysShow">
          <el-switch
            v-model="ruleForm.AlwaysShow"
            active-color="#13ce66"
            inactive-color="#ff4949"
          />
        </el-form-item>
        <el-form-item label="是否显示" prop="Hidden">
          <el-switch
            v-model="ruleForm.Hidden"
            active-color="#ff4949"
            inactive-color="#13ce66"
          />
        </el-form-item>
        <el-form-item label="是否被缓存" prop="Meta.NoCache">
          <el-switch
            v-model="ruleForm.Meta.NoCache"
            active-color="#ff4949"
            inactive-color="#13ce66"
          />
          <el-tag type="warning" style="margin-left: 20px">
            <el-icon class="el-icon-warning"></el-icon>
            菜单名称必须与页面{ name }属性相同，缓存才会生效
          </el-tag>
        </el-form-item>
        <el-form-item label="是否显示在导航" prop="Meta.BreadCrumb">
          <el-switch
            v-model="ruleForm.Meta.BreadCrumb"
            active-color="#13ce66"
            inactive-color="#ff4949"
          />
        </el-form-item>
        <el-form-item label="隶属模块" prop="Modules">
          <el-select v-model="ruleForm.Modules" multiple placeholder="请选择">
            <el-option
              v-for="item in modelTypes"
              :key="item.value"
              :label="item.label"
              :value="item.value"
            >
            </el-option>
          </el-select>
        </el-form-item>
        <el-form-item label="图标" prop="Meta.Icon">
          <svg-icon
            :icon-class="ruleForm.Meta.Icon"
            class-name="disabled icoRow"
          />
          <el-button
            style="float: left; margin-right: 15px"
            @click="ico_visible = true"
          >
            选择
          </el-button>
          <el-dialog
            title="图标"
            width="50%"
            center
            :visible.sync="ico_visible"
            :show-close="ico_visible"
          >
            <div style="height: 500px; overflow: auto">
              <div
                v-for="item of svgIcons"
                :key="item.key"
                @click="handleClipboard(item, $event)"
              >
                <div class="icon-item">
                  <svg-icon :icon-class="item" class-name="disabled" />
                  <span>{{ item }}</span>
                </div>
              </div>
            </div>
          </el-dialog>
        </el-form-item>
        <el-form-item>
          <el-button v-if="isModify" type="primary" @click="modify('ruleForm')">
            确认保存
          </el-button>
          <el-button v-else type="primary" @click="submitForm('ruleForm')">
            立即创建
          </el-button>
          <!-- <el-button @click="resetForm('ruleForm')">重置</el-button> -->
          <el-button v-if="isModify" @click="cancel">取消</el-button>
        </el-form-item>
      </el-form>
    </el-col>
  </el-row>
  <!-- <el-container> -->
  <!-- <el-aside
      width="300px"
      style="
        background-color: rgb(238, 241, 246);
        padding: 10px;
        height: 500px;
        overflow-y: scroll;
      "
    >
      <el-tree
        ref="tree"
        :check-strictly="checkStrictly"
        :data="routesData"
        :props="defaultProps"
        node-key="index"
        class="permission-tree"
        :default-expand-all="true"
      >
        <span slot-scope="{ node, data }" class="custom-tree-node">
          <span>{{ data.meta.title }}</span>
          <span class="buttons">
            <a @click.stop="btnmodify(node, data)"> 编辑 </a>
            <a @click.stop="remove(node, data)"> 删除 </a>
            <a @click.stop="orderUp(node, data)">↑</a>
            <a type="text" @click.stop="orderDown(node, data)">↓</a>
          </span>
        </span>
      </el-tree>
    </el-aside> -->
  <!-- <el-main> </el-main> -->
  <!-- </el-container> -->
</template>
<script>
import { asyncRoutes, getRoles } from "@/api/role";
import { MODULE_NAME_TYPE } from "@/global/const";
import svgIcons from "../icons/svg-icons";
import { MessageBox } from "element-ui";
const defaultRole = {
  key: "",
  name: "",
  description: "",
  routes: [],
};
export default {
  data() {
    return {
      centerDialogVisible: false,
      ico_visible: false,
      role: Object.assign({}, defaultRole),
      proutes: [],
      rolesList: [],
      dialogVisible: false,
      dialogType: "new",
      checkStrictly: false,
      defaultProps: {
        children: "children",
        label: "name",
      },
      defaultDroup: {
        label: "metaTitle",
        value: "id",
        children: "children",
        checkStrictly: true,
      },
      currentCheck: [],
      currentNode: {},
      ruleForm: {
        ID: "",
        PaMenuId: "", // / 父级菜单ID
        Name: "", // / 菜单名称
        Path: "", // / 菜单地址
        Component: "", // / 组件视图路径
        Meta: {
          // / Meta元数据
          Icon: "", // / 图标
          Title: "", // / 菜单窗口标题
          NoCache: false, // / 如果设置为true，则不会被 <keep-alive> 缓存(默认 false)
          BreadCrumb: false, // / 如果设置为false，则不会在breadcrumb面包屑中显示
        },
        Redirect: "", // / 跳转参数
        AlwaysShow: false, // / 根目录展示
        Hidden: false, // / 当设置 true 的时候该路由不会再侧边栏出现 如401，login等页面，或者如一些编辑页面/edit/1
        Modules: [],
      },
      rules: {
        Path: [
          {
            required: true,
            message: "请输入path",
            trigger: "blur",
          },
        ],
        Component: [
          {
            required: true,
            message: "请输入component",
            trigger: "blur",
          },
        ],
        Name: [
          {
            required: true,
            message: "请输入name",
            trigger: "blur",
          },
        ],
        Meta: [
          {
            required: true,
            message: "请输入meta",
            trigger: "blur",
          },
        ],
        Modules: [
          {
            type: "array",
            required: true,
            message: "请选择模块",
            trigger: "change",
          },
        ],
        // Redirect: [
        //   { required: true, message: '请输入redirect', trigger: 'blur' }
        // ]
      },
      curentRole: [],
      curentRouteId: [],
      isModify: false,
      svgIcons,
    };
  },
  computed: {
    routesData() {
      this.oplk(this.proutes);
      return this.proutes;
    },
    modelTypes() {
      return MODULE_NAME_TYPE.map((x) => {
        return {
          value: x.value,
          label: x.name,
        };
      });
    },
    tableHeight() {
      return window.innerHeight - 200;
    },
  },
  watch: {
    curentRouteId(newValue) {
      this.ruleForm.PaMenuId = newValue[newValue.length - 1];
      this.ruleForm.PaMenuPath = newValue.join(",");
    },
  },
  created() {
    // Mock: get all routes and roles list from server
    this.pgetRoutes();
    this.pgetRoles();
  },
  methods: {
    oplk(list) {
      var that = this;
      list.forEach((value) => {
        value.metaTitle = value.meta.title;
        value.hasChildren = value.children ? true : false;
        if (value.children) {
          that.oplk(value.children);
        }
      });
    },
    pgetRoutes() {
      asyncRoutes().then((res) => {
        this.serviceRoutes = res;
        this.proutes = res;

        console.log(this.proutes);
      });
    },
    pgetRoles() {
      getRoles().then((l) => {
        this.rolesList = l;
        console.log(this.rolesList);
      });
    },
    btnmodify(data, rdata) {
      console.log(rdata);
      this.ruleForm.ID = rdata.id;
      this.ruleForm.PaMenuId = rdata.paMenuId;
      this.ruleForm.Name = rdata.name;
      this.ruleForm.Path = rdata.path;
      this.ruleForm.Component = rdata.component;
      this.ruleForm.Meta.Title = rdata.meta.title;
      this.ruleForm.Redirect = rdata.redirect;
      this.ruleForm.Modules = rdata.modules;
      // this.ruleForm.Meta.Roles = rdata.meta.roles != null && rdata.meta.roles.length > 0 ? rdata.meta.roles : [];
      this.ruleForm.PaMenuPath =
        rdata.paMenuPath != null && rdata.paMenuPath.length > 0
          ? rdata.paMenuPath.split(",")
          : [];
      this.isModify = true;
      this.curentRouteId =
        rdata.paMenuPath != null && rdata.paMenuPath.length > 0
          ? rdata.paMenuPath.split(",")
          : [];
      this.curentRole =
        rdata.meta.roles != null && rdata.meta.roles.length > 0
          ? rdata.meta.roles
          : [];
      // Icon: 'dashboard'
      this.ruleForm.Meta.NoCache = rdata.meta.noCache;
      this.ruleForm.Meta.BreadCrumb = rdata.meta.breadCrumb;
      this.ruleForm.Redirect = rdata.redirect;
      this.ruleForm.AlwaysShow = rdata.alwaysShow;
      this.ruleForm.Hidden = rdata.hidden;
      this.ruleForm.Meta.Icon = rdata.meta.icon;
      console.log(this.ruleForm.PaMenuPath);
    },

    // append(data) {
    //   console.log(data)
    //   const newChild = { label: 'testtest', children: [] }
    //   if (!data.children) {
    //     this.$set(data, 'children', [])
    //   }
    //   data.children.push(newChild); console.log(data)
    // },

    remove(node, data) {
      MessageBox.confirm("是否确认删除此菜单", "提示", {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        type: "warning",
      })
        .then(() => {
          console.log(data.id);
          this.$store
            .dispatch("route/remove", {
              id: data.id,
            })
            .then((res) => {
              if (res) {
                this.isModify = false;
                this.resetForm("ruleForm");
                this.pgetRoutes();
              }
            });
        })
        .catch(() => {
          this.$message({
            type: "info",
            message: "已取消删除",
          });
        });
    },
    orderUp(node, data) {
      this.$store
        .dispatch("route/upDown", {
          MenuId: data.id,
          Direction: 0,
        })
        .then((res) => {
          if (res) {
            this.isModify = false;
            this.pgetRoutes();
          }
        })
        .catch((s) => {
          !!s;
          this.$message({
            message: "无法上移",
            type: "warning",
          });
        });
    },
    orderDown(node, data) {
      this.$store
        .dispatch("route/upDown", {
          MenuId: data.id,
          Direction: 1,
        })
        .then((res) => {
          if (res) {
            this.isModify = false;
            this.pgetRoutes();
          }
        })
        .catch((s) => {
          this.$message({
            message: "无法下移",
            type: "warning",
          });
        });
    },
    roleChange(value) {
      this.ruleForm.Meta.Roles = [...value];
    },
    handleChange(value) {
      this.ruleForm.PaMenuId = value[value.length - 1];
      this.ruleForm.PaMenuPath = value.join(",");
      console.log(this.ruleForm);
    },
    submitForm(formName) {
      this.$refs[formName].validate((valid) => {
        if (valid) {
          this.$store.dispatch("route/create", this.ruleForm).then((res) => {
            if (res) {
              this.isModify = false;
              this.resetForm("ruleForm");
              this.pgetRoutes();
            }
          });
        } else {
          return false;
        }
      });
    },
    modify(formName) {
      this.$refs[formName].validate((valid) => {
        if (valid) {
          MessageBox.confirm("是否确认修改此菜单", "提示", {
            confirmButtonText: "确定",
            cancelButtonText: "取消",
            type: "warning",
          })
            .then(() => {
              this.$store
                .dispatch("route/update", this.ruleForm)
                .then((res) => {
                  console.log(res);
                  if (res) {
                    this.isModify = false;
                    this.resetForm("ruleForm");
                    this.curentRouteId = [];
                    this.pgetRoutes();
                  }
                });
            })
            .catch(() => {
              this.$message({
                type: "info",
                message: "已取消删除",
              });
            });
        } else {
          return false;
        }
      });
    },
    resetForm(formName) {
      // this.ruleForm.ID = "";
      // this.ruleForm.PaMenuId = "";
      // this.ruleForm.Name = "";
      // this.ruleForm.Path = "";
      // this.ruleForm.Component = "";
      // this.ruleForm.Meta.Title = "";
      // this.ruleForm.Redirect = "";
      // this.ruleForm.Meta.BreadCrumb = false;
      // this.ruleForm.Hidden = false;
      // this.ruleForm.Modules = [];
      this.$refs[formName].resetFields();
      this.isModify = false;
    },
    cancel() {
      this.isModify = false;
      this.resetForm("ruleForm");
      this.curentRouteId = [];
    },
    // generateIconCode(symbol) {
    //   return `<svg-icon icon-class="${symbol}" />`
    // },
    // generateElementIconCode(symbol) {
    //   return `<i class="el-icon-${symbol}" />`
    // },
    handleClipboard(text, event) {
      !!event;
      this.ruleForm.Meta.Icon = text;
      this.ico_visible = false;
      // clipboard(text, event)
    },
    handleRowClick(row) {
      this.$refs.dataTable.toggleRowExpansion(row);
    },
  },
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

.custom-tree-node {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: space-between;
  font-size: 14px;
  padding-right: 8px;

  .buttons {
    a {
      font-size: 12px;
      display: inline-block;
      padding: 0px 4px;
    }
  }
}

.el-tree {
  padding: 10px 0px;
}

.icon-item {
  margin: 20px;
  height: 85px;
  text-align: center;
  width: 100px;
  float: left;
  font-size: 30px;
  color: #24292e;
  cursor: pointer;
}

.icon-item span {
  display: block;
  font-size: 16px;
  margin-top: 10px;
}

.disabled {
  pointer-events: none;
}

.icoRow {
  width: 36px !important;
  height: 36px !important;
}
</style>
