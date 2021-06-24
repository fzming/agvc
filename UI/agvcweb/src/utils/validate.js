/**
 * Created by PanJiaChen on 16/11/18.
 */

/**
 * @param {string} path
 * @returns {Boolean}
 */
export function isExternal(path) {
  return /^(https?:|mailto:|tel:)/.test(path);
}

/**
 * @param {string} str
 * @returns {Boolean}
 */
export function validUsername(str) {
  return /^[a-zA-Z0-9_-]{4,16}$/.test(str);
}

/**
 * @param {string} url
 * @returns {Boolean}
 */
export function validURL(url) {
  const reg = /^(https?|ftp):\/\/([a-zA-Z0-9.-]+(:[a-zA-Z0-9.&%$-]+)*@)*((25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9][0-9]?)(\.(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9]?[0-9])){3}|([a-zA-Z0-9-]+\.)*[a-zA-Z0-9-]+\.(com|edu|gov|int|mil|net|org|biz|arpa|info|name|pro|aero|coop|museum|[a-zA-Z]{2}))(:[0-9]+)*(\/($|[a-zA-Z0-9.,?'\\+&%$#=~_-]+))*$/;
  return reg.test(url);
}

/**
 * @param {string} str
 * @returns {Boolean}
 */
export function validLowerCase(str) {
  const reg = /^[a-z]+$/;
  return reg.test(str);
}

/**
 * @param {string} str
 * @returns {Boolean}
 */
export function validUpperCase(str) {
  const reg = /^[A-Z]+$/;
  return reg.test(str);
}

/**
 * @param {string} str
 * @returns {Boolean}
 */
export function validAlphabets(str) {
  const reg = /^[A-Za-z]+$/;
  return reg.test(str);
}

/**
 * @param {string} email
 * @returns {Boolean}
 */
export function validEmail(email) {
  const reg = /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
  return reg.test(email);
}

/**
 * @param {string} str
 * @returns {Boolean}
 */
export function isString(str) {
  if (typeof str === "string" || str instanceof String) {
    return true;
  }
  return false;
}

/**
 * @param {Array} arg
 * @returns {Boolean}
 */
export function isArray(arg) {
  if (typeof Array.isArray === "undefined") {
    return Object.prototype.toString.call(arg) === "[object Array]";
  }
  return Array.isArray(arg);
}

// 车牌号正则表达式
const VCLN_PATTERN = /^[京津沪渝冀豫云辽黑湘皖鲁新苏浙赣鄂桂甘晋蒙陕吉闽贵粤青藏川宁琼使领A-Z]{1}[A-Z]{1}[A-Z0-9]{4}[A-Z0-9挂学警港澳]{1}$/;
// 挂车号正则表达式
const TRAILER_PATTERN = /^[京津沪渝冀豫云辽黑湘皖鲁新苏浙赣鄂桂甘晋蒙陕吉闽贵粤青藏川宁琼使领A-Z]{1}[A-Z]{1}[A-Z0-9]{4}[挂]{1}$/;
// 身份证号正则表达式
const IDCARD_PATTERN = /(^\d{15}$)|(^\d{18}$)|(^\d{17}(\d|X|x)$)/;
// 手机号正则表达式
const MOBILE_PATTERN = /^1[3-9]\d{9}$/;
// 包含表达式
const BIAODASHI = /[+\-*/]/gi;

export function isVcln(val) {
  // console.log(val)
  return VCLN_PATTERN.test(val);
}
export function isTrailer(val) {
  return TRAILER_PATTERN.test(val);
}
export function isIdCard(val) {
  return IDCARD_PATTERN.test(val);
}
export function isMobile(val) {
  return MOBILE_PATTERN.test(val);
}
export function isExpression(val) {
  return BIAODASHI.test(val);
}
// 箱号验证
export function isContainerCode(code) {
  if (!code) return false;
  var verifyCode = "0123456789A?BCDEFGHIJK?LMNOPQRSTU?VWXYZ";
  if (code.length !== 11) {
    return false;
  }
  var result = true;
  var num = 0;
  for (var i = 0; i < 10; i++) {
    var idx = verifyCode.indexOf(code[i]);
    if (idx === -1 || verifyCode[idx] === "?") {
      result = false;
      break;
    }
    idx = idx * Math.pow(2, i);
    num += idx;
  }
  num = (num % 11) % 10;
  return result && parseInt(code[10]) === num;
}
