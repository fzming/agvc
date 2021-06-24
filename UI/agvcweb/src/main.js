import Vue from "vue";
import Element from "element-ui";
import "./styles/element-variables.scss"; //element variables
import "./styles/index.scss"; // global css
//ie support
import "core-js/stable";
import "regenerator-runtime/runtime";

import App from "./App";
import store from "./store";
import router from "./router";
import "./icons"; // svg icon
import "./permission"; // permission control

// VUE事件总线
window.eventBus = new Vue();

//element ui config
Vue.use(Element, {
  size: "medium", // set element-ui default size
});
// 引入模块组件
import moduleComponents from "./components";
Vue.use(moduleComponents);

// register global utility filters
import * as filters from "./filters"; // global filters
Object.keys(filters).forEach((key) => {
  Vue.filter(key, filters[key]);
});
// register global utility directives
import directive from "@/directive/index";
Vue.use(directive);
import prototype from "@/prototype/index";
Vue.use(prototype);

// =====================================
Vue.config.productionTip = false;

new Vue({
  el: "#app",
  router,
  store,
  render: (h) => h(App),
});
