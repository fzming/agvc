<template>
  <div>
    <van-nav-bar title="在线打码" />
    <section class="wrapper">
      <div style="margin: 100px auto" v-if="!captcha">
        <van-loading size="24px" vertical>等待指令...</van-loading>
      </div>
      <template v-else>
        <div style="min-height: 200px; text-align: center">
          <img :src="codeUrl" style="max-width: 80%" />
        </div>
        <van-form @submit="onSubmit">
          <van-field
            v-model="code"
            name="验证码"
            label="验证码"
            placeholder="输入验证码"
            :rules="[{ required: true, message: '请填写验证码' }]"
          />

          <div style="margin: 16px">
            <van-button
              round
              block
              type="info"
              :disabled="!code"
              native-type="submit"
              >确认提交</van-button
            >
          </div>
        </van-form>
      </template>
      <div style="margin: 16px; text-align: center">
        系统剩余任务总数：{{ total }}
      </div>
    </section>
  </div>
</template>

<script>
import { $subscribe } from "@/global/subscribe";
import { WEBAPI_BASE_URL } from "@/global/const";
import hmcApi from "@/api/Hmc";
export default {
  name: "mobileIndex",
  data() {
    return {
      code: "",
      //任务总数
      total: 0,
      //当前打码对象
      captcha: null,
      timer: null,
      accepting: false,
    };
  },
  computed: {
    codeUrl() {
      if (!this.captcha) return "";
      return `${WEBAPI_BASE_URL}${this.captcha.path}`;
    },
  },
  beforeDestroy(){
      if (this.timer) this.stopWatcher();
  },
  async mounted() {
    $subscribe({
      //人工打码指令
      HmcMessage: async (arg) => {
        console.log(arg);
        this.total = parseInt(arg._message_content);
        if (!this.captcha) {
          await this.acceptNextCodeAsync();
        }
      },
    });
    //首次启动，获取一次打码任务
    this.startWatcher();
  },
  watch: {
    captcha(value) {
      if (!value) {
        //启动本地轮询线程
        this.startWatcher();
      } else {
        this.stopWatcher();
      }
    },
  },
  methods: {
    startWatcher() {
      this.timer = setInterval(async () => {
        await this.acceptNextCodeAsync();
      }, 1000);
    },
    stopWatcher() {
      clearInterval(this.timer);
    },
    async acceptNextCodeAsync() {
      if (this.accepting) return;
      this.accepting = true;
      const rs = await hmcApi.acceptNextAsync();
      console.log(rs);
      this.captcha = rs.captcha;
      this.total = rs.total;

      this.accepting = false;
    },
    async onSubmit() {
      if (this.timer) this.stopWatcher();
      if (!this.captcha || !this.code) return;
      var ok = await hmcApi.setCodeAsync({
        captchaId: this.captcha.id,
        code: this.code,
      });
      console.log("setCodeAsync", ok);
      this.captcha = null;
      this.code = "";
      this.total-=1;
    },
  },
};
</script>

<style lang="scss" scoped></style>
