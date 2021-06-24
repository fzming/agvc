<template>
  <div class="app-container">
    <caption-container caption="密码修改" />
    <region>
      <el-form
        ref="formdata"
        :rules="rules"
        :model="formdata"
        label-width="80px"
      >
        <el-form-item label="旧密码" prop="oldPwd">
          <el-input
            v-model="formdata.oldPwd"
            type="password"
            placeholder="请输入旧密码"
            style="width: 230px;"
          />
        </el-form-item>
        <el-form-item label="新密码" prop="newPwd">
          <!-- <el-input v-model="formdata.newPwd" type="password" placeholder="请输入新密码" style="width: 230px" /> -->
          <PasswordStrengthMeter
            :pwd="formdata.newPwd"
            @changevlue="changevlue"
            style="width: 230px;"
          >
          </PasswordStrengthMeter>
        </el-form-item>
        <el-form-item label="确认密码" prop="repeatnewPwd">
          <el-input
            v-model="formdata.repeatnewPwd"
            type="password"
            placeholder="请确认密码"
            style="width: 230px;"
          />
        </el-form-item>

        <el-form-item>
          <el-button
            icon="el-icon-check"
            type="primary"
            @click="submitForm('formdata')"
            >确认</el-button
          >
          <el-button icon="el-icon-refresh" @click="Resetform('formdata')"
            >重置</el-button
          >
        </el-form-item>
      </el-form>
    </region>
  </div>
</template>

<script>
import { mapGetters } from "vuex";
import { mapUserActions, mapUserMutations } from "@/store/namespaced/user";
import { mapAccountActions } from "@/store/namespaced/Account";
import { NET_WORK_ERROR_URL } from "@/global/const";
import PasswordStrengthMeter from "@/components/PasswordStrengthMeter";
export default {
  components: { PasswordStrengthMeter },
  name: "ChangePassword",
  data() {
    const validate_password2 = (rule, value, callback) => {
      if (value === "") {
        callback(new Error("请再次输入密码"));
      } else if (value !== this.formdata.newPwd) {
        callback(new Error("两次输入密码不一致!"));
      } else {
        callback();
      }
    };

    const validate_password = (rule, value, callback) => {
      if (!value) {
        callback(new Error("请输入新密码"));
      } else if (value.toString().length < 6 || value.toString().length > 18) {
        callback(new Error("密码长度为6 - 18个字符"));
      } else {
        callback();
      }
    };

    return {
      usertype: 0, // 当前登录的用户 0 是系统用户 1 机构用户
      formdata: {
        oldPwd: "",
        newPwd: "",
        repeatnewPwd: "",
      },
      Isallow: false,
      rules: {
        oldPwd: [{ required: true, message: "请输入旧密码", trigger: "blur" }],
        newPwd: [
          { required: true, validator: validate_password, trigger: "blur" },
        ],
        repeatnewPwd: [
          { required: true, validator: validate_password2, trigger: "blur" },
        ],
      },
    };
  },
  computed: {
    ...mapGetters(["name", "avatar", "roles", "orgName"]),
  },
  created() {
    const a = this.$store.getters.token.userType; // 用户角色的等级
    if (a === "Admin") {
      // 系统用户
      this.usertype = 0;
    } else {
      // 机构用户
      this.usertype = 1;
    }
  },
  methods: {
    ...mapUserActions(["change_password"]),
    ...mapUserMutations(["SET_TOKEN", "SET_ROLES"]),
    ...mapAccountActions([
      "accchange_password",
      "logout",
      "setNeedChangePassword",
    ]),
    toLogin(url) {
      this.logout();
      this.SET_TOKEN({
        token: null,
        userType: null,
      });
      this.SET_ROLES([]);
      this.setNeedChangePassword(false);
      this.$router.replace(`${url}?redirect=${this.$route.fullPath}`);
    },
    // 修改密码提交
    async submitForm(formName) {
      this.$refs[formName].validate(async (valid) => {
        if (valid) {
          var d = { oldPwd: "", newPwd: "" };
          d.oldPwd = this.formdata.oldPwd;
          d.newPwd = this.formdata.newPwd;
          if (this.usertype === 0) {
            const success = await this.change_password(d);
            if (success) {
              // 修改成功
              this.$message({
                type: "success",
                message: "密码修改成，请重新登录!",
              });

              this.toLogin(NET_WORK_ERROR_URL[0]);
            } else {
              this.$message({
                type: "error",
                message: "密码修改失败，请检查旧密码",
              });
            }
          } else {
            const success = await this.accchange_password(d);
            if (success) {
              // 修改成功
              this.$message({
                type: "success",
                message: "密码修改成，请重新登录!",
              });
              this.toLogin(NET_WORK_ERROR_URL[1]);
            } else {
              this.$message({
                type: "error",
                message: "密码修改失败，请检查旧密码!",
              });
            }
          }
        } else {
          return false;
        }
      });
    },
    // 表单重置
    Resetform(formname) {
      this.$refs[formname].resetFields();
    },
    changevlue(arg) {
      this.formdata.newPwd = arg.pwd;
      this.Isallow = arg.Isallow;
    },
  },
};
</script>
