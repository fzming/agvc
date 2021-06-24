export default {
  bind(el, binding) {
    const selectWrap = el.querySelector(".el-table__body-wrapper");
    selectWrap.addEventListener("scroll", function () {
      let sign = 0;
      const scrollDistance =
        this.scrollHeight - this.scrollTop - this.clientHeight;
      if (scrollDistance <= sign) {
        binding.value();
      }
    });
  },
};