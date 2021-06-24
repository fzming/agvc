<template>
  <div class="flex uc" :style="style" @click="clickCard(clickName)">
    <svg-icon :icon-class="icon" />
    <div class="flex1">
      <div>
        <span class="count">
          <count-to
            ref="counter"
            :start-val="va.setStartVal"
            :end-val="va.setEndVal"
            :duration="va.setDuration"
            :decimals="va.setDecimals"
            :separator="va.setSeparator"
            :prefix="va.setPrefix"
            :suffix="va.setSuffix"
            :autoplay="false"
          />
        </span>
        <span class="unit">{{ unit }}</span>
      </div>
      <div class="des">{{ title }}</div>
    </div>
  </div>
</template>
<script>
import countTo from "vue-count-to";

export default {
  name: "Card",
  components: { countTo },
  props: {
    title: {
      type: String,
      default: "默认标题"
    },
    count: {
      type: Number,
      default: 0
    },
    format: {
      type: Boolean,
      default: false
    },
    unit: {
      type: String,
      default: ""
    },
    clickName: {
      type: String,
      default: "doClick"
    },
    icon: {
      type: String,
      default: "card-1"
    },
    color: {
      type: String,
      default: "white"
    }
  },
  data() {
    return {};
  },
  computed: {
    va() {
      return {
        setStartVal: 0,
        setEndVal: this.count,
        setDuration: 500,
        setDecimals: 0,
        setSeparator: ",",
        setSuffix: "",
        setPrefix: this.format ? "¥ " : ""
      };
    },
    style() {
      return {
        backgroundColor: this.color
      };
    }
  },
  watch: {
    count() {
      this.start();
    }
  },
  mounted() {
    this.start();
  },
  methods: {
    start() {
      this.$refs.counter.start();
    },
    pauseResume() {
      this.$refs.counter.pauseResume();
    },
    clickCard(clickName) {
      this.$emit(clickName);
    }
  }
};
</script>
<style lang="scss" scoped>
.uc {
  padding: 16px;
  cursor: pointer;
  color: white;
  box-shadow: 0 1px 3px rgba(black, 0.2);
  min-height: 100px;
  transition: all 0.3s;
  box-sizing: border-box;
  background: url("~@/assets/images/bg_wave2.png") white right bottom no-repeat;
  background-size: 60%;
  &:hover {
    box-shadow: 0 3px 5px rgba(black, 0.5);
    .count {
      font-size: 30px;
    }
  }
  .svg-icon {
    width: 50px;
    height: 50px;
    opacity: 0.5;
    margin-right: 15px;
    margin-left: 5px;
  }
  .count {
    font-size: 25px;
    font-weight: bold;
    transition: all 0.3s;
    font-family: "dinot";
  }
  .unit {
    color: rgba(white, 0.7);
    margin-left: 10px;
  }
  .des {
    padding-top: 5px;
    font-size: 14px;
    color: rgba(white, 0.7);
  }
}
</style>
