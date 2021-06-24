<template>
  <div class="login-container">
    <div class="account-login">
      <div class="title-container">
        <div class="logo flex-start">
          <img style="height: 40px" src="@/assets/images/logo.png" />
          <span class="log-w">管理登陆</span>
        </div>
      </div>

      <div v-if="needSafetyValidation" class="captcha">
        <SocialCaptcha appid="2065862553" @success="captchaTicketSuccess" />
        <div style="text-align: center; padding-top: 20px">
          <el-button
            icon="el-icon-close"
            type="danger"
            circle
            @click="cancelCaptcha"
          />
        </div>
      </div>
      <template v-else-if="needChangePassword">
        <div style="min-width: 400px">
          <h2>更改密码</h2>
          <NeedChangePass @restok="continueRedirect" />
        </div>
      </template>
      <template v-else>
        <div class="lgc">
          <el-form
            v-show="!needSafetyValidation"
            ref="loginForm"
            :model="loginForm"
            :rules="loginRules"
            class="login-form"
            autocomplete="off"
            label-position="left"
          >
            <el-form-item prop="username">
              <span class="svg-container">
                <svg-icon icon-class="user" />
              </span>
              <el-input
                ref="username"
                v-model="loginForm.username"
                placeholder="请输入账号、手机号"
                name="username"
                type="text"
                tabindex="1"
                autocomplete="on"
              />
            </el-form-item>
            <el-tooltip
              v-model="capsTooltip"
              content="大写模式已打开"
              placement="right"
              manual
            >
              <el-form-item prop="password">
                <span class="svg-container">
                  <svg-icon icon-class="password" />
                </span>
                <el-input
                  :key="passwordType"
                  ref="password"
                  v-model="loginForm.password"
                  :type="passwordType"
                  placeholder="请输入密码"
                  name="password"
                  tabindex="2"
                  autocomplete="on"
                  @keyup.native="checkCapslock"
                  @blur="capsTooltip = false"
                  @keyup.enter.native="handleLogin"
                />
                <span class="show-pwd" @click="showPwd">
                  <svg-icon
                    :icon-class="
                      passwordType === 'password' ? 'eye' : 'eye-open'
                    "
                  />
                </span>
              </el-form-item>
            </el-tooltip>
            <el-button
              :loading="loading"
              type="primary"
              style="width: 100%; margin-top: 50px"
              @click.native.prevent="handleLogin"
            >
              系统登录
            </el-button>
          </el-form>
        </div>
      </template>
      <div class="copy">上海杰燊科技有限公司</div>
    </div>
    <Wave width="100%" height="100%"></Wave>
  </div>
</template>

<script>
// eslint-disable-next-line no-unused-vars
import { mapUserActions, mapUserState } from "@/store/namespaced/user";

export default {
  name: "Login",
  components: {
    SocialCaptcha: () => import("./components/SocialCaptcha"),
    NeedChangePass: () => import("./components/NeedChangePass"),
    Wave: () => import("./components/Wave"),
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
      loginType: true,
      loginForm: {
        username: "",
        password: "",
        grant_type: "password",
        loginType: "syslogin",
        vaptchaToken: "",
      },
      loginRules: {
        username: [
          { required: true, trigger: "blur", validator: validateUsername },
        ],
        password: [
          { required: true, trigger: "blur", validator: validatePassword },
        ],
      },
      passwordType: "password",
      capsTooltip: false,
      loading: false,
      showDialog: false,
      redirect: undefined,
      otherQuery: {},
    };
  },
  computed: {
    ...mapUserState(["needSafetyValidation", "needChangePassword"]),
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
  created() {},
  mounted() {
    if (this.loginForm.username === "") {
      this.$refs.username.focus();
    } else if (this.loginForm.password === "") {
      this.$refs.password.focus();
    }
  },

  methods: {
    ...mapUserActions(["login", "setNeedSafetyValidation"]),
    captchaTicketSuccess(token) {
      this.loginForm.vaptchaToken = JSON.stringify(token);
      this.setNeedSafetyValidation(false);
      this.handleLogin(); // 重新登陆
    },
    cancelCaptcha() {
      this.loginForm.vaptchaToken = "";
      this.setNeedSafetyValidation(false);
    },
    checkCapslock({ shiftKey, key } = {}) {
      if (key && key.length === 1) {
        if (
          (shiftKey && key >= "a" && key <= "z") ||
          (!shiftKey && key >= "A" && key <= "Z")
        ) {
          this.capsTooltip = true;
        } else {
          this.capsTooltip = false;
        }
      }
      if (key === "CapsLock" && this.capsTooltip === true) {
        this.capsTooltip = false;
      }
    },
    showPwd() {
      if (this.passwordType === "password") {
        this.passwordType = "";
      } else {
        this.passwordType = "password";
      }
      this.$nextTick(() => {
        this.$refs.password.focus();
      });
    },
    handleLogin() {
      this.$refs.loginForm.validate(async (valid) => {
        if (valid) {
          this.loading = true;
          const lf = this.loginForm;
          const a = await this.login(lf);
          this.loading = false;
          if (a) {
            this.continueRedirect();
          }
        } else {
          console.log("error submit!!");
          return false;
        }
      });
    },
    continueRedirect() {
      window.location.replace(this.redirect || "/dashboard");
      // await this.$router
      //   .push({
      //     path: this.redirect || "/dashboard",
      //     query: this.otherQuery,
      //   })
      //   .catch((e) => {
      //     console.log(e);
      //   });
    },
    getOtherQuery(query) {
      return Object.keys(query).reduce((acc, cur) => {
        if (cur !== "redirect") {
          acc[cur] = query[cur];
        }
        return acc;
      }, {});
    },
  },
};
</script>

<style lang="scss" scoped>
$bg: #283443;
$light_gray: #ccc;
$cursor: #666;
@supports (-webkit-mask: none) and (not (cater-color: $cursor)) {
  .login-container .el-input input {
    color: $cursor;
  }
}

/* reset element-ui css */
.lgc {
  .el-form-item__error {
    margin-top: 5px;
  }
  .el-input {
    display: inline-block;
    border-radius: 0;
    border: none;
    width: 80%;
    margin-left: 10px;

    input {
      background: transparent;
      border: 0px;
      -webkit-appearance: none;
      border-radius: 0px;
      padding: 12px 5px 12px 15px;
      color: #333;
      height: 47px;

      &:-webkit-autofill {
        box-shadow: 0 0 0px 1000px #fff inset !important;
        -webkit-text-fill-color: #454545 !important;
      }
    }
  }
  button.el-button--primary {
    width: 100%;
    display: block;
    margin-top: 40px;
    box-shadow: 0 0 0.266667rem 0.026667rem rgba(0, 122, 255, 0.52);
    line-height: 1.2rem;
  }
  .el-form-item {
    background: #fff;
    border-radius: 5px;
    color: #454545;
  }
}
</style>

<style lang="scss" scoped>
$bg: #2d3a4b;
$dark_gray: #889aa4;
$light_gray: #eee;

.login-container {
  min-height: 100%;
  width: 100%;
  background: url("~@/assets/images/01.cfd426fb.jpg") #256af6 no-repeat;
  background-size: cover;
  overflow: hidden;
  position: relative;
  .account-login {
    position: absolute;
    left: 50%;
    top: 50%;
    z-index: 100;
    transform: translate(-50%, -50%);
    background: #fff;
    transition: all 0.3s;
    padding: 40px 50px;
    box-shadow: 0px 0 15px rgba(#333, 0.3);
    border-radius: 10px;
    .logo {
      padding-bottom: 40px;
    }
    .log-w {
      font-style: 36px;
      font-weight: normal;
      color: #666;
      line-height: 40px;
      padding: 0 20px;
      border-left: 1px solid #cfcfcf;
      font-weight: bold;
      margin-left: 20px;
    }
    .copy {
      text-align: center;
      padding-top: 20px;
      font-size: 12px;
      color: #888;
    }
  }

  .login-form {
    position: relative;
    width: 420px;
    max-width: 100%;
    margin: 0 auto;
    overflow: hidden;
  }

  .tips {
    font-size: 14px;
    margin-bottom: 10px;
    span {
      &:first-of-type {
        margin-right: 16px;
      }
    }
  }

  .svg-container {
    padding: 6px 5px 6px 15px;
    color: $dark_gray;
    vertical-align: middle;
    width: 30px;
    display: inline-block;
  }
  .show-pwd {
    position: absolute;
    right: 10px;
    top: 7px;
    font-size: 16px;
    color: $dark_gray;
    cursor: pointer;
    user-select: none;
  }

  .thirdparty-button {
    position: absolute;
    right: 0;
    bottom: 6px;
  }

  @media only screen and (max-width: 470px) {
    .thirdparty-button {
      display: none;
    }
  }
}
</style>
