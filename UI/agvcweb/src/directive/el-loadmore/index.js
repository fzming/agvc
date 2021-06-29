import loadmore from './v-loadmore'

const install = function (Vue) {
  Vue.directive('el-loadmore', loadmore)
}

if (window.Vue) {
  Vue.use(install); // eslint-disable-line
}

 
