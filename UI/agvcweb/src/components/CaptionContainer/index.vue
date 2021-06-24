<template>
  <div class="app-main-caption flex">
    <div class="flex1">{{ caption }}</div>
    <div><slot /></div>
  </div>
</template>
<script>
export default {
  name: 'CaptionContainer',
  props: {
    caption: {
      type: String,
      default: ''
    }
  },
  data() {
    return {};
  }
};
</script>
<style lang="scss">
  .app-main-caption {
    position: relative;
    padding-left: 20px;

    &::before {
      content: " ";
      position: absolute;
      width: 7px;
      height: 20px;
      background: $light-blue;
      border-radius: 5px;
      left: 0;
      transform: translateY(-50%);
      top: 50%;
      box-shadow: 2px 2px 4px 2px rgba($light-blue, 0.2);
    }
    button.el-button--primary {
      box-shadow: 0px 5px 5px 5px rgba($light-blue, 0.1);
      font-weight: bold;
    }
  }
</style>
