<template>
  <div>
    <el-tree ref="tree" :data="routesData" show-checkbox node-key="id" :check-strictly="true" :props="defaultProps"
      :expand-on-click-node="false" @check="checkGroupNode">
      <span slot-scope="{ node, data }" class="custom-tree-node">
        <span>{{ data.meta.title }}</span>
      </span>
    </el-tree>
  </div>
</template>
<script>
  import { newasyncRoutes, urseasyncRoutes } from "@/api/role";
  export default {
    name: "RoleMenuTree",
    model: {
      prop: "selectedMenus",
      event: "changeSelectedMenus",
    },
    props: {
      selectedMenus: {
        type: Array,
        default: function () {
          return [];
        },
      },
      roleId: {
        type: String,
        default: "",
      },
      userId: {
        type: String,
        default: "",
      },
      Mtype: {
        type: Boolean,
        default: true
      }
    },
    data() {
      return {
        metaroutes: [],
        routesData: [],
        checkStrictly: false,
        defaultProps: {
          children: "children",
          label: "name",
        },
      };
    },
    watch: {
      async roleId() {
        await this.initTree();
      },
      metaroutes(value) {
        this.$emit("changeSelectedMenus", value);
      },
    },
    async mounted() {
      await this.initTree();
    },
    methods: {
      // asyncRoutes,
      newasyncRoutes,
      urseasyncRoutes,
      async initTree() {
        console.log(this.roleId);
        // this.routesData = await asyncRoutes(true, true, this.roleId, this.userId);
        // if (this.Mtype) { this.routesData = await newasyncRoutes(true) }
        // else { var userMdata = { roleId: this.roleId, userId: this.userId }; this.routesData = await urseasyncRoutes(userMdata) }
        var userMdata = { roleId: this.roleId, userId: this.userId };
        this.routesData = await urseasyncRoutes(userMdata)
        console.log(this.routesData);
        if (!this.roleId) {
          if (this.$refs.tree) {
            this.$refs.tree.setCheckedNodes([]);
          }
        } else {
          this.checkStrictly = true;
          this.$nextTick(() => {
            this.$refs.tree.setCheckedNodes(
              this.generateArr(this.routesData, this.roleId)
            );
            this.checkStrictly = false;
          });
        }
      },
      checkGroupNode() {
        this.metaroutes = [];
        const data = this.$refs.tree
          .getCheckedKeys()
          .concat(this.$refs.tree.getHalfCheckedKeys());
        this.metaroutes = data;
      },

      generateArr(routes, roleId) {
        let data = [];
        console.log(routes);
        if (routes) {
          routes.forEach((route) => {
            let bj = 0;
            if (route.meta && route.meta.roles) {
              route.meta.roles.forEach((role) => {
                if (role === roleId) {
                  bj = bj + 1;
                }
              });
            }
            if (bj > 0) {
              data.push(route);
              this.metaroutes.push(route.id);
              if (route.children) {
                const temp = this.generateArr(route.children, roleId);
                if (temp.length > 0) {
                  data = [...data, ...temp];
                }
              }
            }
          });
        }
        return data;
      },
    },
  };
</script>