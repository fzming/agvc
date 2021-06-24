/**
 * Created by PanJiaChen on 16/11/18.
 */
import { PAY_UNITS } from "@/global/const";
export function replacePhone(str) {
  if (!str) {
    return "";
  }
  return str.replace(/(\d{3})\d{5}(\d{3})/, "$1****$2");
}

/**
 * Parse the time to string
 * @param {(Object|string|number)} time
 * @param {string} cFormat
 * @returns {string}
 */
export function parseTime(time, cFormat) {
  if (arguments.length === 0) {
    return null;
  }
  if (!time || time === "") {
    return "";
  }
  const format = cFormat || "{y}-{m}-{d} {h}:{i}:{s}";
  let date;
  if (typeof time === "object") {
    date = time;
  } else {
    if (typeof time === "string" && /^[0-9]+$/.test(time)) {
      time = parseInt(time);
    }
    if (typeof time === "number" && time.toString().length === 10) {
      time = time * 1000;
    }
    date = new Date(time);
  }
  const formatObj = {
    y: date.getFullYear(),
    m: date.getMonth() + 1,
    d: date.getDate(),
    h: date.getHours(),
    i: date.getMinutes(),
    s: date.getSeconds(),
    a: date.getDay(),
  };
  const time_str = format.replace(/{(y|m|d|h|i|s|a)+}/g, (result, key) => {
    let value = formatObj[key];
    // Note: getDay() returns 0 on Sunday
    if (key === "a") {
      return ["日", "一", "二", "三", "四", "五", "六"][value];
    }
    if (result.length > 0 && value < 10) {
      value = "0" + value;
    }
    return value || 0;
  });
  return time_str;
}

/**
 * @param {number} time
 * @param {string} option
 * @returns {string}
 */
export function formatTime(time, option) {
  if (("" + time).length === 10) {
    time = parseInt(time) * 1000;
  } else {
    time = +time;
  }
  const d = new Date(time);
  const now = Date.now();

  const diff = (now - d) / 1000;

  if (diff < 30) {
    return "刚刚";
  } else if (diff < 3600) {
    // less 1 hour
    return Math.ceil(diff / 60) + "分钟前";
  } else if (diff < 3600 * 24) {
    return Math.ceil(diff / 3600) + "小时前";
  } else if (diff < 3600 * 24 * 2) {
    return "1天前";
  }
  if (option) {
    return parseTime(time, option);
  } else {
    return (
      d.getMonth() +
      1 +
      "月" +
      d.getDate() +
      "日" +
      d.getHours() +
      "时" +
      d.getMinutes() +
      "分"
    );
  }
}

/**
 * @param {string} url
 * @returns {Object}
 */
export function getQueryObject(url) {
  url = url == null ? window.location.href : url;
  const search = url.substring(url.lastIndexOf("?") + 1);
  const obj = {};
  const reg = /([^?&=]+)=([^?&=]*)/g;
  search.replace(reg, (rs, $1, $2) => {
    const name = decodeURIComponent($1);
    let val = decodeURIComponent($2);
    val = String(val);
    obj[name] = val;
    return rs;
  });
  return obj;
}

/**
 * @param {string} input value
 * @returns {number} output value
 */
export function byteLength(str) {
  // returns the byte length of an utf8 string
  let s = str.length;
  for (var i = str.length - 1; i >= 0; i--) {
    const code = str.charCodeAt(i);
    if (code > 0x7f && code <= 0x7ff) s++;
    else if (code > 0x7ff && code <= 0xffff) s += 2;
    if (code >= 0xdc00 && code <= 0xdfff) i--;
  }
  return s;
}

/**
 * @param {Array} actual
 * @returns {Array}
 */
export function cleanArray(actual) {
  const newArray = [];
  for (let i = 0; i < actual.length; i++) {
    if (actual[i]) {
      newArray.push(actual[i]);
    }
  }
  return newArray;
}

/**
 * @param {Object} json
 * @returns {Array}
 */
export function param(json) {
  if (!json) return "";
  return cleanArray(
    Object.keys(json).map((key) => {
      if (json[key] === undefined) return "";
      return encodeURIComponent(key) + "=" + encodeURIComponent(json[key]);
    })
  ).join("&");
}

/**
 * @param {string} url
 * @returns {Object}
 */
export function param2Obj(url) {
  const search = url.split("?")[1];
  if (!search) {
    return {};
  }
  return JSON.parse(
    '{"' +
      decodeURIComponent(search)
        .replace(/"/g, '\\"')
        .replace(/&/g, '","')
        .replace(/=/g, '":"')
        .replace(/\+/g, " ") +
      '"}'
  );
}

/**
 * @param {string} val
 * @returns {string}
 */
export function html2Text(val) {
  const div = document.createElement("div");
  div.innerHTML = val;
  return div.textContent || div.innerText;
}

/**
 * Merges two objects, giving the last one precedence
 * @param {Object} target
 * @param {(Object|Array)} source
 * @returns {Object}
 */
export function objectMerge(target, source) {
  if (typeof target !== "object") {
    target = {};
  }
  if (Array.isArray(source)) {
    return source.slice();
  }
  Object.keys(source).forEach((property) => {
    const sourceProperty = source[property];
    if (typeof sourceProperty === "object") {
      target[property] = objectMerge(target[property], sourceProperty);
    } else {
      target[property] = sourceProperty;
    }
  });
  return target;
}

/**
 * @param {HTMLElement} element
 * @param {string} className
 */
export function toggleClass(element, className) {
  if (!element || !className) {
    return;
  }
  let classString = element.className;
  const nameIndex = classString.indexOf(className);
  if (nameIndex === -1) {
    classString += "" + className;
  } else {
    classString =
      classString.substr(0, nameIndex) +
      classString.substr(nameIndex + className.length);
  }
  element.className = classString;
}

/**
 * @param {string} type
 * @returns {Date}
 */
export function getTime(type) {
  if (type === "start") {
    return new Date().getTime() - 3600 * 1000 * 24 * 90;
  } else {
    return new Date(new Date().toDateString());
  }
}

/**
 * @param {Function} func
 * @param {number} wait
 * @param {boolean} immediate
 * @return {*}
 */
export function debounce(func, wait, immediate) {
  let timeout, args, context, timestamp, result;

  const later = function () {
    // 据上一次触发时间间隔
    const last = +new Date() - timestamp;

    // 上次被包装函数被调用时间间隔 last 小于设定时间间隔 wait
    if (last < wait && last > 0) {
      timeout = setTimeout(later, wait - last);
    } else {
      timeout = null;
      // 如果设定为immediate===true，因为开始边界已经调用过了此处无需调用
      if (!immediate) {
        result = func.apply(context, args);
        if (!timeout) context = args = null;
      }
    }
  };

  return function (...args) {
    context = this;
    timestamp = +new Date();
    const callNow = immediate && !timeout;
    // 如果延时不存在，重新设定延时
    if (!timeout) timeout = setTimeout(later, wait);
    if (callNow) {
      result = func.apply(context, args);
      context = args = null;
    }

    return result;
  };
}

/**
 * This is just a simple version of deep copy
 * Has a lot of edge cases bug
 * If you want to use a perfect deep copy, use lodash's _.cloneDeep
 * @param {Object} source
 * @returns {Object}
 */
export function deepClone(source) {
  if (!source && typeof source !== "object") {
    throw new Error("error arguments", "deepClone");
  }
  const targetObj = source.constructor === Array ? [] : {};
  Object.keys(source).forEach((keys) => {
    if (source[keys] && typeof source[keys] === "object") {
      targetObj[keys] = deepClone(source[keys]);
    } else {
      targetObj[keys] = source[keys];
    }
  });
  return targetObj;
}

/**
 * @param {Array} arr
 * @returns {Array}
 */
export function uniqueArr(arr) {
  return Array.from(new Set(arr));
}

/**
 * @returns {string}
 */
export function createUniqueString() {
  const timestamp = +new Date() + "";
  const randomNum = parseInt((1 + Math.random()) * 65536) + "";
  return (+(randomNum + timestamp)).toString(32);
}

/**
 * Check if an element has a class
 * @param {HTMLElement} elm
 * @param {string} cls
 * @returns {boolean}
 */
export function hasClass(ele, cls) {
  return !!ele.className.match(new RegExp("(\\s|^)" + cls + "(\\s|$)"));
}

/**
 * Add class to element
 * @param {HTMLElement} elm
 * @param {string} cls
 */
export function addClass(ele, cls) {
  if (!hasClass(ele, cls)) ele.className += " " + cls;
}

/**
 * Remove class from element
 * @param {HTMLElement} elm
 * @param {string} cls
 */
export function removeClass(ele, cls) {
  if (hasClass(ele, cls)) {
    const reg = new RegExp("(\\s|^)" + cls + "(\\s|$)");
    ele.className = ele.className.replace(reg, " ");
  }
}

export function getTypeName(value, list) {
  if (value !== 0) return list.find((item) => item.id === value).label;
  else return "";
}

export function getModifyO(value, list) {
  return list.find((item) => item.id === value);
}

export function formatNumber(num, precision, separator) {
  // console.log(num)
  // 判断是否为数字
  if (!isNaN(parseFloat(num)) && isFinite(num)) {
    // 把类似 .5, 5. 之类的数据转化成0.5, 5, 为数据精度处理做准, 至于为什么
    // 不在判断中直接写 if (!isNaN(num = parseFloat(num)) && isFinite(num))
    // 是因为parseFloat有一个奇怪的精度问题, 比如 parseFloat(12312312.1234567119)
    // 的值变成了 12312312.123456713
    num = Number(num);
    // 处理小数点位数
    num = (typeof precision !== "undefined"
      ? (
          Math.round(num * Math.pow(10, precision)) / Math.pow(10, precision)
        ).toFixed(precision)
      : num
    ).toString();
    // 分离数字的小数部分和整数部分
    const parts = num.split(".");
    // 整数部分加[separator]分隔, 借用一个著名的正则表达式
    parts[0] = parts[0]
      .toString()
      .replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1" + (separator || ","));

    return parts.join(".");
  }
  return "";
}
const load_config = {
  tcaptcha: {
    url: "https://ssl.captcha.qq.com/TCaptcha.js",
    isReady: false,
  },
  xlsx: {
    url: "//js.guanmai.cn/build/libs/node_modules/xlsx/dist/xlsx.full.min.js",
    isReady: false, // 是否已经加载过，避免重复加载
  },
  xlsxStyle: {
    url: "http://image.yy5156.com/js/xlsx.full.min.js",
    isReady: false, // 是否已经加载过，避免重复加载
  },
  wxLogin: {
    url: "http://res.wx.qq.com/connect/zh_CN/htmledition/js/wxLogin.js",
    isReady: false, // 是否已经加载过，避免重复加载
  },
};

function loadScript(url) {
  return new Promise((resolve, reject) => {
    !!reject;
    var script = document.createElement("script");
    script.type = "text/javascript";
    if (script.readyState) {
      // IE
      script.onreadystatechange = function () {
        if (
          script.readyState === "loaded" ||
          script.readyState === "complete"
        ) {
          script.onreadystatechange = null;
          return resolve();
        }
      };
    } else {
      // Others
      script.onload = function () {
        resolve();
      };
    }
    script.src = url;
    document.getElementsByTagName("head")[0].appendChild(script);
  });
}
export const loadJsAsync = function (conf) {
  const target = load_config[conf];
  if (!target) {
    Promise.reject("target is not exists");
    return;
  }

  return new Promise((resolve, reject) => {
    if (target.isReady) {
      // 已经加载过
      resolve();
      return;
    }
    if (!target.url) {
      reject("url is empty");
      return;
    }
    loadScript(target.url)
      .then(() => {
        target.isReady = true;
        resolve();
      })
      .catch(reject);
  });
};

export function getSlotNoFour(t) {
  if (t) return t.substring(0, 4);
  else return "";
}
export function getSlotNoLast(t) {
  if (t) return t.substring(4, t.length);
  else return "";
}

export function getSubstring(t, s, e) {
  if (t) return t.substring(s, e);
  else return "";
}

import { OLTYPE } from "@/global/const";
export function getTakeInType(m) {
  if (m === 0) {
    return "未选择入库类型";
  } else {
    return OLTYPE.find((x) => x.value === m * 1).name;
  }
}

export function compress(fileObj, callback) {
  try {
    const image = new Image();
    image.src = URL.createObjectURL(fileObj);
    image.onload = function () {
      const that = this;
      // 默认按比例压缩
      let w = that.width;
      let h = that.height;
      let quality = w > 2000 || h > 1500 ? 0.7 : 1; // 默认图片质量为0.7;
      const scale = w / h;
      w = fileObj.width || w * quality;
      h = fileObj.height || w / scale;
      console.log(w, h);

      // 生成canvas
      const canvas = document.createElement("canvas");
      const ctx = canvas.getContext("2d");
      // 创建属性节点
      const anw = document.createAttribute("width");
      anw.nodeValue = w;
      const anh = document.createAttribute("height");
      anh.nodeValue = h;
      canvas.setAttributeNode(anw);
      canvas.setAttributeNode(anh);
      ctx.drawImage(that, 0, 0, w, h);
      // 图像质量
      if (fileObj.quality && fileObj.quality <= 1 && fileObj.quality > 0) {
        quality = fileObj.quality;
      }
      // quality值越小，所绘制出的图像越模糊
      const data = canvas.toDataURL("image/jpeg", quality);
      // 压缩完成执行回调
      const newFile = convertBase64UrlToBlob(data, fileObj);
      callback(newFile);
    };
  } catch (e) {
    console.log("压缩失败!");
  }
}
function convertBase64UrlToBlob(urlData, fileObj) {
  const bytes = window.atob(urlData.split(",")[1]); // 去掉url的头，并转换为byte
  // 处理异常,将ascii码小于0的转换为大于0
  const ab = new ArrayBuffer(bytes.length);
  const ia = new Uint8Array(ab);
  for (let i = 0; i < bytes.length; i++) {
    ia[i] = bytes.charCodeAt(i);
  }
  // console.log();
  var a = new Blob([ab], { type: "image/jpeg" });
  return new File([a], fileObj.name);
}

export function packUnitName(val) {
  // console.log(PAY_UNITS);
  return PAY_UNITS.find((x) => x.value === val).title;
}
