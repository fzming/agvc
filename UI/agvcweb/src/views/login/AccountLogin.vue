<template>
  <div class="flex-start login-wrapper">
    <SlideShow></SlideShow>
    <div class="flex1 content">
      <div id="featuredInformation">
        <section>
          <h1>
            <span class="title">AGVC支持系统V1.0</span>
          </h1>
          <p class="description">
            专业从事半导体技术平台研发的高科技企业,创业团队拥有多年的半导体工业控制运营经验和强大的技术研发能力
          </p>
          <p class="action">欢迎登入进行体验。</p>
          <p class="copyright">
            © 2021 Reserve, Inc. 版权所有 本服务由上海杰燊科技有限公司提供
          </p>
        </section>
      </div>
    </div>
    <section class="login-in">
      <div class="flex1">
        <div class="logo">
          <img src="@/assets/images/logo.png" />
        </div>
        <div
          class="login-form"
          v-loading="
            (loginSuccess && !needChangePassword) || loading || logining
          "
        >
          <div>
            <WxOpenUser :user="qrCodeScanUser"> </WxOpenUser>
          </div>

          <div v-if="qrCodeScanShow">
            <h2>扫码登录</h2>
            <h3>
              <div v-if="orgnazition" style="margin-left: 16px">
                {{ orgnazition.name }}
              </div>
            </h3>
            <div style="height: 80px"></div>
            <WxOpenQrCode
              state="123"
              redirectUri="http://api.yy5156.com/api/wx-open/callback"
              @on-scan="handleWxQrCodeScan"
            ></WxOpenQrCode>
            <div style="text-align: center" v-if="false">
              <el-button type="danger" @click="toggleQrCodeMode(false)"
                >取消扫码操作</el-button
              >
            </div>
          </div>
          <template v-else-if="needChangePassword">
            <h2>首次登录需更改密码</h2>
            <need-change-pass
              @restok="continueRedirect"
              @setNeedChange="setNeedChange"
            />
          </template>
          <template v-else-if="forgetPwd">
            <h2>找回密码</h2>
            <forget-reset-pass @cancel="forgetPwd = false" />
          </template>
          <template v-else-if="!netWorkError">
            <template v-if="qrCodeScanUser.nickName">
              <div class="qrcode-bind-info">
                扫码成功，首次微信扫码登录需绑定现有系统账号。下次进行微信扫码时，将自动采用此账号进行登录。
              </div>
            </template>
            <template v-else>
              <h2>
                {{ !needSafetyValidation ? "系统登录" : "账户安全验证" }}
                <div
                  class="qrcode"
                  @click="toggleQrCodeMode(true)"
                  v-if="qrCodeSupport"
                  title="微信扫码登录"
                >
                  <img src="@/assets/images/qrcode.png" />
                </div>
              </h2>
              <h3>
                <div v-if="orgnazition">{{ orgnazition.name }}</div>
              </h3>
            </template>
            <div v-if="needSafetyValidation" class="captcha">
              <social-captcha
                appid="2065862553"
                @success="captchaTicketSuccess"
              />
              <div style="text-align: center; padding-top: 20px">
                <el-button
                  icon="el-icon-close"
                  type="danger"
                  circle
                  @click="cancelCaptcha"
                />
              </div>
            </div>
            <el-form
              v-if="!needSafetyValidation"
              ref="orgloginForm"
              :hide-required-asterisk="true"
              :model="loginData"
              :rules="loginRules"
              @submit.native.prevent
            >
              <el-form-item prop="username" label="账 号" label-position="top">
                <el-input
                  v-model="loginData.username"
                  placeholder="请输入账号、手机号"
                  @keyup.enter.native="doLogin"
                />
              </el-form-item>
              <el-form-item prop="password" label="密  码" label-position="top">
                <el-input
                  v-model="loginData.password"
                  type="password"
                  placeholder="请输入密码"
                />
              </el-form-item>
              <p class="form-forgot-password">
                忘了你的密码？
                <a @click="forgetPwd = true" class="find">找回密码</a>
              </p>
              <div class="moreOptions clear" v-if="false">
                <input
                  id="privateComputer"
                  v-model="loginData.remember"
                  type="checkbox"
                  name="privateComputer"
                />
                <label
                  id="privateComputerLabel"
                  for="privateComputer"
                  class="alt_label"
                  >保持登录状态2周</label
                >
                <div v-show="loginData.remember" class="why-text-wrapper">
                  <div class="why-text fs-alert fs-alert--warning">
                    <p id="whyText">
                      使用公共或共用的电脑吗？
                      <br />请勿选取这个选项。
                    </p>
                  </div>
                </div>
              </div>
              <el-form-item>
                <el-button
                  type="primary"
                  :loading="logining"
                  native-type="submit"
                  @click="doLogin"
                  >立即登录</el-button
                >
              </el-form-item>
            </el-form>
          </template>
          <template v-else>
            <div style="color: red">抱歉：网络错误，请刷新页面后重新扫码。</div>
          </template>
        </div>
      </div>
      <div class="border" />

      <div class="policy">
        点选
        <b>登录</b>按钮，表示已阅读并且同意
        <a>《杰燊科技隐私条款》</a>
      </div>
    </section>
  </div>
</template>
<script>
import userApi from "@/api/user";
import { mapUserActions, mapUserState } from "@/store/namespaced/user";
export default {
  name: "AccountLogin",
  components: {
    SocialCaptcha: () => import("./components/SocialCaptcha"),
    NeedChangePass: () => import("./components/NeedChangePass"),
    ForgetResetPass: () => import("./components/ForgetResetPass"),
    WxOpenQrCode: () => import("./components/WxOpenQrCode"),
    WxOpenUser: () => import("./components/WxOpenUser"),
    SlideShow: () => import("./components/SlideShow"),
  },
  data() {
    const validateUsername = (rule, value, callback) => {
      if (value.length < 0) {
        callback(new Error("用户名不为空！"));
      } else {
        callback();
      }
    };
    const validatePassword = (rule, value, callback) => {
      if (value.length < 5) {
        callback(new Error("密码不能小于5位数"));
      } else {
        callback();
      }
    };
    return {
      logining: false,
      loading: false,
      qrCodeScanLogin: false,
      qrCodeScanShow: false,
      qrCodeScanUser: {},
      qrCodeSupport: false, //是否支持扫码登录模式
      netWorkError: false,
      loginSuccess: false,
      forgetPwd: false,
      redirect: undefined,
      otherQuery: {},
      orgnazition: null,
      loginData: {
        domain: "",
        username: "",
        password: "",
        grant_type: "password",
        loginType: "pwdlogin",
        vaptchaToken: "",
        appIndentify: null,
        appUserInfo: null,
        remember: false,
      },
      loginRules: {
        username: [
          { required: true, trigger: "blur", validator: validateUsername },
        ],
        password: [
          { required: true, trigger: "blur", validator: validatePassword },
        ],
      },
    };
  },
  computed: {
    ...mapUserState(["needSafetyValidation", "needChangePassword", "features"]),
    domain() {
      const a = location.hostname.split(".")[0];
      return a;
    },
  },
  watch: {
    $route: {
      handler: function (route) {
        const query = route.query;
        if (query) {
          this.redirect = query.redirect;
          this.otherQuery = this.getOtherQuery(query);
        }
      },
      immediate: true,
    },
  },
  mounted() {},
  methods: {
    ...mapUserActions([
      "orglogin",
      "wxlogin",
      "setNeedSafetyValidation",
      "updateFeatures",
    ]),
    captchaTicketSuccess(token) {
      this.loginData.vaptchaToken = JSON.stringify(token);
      this.setNeedSafetyValidation(false);
      this.doLogin(); // 重新登陆
    },
    cancelCaptcha() {
      this.loginData.vaptchaToken = "";
      this.setNeedSafetyValidation(false);
    },
    async handleWxQrCodeScan({ identify, user }) {
      this.toggleQrCodeMode(false); //关闭二维码界面
      this.qrCodeScanUser = user;
      console.log(identify, user);
      if (user) {
        //判断当前微信用户在当前domain机构下是否已绑定用户
        const userModel = {
          appId: identify.AppId,
          openId: identify.OpenId,
          unionId: identify.UnionId,
          domain: this.domain,
        };
        console.log(userModel);
        const bAppUser = await userApi.isAppUser(userModel);
        console.log("bAppUser", bAppUser);
        if (typeof bAppUser == "undefined") {
          //网络故障
          this.netWorkError = true;
          return;
        }
        if (bAppUser) {
          //直接进行微信登录
          this.logining = true;
          this.loading = true;
          const success = await this.wxlogin(userModel);

          this.logining = false;
          this.loading = false;
          if (success) {
            //登录成功
            this.loginSuccess = success;
            this.continueRedirect();
          }
          return;
        }
        this.loginData.appIndentify = userModel;
        this.loginData.appUserInfo = user;
      }
    },
    doLogin() {
      this.$refs.orgloginForm.validate(async (valid) => {
        if (valid) {
          this.loginData.domain = this.domain;
          const lf = this.loginData;
          this.logining = true;

          const success = await this.orglogin(lf);
          this.logining = false;
          if (success) {
            this.loginSuccess = success;
            this.continueRedirect();
          }
        } else {
          return false;
        }
      });
    },
    setNeedChange(e) {
      console.log("eeeee", e);
      this.loginSuccess = e;
    },
    continueRedirect() {
      this.$router
        .push({ path: "/dashboard" })
        .then(() => {})
        .catch(() => {});
    },
    getOtherQuery(query) {
      return Object.keys(query).reduce((acc, cur) => {
        if (cur !== "redirect") {
          acc[cur] = query[cur];
        }
        return acc;
      }, {});
    },
    toggleQrCodeMode(b) {
      this.qrCodeScanLogin = b;
      this.qrCodeScanShow = b;
      if (!b) {
        this.loginData.appIndentify = null;
        this.loginData.appUserInfo = null;
      }
    },
  },
};
</script>

<style lang="scss" scoped>
@import "./checkbox.scss";

html,
body {
  overflow: hidden;
}

.captcha {
  padding: 20px 0;
  width: 300px;
  margin: 0 auto;
}

.border {
  height: 40px;
  border-bottom: 1px solid #ccc;
}

.qrcode-bind-info {
  padding: 10px;
  font-size: 14px;
  color: $primary-color;
  background: rgba($primary-color, 0.1);
}

.form-forgot-password {
  text-align: right;
  font-size: 100%;
  text-shadow: none;
  padding-top: 0;
  margin-top: -8px;
  font-size: 14px;
  color: #888888;

  a {
    color: $primary-color;
    text-decoration: underline;
  }
}

#featuredInformation {
  align-self: flex-end;
  padding: 0 32px 52px;

  .title {
    color: #fff;
    font-family: museo, sans-serif;
    font-size: 50px;
    margin-bottom: 12px;
    font-weight: 800;
    letter-spacing: 0.06rem;
    line-height: 2.5rem;
    position: relative;
    &::after {
      content: " ";
      position: absolute;
      width: 100%;
      height: 10px;
      left: 0;
      bottom: -2px;
      background: $primary-color;
    }
  }

  p + * {
    margin-top: 1.625rem;
  }

  .description {
    font-family: Museo, sans-serif;
    font-size: 26px;
    color: #fff;
  }

  .action {
    font-family: Verdana, sans-serif;
    font-size: 18px;
    line-height: 1.125rem;
    color: #fff;
  }

  .copyright {
    font-size: 12px;
    color: rgba(white, 0.5);
    line-height: 100%;
    margin-top: 1.625rem;
  }
}

.policy {
  font-size: 12px;
  padding-top: 20px;
  text-align: center;
  color: #999;
  padding: 20px 57px;
  align-self: flex-end;
  min-height: 50px;

  a {
    color: $primary-color;
  }
}

.login-wrapper {
  background: linear-gradient(
    to bottom,
    rgba(0, 0, 0, 0) 50%,
    rgba(0, 0, 0, 1)
  );
  // background-image: url(~@/assets/images/desktop4.jpg);
  padding: 0;
  position: relative;
  height: 100%;
  background-size: cover;
  display: grid;
  grid-template-areas: "content" "side";
  grid-template-columns: 1fr 453px;

  .content {
    grid-area: content;
    height: 100vh;
    position: relative;
    z-index: 1;
    display: flex;
    background: linear-gradient(
      to bottom,
      rgba(0, 0, 0, 0) 50%,
      rgba(0, 0, 0, 1)
    );
  }
}

.login-in {
  width: 453px;
  background: white;
  /* background: url("~@/assets/images/newyear/login.png") white bottom left
    no-repeat; */
  background-size: 100%;
  height: 100%;
  box-shadow: 5px 0 15px #333;
  display: flex;
  flex-direction: column;
  align-self: flex-end;
  flex-wrap: nowrap;
  max-width: 100%;
  transition: all 0.3s linear;
  position: relative;
  z-index: 10;

  // &::before {
  //   content: " ";
  //   bottom: 0;
  //   left: 0;
  //   width: 100%;
  //   height: 50%;
  //   pointer-events: none;
  //   position: absolute;
  //   background: url("~@/assets/images/bg_wave.png") left bottom no-repeat;
  //   z-index: 1;
  // }
  .logo {
    padding-top: 30px;
    padding-left: 32px;

    img {
      max-height: 50px;
    }
  }

  h2 {
    font-size: 25px;
    text-shadow: 0 2px 0 #eee;
    padding: 0px;
    margin: 0;
    color: #333333;
    font-weight: normal;
    position: relative;
    padding-left: 10px;

    &::before {
      content: " ";
      top: 7px;
      left: -4px;
      width: 6px;
      height: 22px;
      pointer-events: none;
      position: absolute;
      background: $primary-color;
      border-right: 2px solid #eee;
    }

    .qrcode {
      position: absolute;
      right: 10px;
      top: 0px;
      cursor: pointer;
      opacity: 0.7;

      &:hover {
        opacity: 1;
      }
    }
  }

  h3 {
    font-weight: normal;
    font-size: 14px;
    white-space: nowrap;
    overflow: hidden;
    color: gray;
  }

  .login-form {
    padding: 20px 57px;
    margin-top: 50px;
  }
}
</style>
<style lang="scss">
.login-in {
  p.cancel {
    color: red;
    cursor: pointer;
  }

  label {
    color: #666662;
    display: block;
    font-size: 14px;
    font-weight: 400;
    line-height: 1.35rem;
  }

  button.el-button--primary {
    width: 100%;
    display: block;
    margin-top: 40px;
    padding: 12px 20px;
    font-size: 16px;
    box-shadow: 0 0 0.266667rem 0.026667rem rgba($primary-color, 0.52);
  }

  button.el-button--medium.is-circle {
    box-shadow: 0 0 0.266667rem 0.026667rem rgba($red, 0.52);
  }

  .login-form {
    input:-internal-autofill-previewed,
    input:-internal-autofill-selected,
    textarea:-internal-autofill-previewed,
    textarea:-internal-autofill-selected,
    select:-internal-autofill-previewed,
    select:-internal-autofill-selected {
      -webkit-text-fill-color: #333 !important;
      transition: background-color 5000s ease-in-out 0s !important;
      background-color: transparent !important;
    }
  }

  .moreOptions {
    label {
      font-size: 14px;
      cursor: pointer;
    }

    input[type="checkbox"][id],
    input[type="radio"][id] {
      position: absolute;
      width: 1px;
      height: 1px;
      padding: 0;
      margin: -1px;
      overflow: hidden;
      clip: rect(0, 0, 0, 0);
      border: 0;
      outline: none;
    }
  }
}
</style>
