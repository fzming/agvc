import commend from './v-commend'

const install = function(Vue) {
  Vue.directive('commend', { bind: commend })
}

if (window.Vue) {
  window['commend'] = commend
  Vue.use(install) // eslint-disable-line
}

commend.install = install
export default commend
