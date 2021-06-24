<template>
  <el-input v-model="inputCode" placeholder="请输入验证码">
    <a
      slot="append"
      :loading="smsCodeFetching"
      @click="getVerification"
      :class="linkClass"
    >
      {{ smsCodeButtonText }}</a
    >
  </el-input>
</template>
<script>
import { mapAccountActions } from "@/store/namespaced/Account";
export default {
  name: "SmsCodeInput",
  model: {
    prop: "code",
    event: "changeCode",
  },
  props: {
    buttonName: {
      type: String,
      default: "获取验证码",
    },
    mobile: {
      type: String,
      required: true,
    },
    smsCodeKey: {
      type: String,
      required: true,
    },
    smsCodeTimeoutMax: {
      type: Number,
      default: 30,
    },
  },
  data() {
    return {
      smsCodeFetching: false,
      smsCodeTimeout: 0,
      smsCodeTimer: null,
      inputCode: "",
    };
  },
  computed: {
    linkClass() {
      return {
        link: true,
        active: !this.smsCodeDisabled,
      };
    },
    smsCodeDisabled() {
      return !this.validMobile || this.smsCodeTimeout > 0;
    },
    smsCodeButtonText() {
      return this.smsCodeTimeout > 0
        ? `${this.smsCodeTimeout}秒后可再获取`
        : this.buttonName;
    },
    validMobile() {
      return /^1[\d]{10}$/.test(this.mobile);
    },
  },
  watch: {
    inputCode(value) {
      this.$emit("changeCode", value);
      this.$emit("update:code", value); // 兼容:open.sync语法调用
    },
    smsCodeTimeout(value) {
      const that = this;
      if (value === this.smsCodeTimeoutMax) {
        if (this.smsCodeTimer) clearInterval(this.smsCodeTimer);
        this.smsCodeTimer = setInterval(function () {
          that.smsCodeTimeout--;
        }, 1000);
      } else if (value === 0) {
        clearInterval(this.smsCodeTimer);
      }
    },
  },
  methods: {
    ...mapAccountActions(["getfetchSmsSecurity"]),
    async getVerification() {
      if (!this.validMobile) {
        console.error("错误的手机号");
        return;
      }
      const securityKey = this.getSecurityKey();
      console.log("securityKey:" + securityKey);

      this.smsCodeFetching = true;
      // 获取验证码
      const data = {
        mobile: this.mobile,
        key: this.smsCodeKey,
        securityKey: securityKey,
      };
      const b = await this.getfetchSmsSecurity(data);
      this.smsCodeFetching = false;
      if (b) this.smsCodeTimeout = this.smsCodeTimeoutMax;
    },
    /*
    @legth 长度必须16位以上
    */
    getSecurityKey(length) {
      let i = 0;
      const words = [];
      const timestamp = new Date().valueOf() + ""; // 毫秒级时间戳
      console.log(timestamp);

      if (!length || length > timestamp.length) {
        length = timestamp.length;
      }
      const tsArray = timestamp.split("").reverse();
      const endprex = Math.floor(Math.random() * 10);
      const start = "A".charCodeAt(0);
      while (i < length) {
        var n = Math.floor(Math.random() * 26);
        const word = String.fromCharCode(n + start);
        let ts = "";
        if (tsArray.length) {
          ts = String.fromCharCode(parseInt(tsArray.shift()) + start);
        }
        var ns = n + endprex;
        words.push(ns + "" + word + "" + ts);
        i++;
      }
      return words.join("") + endprex;
    },
  },
};
</script>
<style lang="scss" scoped>
a.link {
  color: gray;
  &.active {
    color: $primary-color;
  }
}
</style>
