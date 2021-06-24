<template>
  <div id="tags-view-container" class="tags-view-container">
    <ScrollPane ref="scrollPane" class="tags-view-wrapper">
      <router-link
        v-for="tag in visitedViews"
        ref="tag"
        :key="`${tag.fullPath}`"
        :class="isActive(tag) ? 'active' : ''"
        :to="{ path: tag.path, query: tag.query, fullPath: tag.fullPath }"
        tag="span"
        class="tags-view-item"
        @click.middle.native="closeSelectedTag(tag)"
        @contextmenu.prevent.native="openMenu(tag, $event)"
      >
        {{ tag.title }}
        {{ tag.tagsViewIndex == 0 ? "" : "-" + tag.tagsViewIndex }}
        <span
          v-if="!tag.meta.affix"
          class="el-icon-close"
          @click.prevent.stop="closeSelectedTag(tag)"
        />
      </router-link>
    </ScrollPane>
    <ul
      v-show="visible"
      :style="{ left: left + 'px', top: top + 'px' }"
      class="contextmenu"
    >
      <li @click="refreshSelectedTag(selectedTag)">刷新</li>
      <li
        v-if="!(selectedTag.meta && selectedTag.meta.affix)"
        @click="openNewTag(selectedTag)"
      >
        从新标签页中打开
      </li>
      <!-- <li @click="addCallBack(selectedTag)">
        添加关闭确认询问
      </li> -->
      <li
        v-if="!(selectedTag.meta && selectedTag.meta.affix)"
        @click="closeSelectedTag(selectedTag)"
      >
        关闭
      </li>
      <li @click="closeOthersTags(selectedTag)">关闭其他</li>
      <li @click="closeAllTags(selectedTag)">关闭所有</li>
    </ul>
  </div>
</template>

<script>
import ScrollPane from "./ScrollPane";
import path from "path";
import { mapGetters } from "vuex";
import variables from "@/styles/variables.scss";
import { mapTagsViewGetters } from "@/store/namespaced/kernel/tagsView";
export default {
  components: { ScrollPane },
  data() {
    return {
      visible: false,
      top: 0,
      left: 0,
      selectedTag: {},
      affixTags: [],
      tagIndex: 0,
    };
  },
  computed: {
    ...mapTagsViewGetters(["visitedViews", "currentPath"]),
    ...mapGetters(["sidebar"]),
    routes() {
      return this.$store.state.permission.routes;
    },
    variables() {
      return variables;
    },
    sidebarWidth() {
      let w = this.variables.sideBarWidth || "190px";
      return !this.sidebar.opened ? 54 : parseInt(w.replace(/[^\d]/gi, ""));
    },
  },
  watch: {
    "$route.fullPath"() {
      // console.log("======", val, this.$route);
      this.addTags(this.$route);
      this.moveToCurrentTag();
    },
    visible(value) {
      if (value) {
        document.body.addEventListener("click", this.closeMenu);
      } else {
        document.body.removeEventListener("click", this.closeMenu);
      }
    },
  },
  mounted() {
    // console.log("======", this.visitedViews);
    this.initTags();
    this.addTags();
  },
  methods: {
    isActive(route) {
      return route.fullPath === this.$route.fullPath;
    },
    filterAffixTags(routes, basePath = "/") {
      let tags = [];
      routes.forEach((route) => {
        if (route.meta && route.meta.affix) {
          const tagPath = path.resolve(basePath, route.path);
          tags.push({
            fullPath: tagPath,
            path: tagPath,
            name: route.name,
            meta: { ...route.meta },
          });
        }
        if (route.children) {
          const tempTags = this.filterAffixTags(route.children, route.path);
          if (tempTags.length >= 1) {
            tags = [...tags, ...tempTags];
          }
        }
      });
      return tags;
    },
    initTags() {
      const affixTags = (this.affixTags = this.filterAffixTags(this.routes));
      for (const tag of affixTags) {
        // Must have tag name
        if (tag.name) {
          this.$store.dispatch("tagsView/addVisitedView", tag);
        }
      }
    },
    addTags(view) {
      // console.log("------========", view);
      const oview = view && view !== undefined ? view : this.$route;
      // console.log(oview);
      const { name } = oview;
      if (name) {
        // console.log(name);
        this.$store.dispatch("tagsView/addView", oview);
      }
      return false;
    },
    moveToCurrentTag() {
      const tags = this.$refs.tag;
      this.$nextTick(() => {
        for (const tag of tags) {
          if (tag.to.path === this.$route.path) {
            this.$refs.scrollPane.moveToTarget(tag);
            if (tag.to.fullPath !== this.$route.fullPath) {
              this.$store.dispatch("tagsView/updateVisitedView", this.$route);
            }
            break;
          }
        }
      });
    },
    async refreshSelectedTag(view) {
      //this.$store.dispatch("tagsView/delCachedView", view);
      await this.$store.dispatch("tagsView/delView", view);
      this.$nextTick(() => {
        this.$router.replace({ path: "/redirect" + view.fullPath });
      });
    },
    async closeSelectedTag(view) {
      const os = view;
      if (!Object.prototype.hasOwnProperty.call(os, "callBack")) {
        this.delView(view);
      } else {
        const res = await os.callBack();
        if (res) {
          this.$store.dispatch("tagsView/setModifyTag", false);
          this.delView(view);
        }
      }
    },
    delView(view) {
      this.$store.dispatch("tagsView/delView", view);
      if (this.isActive(view)) {
        this.toLastView(this.visitedViews, view);
      }
    },
    closeOthersTags(view) {
      this.$store.dispatch("tagsView/delOthersViews", view);
      this.toLastView(this.visitedViews, view);
    },
    async closeAllTags(view) {
      await this.$store.dispatch("tagsView/delAllViews");
      this.toLastView(this.visitedViews, view);
    },
    toLastView(visitedViews, view) {
      const latestView = visitedViews.slice(-1)[0];
      // console.log(view.name);
      if (latestView) {
        this.$router
          .push(latestView)
          .then(() => {
            //console.log(res);
          })
          .catch(() => {
            //console.log(err);
          });
      } else {
        if (view.name === "Dashboard") {
          this.$router.replace({ path: "/redirect" + view.fullPath });
        } else {
          this.$router.push("/");
        }
      }
    },
    openNewTag(view) {
      // console.log(view);
      var idx = view.tagsViewIndex + 1;

      if (view.fullPath.indexOf("?") > -1) {
        var f = view.fullPath.split("&");
        var s = view.fullPath.split("tag");
        s[1] = `=${idx}`;
        if (f[1]) {
          s[1] = `=${idx}&${f[1]}`;
        }
        this.$router.push(s.join("&tag"));
      } else {
        this.$router.push(`${view.fullPath}?tag=${idx}`);
      }
    },
    addCallBack() {
      this.$store.dispatch("tagsView/setCallBackAlert", {
        fullPath: this.$route.fullPath,
        options: { title: "关闭确认", message: "是否确认关闭此页面?" },
      });
      // this.$store.dispatch("tagsView/setCallBack", {
      //   fullPath: this.$route.fullPath,
      //   callBack: () => {
      //     return this.$confirmAsync("关闭确认", "是否确认关闭此页面?");
      //   },
      // });
    },
    openMenu(tag, e) {
      const menuMinWidth = 102;
      const offsetLeft = this.$el.getBoundingClientRect().left; // container margin left
      const offsetWidth = this.$el.offsetWidth; // container width
      const maxLeft = offsetWidth - menuMinWidth; // left boundary
      const left = e.clientX + this.sidebarWidth - offsetLeft - +15; // 15: margin right

      if (left > maxLeft) {
        this.left = maxLeft;
      } else {
        this.left = left;
      }

      this.top = e.clientY;
      this.visible = true;
      this.selectedTag = tag;
    },
    closeMenu() {
      this.visible = false;
    },
  },
};
</script>

<style lang="scss" scoped>
.tags-view-container {
  width: 100%;
  border-bottom: 2px solid $primary-color;

  .tags-view-wrapper {
    .tags-view-item {
      display: inline-block;
      position: relative;
      cursor: pointer;
      border: 1px solid $light-blue-fade;
      color: #495060;
      background: transparent;
      /* border-radius: 15px; */
      padding: 5px 8px;
      min-width: 70px;
      font-size: 13px;
      margin-left: 0px;
      text-align: center;

      &:first-of-type {
        margin-left: 15px;
        border-top-left-radius: 3px;
      }

      &:last-of-type {
        margin-right: 15px;
        border-top-right-radius: 3px;
      }

      &:hover {
        color: $light-blue;
        border: 1px solid $light-blue;
        box-shadow: 2px 2px 3px $light-blue-fade;
      }

      &.active {
        background: $menuBg;
        color: #ffffff;
        border-color: $light-blue;
        box-shadow: 2px 3px 5px $light-blue-fade;
        font-weight: bold;
        &::before {
          content: "";
          background: $yellow;
          display: inline-block;
          width: 8px;
          height: 8px;
          border-radius: 50%;
          position: relative;
          margin-right: 2px;
          top: -1px;
        }
      }
    }
  }

  .contextmenu {
    margin: 0;
    background: #fff;
    z-index: 3000;
    position: absolute;
    list-style-type: none;
    padding: 0px;
    border: 1px solid #e6ebf5;
    margin: 5px;
    font-size: 12px;
    font-weight: 400;
    color: #333;
    box-shadow: 0 2px 12px 0 rgba(0, 0, 0, 0.1);

    li {
      margin: 0;
      padding: 7px 26px;
      cursor: pointer;

      &:hover {
        color: $light-blue;
        background: $light-blue-fade;
      }
    }
  }
}
</style>

<style lang="scss">
//reset element css of el-icon-close
.tags-view-wrapper {
  .tags-view-item {
    .el-icon-close {
      width: 16px;
      height: 16px;
      vertical-align: 3px;
      border-radius: 50%;
      text-align: center;
      transition: all 0.3s cubic-bezier(0.645, 0.045, 0.355, 1);
      transform-origin: 100% 50%;
      margin-right: -2px;

      &:before {
        display: inline-block;
        vertical-align: -2px;
      }

      &:hover {
        background-color: $red;
        color: #fff;
      }
    }
  }
}
</style>
