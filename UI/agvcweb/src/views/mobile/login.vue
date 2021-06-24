<template>
  <div>
    <van-nav-bar title="用户登录" />
    <section class="wrapper">
      <van-form @submit="onSubmit">
        <van-field
          v-model="username"
          name="用户名"
          label="用户名"
          placeholder="用户名"
          :rules="[{ required: true, message: '请填写用户名' }]"
        />
        <van-field
          v-model="password"
          type="password"
          name="密码"
          label="密码"
          placeholder="密码"
          :rules="[{ required: true, message: '请填写密码' }]"
        />
        <div style="margin: 16px">
          <van-button round block type="info" native-type="submit"
            >确认登录</van-button
          >
        </div>
      </van-form>
    </section>
  </div>
</template>
<script>
import { mapUserActions } from "@/store/namespaced/user";
export default {
  name: "mobileLogin",
  data() {
    return {
      username: "",
      password: "",
      logining:false
    };
  },
  methods: {
     ...mapUserActions([
      "orglogin"
    ]),
    async onSubmit(values) {
      this.logining = true;
         let loginData= {
        domain: "wmhc",
        username: this.username,
        password: this.password,
        grant_type: "password",
        loginType: "pwdlogin",
      }
       const success = await this.orglogin(loginData);
          // console.log("success", success);
          this.logining = false;
          if (success) {
            this.continueRedirect();
            // console.log("2222", this.needChangePassword);
          }
          
    },
     continueRedirect() {
      this.$router
        .push({ path: "/m/index" })
        .then(() => {})
        .catch(() => {});
    },
  },
};
</script>
