import Vue from "vue";
import Vuex from "vuex";
import getters from "./getters";
import createPersistedState from "vuex-persistedstate";
Vue.use(Vuex);
const modulesFiles = require.context("./modules", true, /\.js$/);
const modules = modulesFiles.keys().reduce((modules, modulePath) => {
  let moduleName = modulePath.replace(/^\.\/(.*)\.\w+$/, "$1");
  moduleName = moduleName.substr(moduleName.lastIndexOf("/") + 1);
  //  console.log(moduleName);
  const value = modulesFiles(modulePath);
  modules[moduleName] = value.default;
  return modules;
}, {});

const store = new Vuex.Store({
  modules,
  getters,
  plugins: [
    createPersistedState({
      key: "vuex-wms",
      paths: [
        "app",
        "Account",
        "user.token",
        "user.userType",
        "user.userModules",
        "user.currentModules",
        "client.client",
      ],
      storage: window.sessionStorage,
    }),
  ],
});

export default store;
