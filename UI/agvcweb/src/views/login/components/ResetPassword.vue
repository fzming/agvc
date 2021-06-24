<template>
  <div class="reset-password" style="margin-top: 20px;">
    <el-form
      ref="pwdform"
      :model="form"
      :rules="rules"
      class="login-form"
      :hide-required-asterisk="true"
      autocomplete="off"
      label-position="left"
    >
      <el-form-item prop="password1" label="新密码" label-position="top">
        <el-input
          ref="password"
          v-model="form.password1"
          :type="passwordType"
          placeholder="请输入新的密码"
          name="password"
          tabindex="2"
          autocomplete="on"
          @keyup.enter.native="handleSumbmit"
        />
      </el-form-item>
      <el-form-item prop="password2" label="确认密码" label-position="top">
        <el-input
          ref="password"
          v-model="form.password2"
          :type="passwordType"
          placeholder="请再次输入一次新密码"
          name="password"
          tabindex="2"
          autocomplete="on"
          @keyup.native="checkCapslock"
          @blur="capsTooltip = false"
          @keyup.enter.native="handleSumbmit"
        />
      </el-form-item>
      <el-form-item>
        <el-button type="primary" :loading="loading" @click="handleSumbmit"
          >确认重设密码</el-button
        >
      </el-form-item>
    </el-form>
  </div>
</template>
<script>
export default {
  name: "ResetPassword",
  data() {
    const validatePassword = (rule, value, callback) => {
      if (value.length < 6) {
        callback(new Error("密码不能小于6位数"));
      } else {
        callback();
      }
    };
    const validatePassword2 = (rule, value, callback) => {
      if (this.form.password1 !== value) {
        callback(new Error("两次密码输入不一致"));
      } else {
        callback();
      }
    };
    return {
      loading: false,
      reseted: false,
      form: {
        password1: "",
        password2: "",
      },
      rules: {
        password1: [
          { required: true, trigger: "submit", validator: validatePassword },
        ],
        password2: [
          { required: true, trigger: "submit", validator: validatePassword },
          { required: true, trigger: "submit", validator: validatePassword2 },
        ],
      },
      capsTooltip: false,
      passwordType: "password",
    };
  },
  methods: {
    handleSumbmit() {
      this.$refs.pwdform.validate(async (valid) => {
        if (valid) {
          this.$emit("restpassok", { password: this.form.password2 });
        } else {
          console.log("error submit!!");
          return false;
        }
      });
    },
  },
};
</script>
