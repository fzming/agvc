import { MODULE_NAME_TYPE } from "@/global/const";
// import _ from "lodash";
function initState() {
  return {
    showViewsList: [],
    viewList: [],
    currentPathList: [],
    modifyTag: false,
  };
}

const getters = {
  visitedViews: (state, getters, rootState) => {
    return state.showViewsList[rootState.user.currentModules];
  },
  cachedViews: (state, getters, rootState) => {
    return state.viewList[rootState.user.currentModules];
  },
  currentPath: (state, getters, rootState) => {
    return state.currentPathList[rootState.user.currentModules];
  },
};

const mutations = {
  ADD_VISITED_VIEW: (state, view) => {
    // console.log("ADD_VISITED_VIEW", view, MODULE_NAME_TYPE);
    if (state.showViewsList.length === 0) {
      MODULE_NAME_TYPE.forEach(() => {
        state.showViewsList.push([
          Object.assign({}, view.view, {
            title: view.view.meta.title || "no-name",
            tagsViewIndex: 0,
          }),
        ]);
      });
    } else {
      if (
        state.showViewsList[view.tag].some(
          (v) => v.fullPath === view.view.fullPath
        )
      )
        return;
      state.showViewsList[view.tag].push(
        Object.assign({}, view.view, {
          title: view.view.meta.title || "no-name",
          tagsViewIndex: view.view.query.tag ? view.view.query.tag * 1 : 0,
        })
      );
    }
  },
  ADD_CACHED_VIEW: (state, view) => {
    if (state.viewList.length === 0) {
      MODULE_NAME_TYPE.forEach(() => {
        state.viewList.push([]);
      });
    }
    if (state.viewList[view.tag].includes(view.view.fullPath)) {
      // console.log("页面已存在" + view.view.fullPath);
    } else {
      if (
        !view.view.meta.noCache &&
        view.view &&
        view.view.fullPath !== undefined
      ) {
        state.viewList[view.tag].push(view.view.fullPath);
      }
    }
  },

  DEL_VISITED_VIEW: (state, view) => {
    const i = state.showViewsList[view.tag].findIndex(
      (x) => x.fullPath === view.view.fullPath
    );
    if (i >= 0) {
      state.showViewsList[view.tag].splice(i, 1);
    }
  },
  DEL_CACHED_VIEW: (state, view) => {
    const i = state.viewList[view.tag].findIndex(
      (x) => x === view.view.fullPath
    );
    state.viewList[view.tag].splice(i, 1);
  },

  DEL_OTHERS_VISITED_VIEWS: (state, view) => {
    const a = state.showViewsList[view.tag].length;
    const b = state.showViewsList[view.tag].find(
      (x) => x.fullPath === view.view.fullPath
    );
    state.showViewsList[view.tag].splice(1, a);
    if (view.view.name !== "Dashboard") {
      state.showViewsList[view.tag].push(b);
    }
  },
  DEL_OTHERS_CACHED_VIEWS: (state, view) => {
    for (const i of state.viewList[view.tag]) {
      if (i === view.name) {
        const index = state.viewList[view.tag].indexOf(i);
        state.viewList[view.tag] = state.viewList[view.tag].splice(
          index,
          index + 1
        );
        break;
      }
    }
  },

  DEL_ALL_VISITED_VIEWS: (state, view) => {
    const tagValue = view.tag;
    const b = state.showViewsList[tagValue].length;
    state.showViewsList[tagValue].splice(1, b - 1);
  },
  DEL_ALL_CACHED_VIEWS: (state, view) => {
    const tagValue = view.tag;
    state.viewList[tagValue] = [];
  },
  SET_CALLBACK: (state, { tag, fullPath, callBack }) => {
    const tagValue = tag;
    var a = state.showViewsList[tagValue].find((x) => x.fullPath === fullPath);
    a.callBack = callBack;
  },
  UPDATE_VISITED_VIEW: (state, view) => {
    const tagValue = view.tag;
    for (let v of state.showViewsList[tagValue]) {
      if (v.fullPath === view.fullPath) {
        v = Object.assign(v, view);
        break;
      }
    }
  },
  SET_CURRENTPATH: (state, view) => {
    if (state.currentPathList.length === 0) {
      MODULE_NAME_TYPE.map(() => {
        state.currentPathList.push("");
      });
    }
    // console.log("SET_CURRENTPATH", view);
    state.currentPathList[view.tag] = view.view.fullPath;
  },

  SET_TAG_TITLE: (state, { tag, fullPath, options }) => {
    const tagValue = tag;
    // console.log(state.showViewsList, tag, options, fullPath);
    var a = state.showViewsList[tagValue].find((x) => x.fullPath === fullPath);
    a.title = options.title;
  },

  SET_MODIFYTAG: (state, data) => {
    state.modifyTag = data;
  },
};

const actions = {
  addView({ dispatch }, view) {
    // console.log("addView", view);
    dispatch("addVisitedView", view);
    dispatch("addCachedView", view);
  },
  addVisitedView({ rootState, commit }, view) {
    !!rootState;
    const tagValue = rootState.user.currentModules;
    // console.log("addVisitedView", view, tagValue);
    commit("ADD_VISITED_VIEW", {
      tag: tagValue,
      view: view,
    });
  },
  addCachedView({ rootState, commit }, view) {
    !!rootState;
    //const m = window.localStorage.getItem("CurrentModule");
    var tagValue = rootState.user.currentModules;
    // console.log("===", tagValue);
    // let tagValue = 0;
    //if (m) {
    //const a = JSON.parse(m);
    //tagValue = (a.v || 0) * 1;
    //}
    commit("ADD_CACHED_VIEW", {
      tag: tagValue,
      view: view,
    });
    commit("SET_CURRENTPATH", {
      tag: tagValue,
      view: view,
    });
  },

  async delView({ dispatch }, view) {
    dispatch("delVisitedView", view);
    dispatch("delCachedView", view);
  },

  delVisitedView({ commit, rootState }, view) {
    const tagValue = rootState.user.currentModules;
    commit("DEL_VISITED_VIEW", {
      tag: tagValue,
      view: view,
    });
  },
  delCachedView({ commit, state, rootState }, view) {
    !!state;
    const tagValue = rootState.user.currentModules;
    commit("DEL_CACHED_VIEW", {
      tag: tagValue,
      view: view,
    });
  },

  delOthersViews({ dispatch, getters }, view) {
    !!getters;
    dispatch("delOthersVisitedViews", view);
    dispatch("delOthersCachedViews", view);
  },

  delOthersVisitedViews({ commit, rootState }, view) {
    const tagValue = rootState.user.currentModules;
    commit("DEL_OTHERS_VISITED_VIEWS", {
      tag: tagValue,
      view: view,
    });
  },
  delOthersCachedViews({ commit, rootState }, view) {
    const tagValue = rootState.user.currentModules;
    commit("DEL_OTHERS_CACHED_VIEWS", {
      tag: tagValue,
      view: view,
    });
  },

  async delAllViews({ dispatch }, view) {
    dispatch("delAllVisitedViews", view);
    dispatch("delAllCachedViews", view);
  },
  delAllVisitedViews({ commit, rootState }) {
    const tagValue = rootState.user.currentModules;
    commit("DEL_ALL_VISITED_VIEWS", {
      tag: tagValue,
    });
  },
  delAllCachedViews({ commit, rootState }) {
    const tagValue = rootState.user.currentModules;
    commit("DEL_ALL_CACHED_VIEWS", {
      tag: tagValue,
    });
  },
  updateVisitedView({ commit, rootState }, view) {
    const tagValue = rootState.user.currentModules;
    commit("UPDATE_VISITED_VIEW", {
      view: view,
      tag: tagValue,
    });
  },
  setTagTitle({ commit, rootState }, { fullPath, options }) {
    const tagValue = rootState.user.currentModules;
    commit("SET_TAG_TITLE", {
      tag: tagValue,
      fullPath: fullPath,
      options: options,
    });
  },
  setModifyTag({ commit }, data) {
    commit("SET_MODIFYTAG", data);
  },
};

export default {
  namespaced: true,
  state: initState,
  getters,
  mutations,
  actions,
};
