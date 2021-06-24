<template>
  <div class="forget-reset">
    <template v-if="!security.securityKey">
      <el-form
        ref="resetform"
        :model="form"
        :rules="rules"
        class="login-form"
        :hide-required-asterisk="true"
        autocomplete="off"
        label-position="top"
      >
        <el-form-item prop="mobile" label="手机号">
          <el-input
            v-model="form.mobile"
            type="text"
            placeholder="请输入手机号码"
            name="mobile"
            tabindex="2"
            autocomplete="on"
            @keyup.enter.native="handleSumbmit"
          />
        </el-form-item>
        <el-form-item prop="code" label="验证码">
          <sms-code-input
            v-model="form.code"
            :sms-code-key="form.key"
            :mobile="form.mobile"
          />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" :loading="loading" @click="handleSumbmit"
            >下一步</el-button
          >
        </el-form-item>
        <el-form-item style="text-align: center;">
          <p class="cancel" @click="cancelSumbmit">取消操作</p>
        </el-form-item>
      </el-form>
    </template>
    <template v-else>
      <ResetPassword @restpassok="handleResetPassword" />

      <p class="cancel" @click="cancelSumbmit">取消操作</p>
    </template>
  </div>
</template>
<script>
import SmsCodeInput from "@/components/Messages/SmsCodeInput";
import ResetPassword from "./ResetPassword";
import { mapUserActions } from "@/store/namespaced/user";
import accountApi from "@/api/account";
export default {
  name: "ForgetResetPass",
  components: {
    SmsCodeInput,
    ResetPassword,
  },
  data() {
    return {
      loading: false,
      security: {
        id: "",
        securityKey: "",
      },
      form: {
        key: "forgetPasswordAndReset",
        code: "",
        mobile: "",
      },
      rules: {
        mobile: [
          { required: true, message: "手机号不能为空", trigger: "submit" },
        ],
        code: [
          { required: true, message: "验证码不能为空", trigger: "submit" },
        ],
      },
    };
  },
  methods: {
    ...mapUserActions(["logout"]),
    cancelSumbmit() {
      this.logout();
      this.security = {
        id: "",
        securityKey: "",
      };
      this.$emit("cancel", true);
    },
    async handleResetPassword(arg) {
      const r = await accountApi.reset_password({
        security: this.security,
        newPassword: arg.password,
      });
      console.log(r);
      if (r) {
        this.$succ("您已成功找回并重设密码，请使用手机号+密码进行登录");
        this.cancelSumbmit();
      }
    },
    handleSumbmit() {
      this.$refs.resetform.validate(async (valid) => {
        if (valid) {
          this.loading = true;
          const security = await accountApi.find_password({
            mobile: this.form.mobile,
            smsCode: {
              key: this.form.key,
              code: this.form.code,
            },
          });
          this.loading = false;
          console.log(security);
          if (security) {
            this.security = security;
          } else {
            this.security = {
              id: "",
              securityKey: "",
            };
          }
        } else {
          console.log("error submit!!");
          return false;
        }
      });
    },
  },
};
</script>
<style lang="scss" scoped>
.forget-reset {
  margin-top: 20px;
  p.cancel {
    color: red;
    cursor: pointer;
    margin: 0;
    padding: 0;
    text-align: center;
  }
}
</style>
