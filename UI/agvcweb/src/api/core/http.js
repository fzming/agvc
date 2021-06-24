import Qs from "qs";
import request from "./request";
import FileSaver from "file-saver";
// ===============================================================
const isObject = (v) => {
  return Object.prototype.toString.call(v) === "[object Object]";
};
const isFormData = (v) => {
  return (
    Object.prototype.toString.call(v) === "[object FormData]" ||
    v instanceof FormData
  );
};

// JSON对象转成formData对象
const jsonToFormData = (params) => {
  if (isObject(params)) {
    const formData = new FormData();
    Object.keys(params).forEach((key) => {
      var value = params[key];
      console.log("stringify");
      if (isObject(value)) value = JSON.stringify(encodeFormObject(value));

      if (Array.isArray(value) && value.length > 0) {
        var n = value[0];
        var isFile = n instanceof File;
        if (isFile) {
          value.forEach((file, index) => {
            formData.append("file" + index, file);
          });
          return;
        }
        value = JSON.stringify(value);
      }
      formData.append(key, value);
    });
    return formData;
  }
  return params;
};
const encodeFormObject = (param) => {
  var data = {};
  for (var p in param) {
    if (!Object.prototype.hasOwnProperty.call(param, p)) continue;
    var v = param[p];
    if (typeof v === "object" || Array.isArray(v)) {
      data[p] = JSON.stringify(v);
    } else {
      data[p] = v;
    }
  }
  return data;
};
// formData对象转成JSON对象
// eslint-disable-next-line no-unused-vars
const formDataToJson = (formData) => {
  if (isFormData(formData)) {
    const jsonData = {};
    formData.forEach((value, key) => (jsonData[key] = value));
    return jsonData;
  }
  return formData;
};
const apiData = (resp) => {
  if (!resp) return;
  const ret = resp.data;
  if (resp.fileName) {
    console.log("==============ApiData", resp);
    var blob = new Blob([resp.data], {
      type: "application/vnd.ms-excel;charset=utf-8",
    });
    FileSaver.saveAs(blob, resp.fileName || "download.xlsx");
  }
  if (resp.status === 200) {
    if (ret.access_token) {
      // 兼容access_token
      return ret;
    } else {
      return ret.data || "";
    }
  }
};
export default {
  /** get 请求
   * @param  {接口地址} url
   * @param  {请求参数} params
   */
  async get(url, params) {
    const resp = await request.get(url, {
      params: params,
    });
    return apiData(resp);
  },
  /** post 请求
   * @param  {接口地址} url
   * @param  {请求参数} params
   */
  async post(url, params) {
    if (isFormData(params)) {
      // 兼容上传文件
      return await this.upload(url, params);
    }
    const resp = await request.post(url, Qs.stringify(params));
    return apiData(resp);
  },
  /*
    支持文件上传post请求
  */
  async upload(url, params, blob = false) {
    const formData = jsonToFormData(params); // 转成formData对象
    let p = {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    };
    if (blob)
      p = Object.assign(p, {
        responseType: "blob",
      });
    const resp = await request.post(url, formData, p);
    return apiData(resp);
  },

  /** put 请求
   * @param  {接口地址} url
   * @param  {请求参数} params
   */
  async put(url, params) {
    const resp = await request.put(url, Qs.stringify(params));
    return apiData(resp);
  },
  /** delete 请求
   * @param  {接口地址} url
   * @param  {请求参数} params
   */
  async delete(url, params) {
    const resp = await request.delete(url, Qs.stringify(params));
    return apiData(resp);
  },
  async download(url, params, type = "get") {
    const res =
      type == "get"
        ? await request.get(url, {
            params: Qs.stringify(params),
            responseType: "blob",
          })
        : await request.post(url, Qs.stringify(params), {
            responseType: "blob",
          });
    if (!res || !res.data) return;
    var blob = new Blob([res.data], {
      type: "application/vnd.ms-excel;charset=utf-8",
      //type: "application/octet-stream;charset=utf-8"
    });
    // debugger;

    FileSaver.saveAs(blob, res.fileName || params.fileName || "download.xlsx");
    /*
    // 针对于IE浏览器的处理, 因部分IE浏览器不支持createObjectURL
    if (window.navigator && window.navigator.msSaveOrOpenBlob) {
      window.navigator.msSaveOrOpenBlob(blob, res.fileName)
    } else {
      var downloadElement = document.createElement('a')
      var href = window.URL.createObjectURL(blob) // 创建下载的链接
      downloadElement.href = href
      downloadElement.download = res.fileName // 下载后文件名
      document.body.appendChild(downloadElement)
      downloadElement.click() // 点击下载
      document.body.removeChild(downloadElement) // 下载完成移除元素
      window.URL.revokeObjectURL(href) // 释放掉blob对象
    }*/
  },
};
