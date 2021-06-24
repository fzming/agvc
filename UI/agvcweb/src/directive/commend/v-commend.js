// 全局注册自定义指令，用于判断当前图片是否能够加载成功，可以加载成功则赋值为img的src属性，否则使用默认图片
export default async function(el, binding) {
  // console.log(binding.value)
  switch (binding.value) {
    case 0:
      // console.log('隐藏')
      break
    case 1:
      // console.log('禁用')
      break
    case 2:
      // console.log('提示')
      break
  }
}
