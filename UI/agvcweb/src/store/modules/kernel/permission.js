import { constantRoutes } from "@/router";
import {
  //asyncRoutes,
  asyncCodes,
  newasyncRoutes,
} from "@/api/role";
import Layout from "@/layout";
const allFlatConstantRoutes = [];
const allFlatRoutes = [];
/**
 * Use meta.role to determine if the current user has permission
 * @param roles
 * @param route
 */
export function hasPermission(roles, route) {
  if (route.meta && route.meta.roles) {
    const a = roles.some((role) => route.meta.roles.includes(role.id));
    return a;
  } else {
    return true;
  }
}
function filterConstantRoutes(routes, parent_path) {
  routes.forEach((route) => {
    const tmp = {
      ...route,
    };
    parent_path = parent_path || "";
    let prefix = "/";
    if (parent_path.substr(0, 1) === "/" || tmp.path.substr(0, 1) === "/")
      prefix = "";
    const path = parent_path + prefix + tmp.path;
    allFlatConstantRoutes.push(path.replace("//", "/"));
    if (tmp.children) {
      tmp.children = filterConstantRoutes(tmp.children, path);
    }
  });
}
export function filterAsyncRoutes(routes, roles, parent_path) {
  const res = [];
  routes.forEach((route) => {
    const tmp = {
      ...route,
    };
    if (hasPermission(roles, tmp)) {
      const path = (parent_path ? parent_path + "/" : "") + tmp.path;
      allFlatRoutes.push(path);
      const component_path = tmp.component + "";
      // console.log(`./views${component_path}`)
      tmp.component =
        component_path === "layout"
          ? Layout
          : () => import(`@/views${component_path}`); // view(component_path) //lazyLoadView

      // console.log(tmp.component)
      if (tmp.children) {
        tmp.children = filterAsyncRoutes(tmp.children, roles, path);
      }
      res.push(tmp);
    }
  });

  return res;
}

const state = {
  routes: [],
  flat_routes: [],
  codes: [],
  addRoutes: [],
};

const mutations = {
  SET_ROUTES: (state, routes) => {
    state.addRoutes = routes;
    state.routes = constantRoutes.concat(routes);
  },
  SET_FLAT_ROUTES: (state, flat_routes) => {
    filterConstantRoutes(constantRoutes);
    state.flat_routes = Array.from(
      new Set(allFlatConstantRoutes.concat(flat_routes))
    );
  },
  SET_CODES: (state, codes) => {
    state.codes = codes;
  },
};

const actions = {
  async generateRoutes({ commit }, roles) {
    // var d = await asyncRoutes();
    var d = await newasyncRoutes();
    var filterRoutes = filterAsyncRoutes(d, roles);
    const accessedRoutes = filterRoutes;
    // console.log(accessedRoutes);
    // console.log("allFlatRoutes", allFlatRoutes);
    commit("SET_ROUTES", accessedRoutes);
    commit("SET_FLAT_ROUTES", allFlatRoutes);
    return accessedRoutes;
  },
  async generateCodes({ commit }) {
    var codes = await asyncCodes();
    commit("SET_CODES", codes);
    return codes;
  },
};

export default {
  namespaced: true,
  state,
  mutations,
  actions,
};
