<template>
  <span class="elemicon" @click="handleClick" :disabled="!enable">
    <i
      v-if="visible"
      :class="iconClass"
      :style="iconStyle"
      :disabled="!enable"
    />
    <a href="javascript:;">
      <slot> </slot>
    </a>
  </span>
</template>
<script>
export default {
  name: "ElIcon",
  props: {
    visible: {
      type: Boolean,
      default: true
    },
    family: {
      type: String,
      default: "el-icon"
    },
    prex: {
      type: String,
      default: "icon"
    },
    name: {
      type: String,
      default: "",
      requried: true
    },
    size: {
      type: String,
      default: "16px"
    },
    color: {
      type: String,
      default: ""
    },
    enable: {
      type: Boolean,
      default: true
    }
  },
  data() {
    return {};
  },
  computed: {
    iconClass() {
      return `${this.family}-${this.name}`; // `${this.family} ${this.prex}-${this.name}`
    },
    iconStyle() {
      return {
        "font-size": this.size !== "18px" ? this.size : "",
        color: this.color
      };
    }
  },
  methods: {
    handleClick() {
      this.$emit("click");
    }
  }
};
</script>
<style lang="scss">
.elemicon {
  a {
    vertical-align: middle;
    color: $primary-color;
    font-size: 12px;
  }
}
</style>
