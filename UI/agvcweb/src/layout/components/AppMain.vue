<template>
  <section class="app-main">
    <!-- <transition name="fade-transform" mode="out-in"> -->
    <!-- :include="cachedViews" v-if="keepAlive"-->
    <keep-alive :include="cachedViews">
      <router-view :key="$route.fullPath" />
    </keep-alive>
    <!-- <router-view v-else :key="key" /> -->
    <!-- </transition> -->
    <WindowContainer />
  </section>
</template>

<script>
export default {
  name: "AppMain",
  computed: {
    cachedViews() {
      const a = this.$store.state.tagsView.showViewsList[
        this.$store.state.user.currentModules
      ];
      var b = [];
      if (a) {
        b = a.map((x) => {
          return x.name;
        });
      }
      b = [...new Set(b)];
      console.log("this.$store.user", b);
      return b;
    },
    keepAlive() {
      // console.log(this.cachedViews, this.$route.name)
      return this.cachedViews.includes(this.$route.fullPath);
    },
    key() {
      return this.$route.fullPath; //组件被强制不复用
    },
    queryKeys() {
      return this.$route.key;
    },
  },
  watch: {
    key(v) {
      !!v;
      // console.log(this.$route.path, v)
      // console.log(this.$store.state.tagsView.cachedViews)
    },
  },
};
</script>

<style lang="scss">
.app-main {
  padding: 20px 20px;
  .el-header,
  .el-main,
  .el-aside {
    padding: 0 !important;
  }
}
</style>
