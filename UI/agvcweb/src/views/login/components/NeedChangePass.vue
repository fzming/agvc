<template>
  <div class="need-change-pwd">
    <el-alert
      title="为了您的账户安全，首次系统登录需更换默认密码"
      type="warning"
      :closable="false"
      :center="false"
    />
    <el-form
      ref="pwdform"
      :model="form"
      :rules="rules"
      class="login-form"
      :hide-required-asterisk="true"
      autocomplete="off"
      label-position="left"
    >
      <el-tooltip
        v-model="capsTooltip"
        content="大写模式已打开"
        placement="right"
        manual
      >
        <el-form-item prop="password1" label="新密码" label-position="top">
          <!-- <el-input ref="password" v-model="form.password1" :type="passwordType" placeholder="请输入新的密码" name="password"
            tabindex="2" autocomplete="on" @keyup.native="checkCapslock" @blur="capsTooltip = false"
            @keyup.enter.native="handleSumbmit" /> -->
          <PasswordStrengthMeter :pwd="form.password1" @changevlue="changevlue">
          </PasswordStrengthMeter>
        </el-form-item>
      </el-tooltip>
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
        <div style="text-align: center; padding-top: 20px">
          <el-button type="primary" @click="handleSumbmit"
            >更改密码并登录</el-button
          >
        </div>
      </el-form-item>
      <el-form-item style="text-align: center">
        <p class="cancel" @click="cancelSumbmit">取消操作</p>
      </el-form-item>
    </el-form>
  </div>
</template>
<script>
// import { getUserType } from "@/utils/auth";
import { mapAccountActions } from "@/store/namespaced/Account";
import { mapUserActions, mapUserState } from "@/store/namespaced/user";
import PasswordStrengthMeter from "@/components/PasswordStrengthMeter";
import router, { resetRouter } from "@/router";
import store from "@/store";
export default {
  components: { PasswordStrengthMeter },
  name: "NeedChangePass",
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
      Isallow: false,
    };
  },

  computed: {
    ...mapUserState(["id"]),
  },
  beforeDestroy() {
    console.log("beforeDestroy[this.reseted] ", this.reseted);

    if (!this.reseted) {
      this.cancelSumbmit();
    }
  },
  methods: {
    // getUserType,
    ...mapAccountActions(["accchange_password"]),
    ...mapUserActions(["change_password", "setNeedChangePassword", "logout"]),
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
    cancelSumbmit() {
      this.logout();
      this.setNeedChangePassword(false);
      this.$emit("setNeedChange", false);
    },
    handleSumbmit() {
      this.$refs.pwdform.validate(async (valid) => {
        if (valid) {
          this.loading = true;
          const userType = store.state.user.token.userType;
          const changepwd =
            userType === "Admin"
              ? this.change_password
              : this.accchange_password;
          if (!this.Isallow) {
            this.$message({
              type: "error",
              message: "密码不合规则!",
            });
            return;
          }
          const r = await changepwd({
            oldPwd: this.id,
            newPwd: this.form.password2,
          });
          this.loading = false;
          if (r) {
            this.$succ("密码重设成功");
            this.setNeedChangePassword(false);
            this.reseted = true;
            //重新获取路由
            const profile = await store.dispatch(
              userType === "Account" ? "user/getInfoOrg" : "user/getInfo"
            );
            const accessRoutes = await store.dispatch(
              "permission/generateRoutes",
              profile.roles
            );
            resetRouter();
            accessRoutes.forEach(function (value) {
              if (!value.children && !value.meta.noCache) {
                store.dispatch("tagsView/addCachedView", value);
              } else {
                value.children.forEach(function (l_value) {
                  if (!l_value.children && !l_value.meta.noCache) {
                    store.dispatch("tagsView/addCachedView", l_value);
                  }
                });
              }
            });
            // 路由动态装配
            router.addRoutes(accessRoutes);
            // 生成指令
            await store.dispatch("permission/generateCodes");
            this.$emit("restok");
            location.reload(true);
          }
        } else {
          console.log("error submit!!");
          return false;
        }
      });
    },
    changevlue(arg) {
      this.form.password1 = arg.pwd;
      this.Isallow = arg.Isallow;
    },
  },
};
</script>
<style lang="scss" scoped>
p.cancel {
  color: red;
  cursor: pointer;
  margin: 0;
  padding: 0;
  text-align: center;
}

.need-change-pwd {
  margin-top: 20px;

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
    color: #888;
    vertical-align: middle;
    width: 30px;
    display: inline-block;
  }

  .show-pwd {
    position: absolute;
    right: 10px;
    top: 7px;
    font-size: 16px;
    color: #888;
    cursor: pointer;
    user-select: none;
  }
}
</style>
