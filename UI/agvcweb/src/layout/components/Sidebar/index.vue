<template>
  <div :class="{ 'has-logo': showLogo }">
    <logo v-if="showLogo" :collapse="isCollapse" />
    <el-scrollbar wrap-class="scrollbar-wrapper">
      <el-menu
        :default-active="activeMenu"
        :collapse="isCollapse"
        :background-color="variables.menuBg"
        :text-color="variables.menuText"
        :unique-opened="true"
        :active-text-color="variables.menuActiveText"
        :collapse-transition="true"
        mode="vertical"
      >
        <sidebar-item
          v-for="route in tempMenuList"
          :key="route.path"
          :item="route"
          :base-path="route.path"
        />
      </el-menu>
    </el-scrollbar>
  </div>
</template>

<script>
import { mapGetters } from "vuex";
import Logo from "./Logo";
import SidebarItem from "./SidebarItem";
import variables from "@/styles/variables.scss";
export default {
  components: { SidebarItem, Logo },
  watch: {
    isCollapse() {
      this.$broadcast("$EventCollapseChange$");
    },
  },
  computed: {
    ...mapGetters(["permission_routes", "sidebar"]),
    activeMenu() {
      const route = this.$route;
      const { meta, path } = route;
      // if set path, the sidebar will highlight the path you set
      if (meta.activeMenu) {
        return meta.activeMenu;
      }
      return path;
    },
    showLogo() {
      return this.$store.state.settings.sidebarLogo;
    },
    variables() {
      return variables;
    },
    isCollapse() {
      return !this.sidebar.opened;
    },
    tempMenuList() {
      const module = this.$store.getters.module;
      const a = [];
      let tempRout = {};
      //console.log("permission_routes", this.permission_routes, module);
      this.permission_routes.forEach((x) => {
        if (x.modules) {
          if (x.modules.length > 0) {
            // console.log(`${x.modules.indexOf(module)}`);
            if (x.modules.indexOf(module) > -1) {
              tempRout = JSON.parse(JSON.stringify(x));
              if (tempRout.children) {
                this.filterRouters(tempRout, module);
              }
              a.push(tempRout);
            }
          } else {
            a.push(x);
          }
        } else {
          a.push(x);
        }
      });
      return a;
    },
  },
  methods: {
    filterRouters(temp, t) {
      const list = JSON.parse(JSON.stringify(temp.children));
      temp.children = [];
      list.map((x) => {
        if (x.modules && x.modules.length > 0) {
          // console.log(
          //   `child ${x.modules.indexOf(t)} ${list[index].meta.title}`
          // );
          if (x.modules.indexOf(t) > -1) {
            if (x.children && x.children.length > 0) {
              this.filterRouters(x, t);
            }
            temp.children.push(x);
          }
        }
      });
    },
  },
};
</script>
<style lang="scss" scoped>
.device {
  text-align: center;
  line-height: 100%;
  color: white;
  font-size: 14px;
}
</style>
