import axios from "axios";
import { WEBAPI_BASE_URL, WEBAPI_REQUEST_TIMEOUT } from "@/global/const";
import { Notification, MessageBox } from "element-ui";

import store from "@/store";

const service = axios.create({
  baseURL: WEBAPI_BASE_URL,
  headers: {
    "Content-Type": "application/json;charset=utf-8",
  }, // 设置传输内容的类型和编码
  withCredentials: false, // 指定某个请求应该发送凭据。允许客户端携带跨域cookie，也需要此配置
  timeout: WEBAPI_REQUEST_TIMEOUT,
});

// request interceptor
service.interceptors.request.use(
  (config) => {
    //app.$Progress.start(); // for every request start the progress

    config.headers["Content-Type"] = "application/x-www-form-urlencoded";

    var s = store.state.user.token;
    // 判断是否存在token，如果存在的话，则每个http header都加上token
    if (s && s.token) {
      config.headers.Authorization = "Bearer " + s.token.access_token;
    }

    return config;
  },
  (error) => {
    //app.$Progress.fail(); //结束进度条
    // do something with request error
    //console.log(error); // for debug
    return Promise.reject(error);
  }
);

// response interceptor
service.interceptors.response.use(
  (response) => {
    //console.log(response);
    try {
      // console.log("response=>", response);
      const disposition = response.headers["content-disposition"];
      if (disposition && disposition.indexOf("attachment") !== -1) {
        const filenameRegex = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/;
        const matches = filenameRegex.exec(disposition);
        if (matches != null && matches[1]) {
          const filename = decodeURIComponent(matches[1].replace(/['"]/g, ""));
          // 返回文件流内容，以及获取文件名, response.headers[‘content-disposition’]的获取, 默认是获取不到的,需要对服务端webapi进行配置
          return Promise.resolve({
            data: response.data,
            fileName: decodeURIComponent(filename),
          });
        }
      }
    } catch (e) {
      console.warn("错误内容", e);
    } finally {
      //app.$Progress.finish(); // 结束进度条
    }
    return response;
  },
  async (error) => {
    // console.log("error=>", error.response); // for debug
    // console.error("response错误", error);
    let err = error.message;
    console.log(err);
    let err_title = "操作失败";

    if (error.response) {
      err_title = `操作失败`;
      err = error.response.statusText;
      const data = error.response.data; // 有数据响应，接口调用成功

      if (data) {
        if (typeof data == "object" && data["type"] == "application/json") {
          //blob下载失败
          let reader = new FileReader();
          reader.readAsText(data, "utf-8");
          reader.onload = (e) => {
            var rs = JSON.parse(e.target.result);
            MessageBox.alert(rs.error, "文件下载失败");
          };
          return;
        }

        if (data.error) {
          err = `${data.error}`;
        }
      } else {
        err = error.response.error || "服务端错误";
      }
    }
    console.log(error);
    if (error.response.status == 401) {
      console.log(err);
      err = "您的登录状态已失效，请重新登录";
      err_title = "认证失败";
    }
    if (err === "NeedSafetyValidation") {
      // 需要人机安全验证
      return Promise.reject(new Error("NeedSafetyValidation"));
    } else {
      Notification({
        message: err,
        type: "error",
        title: err_title,
        duration: 5 * 1000,
        position: "bottom-right",
        customClass: "NotificationZindex",
      });
      return Promise.reject(new Error(err));
    }
  }
);

export default service;
