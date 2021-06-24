<template>
  <keep-alive v-if="keepAlive">
    <component :is="AsyncComp" v-bind="$attrs" v-on="$listeners" />
  </keep-alive>
  <component :is="AsyncComp" v-else v-bind="$attrs" v-on="$listeners" />
</template>

<script>
/**
 * 动态文件加载器
 */
export default {
  name: "AsyncComponent",
  inheritAttrs: false, // 组件将不会把未被注册的props呈现为普通的HTML属性
  // 外部传入属性
  props: {
    // 文件的路径
    path: {
      type: String,
      default: null,
    },
    // 是否保持缓存
    keepAlive: {
      type: Boolean,
      required: false,
      default: false,
    },
    // 延迟加载时间
    delay: {
      type: Number,
      default: 10,
    },
    // 超时警告时间
    timeout: {
      type: Number,
      default: 2000,
    },
    // 是否显示loading
    loading: {
      type: Boolean,
      default: false,
    },
  },
  data() {
    return {
      AsyncComp: null,
    };
  },
  watch: {
    path() {
      this.AsyncComp = this.loadAsyncComp();
    },
  },
  created() {
    this.AsyncComp = this.loadAsyncComp();
  },
  methods: {
    loadAsyncComp() {
      if (!this.path) return null;
      let path = this.path.startsWith("/") ? this.path.substr(1) : this.path;
      console.log(path);
      return () => ({
        // https://github.com/webpack/webpack/issues/8557 编译太慢的问题
        // 需要加载的组件 (应该是一个 `Promise` 对象)
        // 这个 `import` 函数会返回一个 `Promise` 对象。
        // 注意import()函数的参数不能是表达式，所以必须使用字符串包裹变量进行修正
        // 注意：不支持babel-plugin-dynamic-import-node
        component: import("@/" + path), // @babel/plugin-syntax-dynamic-import

        //  异步组件加载时使用的组件
        loading: {
          template: this.loading
            ? `<div style="position:relative; padding:40px; width:100%;"></div>`
            : "<div></div>",
        },
        // 加载失败时使用的组件
        error: {
          template: ``,
        },
        // 展示加载时组件的延时时间。默认值是 200 (毫秒)
        // 这个延时并不能让loading显示更长时间
        delay: this.delay,
        // 如果提供了超时时间且组件加载也超时了，
        // 则使用加载失败时使用的组件。默认值是：`Infinity`
        timeout: this.timeout,
      });
    },
  },
};
</script>
