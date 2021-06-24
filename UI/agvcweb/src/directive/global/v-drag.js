// 被绑定元素插入父节点时调用（父节点存在即可调用，不必存在于 document 中）。
function inserted(el, binding, vNode) {
  !binding;
  !vNode;
  // el.setAttribute("style", "position: fixed; z-index: 9999");
}
// 只调用一次，指令第一次绑定到元素时调用，用这个钩子函数可以定义一个在绑定时执行一次的初始化动作。
function bind(el, binding, vNode) {
  const option = binding.value;
  !!vNode;
  if (!option) el.setAttribute('draggable', true);
  let left, top; // width, height;
  let enableDrag = true;
  el._mousedown = function(e) {
    enableDrag = e.srcElement.className === option.title;
    el.setAttribute('draggable', enableDrag);
  };
  el._mouseup = function() {
    enableDrag = false;
    el.setAttribute('draggable', false);
  };
  el._dragstart = function(event) {
    event.stopPropagation();
    if (!enableDrag) event.preventDefault();
    left = event.clientX - el.offsetLeft;
    top = event.clientY - el.offsetTop;

    // width = el.offsetWidth;
    // height = el.offsetHeight;
    // el.style.cursor = "move";
  };
  el._checkPosition = function() {
    // 防止被拖出边界
    // let width = el.offsetWidth;
    // let height = el.offsetHeight;
    // let left = Math.min(el.offsetLeft, document.body.clientWidth - width);
    // left = Math.max(0, left);
    // let top = Math.min(el.offsetTop, document.body.clientHeight - height);
    // top = Math.max(0, top);
    // el.style.left = left + "px";
    // el.style.top = top + "px";
    // el.style.width = width + "px";
    // el.style.height = height + "px";
  };
  el._dragEnd = function(event) {
    event.stopPropagation();
    left = event.clientX - left;
    top = event.clientY - top;
    el.style.left = left + 'px';
    el.style.top = top + 'px';
    // el.style.cursor = "default";
    // el.style.width = width + "px";
    // el.style.height = height + "px";
    // el._checkPosition();
  };
  el._documentAllowDraop = function(event) {
    // // Prevent default select and drag behavior
    event.preventDefault();
  };
  document.body.addEventListener('dragover', el._documentAllowDraop);
  if (option && option.title) {
    el.addEventListener('mousedown', el._mousedown);
    el.addEventListener('mouseup', el._mouseup);
  }
  el.addEventListener('dragstart', el._dragstart);
  el.addEventListener('dragend', el._dragEnd);
  // window.addEventListener("resize", el._checkPosition);
}
// 只调用一次， 指令与元素解绑时调用
function unbind(el, binding, vNode) {
  !binding;
  !vNode;
  document.body.removeEventListener('dragover', el._documentAllowDraop);
  el.removeEventListener('mousedown', el._mousedown);
  el.removeEventListener('mouseup', el._mouseup);
  el.removeEventListener('dragstart', el._dragstart);
  el.removeEventListener('dragend', el._dragEnd);
  //  window.removeEventListener("resize", el._checkPosition);
  delete el._documentAllowDraop;
  delete el._dragstart;
  delete el._dragEnd;
  // delete el._checkPosition;
}

export default {
  bind,
  unbind,
  inserted
  // componentUpdated: 被绑定元素所在模板完成一次更新周期时调用。
};
