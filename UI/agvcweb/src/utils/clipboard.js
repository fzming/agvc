import Vue from "vue";
import Clipboard from "clipboard";
import $ from "jquery";
import { Message } from "element-ui";
function clipboardSuccess() {
  Vue.prototype.$message({
    message: "复制成功",
    type: "success",
    duration: 1500,
  });
}

function clipboardError() {
  Vue.prototype.$message({
    message: "复制失败",
    type: "error",
  });
}
function SelectText(element) {
  var doc = document;
  if (doc.body.createTextRange) {
    let range = document.body.createTextRange();
    range.moveToElementText(element);
    range.select();
  } else if (window.getSelection) {
    var selection = window.getSelection();
    let range = document.createRange();
    range.selectNodeContents(element);
    selection.removeAllRanges();
    selection.addRange(range);
    console.log(selection);
  }
}
export default function handleClipboard(text, event) {
  const clipboard = new Clipboard(event.target, {
    text: () => text,
  });
  clipboard.on("success", () => {
    clipboardSuccess();
    clipboard.off("error");
    clipboard.off("success");
    clipboard.destroy();
  });
  clipboard.on("error", () => {
    clipboardError();
    clipboard.off("error");
    clipboard.off("success");
    clipboard.destroy();
  });
  clipboard.onClick(event);
}

export function clipboardImg(tag) {
  $(tag).attr("contenteditable", true);
  SelectText($(tag).get(0));
  document.execCommand("copy");
  window.getSelection().removeAllRanges();
  $(tag).removeAttr("contenteditable");
  Message.success({
    message: "二维码复制成功",
  });
}
