<template>
  <div
    v-if="open"
    v-drag="{ title: 'title' }"
    class="win"
    :style="winStyle"
    @click.stop
  >
    <template v-if="!isModal">
      <h2 class="title">
        <div class="flex1">
          <slot name="title">{{ title }}</slot>
        </div>
        <div class="close">
          <slot name="close">
            <el-icon name="close" size="26px" @click="close" />
          </slot>
        </div>
      </h2>
    </template>
    <template v-if="isModal && headerPath">
      <AsyncComponent :loading="false" :keep-alive="true" :path="headerPath" />
    </template>

    <div class="inner-body" :style="innerStyle" @click.stop v-loading="loading">
      <slot>
        这里编辑你的内容
      </slot>
    </div>

    <slot name="button" />
  </div>
</template>
<script>
export default {
  name: "ModalWindow",
  model: {
    prop: "open",
    event: "changeOpen",
  },
  props: {
    open: Boolean,
    ws: {
      type: Object,
      default: function () {
        return {};
      },
    }, // 传递的窗口对象
    title: {
      type: String,
      default: "窗口标题",
    },
  },
  data() {
    return {};
  },
  computed: {
    winStyle() {
      const style = {
        "z-index": this.ws.zIndex,
      };
      return style;
    },
    innerStyle() {
      return this.ws.innerStyle;
    },
    isModal() {
      return this.ws.modal || false;
    },
    headerPath() {
      return this.ws.header || "";
    },
    loading() {
      return this.ws.loading || false;
    },
  },
  created() {
    console.log("ModalWindow");
  },
  methods: {
    close() {
      this.$emit("changeOpen", false);
      this.$emit("update:open", false); // 兼容:open.sync语法调用
      this.$close(this.ws.name); // 触发容器关闭事件
    },
  },
};
</script>
