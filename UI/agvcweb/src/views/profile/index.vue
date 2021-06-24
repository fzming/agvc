<template>
  <div class="app-container">
    <caption-container caption="个人中心">
      <el-button type="primary" icon="el-icon-edit" plain @click="handleEdit">资料编辑</el-button>
    </caption-container>
    <region>
      <el-form ref="form" :model="user" label-width="100px">
        <el-form-item label="头像：">
          <img v-if="user.avatar" :src="user.avatar" class="user-avatar" style="width: 46px;" />
          <img v-else src="http://www.yy5156.com/assets/images/avatar.png" class="user-avatar" style="width: 46px;" />
        </el-form-item>
        <el-divider />
        <el-form-item label="昵称：">
          {{ user.nick }}
        </el-form-item>
        <el-divider />
        <el-form-item label="邮箱：">
          {{ user.email }}
        </el-form-item>
        <el-divider />
        <el-form-item label="绑定电话：">
          {{ user.mobile | replacePhone }}
          <el-button type="primary" icon="el-icon-phone" plain style="margin-left: 20px;" @click="bdhandleEdit">
            {{!user.mobile ? "绑定手机" : "解除绑定" }}</el-button>
        </el-form-item>
        <el-divider v-if="isbdwx" />
        <el-form-item label="绑定微信：" v-if="isbdwx">
          <el-row>
            <el-col>
              <el-image v-if="user.appUserInfo" style="width: 46px; height: 46px" class="user-avatar"
                :src="user.appUserInfo.avatarUrl"></el-image>
            </el-col>
            <el-col style="line-height: 45px;">
              {{user.appUserInfo ? user.appUserInfo.nickName : "未绑定"}}
              <el-button type="text" plain icon="el-icon-close" @click="jbwxEide">微信解绑</el-button>
            </el-col>

          </el-row>
        </el-form-item>
        <el-divider />
        <el-form-item label="电话：">
          {{ user.contactPhone }}
        </el-form-item>
        <el-divider />
        <el-form-item label="性别：">
          {{ user.gender }}
        </el-form-item>
        <el-divider />
        <el-form-item label="个人介绍：">
          {{ user.introduction }}
        </el-form-item>
      </el-form>
    </region>

    <el-dialog :visible.sync="dialogVisible" title="个人资料编辑" :before-close="handleClose">
      <el-form ref="user" :model="user" label-width="100px" label-position="left">
        <el-form-item label="头像:">
          <el-upload ref="upload" class="avatar-uploader" :on-change="handleChange" :auto-upload="false"
            :show-file-list="false" action="">
            <img v-if="user.avatar" :src="user.avatar" class="avatar" />
            <i v-else class="el-icon-plus avatar-uploader-icon" />
          </el-upload>
        </el-form-item>
        <el-form-item label="昵称：">
          <el-input v-model="user.nick" placeholder="" style="width: 230px;" />
        </el-form-item>
        <el-form-item label="性别：">
          <el-radio-group v-model="user.gender">
            <el-radio label="男">男</el-radio>
            <el-radio label="女">女</el-radio>
          </el-radio-group>
        </el-form-item>
        <el-form-item label="联系电话：">
          <el-input v-model="user.contactPhone" placeholder="" style="width: 230px;" />
        </el-form-item>
        <el-form-item label="邮箱：">
          <el-input v-model="user.email" placeholder="" style="width: 230px;" />
        </el-form-item>
        <el-form-item label="个人介绍：">
          <el-input v-model="user.introduction" type="textarea" style="width: 330px;" />
        </el-form-item>
        <el-form-item>
          <el-button icon="el-icon-check" type="primary" @click="confirmUser('user')">确定更新</el-button>
          <el-button icon="el-icon-circle-close" type="danger" @click="handleClose">取消</el-button>
        </el-form-item>
      </el-form>
    </el-dialog>

    <el-dialog :title="bdtitlename" :visible.sync="bddialogVisible" width="460px" :destroy-on-close="true">
      <el-form v-if="Isbdview" ref="mobilecode" :model="mobilecode" label-width="80px">
        <el-form-item label="手机号">
          <el-input v-model="mobilecode.mobile" placeholder="输入手机号码" />
        </el-form-item>
        <el-form-item label="验证码">
          <sms-code-input v-model="mobilecode.smsCode.code" :sms-code-key="mobilecode.smsCode.key"
            :mobile="mobilecode.mobile" />
        </el-form-item>
        <el-form-item style="text-align: right;">
          <el-button type="primary" icon="el-icon-check" @click="bdonSubmit">确定绑定</el-button>
        </el-form-item>
      </el-form>
      <el-form v-else :model="mobilecode" label-width="80px" width="460px" :destroy-on-close="true">
        <el-form-item label="手机号:">
          {{ mobile | replacePhone }}
        </el-form-item>
        <el-form-item label="验证码:">
          <sms-code-input v-model="mobilecode.smsCode.code" :sms-code-key="mobilecode.smsCode.key" :mobile="mobile" />
        </el-form-item>
        <el-form-item label="" style="text-align: left;">
          <el-button type="primary" icon="el-icon-check" @click="jbonSubmit">确定解绑</el-button>
        </el-form-item>
      </el-form>
    </el-dialog>
  </div>
</template>

<script>
  import { mapGetters } from "vuex";
  import { mapUserActions } from "@/store/namespaced/user";
  import { mapAccountActions } from "@/store/namespaced/Account";
  import SmsCodeInput from "@/components/Messages/SmsCodeInput";
  import {
    WXAPPID
  } from "@/global/const";
  export default {
    name: "Profile",
    components: {
      SmsCodeInput,
    },

    // components: { UserCard, Activity, Timeline, Account },
    data() {
      return {
        user: {
          appUserIdentifys: [],
          appUserInfo: "",
          nick: "", // 昵称
          email: "", // 邮箱
          mobile: "", // 绑定手机号
          contactPhone: "", // 联系方式
          introduction: "", // 个人介绍
          gender: "", // 性别
          avatar: "", // 头像
        },
        activeTab: "activity",
        usertype: 0, // 当前登录的用户 0 是系统用户 1 机构用户
        dialogVisible: false,
        bddialogVisible: false,
        fileList: [],
        isupdate: false,
        fileTypes: ["image/jpeg", "image/gif", "image/png"],
        bdtitlename: "", //
        Isbdview: true,
        buttonName: "获取验证码",
        isDisabled: false,
        isstop: true,
        time: 60,
        mobilecode: {
          mobile: "",
          smsCode: {
            key: "",
            code: "",
          },
        },
      };
    },

    computed: {
      ...mapGetters(["name", "avatar", "roles", "orgName", "mobile"]),
      isbdwx() {
        if (this.usertype === 1) {
          if (this.user.appUserIdentifys.length > 0) {
            var res = '';
            this.user.appUserIdentifys.map((x) => {
              if (x.appId === WXAPPID) {
                res = x;
              }
            })
            if (res) return true; else return false;
          } else {
            return false;
          }
        } else {
          return false
        }
        // return false;
      }
    },
    async created() {
      const a = this.$store.getters.token.userType; // 用户角色的等级
      console.log(this.$store.getters.token.userType);
      if (a === "Admin") {
        // 系统用户
        this.usertype = 0;
      } else {
        // 机构用户
        this.usertype = 1;
      }
      await this.getProfileinfo();
    },
    methods: {
      ...mapUserActions([
        "update_profile",
        "sysprofile",
        "changeAvatar",
        "changemobile",
      ]),
      ...mapAccountActions([
        "accupdate_profile",
        "accprofile",
        "getfetchSmsSecurity",
        "bind_mobile",
        "unbind_mobile",
        "unbind_wx"
      ]),
      // 获取当前登录的基本资料
      async getProfileinfo() {
        if (this.usertype === 0) {
          const l = await this.sysprofile();
          console.log("1111", l);
          this.user.nick = l.nick;
          this.user.introduction = l.introduction;
          this.user.avatar = l.avatar;
          this.user.email = l.email;
          this.user.mobile = l.mobile;
          this.user.gender = l.gender;
          this.user.contactPhone = l.contactPhone;
        } else {
          const l = await this.accprofile();
          console.log("1111222", l);
          this.user.appUserIdentifys = l.appUserIdentifys;
          this.user.appUserInfo = l.appUserInfo;
          this.user.nick = l.nick;
          this.user.email = l.email;
          this.user.mobile = l.mobile;
          this.user.gender = l.gender;
          this.user.avatar = l.avatar;
          this.user.introduction = l.introduction;
          this.user.contactPhone = l.contactPhone;
        }
        if (this.mobile) {
          this.bdtitlename = "解除绑定";
          this.mobilecode.mobile = this.mobile;
          this.mobilecode.smsCode.key = "unbind_mobile";
          this.Isbdview = false;
        } else {
          this.bdtitlename = "绑定手机";
          this.mobilecode.smsCode.key = "bind_mobile";
          this.Isbdview = true;
        }
      },

      async uploadSectionFile(param) {
        // 自定义文件上传
        const fileObj = param;
        var l = await this.$store.dispatch("common/upload", {
          file: fileObj,
          option: {
            UpYun: true,
            AllowExtensions: ["png", "jpg", "jpeg", "gif", "bmp"].join(","),
            AllowMaxSize: 2,
            // 自动切图
            AllowMaxWidth: 100,
            AllowMaxHeight: 100,
          },
        });
        this.user.avatar = l;
      },

      async handleChange(file, fileList) {
        !!fileList;
        this.fileList = [];
        this.fileList = file.raw;
        if (this.onBeforeUpload(file)) {
          this.isupdate = true;
          this.user.avatar = URL.createObjectURL(file.raw);
        }
      },
      onBeforeUpload(file) {
        const isIMAGE = this.fileTypes.findIndex((v) => v === file.raw.type) > -1;
        const isLt1M = file.size / 1024 / 1024 < 2;
        if (!isIMAGE) {
          this.$message.error("上传文件只能是图片格式!");
        }
        if (!isLt1M) {
          this.$message.error("上传文件大小不能超过 2MB!");
        }
        return isIMAGE && isLt1M;
      },
      // 编辑个人资料
      async handleEdit() {
        this.dialogVisible = true;
      },
      // 提交个人资料
      async confirmUser(formName) {
        !!formName;
        if (this.isupdate) {
          await this.uploadSectionFile(this.fileList);
          this.changeAvatar(this.user.avatar); // 更新头像地址
        }
        this.isupdate = false;
        if (this.usertype === 0) {
          const l = await this.update_profile(this.user);
          if (l) {
            this.$message({
              type: "success",
              message: "修改成功!",
            });
            this.dialogVisible = false;
          }
        } else {
          const l = await this.accupdate_profile(this.user);
          if (l) {
            this.$message({
              type: "success",
              message: "修改成功!",
            });
            this.dialogVisible = false;
          }
        }
      },

      // 关闭弹框 右上角×
      handleClose() {
        this.dialogVisible = false;
      },

      //微信解绑
      async jbwxEide() {
        const z = await this.$confirmAsync("提示", "是否确定解除微信绑定？");
        if (z) {
          var a = await this.unbind_wx(WXAPPID);
          if (a) {
            this.$message.success("解绑成功!");
            await this.getProfileinfo();
          }
        }
      },
      // 绑定解绑手机
      async bdhandleEdit() {
        this.bddialogVisible = true;
      },
      // 绑定手机
      async bdonSubmit() {
        if (!this.mobilecode.mobile) {
          this.$message.error("请填入手机号!");
          return;
        }
        if (!this.mobilecode.smsCode.code) {
          this.$message.error("请填入短信验证码!");
          return;
        }
        const u = await this.bind_mobile(this.mobilecode);
        if (u) {
          this.$message.success("绑定成功!");
          this.mobilecode.smsCode.code = "";
          this.bddialogVisible = false;
          await this.changemobile(this.mobilecode.mobile);
          this.getProfileinfo();
        }
      },
      // 解绑手机
      async jbonSubmit() {
        if (!this.mobilecode.smsCode.code) {
          this.$message.error("请填入短信验证码!");
          return;
        }
        var data = {};
        data = this.mobilecode.smsCode;
        const u = await this.unbind_mobile(data);
        if (u) {
          this.$message.success("解绑成功!");
          this.mobilecode.smsCode.code = "";
          this.bddialogVisible = false;
          await this.changemobile("");
          this.getProfileinfo();
        }
      },
    },
  };
</script>

<style lang="scss" scoped>
  .user-avatar {
    cursor: pointer;
    width: 46px;
    height: 46px;
    border-radius: 50%;
    border: 1px solid $light-blue-fade;
    padding: 2px;

    &:hover {
      border: 1px solid $light-blue;
    }

    margin-right: 10px;
  }

  .avatar-uploader .el-upload {
    border: 1px dashed #d9d9d9;
    border-radius: 6px;
    cursor: pointer;
    position: relative;
    overflow: hidden;
  }

  .avatar-uploader .el-upload:hover {
    border-color: #409eff;
  }

  .avatar-uploader-icon {
    font-size: 28px;
    color: #8c939d;
    width: 80px;
    height: 80px;
    line-height: 80px;
    text-align: center;
  }

  .avatar {
    width: 80px;
    height: 80px;
    display: block;
  }

  .fontstyle {
    font-size: 14px;
  }
</style>