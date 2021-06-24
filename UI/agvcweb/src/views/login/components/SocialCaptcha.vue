<template>
  <div id="tcaptcha" />
</template>
<script>
import { loadJsAsync } from '@/utils';
export default {
  name: 'SocialCaptcha',
  props: {
    appid: {
      type: String,
      default: ''
    }
  },
  data() {
    return {
      tencentCaptcha: null
    }
  },
  beforeDestroy() {
    if (this.tencentCaptcha) this.tencentCaptcha.destroy();
  },
  mounted() {
    this.createCaptcha();
  },
  methods: {
    createCaptcha() {
      const that = this;
      loadJsAsync('tcaptcha').then(() => {
        const container = document.getElementById('tcaptcha');
        that.tencentCaptcha = new window.TencentCaptcha(container, {
          type: 'embed',
          pos: 'relative',
          appid: that.appid,
          callback(res) {
            if (res.ret === 0) {
              that.$emit('success', res);
            } else {
              that.$emit('error', res);
            }
          }
        });
        that.tencentCaptcha.show(); // 显示验证码
      });
    }
  }
};
</script>
<style lang="scss" scoped>
    #tcaptcha{
        min-width: 300px;
        min-height: 100px;
        margin: auto;
    }
</style>
