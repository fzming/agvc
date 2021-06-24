<template>
  <div id="app">
    <div :class="appClass" :disabled="sleep">
      <router-view />
    </div>
  </div>
</template>

<script>
export default {
  name: "App",
  data() {
    return {
      sleep: false,
    };
  },
  computed: {
    appClass() {
      return {
        routeview: true,
        sleep: this.sleep,
      };
    }
  }
};
</script>
<style lang="scss" scoped>
.routeview {
  width: 100%;
  height: 100%;
}
.sleep {
  pointer-events: none;
  display: none;
}
</style>
