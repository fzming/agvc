import confirm from "./confirm";
import { Notification } from "element-ui";
var path = require("path");
const W_EVENT = "W_EVENT";
const $resolve_view = (path, folder) => {
  const view_path = folder === "" ? "views" : `views/${folder}`;

  return `${view_path}/${path}`;
};
const $open = (wArg, oArg) => {
  if (!wArg) return;

  window.eventBus.$emit(
    W_EVENT,
    Object.assign(
      {
        visible: true,
        type: "open",
      },
      wArg,
      oArg || {}
    )
  );
};
const Prototype = function () {};
Prototype.install = function (Vue, options) {
  !!options;
  // 全局挂载 $http
  Vue.prototype.$confirmAsync = confirm;
  // ---
  Vue.prototype.$resolve = $resolve_view;
  // // ---
  // Vue.prototype.$alert = (title, desc, onClose) => {
  //   // console.error(title, desc)
  //   Notification({
  //     type: "error",
  //     title: title,
  //     message: desc,
  //     onClose
  //   });
  // };

  Vue.prototype.$info = (title, desc) => {
    // console.error(title, desc)
    Notification({
      title: title,
      message: desc,
      type: "info",
    });
  };
  Vue.prototype.$succ = (content) => {
    // console.error(content)
    Notification({
      title: "成功",
      message: content,
      type: "success",
    });
  };
  /*
    支持3种调用：
    this.$confirm("标题",(ok)=>{});
     this.$confirm(["标题","内容"],(ok)=>{});

    this.$confirm({
       text:"abc",
       icon:"",
       buttons:["确认","取消"]
    },ok=>{})
   */
  Vue.prototype.$confirm = (arg, callback) => {
    const wArg = {
      name: "confirmModal",
      modal: true,
      overlayCloseable: false,
      path: $resolve_view("confirm.vue", "common"),
      callback,
    };
    var opt = {
      text: "",
      des: "",
      icon: "warning",
      buttons: ["确认", "取消"],
    };
    if (typeof arg === "string") {
      arg = {
        text: arg,
      };
    } else if (Array.isArray(arg)) {
      arg = {
        text: arg[0],
        des: arg[1] || "",
        icon: arg[2] || "warning",
      };
    }
    $open(wArg, Object.assign({}, opt, arg));
  };
  Vue.prototype.$broadcast = (event, eventArg) => {
    window.eventBus.$emit(event, eventArg);
  };

  // 加载窗口
  Vue.prototype.$open = $open;
  Vue.prototype.$openView = (dir, file, title, arg) => {
    let view_path = path.resolve(
      /\\(views.*)/i.exec(dir)[1].replace(/\\/gi, "/"),
      file
    );
    const name = /.*\/(\w+)/i.exec(view_path)[1];
    const wArg = {
      name,
      title: title || "",
      path: view_path,
    };
    console.log(wArg);
    $open(wArg, arg);
  };
  Vue.prototype.$set_loading = function (loading) {
    const ws = this["ws"]; // 注意：使用ES2015箭头函数将无法读取this作用域
    if (!ws) {
      this.$alert(
        `${this.name}：props缺少ws对象`,
        "Vue.prototype.$set_loading"
      );
      return;
    }
    ws.loading = loading;
  };
  Vue.prototype.$set_title = function (title) {
    const ws = this["ws"]; // 注意：使用ES2015箭头函数将无法读取this作用域

    if (!ws) {
      this.$alert(`${this.name}：props缺少ws对象`, "Vue.prototype.$set_title");
      return;
    }
    ws.title = title;
  };
  Vue.prototype.__defineSetter__("$loading", function (loading) {
    const ws = this["ws"]; // 注意：使用ES2015箭头函数将无法读取this作用域
    if (!ws) {
      this.$alert(`${this.name}：props缺少ws对象`, "Vue.prototype.$loading");
      return;
    }
    ws.loading = loading;
  });
  Vue.prototype.__defineSetter__("$title", function (title) {
    const ws = this["ws"]; // 注意：使用ES2015箭头函数将无法读取this作用域
    if (!ws) {
      this.$alert(`${this.name}：props缺少ws对象`, "Vue.prototype.$title");
      return;
    }
    ws.title = title;
  });
  // 关闭窗口事件
  Vue.prototype.$close = (name) => {
    window.eventBus.$emit(W_EVENT, {
      visible: false,
      type: "close",
      name: name,
    });
  };
  // 关闭所有窗口
  Vue.prototype.$closeAll = () => {
    window.eventBus.$emit(W_EVENT, {
      type: "closeAll",
    });
  };
};
export default Prototype;
