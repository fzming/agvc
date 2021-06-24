import router, { resetRouter } from "./router";
import store from "./store";
import { MessageBox } from "element-ui";
//import { getToken, getUserType, removeUserType } from "@/utils/auth"; // get token from cookie
import defaultSettings from "@/settings";
import {
  NET_WORK_ERROR_URL
} from "@/global/const"
function _isMobile() {
  const regex = /(phone|pad|pod|iPhone|iPod|ios|iPad|Android|Mobile|BlackBerry|IEMobile|MQQBrowser|JUC|Fennec|wOSBrowser|BrowserNG|WebOS|Symbian|Windows Phone)/i;
  return regex.test(navigator.userAgent);
}
const whiteList = [...NET_WORK_ERROR_URL, "/404", "/401", "/redirect", ]; // no redirect whitelist
router.beforeEach(async (to, from, next) => {
  // start progress bar
  // NProgress.start();
  // console.log("router.beforeEach=>", to);
  // set page title
  document.title = defaultSettings.title;
  const mobile = _isMobile();
  if (mobile && !to.path.startsWith("/m")) {
     //转到手机版
    next('/m');
    return;
  }

  // determine whether the user has logged in
  // console.log("determine whether the user has logged in=>", store.state.user.token.token);
  const hasToken = store.state.user.token.token;
  const userType = store.state.user.token.userType;
  const loginUrl = mobile ? NET_WORK_ERROR_URL[2] :
    (userType === "Admin"
      ? `${NET_WORK_ERROR_URL[0]}`
      : `${NET_WORK_ERROR_URL[1]}`);

  // =======================尚未登录===============================
  if (!hasToken) {
    if (whiteList.indexOf(to.path) !== -1) {
      if (userType) {
        // removeUserType();
        next();
      } else {
        next();
      }
    } else {
      console.log("未登录，跳转到：", loginUrl);
      next(`${loginUrl}?redirect=${to.path}`);
      //NProgress.done();
    }
    return;
  }
  // ======================重复登录=======================================
  const repeatLogin =
    hasToken &&
    NET_WORK_ERROR_URL.includes(to.path) &&
    !store.getters.needChangePassword;
  //console.log("store.getters.roles", store.getters.roles);
  const hasRoles = store.getters.roles && store.getters.roles.length > 0;
  if (repeatLogin) {
    next({
      path: "/dashboard",
    });
    return;
  }
  // ================登录后-加载角色权限===========================
  if (hasToken && hasRoles) {
    console.log("登录后-加载角色权限", hasToken, hasRoles, store.getters.roles);
    // 是否有菜单权限判断
    const flat_routes = store.getters.flat_routes;
    var isRoutePermitted = flat_routes.includes(to.path);
    if (
      to.path.startsWith("/redirect/") ||
      to.path.startsWith("/login") ||
      to.path.startsWith("/syslogin") ||
      to.path.startsWith("/m") //移动端无需验证权限
    ) {
      isRoutePermitted = true;
    }
    // console.log(flat_routes, to.path);
    if (isRoutePermitted) {
      if (store.state.tagsView.modifyTag) {
        MessageBox.confirm("此时离开将不保存之前的修改,确定要离开吗？", "提示")
          .then(async () => {
            //确定，跳转页面并清除修改状态
            await store.dispatch("tagsView/setModifyTag", false);
            next();
          })
          .catch(() => {
            //取消，不跳转页面
          });
      } else {
        next(); //ok
      }
    } else {
      MessageBox.alert(`当前角色权限不足操作此功能：\n${to.path}`, "权限不足");
      return;
    }
  } else {
    // console.log("==============================");
    try {
      const profile = await store.dispatch(
        userType === "Account" ? "user/getInfoOrg" : "user/getInfo"
      );
      // console.log("profile", profile);
      if (profile.needChangePassword) {
        console.log("[检测到需要重设密码] needChangePassword 设置为 true");
        next(`${loginUrl}`);
        //NProgress.done();
        return;
      }
      const accessRoutes = await store.dispatch(
        "permission/generateRoutes",
        profile.roles
      );
      resetRouter();
      // 路由动态装配
      router.addRoutes(accessRoutes);
      // 生成指令
      await store.dispatch("permission/generateCodes");
      next({
        ...to,
        replace: true,
      });
    } catch (error) {
      await store.dispatch("user/resetToken");
      MessageBox.alert("登录状态已失效，请重新登录", "状态失效");
      next(`/${loginUrl}?redirect=${to.path}`);
    
    }
  }
});