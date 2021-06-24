<template>
  <div class="region" :style="regionStyle">
    <div class="right">
      <slot name="right"></slot>
    </div>
    <slot />
  </div>
</template>
<script>
export default {
  name: "Region",
  props: {
    marginTop: {
      type: Number,
      default: 20,
    },
    marginLeft: {
      type: Number,
      default: 0,
    },
  },
  computed: {
    regionStyle() {
      return {
        "margin-top": this.marginTop + "px",
        "margin-left": this.marginLeft + "px",
      };
    },
  },
};
</script>
<style lang="scss">
.region {
  background: #fff;
  border: 1px solid $gray-border;
  padding: 14px 20px;
  width: 100%;
  max-height: 100%;
  box-sizing: border-box;
  position: relative;
  .right {
    position: absolute;
    right: 25px;
    z-index: 1;
    top: 15px;
  }
  &:hover {
    box-shadow: 0 2px 12px 0 rgba(0, 0, 0, 0.2);
  }
  .el-table {
    th.is-leaf,
    th.is-center {
      /* font-weight: bold; */
      color: black;
    }
  }
}
</style>
