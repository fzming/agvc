let has_subscribed = false;
import { SIG_EVENT } from './const';
const subscribes = {};
export const $subscribe = sub => {
  if (sub) Object.assign(subscribes, sub);
  if (has_subscribed) return;

  has_subscribed = true;
  window.eventBus.$on(SIG_EVENT, data => {
    const _message_id = data._message_id;
    var callevent = subscribes[_message_id];
    if (typeof callevent === 'function') {
      callevent(data);
    } else {
      console.warn(`警告:${_message_id} 未订阅处理函数`);
    }
  });
};
