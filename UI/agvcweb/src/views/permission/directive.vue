<template>
  <div>
    <el-row>
      <el-col :span="24">
        <el-button type="primary" @click="addCommend">新增指令</el-button>
        <el-dialog
          :title="isModify ? '修改' : '新增'"
          :visible.sync="showDialog"
          width="700px"
          center
          :show-close="showDialog"
          :close-on-click-modal="false"
        >
          <el-form
            ref="formData"
            :model="formData"
            :rules="rules"
            style="width: 600px"
            label-width="120px"
          >
            <el-form-item label="所属菜单" prop="MenuName">
              <el-cascader
                v-model="currentMenu"
                :options="routesData"
                :props="defaultProps"
                :change-on-select="true"
                clearable
                @change="handleChange"
              >
                <template slot-scope="{ node, data }">
                  <span>{{ data.meta.title }}</span>
                </template>
              </el-cascader>
            </el-form-item>
            <el-form-item label="指令名称" prop="Name">
              <el-input v-model="formData.Name" placeholder="请输入授权名称" />
            </el-form-item>
            <el-form-item label="指令描述" prop="Desc">
              <el-input
                v-model="formData.Desc"
                placeholder="请输入指令描述内容"
                type="textarea"
                :autosize="{ minRows: 2, maxRows: 8 }"
                maxlength="300"
                show-word-limit
              />
            </el-form-item>
            <el-form-item label="是否禁用" prop="Disabled">
              <el-select
                v-model="formData.Disabled"
                placeholder="请选择是否禁用"
                @change="disableDataChange"
              >
                <el-option
                  v-for="item in disabledData"
                  :key="item.value"
                  :label="item.label"
                  :value="item.value"
                />
              </el-select>
              <el-tag style="margin-left: 10px" type="danger"
                >禁用会使指令权限无效，可以无限制使用</el-tag
              >
            </el-form-item>
            <el-form-item label="禁用类型" prop="DisableType">
              <el-select
                v-model="formData.DisableType"
                placeholder="请选择禁用类型"
              >
                <el-option
                  v-for="item in disabledType"
                  :key="item.value"
                  :label="item.label"
                  :value="item.value"
                />
              </el-select>
            </el-form-item>
            <el-form-item>
              <el-button
                v-if="isModify"
                type="primary"
                size="mini"
                @click="modify"
                >保存</el-button
              >
              <el-button v-else type="primary" size="mini" @click="save"
                >保存</el-button
              >
              <el-button
                v-if="isModify"
                type="primary"
                size="mini"
                @click="cancel"
                >取消</el-button
              >
            </el-form-item>
          </el-form>
        </el-dialog>
      </el-col>
    </el-row>
    <el-row :gutter="20">
      <el-col :span="6">
        <!-- <el-tree
          ref="tree"
          :check-strictly="checkStrictly"
          :data="routesData"
          :props="defaultProps"
          node-key="index"
          class="permission-tree"
          :default-expand-all="true"
          min-height="600px"
          height="100%"
        >
          <span slot-scope="{ node, data }" class="custom-tree-node">
            <span>{{ data.meta.title }}</span>
            <span>
              <el-button
                type="text"
                size="mini"
                @click.stop="() => btnmodify(node, data)"
                >管理</el-button
              >
            </span>
          </span>
        </el-tree> -->
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
          <el-table-column label="操作" width="60" align="center">
            <template slot-scope="{ node, row }">
              <el-button
                circle
                type="primary"
                icon="el-icon-edit-outline"
                @click.stop="btnmodify(node, row)"
                size="mini"
              >
              </el-button>
            </template>
          </el-table-column>
        </el-table>
      </el-col>
      <el-col :span="18">
        <el-main>
          <el-table
            ref="multipleTable"
            :data="tableData"
            :height="tableHeight"
            tooltip-effect="dark"
            style="border: solid 1px #dfe6ec"
            min-height="600px"
          >
            <el-table-column label="序号" type="index" align="center" />
            <el-table-column
              prop="name"
              width="150"
              label="指令名称"
              align="center"
            >
              <template slot-scope="scope">{{ scope.row.name }}</template>
            </el-table-column>
            <el-table-column
              prop="menuName"
              width="120"
              label="所属页面"
              align="center"
            />
            <el-table-column
              prop="desc"
              label="指令描述"
              align="center"
              :show-overflow-tooltip="true"
            >
              <template slot-scope="scope">{{ scope.row.desc }}</template>
            </el-table-column>
            <el-table-column
              prop="disabled"
              width="100"
              align="center"
              label="是否失效"
            >
              <template slot-scope="scope">{{
                scope.row.disabled ? "无效" : "有效"
              }}</template>
            </el-table-column>
            <el-table-column
              prop="code"
              width="150"
              label="指令编号"
              align="center"
            >
              <template slot-scope="scope">
                <el-popover
                  placement="right"
                  width="400"
                  trigger="hover"
                  :visible-arrow="false"
                >
                  <el-tag>{{ scope.row.code }}</el-tag>
                  <el-button
                    style="float: right"
                    type="primary"
                    size="mini"
                    @click="handleClipboard(scope.row.code, $event)"
                    >复制</el-button
                  >
                  <el-button slot="reference">指令编号</el-button>
                </el-popover>
              </template>
            </el-table-column>
            <el-table-column
              prop="disableType"
              width="100"
              align="center"
              label="禁用状态"
              show-overflow-tooltip
            >
              <template slot-scope="scope">{{
                disabledType[scope.row.disableType].label
              }}</template>
            </el-table-column>
            <el-table-column label="日期" width="160" align="center">
              <template slot-scope="scope">{{ scope.row._c }}</template>
            </el-table-column>
            <el-table-column label="操作" width="160" align="center">
              <template slot-scope="scope">
                <el-button
                  size="mini"
                  @click="handleEdit(scope.$index, scope.row)"
                  >编辑</el-button
                >
                <el-button
                  size="mini"
                  type="danger"
                  @click="handleDelete(scope.$index, scope.row)"
                  >删除</el-button
                >
              </template>
            </el-table-column>
          </el-table>
        </el-main>
      </el-col>
    </el-row>
  </div>
</template>

<script>
import { asyncRoutes } from "@/api/role";
import { Message, MessageBox } from "element-ui";
import clipboard from "@/utils/clipboard";
import { mapCommendActions } from "@/store/namespaced/commend";
export default {
  name: "DirectivePermission",
  data() {
    return {
      routes: [],
      defaultProps: {
        label: "metaTitle",
        value: "id",
        children: "children",
        checkStrictly: true,
      },
      checkStrictly: false,
      defaultDroup: {
        label: "name",
        value: "id",
        children: "children",
        checkStrictly: true,
        codeDisableType: "隐藏",
      },
      curentRouteId: [],
      tableData: [],
      tableDatas: [],
      disabledData: [
        { value: false, label: "否" },
        { value: true, label: "是" },
      ],
      disabledType: [
        { value: 0, label: "隐藏" },
        { value: 1, label: "禁用" },
        { value: 2, label: "提示" },
      ],
      formData: {
        CodeId: "",
        Name: "",
        Desc: "",
        Disabled: false,
        DisableType: 0,
        MenuId: "",
      },
      multipleSelection: [],
      MenuName: "",
      rules: {
        Disabled: [
          { required: true, message: "请选择是否禁用", trigger: "blur" },
        ],
        Name: [{ required: true, message: "请输入指令名称", trigger: "blur" }],
        DisableType: [
          { required: true, message: "请选择禁用类型", trigger: "blur" },
        ],
        Desc: [
          { required: true, message: "请输入指令描述内容", trigger: "blur" },
        ],
      },
      currentMenu: [],
      isModify: false,
      showDialog: false,
      metas: [],
    };
  },
  computed: {
    routesData() {
      this.oplk(this.routes);
      return this.routes;
    },
    tableHeight() {
      return window.innerHeight - 200;
    },
  },
  watch: {
    currentMenu() {
      this.formData.MenuId =
        this.currentMenu.length > 0
          ? this.currentMenu[this.currentMenu.length - 1]
          : null;
    },
  },
  created() {
    this.pgetRoutes();
  },
  methods: {
    ...mapCommendActions(["remove", "create", "update", "query"]),
    oplk(list) {
      var that = this;
      list.forEach((value) => {
        value.metaTitle = value.meta.title;
        if (value.children) {
          that.oplk(value.children);
        }
      });
    },
    pgetRoutes() {
      asyncRoutes().then((res) => {
        this.routes = res;
        this.selectMenuName(res);
        this.mquery();
      });
    },
    handleChange(value) {
      this.currentMenu = value;
      this.formData.MenuId = value[value.length - 1];
    },
    handleEdit(index, row) {
      this.formData.MenuId = row.menuId;
      this.formData.CodeId = row.id;
      this.formData.Name = row.name;
      this.formData.Disabled = row.disabled;
      this.formData.DisableType = row.disableType;
      this.selectMenuPath(this.routes, row.menuId);
      this.formData.Desc = row.desc;
      this.isModify = true;
      this.showDialog = true;
    },
    selectMenuPath(list, menuid) {
      var that = this;
      if (list) {
        list.forEach(function (value, index) {
          if (value.id === menuid) {
            that.currentMenu =
              value.paMenuPath != null
                ? (value.paMenuPath + "," + value.id).split(",")
                : [menuid];
          } else {
            if (value.children) {
              that.selectMenuPath(value.children, menuid);
            }
          }
        });
      }
    },
    selectMenuName(list) {
      var that = this;
      if (list) {
        list.forEach(function (value, index) {
          that.metas.push({ id: value.id, label: value.meta.title });
          if (value.children) {
            that.selectMenuName(value.children);
          }
        });
      }
    },
    handleClipboard(text, event) {
      clipboard(text, event);
    },
    handleDelete(index, row) {
      MessageBox.confirm("是否确认删除此指令", "提示", {
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        type: "warning",
      })
        .then(async () => {
          const a = await this.remove({ codeId: row.id });
          console.log(a);
          if (a) {
            this.mquery();
          }
        })
        .catch(() => {
          this.$message({
            type: "info",
            message: "已取消删除",
          });
        });
    },
    addCommend() {
      this.isModify = false;
      this.showDialog = true;
    },
    disableDataChange(value) {
      console.log(this.formData);
    },
    toggleSelection(rows) {
      if (rows) {
        rows.forEach((row) => {
          this.$refs.multipleTable.toggleRowSelection(row);
        });
      } else {
        this.$refs.multipleTable.clearSelection();
      }
    },
    handleSelectionChange(val) {
      this.multipleSelection = val;
    },
    btnmodify(data, rdata) {
      this.MenuName = rdata.path + "/" + rdata.name;
      console.log(rdata);
      this.currentMenu =
        rdata.paMenuPath != null
          ? (rdata.paMenuPath + "," + rdata.id).split(",")
          : [rdata.id];
      this.tableData = this.tableDatas.filter((x) => x.menuId === rdata.id);
    },
    async save() {
      this.$refs["formData"].validate(async (valid) => {
        if (valid) {
          const a = await this.create(this.formData);
          if (a) {
            Message({
              message: "保存成功",
              type: "success",
              duration: 2 * 1000,
            });
          }
          await this.mquery();
        }
      });
    },
    async mquery() {
      const a = await this.query();
      console.log(a);
      a.forEach((value) => {
        this.metas.find((x) => {
          if (x.id === value.menuId) {
            value.menuName = x.label;
          }
        });
      });
      this.tableData = a;
      this.tableDatas = a;
    },
    async modify() {
      const a = await this.update(this.formData);
      console.log(a);
      this.mquery();
      Message({
        message: "修改成功",
        type: "success",
        duration: 2 * 1000,
      });
    },
    cancel() {
      this.isModify = false;
      this.showDialog = false;
      this.formData.MenuId = "";
      this.formData.CodeId = "";
      this.formData.Name = "";
      this.formData.Disabled = false;
      this.formData.DisableType = 0;
      this.formData.Desc = "";
      this.currentMenu = [];
    },
    handleRowClick(row) {
      this.$refs.dataTable.toggleRowExpansion(row);
    },
  },
};
</script>

<style lang="scss" scoped>
.app-container {
  .permission-alert {
    width: 320px;
    margin-top: 15px;
    background-color: #f0f9eb;
    color: #67c23a;
    padding: 8px 16px;
    border-radius: 4px;
    display: inline-block;
  }

  .permission-sourceCode {
    margin-left: 15px;
  }

  .permission-tag {
    background-color: #ecf5ff;
  }
}

.custom-tree-node {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: space-between;
  font-size: 14px;
  padding-right: 8px;
}

.el-tree {
  padding: 10px 0px;
}

.pagination {
  margin-top: 10px;
}
</style>
<style lang="scss" scoped>
.el-tooltip__popper {
  min-width: 10px;
  width: auto;
  max-width: 300px !important;
  line-height: 20px;
  letter-spacing: 1px;
}
</style>
