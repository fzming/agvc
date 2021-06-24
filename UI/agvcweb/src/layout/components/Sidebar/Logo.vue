<template>
  <div class="sidebar-logo-container" :class="{ collapse: collapse }">
    <transition name="sidebarLogoFade">
      <router-link
        v-if="collapse"
        key="collapse"
        class="sidebar-logo-link"
        to="/"
      >
        <img v-if="logo" :src="logo" class="sidebar-logo" :title="title" />
        <h1 v-else class="sidebar-title" />
      </router-link>
      <router-link v-else key="expand" class="sidebar-logo-link" to="/">
        <img v-if="logo" :src="logo" class="sidebar-logo" :title="title" />
        <h1 class="sidebar-title" />
      </router-link>
    </transition>
  </div>
</template>

<script>
import logo from "@/assets/images/logo-b.png";

export default {
  name: "SidebarLogo",
  props: {
    collapse: {
      type: Boolean,
      required: true
    }
  },
  data() {
    return {
      title: "揽 云",
      logo
    };
  }
};
</script>

<style lang="scss" scoped>
.sidebarLogoFade-enter-active {
  transition: opacity 1.5s;
}

.sidebarLogoFade-enter,
.sidebarLogoFade-leave-to {
  opacity: 0;
}

.sidebar-logo-container {
  position: relative;
  width: 100%;
  height: 50px;
  line-height: 50px;
  text-align: center;
  overflow: hidden;
  margin: 25px 0;

  .sidebar-logo-link {
    height: 100%;
    width: 100%;

    .sidebar-logo {
      width: 44px;
      height: 32px;
      vertical-align: middle;
      margin-right: 15px;
    }

    .sidebar-title {
      display: inline-block;
      margin: 0;
      height: 32px;
      width: 76px;
      vertical-align: middle;
      background: url("~@/assets/images/logo-w.png") no-repeat center center;
      background-size: contain;
    }
  }

  &.collapse {
    .sidebar-logo {
      margin-right: 0px;
      width: 32px;
      height: 23px;
    }
    .sidebar-title {
      background: none;
    }
  }
}
</style>
