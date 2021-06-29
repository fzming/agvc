<template>
  <ul class="cb-slideshow" v-if="allBackgroundImages.length>1">
    <li v-for="(image, index) in allBackgroundImages" :key="index">
      <span :style="getImageStyle({ index, image })"></span>
      <!-- <div>
          <h3>Image{{ index }}</h3>
        </div> -->
    </li>
  </ul>
  <div v-else class="cb-single">
    <div class="img" :style="getIndexImageStyle(0)"></div>
  </div>
</template>
<style lang="scss" scoped>
@import "../cb-slideshow.scss";
</style>
<script>
// 引入背景图
import allBackgroundImages from "@/assets/desktop";
export default {
  name: "slideshow",
  data() {
    return {
      allBackgroundImages,
    };
  },
  computed: {
    backgroundImage() {
      // 根据背景图数组的长度随机选择索引
      // const randIndex = Math.floor(
      //   Math.random() * this.allBackgroundImages.length
      // );
      return {
        // 获取对应的图片资源并将其设置到`background-image`属性上
        //backgroundImage: `url(${this.allBackgroundImages[randIndex]})`,
      };
    },
  },
  methods: {
    getImageStyle({ index, image }) {
      !!index;
      // const count = allBackgroundImages.length;
      return {
        backgroundImage: `url(${image})`,
        // "animation-delay": count * index + "s",
        // "animation-duration": Math.pow(count, 2) + "s",
      };
    },
    getIndexImageStyle(index) {
      return {
        backgroundImage: `url(${allBackgroundImages[index]})`,
      };
    },
  },
};
</script>
