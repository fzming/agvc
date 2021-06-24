import store from "@/store";
import { Message } from "element-ui";

const vCodeInserted = (el, inserted, vnode) => {
  !!vnode;
  const { value } = inserted;
  const codes = store.getters && store.getters.role_codes;

  // console.log("==============", codes, value);
  if (value && value.indexOf("zl-") > -1) {
    const codepermisson = codes.find((p) => p.code === value);
    // console.log(codepermisson);
    if (
      codepermisson === undefined ||
      (codepermisson && codepermisson.disabled)
    )
      return; //指令已废除
    let hasPermission = codepermisson && codepermisson.hasPermission; //是否有权限
    if (!hasPermission) {
      const disableType = codepermisson.disableType;
      switch (disableType) {
        case 0: // 隐藏
          el.parentNode && el.parentNode.removeChild(el);
          break;
        case 1: // 禁用
          el.setAttribute("disabled", "disabled");
          el.onclick = null;
          break;
        case 2: // 提示
          var node = el.cloneNode(true);
          node.addEventListener(
            "click",
            function (event) {
              Message.warning({
                message: `权限不足：${codepermisson.name}`,
              });
              event.stopPropagation();
            },
            false
          );
          el.parentNode && el.parentNode.appendChild(node);
          el.parentNode && el.parentNode.removeChild(el);
          break;
      }
    }
  }
};

// const vCodeDirective = (el, binding, vnode) => {
//   !!vnode;
//   const { value } = binding;
//   const codes = store.getters && store.getters.role_codes;
//   // 指令是以zl-开头的序列
//   if (value && value.indexOf("zl-") > -1) {
//     const codepermisson = codes.find((p) => p.code === value);
//     // console.log(codepermisson);
//     if (
//       typeof codepermisson === "undefined" ||
//       (codepermisson && !codepermisson.hasPermission)
//     ) {
//       // console.log("当前角色无指令权限");
//     } else {
//       // console.log("当前角色有指令权限");
//       const disableType = codepermisson.disableType;
//       // console.log(disableType);
//       switch (disableType) {
//         case 0: // 隐藏
//           el.parentNode && el.parentNode.removeChild(el);
//           break;
//         case 1: // 禁用
//           el.setAttribute("disabled", "disabled");
//           el.onclick = null;
//           break;
//         case 2: // 提示
//           var node = el.cloneNode(true);
//           node.addEventListener(
//             "click",
//             function (event) {
//               Message.warning({
//                 message: `权限不足：${codepermisson.name}`,
//               });
//               event.stopPropagation();
//             },
//             false
//           );
//           el.parentNode && el.parentNode.appendChild(node);
//           el.parentNode && el.parentNode.removeChild(el);
//           break;
//       }
//     }
//   }
// };

export default {
  // bind: inserted
  // unbind,
  inserted: vCodeInserted,
  update: vCodeInserted,
  // componentUpdated: 被绑定元素所在模板完成一次更新周期时调用。
};
