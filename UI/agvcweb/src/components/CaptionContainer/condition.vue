<template>
  <section class="condition flex" :style="conditionStyle">
    <div class="flex1">
      <slot />
    </div>
    <div class="button-item">
      <slot name="search" />
    </div>
  </section>
</template>
<script>
export default {
  name: "Condition",
  props: {
    paddingBottom: {
      type: String,
      default: "10px",
    },
    paddingTop: {
      type: String,
      default: "10px",
    },
  },
  computed: {
    conditionStyle() {
      return {
        "padding-top": this.paddingTop,
        "padding-bottom": this.paddingBottom,
      };
    },
  },
};
</script>
<style lang="scss" scoped>
.condition {
  padding: 10px;
  background: #fff;
  border: 1px solid $gray-border;
  position: relative;
  transition: all 0.3s;
  &:hover {
    box-shadow: 0 2px 12px 0 rgba(0, 0, 0, 0.2);
  }
  .el-form-item__label {
    font-weight: normal;
    font-size: 14px;
  }
  &::before {
    content: " ";
    position: absolute;
    width: 7px;
    height: 40px;
    background: $light-blue;

    left: -3px;

    top: 22px;
    box-shadow: 2px 2px 4px 2px rgba($light-blue, 0.2);
  }
  button.el-button--primary {
    box-shadow: 0px 5px 5px 0px rgba($light-blue, 0.1);
    margin-right: 20px;
  }
  .button-item {
    /* margin-bottom: 22px; */
  }
  .button-itemstyle {
    margin-bottom: 22px;
  }
}
</style>
