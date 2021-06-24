<template>
  <div v-show="visibleCount > 0">
    <transition-group :class="containerClass" tag="div" name="fadeScale">
      <ModalWindow
        v-for="w in windows"
        :key="w.name"
        v-model="w.visible"
        :ws="w"
        :disabled="w.zIndex != topZindex"
        :title="w.title"
      >
        <AsyncComponent :path="w.path" :ws="w" :delay="0" />
      </ModalWindow>
    </transition-group>
  </div>
</template>
<style lang="scss">
@import "./animate.scss";
@import "./WindowContainer.scss";
</style>
<script>
import _ from "lodash";
const W_EVENT = "W_EVENT";
export default {
  name: "WindowContainer",
  props: {
    modal: {
      type: Boolean,
      default: false,
    },
    overlayCloseable: {
      type: Boolean,
      default: false,
    },
  },
  data() {
    return {
      containerZIndex: 100,
      windows: [], // 存储所有窗体
    };
  },
  computed: {
    containerClass() {
      return {
        "window-container": true,
        modal: this.modal,
      };
    },
    visibleCount() {
      return this.windows.filter((p) => p.visible).length;
    },
    topZindex() {
      if (this.windows.length === 0) return this.containerZIndex;
      return _.maxBy(this.windows, "zIndex").zIndex;
    },

    topestWindow() {
      return this.windows.find((p) => p.zIndex === this.topZindex);
    },
  },
  created() {
    // console.log(111111);
    this.initEventBus();
  },
  methods: {
    // 打开窗口
    openWindow(arg) {
      // console.log(arg)
      const win = Object.assign(
        {
          zIndex: this.topZindex + 10,
          visible: true,
          overlayCloseable: this.overlayCloseable,
          modal: false,
          loading: false,
        },
        arg
      );
      const _index = this.windows.findIndex((p) => p.name === arg.name);

      if (_index === -1) {
        this.windows.push(win);
      } else {
        this.$set(this.windows, _index, win);
      }
      // console.log(this.windows)
    },
    closeAllWindow() {
      this.windows = [];
    },
    // 关闭窗口
    closeWindow(arg, forceClose) {
      let winname = !arg ? "" : arg.name;
      // console.log("closewindow->" + winname);

      if (!winname) {
        const tp = this.topestWindow;
        if (!tp) return;
        //   // 关闭当前最顶层窗口
        if (tp.modal && !forceClose && !tp.overlayCloseable) return; // 不能关闭Modal窗口，除非强制关闭
        winname = tp.name;
      }

      if (!winname) {
        // console.log("窗口不存在:" + winname);
        return;
      }
      var _index = this.windows.findIndex((p) => p.name === winname);
      if (_index > -1) this.windows.splice(_index, 1); // 删除窗口
    },
    initEventBus() {
      const that = this;
      window.eventBus.$on(W_EVENT, (arg) => {
        // console.log(`${arg.type}->${arg.name}`);
        if (arg.type === "open") {
          that.openWindow(arg);
        } else if (arg.type === "close") {
          that.closeWindow(arg);
        } else if (arg.type === "closeAll") {
          that.closeAllWindow();
        }
      });
    },
  },
};
</script>
