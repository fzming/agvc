/*
  模块组件定义
*/
import AsyncCompoment from "./AsyncComponent/AsyncComponent.vue";
import WindowContatiner from "./WindowContainer/WindowContainer.vue";
import ModalWindow from "./WindowContainer/ModalWindow.vue";
import ElIcon from "./ElIcon.vue";
import CaptionContainer from "./CaptionContainer";
import region from "./CaptionContainer/region.vue";
import condition from "./CaptionContainer/condition.vue";
export default Vue => {
  Vue.component(AsyncCompoment.name, AsyncCompoment);
  Vue.component(WindowContatiner.name, WindowContatiner);
  Vue.component(ModalWindow.name, ModalWindow);
  Vue.component(ElIcon.name, ElIcon);
  Vue.component(CaptionContainer.name, CaptionContainer);
  Vue.component(region.name, region);
  Vue.component(condition.name, condition);
};
