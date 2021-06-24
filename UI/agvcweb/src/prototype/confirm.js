import { MessageBox } from "element-ui";

export default (title, message, options) => {
  return new Promise((resolve) => {
    let option = {
      hasCallBack: false,
      confirmButton: "确认",
      cancelButton: "取消",
      otype: "info",

      callBack: () => {},
    };
    if (options) {
      Object.assign(option, options);
    }
    console.log(option);
    MessageBox.confirm(message, title, {
      confirmButtonText: option.confirmButton,
      cancelButtonText: option.cancelButton,
      closeOnClickModal: false,
      customClass: option.customClass,
      type: option.otype,
      callback(action, instance) {
        !!instance;
        option.callBack(action === "confirm");
        resolve(action === "confirm");
      },
    });
  });
};
