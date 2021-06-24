const directive = function() {}
import code from './global/code.js'
import src from './global/v-src.js'
import drag from './global/v-drag.js'
import loadmore from './el-loadmore/v-loadmore.js'

directive.install = function(Vue, options) {
  !options
  Vue.directive('code', code)
  Vue.directive('src', src)
  Vue.directive('drag', drag)
  Vue.directive('loadmore', loadmore)
  // 注册其他
}

export default directive
