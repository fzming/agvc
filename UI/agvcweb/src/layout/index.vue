<template>
  <div :class="classObj" class="app-wrapper">
    <div
      v-if="device === 'mobile' && sidebar.opened"
      class="drawer-bg"
      @click="handleClickOutside"
    />
    <Sidebar class="sidebar-container" />
    <VSignalr
      :autoreconnected="true"
      :client-id="clientId"
      @on-received="on_received"
    />
    <div :class="{ hasTagsView: needTagsView }" class="main-container">
      <div :class="{ 'fixed-header': fixedHeader }">
        <!-- <div class="glim">
          <img src="@/assets/images/newyear/glim.png" alt="2021新年" />
        </div>
        <div class="glim glim2">
          <img src="@/assets/images/newyear/glim.png" alt="2021新年" />
        </div> -->
        <Navbar />
        <TagsView v-if="needTagsView" />
      </div>
      <div class="fix-header-height" />
      <AppMain />
    </div>
  </div>
</template>

<script>
import { AppMain, Navbar, Sidebar, TagsView } from "./components";
import VSignalr from "./components/VSignalr";
import { mapState } from "vuex";
import { SIG_EVENT } from "@/global/const";
export default {
  name: "Layout",
  components: {
    AppMain,
    Navbar,
    Sidebar,
    TagsView,
    VSignalr,
  },
  computed: {
    ...mapState({
      sidebar: (state) => state.app.sidebar,
      device: (state) => state.app.device,
      showSettings: (state) => state.settings.showSettings,
      needTagsView: (state) => state.settings.tagsView,
      fixedHeader: (state) => state.settings.fixedHeader,
    }),
    clientId() {
      return this.$store.getters.id;
    },
    classObj() {
      return {
        hideSidebar: !this.sidebar.opened,
        openSidebar: this.sidebar.opened,
        withoutAnimation: this.sidebar.withoutAnimation,
        mobile: this.device === "mobile",
      };
    },
  },
  methods: {
    on_received(data) {
      window.eventBus.$emit(SIG_EVENT, data);
    },
    handleClickOutside() {
      this.$store.dispatch("app/closeSideBar", { withoutAnimation: false });
    },
  },
};
</script>

<style lang="scss" scoped>
.app-wrapper {
  min-height: 100%;
  background: #f8f8f8;
  &.mobile.openSidebar {
    position: fixed;
    top: 0;
  }
}
.main-container {
  min-height: 100%;
  margin-left: $sideBarWidth !important;
  overflow: hidden;
  transition: all 0.3s ease;
}
.drawer-bg {
  background: #000;
  opacity: 0.3;
  width: 100%;
  top: 0;
  height: 100%;
  position: absolute;
  z-index: 999;
}

.fixed-header {
  position: fixed;
  top: 0;
  left: 0;
  background: url(http://image.yy5156.com/map/sp-bg0.jpg) white no-repeat;
  background-size: 70% 180%;
  width: 100%;
  padding-left: $sideBarWidth !important;
  box-sizing: border-box;
  z-index: 7;
  transition: all 0.2s;
}
.fix-header-height {
  height: 92px;
}
.mobile .fixed-header {
  width: 100%;
}
.glim {
  position: absolute;
  max-width: 120px;
  top: -5px;
  left: 30%;
  animation: glim-move 9s infinite;
  pointer-events: none;
  transform-origin: top;
  img {
    max-width: 100%;
  }
}
.glim2 {
  max-width: 100px;
  left: 35%;
  animation: glim-move 5s infinite;
}
@keyframes glim-move {
  0% {
    transform: translate(0%) translateZ(0) rotate(0deg);

    opacity: 0.5;
  }
  50% {
    transform: translate(30%) translateZ(600px) rotate(20deg);
    transform-style: preserve-3d;
    perspective: 1300px;
    opacity: 1;
  }
  100% {
    transform: translate(0%) translateZ(0) rotate(0deg);
    opacity: 0.5;
  }
}
</style>
