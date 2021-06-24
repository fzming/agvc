<template>
  <div class="navbar flex">
    <Hamburger
      id="hamburger-container"
      :is-active="sidebar.opened"
      class="hamburger-container"
      @toggleClick="toggleSideBar"
    />
    <div class="flex1">
      <Breadcrumb id="breadcrumb-container" class="breadcrumb-container" />
    </div>

    <div class="right-menu flex">
      <div v-if="modelTypes.length > 1" class="moduls flex">
        <a
          v-for="(item, index) in modelTypes"
          :key="item.value"
          :class="isCurrent(item)"
          @click="modifyCurrentModule(index)"
        >
          <i :class="item.icon" />
          {{ item.label }}
        </a>
      </div>
      <div class="icos flex">
        <span class="ic">
          <el-tooltip
            :class="addclass()"
            :content="bdname"
            placement="top"
            effect="light"
          >
            <router-link to="/profile/index">
              <i class="el-icon-mobile" />
            </router-link>
          </el-tooltip>
        </span>
        <span v-if="connectionAgents.length > 1" class="ic" @click="showAgents">
          <el-badge :hidden="agentNum <= 1" :value="agentNum" class="item">
            <i class="el-icon-monitor" />
          </el-badge>
        </span>
        <span class="ic" @click="showMessageBox">
          <el-badge
            :hidden="navurdcount <= 0"
            :value="navurdcount"
            class="item"
          >
            <i class="el-icon-message" />
          </el-badge>
        </span>
      </div>
      <el-dropdown
        class="avatar-container right-menu-item hover-effect"
        trigger="click"
      >
        <div class="avatar-wrapper flex">
          <img
            v-src="avatar"
            default-img="http://www.yy5156.com/assets/images/avatar.png"
            class="user-avatar"
          />
          <section class="right-menu-item profile hover-effect">
            <div class="nick">
              {{ name }}
              <span class="role" v-if="role">{{ role }}</span>
            </div>
            <div class="orgName">{{ orgName }}</div>
          </section>
          <i class="el-icon-caret-bottom" />
        </div>

        <el-dropdown-menu slot="dropdown">
          <router-link to="/profile/index">
            <el-dropdown-item>个人中心</el-dropdown-item>
          </router-link>
          <router-link to="/profile/changePassword">
            <el-dropdown-item>密码修改</el-dropdown-item>
          </router-link>
          <router-link to="/">
            <el-dropdown-item>首页</el-dropdown-item>
          </router-link>

          <el-dropdown-item divided>
            <span style="display: block" @click="logout(isDialog)"
              >退出登录</span
            >
          </el-dropdown-item>
        </el-dropdown-menu>
      </el-dropdown>
      <Screenfull
        v-if="device !== 'mobile'"
        id="screenfull"
        class="right-menu-item scc hover-effect"
      />
    </div>
    <el-drawer
      title="收件箱"
      :visible.sync="dialogTableVisible"
      :direction="direction"
      :append-to-body="true"
      :before-close="handleClose"
    >
      <SysMessage
        ref="msglist"
        :message-list="messagelist"
        :page-count="pageCount"
        :page-index="queryData.pageIndex"
        @subMessage="subMessage"
        @queryMessage="queryMessage"
      />
    </el-drawer>
    <el-drawer
      title="登录设备列表"
      :visible.sync="dialogAgentVisible"
      :direction="direction"
      size="300"
      :append-to-body="true"
    >
      <ConnectionAgents :agents="connectionAgents" />
    </el-drawer>

  </div>
</template>

<script>
import { mapGetters } from "vuex";
import { mapUserState, mapUserActions } from "@/store/namespaced/user";
import { mapTagsViewGetters } from "@/store/namespaced/kernel/tagsView";
import {
  mapDiscussMessageActions,
  mapDiscussMessageState,
  mapDiscussMessageGetters,
} from "@/store/namespaced/discussMessage";
import Breadcrumb from "@/components/Breadcrumb";
import Hamburger from "@/components/Hamburger";
import Screenfull from "@/components/Screenfull";
import SysMessage from "@/components/Messages";
import ConnectionAgents from "@/components/Messages/ConnectionAgents";
import { $subscribe } from "@/global/subscribe";
import { NET_WORK_ERROR_URL, MODULE_NAME_TYPE } from "@/global/const";
import { formatTime } from "@/utils";
import store from "@/store";
// import { getToken } from "@/utils/auth";
export default {
  name: "Navbar",
  components: {
    Breadcrumb,
    Hamburger,
    Screenfull,
    SysMessage,
    ConnectionAgents,
  },
  data() {
    return {
      isDialog: true,
      first: true,
      direction: "rtl",
      dialogTableVisible: false,
      dialogAgentVisible: false,
      messageList: [],
      isSysMessage: true,
      messaageData: {
        content: "",
        group: "groupMessage",
        file: null,
        isAnnounce: false,
      },
      pageCount: 10,
      queryData: {
        userId: "",
        group: "groupMessage",
        onlyNew: false,
        pageIndex: 1,
        pageSize: 20,
      },
      urd: 0,
      bdname: "",
      nodifyData: null,
      comd: {
        modify: "zl-58d91bbf00944b82be87cf588fadb506",
      },
      querydata: {
        tsTitle: "", //对方账户名
        range: {
          // begin: (new Date()).getMonth - 1,
          // end: new Date(),
          begin: "",
          end: "",
          rangeType: 1,
        },
        invoiceId: "", //我司账户ID
        // pageIndex: 1,
        // pageSize: 8
      },
    };
  },
  computed: {
    ...mapGetters([
      "sidebar",
      "avatar",
      "device",
      "name",
      "roles",
      "orgId",
      "orgName",
      "mobile",
      "id",
      // "visitedViews",
      // "currentPath",
    ]),
    ...mapTagsViewGetters(["visitedViews", "currentPath"]),
    ...mapDiscussMessageState(["navurdcount", "messagelist"]),
    ...mapUserState(["token", "connectionAgents", "currentModules"]),
    ...mapDiscussMessageGetters(["get_groups"]),
    features() {
      return this.$store.getters.features;
    },
    agentNum() {
      return this.connectionAgents.length;
    },
    groups() {
      const a = this.$store.getters.token.userType;
      // console.log("this.userType", a);
      return this.get_groups(a);
    },
    grousString() {
      const a = this.groups
        .map((x) => {
          return x.group;
        })
        .join(",");
      return a;
    },
    role() {
      if (this.roles.length) return this.roles[0].name;
      return "";
    },
    modelTypes() {
      return MODULE_NAME_TYPE.filter((t) => {
        return this.isHasModus(t);
      }).map((x) => {
        return { value: x.value, label: x.name, icon: x.icon };
      });
    },
  },
  watch: {

  },

  mounted() {
    $subscribe({
      DiscussMessageBroadCast: async (arg) => {
        const x = arg.message;
        await this.getUnRead();
        x.sender.avatar = x.sender.avatar
          ? x.sender.avatar
          : "http://www.yy5156.com/assets/images/avatar.png";
        this.nodifyData = this.$notify({
          title: arg.title,
          duration: 15000,
          onClick: () => {
            this.dialogTableVisible = true;
            this.nodifyData.close();
          },
          dangerouslyUseHTMLString: true,
          message: `<div class="flex-start" style='line-height:30px; width:auto;'>
           <img src='${
             x.sender.avatar
           }' width='30' height='30' style='margin:10px; border-radius:30px;'/>${
            x.sender.name
          }
           <span style='color:#888;margin-left:10px'>${formatTime(
             new Date(x._c).valueOf()
           )}</span>
          </div> <div style='color:gray; text-align:left;'>${x.content}</div>
          <div style='color:#888'>(点击查看消息)</div>
          `,
        });
        this.createCard(x);
      },
      AgentsChangeMessage: (arg) => {
        this.setConnectionAgents(arg.connectionAgents);
      },
      // 被服务器踢出
      KickOutMessage: (arg) => {
        this.$alert(arg.message, arg._message_title || "强制下线", {
          confirmButtonText: "确定",
          callback: () => {
            location.reload(true);
          },
        });
        this.logout(false, true);
      },
    });
  },
  async created() {
    const end = new Date();
    const start = new Date();
    // 近俩个月
    start.setMonth(start.getMonth() - 1);
    this.querydata.range.begin = start;
    this.querydata.range.end = end;

    await this.getUnRead();
    const a = store.state.user.currentModules;
    //console.log("当前模块:", a.v * 1);
    this.modifyCurrentModule(a * 1);
  },
  methods: {
    ...mapDiscussMessageActions([
      "unreadcount",
      "query",
      "saveUnreadCount",
      "nextPage",
      "addMessageCard",
      "send_attachment",
      "send",
      "ClearMessageList_Async",
    ]),
    ...mapUserActions(["setConnectionAgents", "setCurrentModule"]),
    // async clearbytypename(data) {
    //   await this.clearbytypename(data);
    // },
    async getUnRead() {
      await this.unreadcount(this.grousString);
    },
    toggleSideBar() {
      this.$store.dispatch("app/toggleSideBar");
    },
    async logout(isDialog, noReload) {
      if (isDialog) {
        var c = await this.$confirmAsync("退出确认", "是否确认退出系统?");
        if (c) {
          const a = this.$store.getters.token.userType;
          await this.ClearMessageList_Async();
          this.$store.dispatch("user/logout");
          this.$router.push(
            `${NET_WORK_ERROR_URL[a === "Admin" ? 0 : 1]}?redirect=${
              this.$route.fullPath
            }`
          );
        } else {
          return;
        }
      } else {
        const a = this.$store.getters.token.userType;
        await this.ClearMessageList_Async();
        this.$store.dispatch("user/logout");

        this.$router.push(
          `${NET_WORK_ERROR_URL[a === "Admin" ? 0 : 1]}?redirect=${
            this.$route.fullPath
          }`
        );
      }
      if (!noReload) {
        setTimeout(function () {
          location.reload();
        }, 2000);
      }
    },
    async showMessageBox() {
      this.dialogTableVisible = true;
      // setTimeout(() => {
      //   this.$refs.msglist.showMessageLists()
      // }, 100);
    },
    async showBankBill() {
      this.openBillsList();
    },
    showAgents() {
      this.dialogAgentVisible = true;
    },
    async queryMessage(arg) {
      //console.log("arg:", arg);
      this.queryData.group = arg.p;
      this.queryData.pageIndex = arg.pageIndex;
      this.queryData.isAddList = !!arg.isAddList;
      const a = await this.query(this.queryData);
      this.pageCount = a.pageCount;
    },
    async createCard(x) {
      // console.log(
      //   x.group,
      //   this.groups.find(z => z.group === x.group),
      //   this.grousString
      // );
      const d = this.groups.find((z) => z.group === x.group);
      if (x.group === d.group) {
        const a = {
          id: x.id,
          avatar: x.sender.avatar
            ? x.sender.avatar
            : "http://www.yy5156.com/assets/images/avatar.png",
          name: x.sender.name ? x.sender.name : "未设置名称",
          content: x.content,
          attachment: x.attachment,
          tm: x._c,
          isSelf: false,
          isRead: x.isRead,
        };
        this.addMessageCard({ type: d, data: a });
      }
    },
    async subMessage(arg) {
      const p = arg.p;
      const a = p.file ? await this.send_attachment(p) : await this.send(p);
      if (a.id || a.status) {
        this.messaageData.content = "";
        this.messaageData.file = null;
        await this.createCard(a);
      }
    },
    handleClose() {
      this.dialogTableVisible = false;
      this.first = false;
    },
    handleBankBill() {
      this.closeBillsList();
    },
    addclass() {
      if (this.mobile) {
        this.bdname = "已经绑定手机";
        return "item";
      } else {
        this.bdname = "未绑定手机";
        return "item icostyle";
      }
    },
    isHasModus(item) {
      const modules = this.$store.getters.modules;
      return modules.indexOf(item.value) > -1;
    },
    isCurrent(item) {
      const module = this.$store.getters.module;
      return item.value === module ? "active" : "";
    },
    modifyCurrentModule(k) {
      //console.log("修改当前模块");
      this.setCurrentModule({ idx: k });
      const a = this.visitedViews;
      const b = this.$route.path;
      if (a.indexOf((x) => x.path === b) > -1) {
        this.redirectPath(this.currentPath);
      } else {
        this.redirectPath(this.currentPath || a[0].path);
      }
    },
    redirectPath(path) {
      const b = this.$route.fullPath;
      //console.log("redirectPath", b, path);
      if (b === path || !path) return;
      this.$router.replace(path);
    },
  },
};
</script>

<style lang="scss" scoped>
.icostyle {
  color: #999999;
}

.navbar {
  height: 60px;

  .hamburger-container {
    cursor: pointer;
    transition: all 0.3s;
    position: relative;
    top: -2px;

    &:hover {
      color: $light-blue !important;
    }
  }

  .right-menu {
    margin-top: 20px;

    &:focus {
      outline: none;
    }

    .right-menu-item {
      color: gray;

      margin-right: 20px;
    }

    .icos {
      margin-right: 25px;
      color: $light-blue;

      span.ic {
        padding: 0 20px;
        cursor: pointer;
        //border-right: 1px solid #e5e5e5;
      }

      i {
        font-size: 30px;
        transition: all 0.3s;

        &:hover {
          font-weight: bold;
          color: $primary-color;
        }
      }
    }

    .profile {
      font-size: 16px;
      color: $primary-color;
      font-weight: bold;

      .nick {
        line-height: 23px;

        span.role {
          font-size: 12px;
          color: $green;
          display: inline-block;
          margin-left: 10px;
          font-weight: normal;
          border: 1px solid $green;
          padding: 0px 5px;
          border-radius: 5px;
        }
      }

      .orgName {
        margin-top: 4px;
        font-weight: normal;
        font-size: 13px;
        color: #666;
      }
    }

    .orgmobile {
      font-weight: normal;
      font-size: 14px;
      color: #999999;
    }

    .scc {
      cursor: pointer;

      &:hover {
        color: $light-blue;
      }
    }

    .avatar-container {
      margin-right: 30px;

      .avatar-wrapper {
        cursor: pointer;

        .user-avatar {
          cursor: pointer;
          width: 46px;
          height: 46px;
          border-radius: 50%;
          border: 1px solid #cccccc;
          padding: 2px;

          &:hover {
            border: 1px solid $primary-color;
          }

          margin-right: 10px;
        }

        .el-icon-caret-bottom {
          cursor: pointer;
          position: absolute;
          right: -5px;
          bottom: 5px;
          font-size: 12px;
        }
      }
    }
  }

  .moduls {
    a {
      font-size: 20px;
      margin-right: 10px;
      display: inline-block;
      padding: 2px 8px;
      border-radius: 7px;
      color: #666;

      &.active {
        background: $light-blue;
        color: white;

        &:hover {
          color: white;
        }
      }

      &:hover {
        color: $light-blue;
      }
    }
  }
}
</style>
