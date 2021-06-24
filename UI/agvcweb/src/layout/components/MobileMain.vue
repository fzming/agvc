<template>
  <section class="mobile-main">
    <router-view />
    <van-tabbar v-model="active" v-if="false">
      <van-tabbar-item name="home" icon="home-o">标签</van-tabbar-item>
      <van-tabbar-item name="search" icon="search">标签</van-tabbar-item>
      <van-tabbar-item name="friends" icon="friends-o">标签</van-tabbar-item>
      <van-tabbar-item name="setting" icon="setting-o">标签</van-tabbar-item>
    </van-tabbar>
  </section>
</template>
<script>
export default {
  name: "MobileMain",
  data() {
    return {
      active: "home",
    };
  },
};
</script>
