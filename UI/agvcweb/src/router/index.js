import Vue from "vue";
import Router from "vue-router";
Vue.use(Router);
/* Layout */
import Layout from "@/layout";

/**
 * constantRoutes
 * a base page that does not have permission requirements
 * all roles can be accessed
 */
export const constantRoutes = [
  {
    path: "/redirect",
    component: Layout,
    hidden: true,
    children: [
      {
        path: "/redirect/:path*",
        component: () => import("@/views/redirect/index"),
      },
    ],
  },
  {
    path: "/404",
    component: () => import("@/views/error-page/404"),
    hidden: true,
  },
  {
    path: "/401",
    component: () => import("@/views/error-page/401"),
    hidden: true,
  },
  // ==================================================
  {
    path: "/syslogin",
    component: () => import("@/views/login/index"),
    hidden: true,
  },
  {
    path: "/login",
    component: () => import("@/views/login/AccountLogin"),
    hidden: true,
  },
  {
    path: "",
    component: Layout,
    redirect: "/dashboard",
    children: [
      {
        path: "/dashboard",
        component: () => import("@/views/dashboard/index"),
        name: "Dashboard",
        meta: {
          title: "首页",
          icon: "dashboard",
          affix: true,
        },
      },
    ],
  },
  {
    path: "/",
    component: Layout,
    hidden: true,
    children: [
      {
        path: "/profile/index",
        component: () => import("@/views/profile/index"),
        name: "profile",
        meta: {
          title: "个人中心",
          icon: "profile",
        },
      },
      {
        path: "/profile/changePassword",
        component: () => import("@/views/profile/changePassword"),
        name: "changePassword",
        meta: {
          title: "密码修改",
          icon: "profile",
        },
      },
      {
        path: "/profile/bindingMoblie",
        component: () => import("@/views/profile/bindingMoblie"),
        name: "bindingMoblie",
        meta: {
          title: "绑定手机",
          icon: "profile",
        },
      },
    ],
  }
];

const createRouter = () =>
  new Router({
    mode: "history", // require service support
    scrollBehavior: () => ({
      y: 0,
    }),
    routes: constantRoutes,
  });

const router = createRouter();

// Detail see: https://github.com/vuejs/vue-router/issues/1234#issuecomment-357941465
export function resetRouter() {
  const newRouter = createRouter();
  router.matcher = newRouter.matcher; // reset router
}

export default router;
