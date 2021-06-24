<template>
  <div style="padding: 0px 20px; height: 100vh;">
    <el-tabs v-model="groupIndex" type="border-card">
      <el-tab-pane
        v-for="(g, gindex) in groups"
        :key="gindex"
        :label="g.name"
        :name="gindex + ''"
      >
        <span slot="label">
          {{ g.name }}
          <el-badge
            :hidden="navurdcountList[g.group] <= 0"
            :value="navurdcountList[g.group]"
            class="item"
          />
        </span>
        <div
          v-if="group.group === g.group"
          id="dody"
          ref="messageList"
          v-infinite-scroll="loadMore"
          :infinite-scroll-disabled="busy"
          :infinite-scroll-immediate="immediate"
          :infinite-scroll-distance="0"
          :style="{
            'min-height': '300px',
            height: group.postMsg
              ? 'calc(100vh - 330px)'
              : 'calc(100vh - 175px)',
            'overflow-y': 'auto'
          }"
        >
          <!-- min-height: 300px; height:calc(100vh - 330px); overflow-y: auto;  -->
          <div
            v-for="(item, index) in g.datas"
            :key="index"
            style="min-height:90px;height: auto;float: left;width: 100%;"
            :class="getImgStyle(item.isSelf)"
            @click="doRead(item.id)"
          >
            <div
              class="messImg"
              style="width:50px; border-radius: 25px; overflow: hidden;"
            >
              <img width="50" height="50" :src="item.avatar" />
            </div>
            <div style="float: right; width:calc(100% - 92px); ">
              <p class="nametm">
                <span class="name">{{ item.name }} #{{ index }}</span>
                <span class="time">{{ item.tm }}</span>
              </p>
              <p class="outContent">
                <span class="content">{{ item.content }}</span>
                <el-button
                  v-if="!item.isRead"
                  class="isRead"
                  style="color:red; padding: 0px; font-size: 14px;"
                  type="text"
                  >未读消息</el-button
                >

                <el-button
                  class="delBtn"
                  style="color:red; padding: 0px; font-size: 14px;"
                  type="text"
                  @click.stop="delMessage(item.id)"
                  >删除</el-button
                >
            
              </p>
            </div>
          </div>
        </div>
      </el-tab-pane>
    </el-tabs>
    <div v-if="group.postMsg" style="width: 100%; margin-top: 10px;">
      <div class="controls">
        <!-- :on-preview="handlePreview" -->
        <el-upload
          ref="upload"
          class="upload-demo"
          :multiple="false"
          :on-change="handleChange"
          accept="image/jpeg,image/png,application/pdf"
          :auto-upload="false"
          :limit="2"
          action=""
          :file-list="fileList"
        >
          <i
            class="el-icon-paperclip"
            style="font-size: 20px; vertical-align: middle; "
          />
          选择附件
        </el-upload>
      </div>
      <el-input
        v-model="messageContent"
        maxlength="100"
        :autosize="{ minRows: 3, maxRows: 6 }"
        show-word-limit
        type="textarea"
        placeholder="请输入消息内容"
      />
      <div style="margin-top: 15px; text-align:right">
        <el-button type="primary" @click="subMessage">{{
          group.buttonText
        }}</el-button>
      </div>
    </div>
  </div>
</template>
<script>
import { Message } from "element-ui";
import {
  mapDiscussMessageActions,
  mapDiscussMessageState,
  mapDiscussMessageGetters
} from "@/store/namespaced/discussMessage";
import { mapUserState } from "@/store/namespaced/user";
export default {
  name: "Messages",
  model: {
    prop: "itemList",
    event: "changeSelectids"
  },
  props: {
    messageList: {
      type: Array,
      default: function() {
        return [];
      }
    },
    pageCount: {
      type: Number,
      default: 1
    },
    pageIndex: {
      type: Number,
      default: 1
    }
  },
  data() {
    return {
      groupIndex: "0",
      fileList: [],
      fileTypes: [
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        "application/vnd.ms-excel",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "application/msword",
        "image/jpeg",
        "image/gif",
        "image/png",
        "application/pdf"
      ],
      canNextPage: false,
      isFirst: true,
      messageContent: "",
      messageFile: null,
      busy: false,
      immediate: false
    };
  },
  computed: {
    ...mapDiscussMessageState(["navurdcount", "navurdcountList"]),
    ...mapDiscussMessageGetters(["get_groups"]),
    ...mapUserState(["token", "userType"]),
    groups() {
      console.log(this.get_groups(this.userType));
      return this.get_groups(this.userType);
    },
    group() {
      return this.groups[parseInt(this.groupIndex)];
    },
    messageData() {
      return {
        content: this.messageContent,
        group: this.group.group,
        file: this.messageFile,
        isAnnounce: this.group.isAnncounce
      };
    },
    grousString() {
      const a = this.groups
        .map(x => {
          return x.group;
        })
        .join(",");
      return a;
    }
  },
  watch: {
    groupIndex() {
      this.canNextPage = false;
      if (this.group.isFirst) {
        this.showMessageLists();
      }
    }
  },
  mounted() {
    console.log("mounted");
  },
  activated() {
    console.log("activated");
  },
  deactivated() {
    console.log("deactivated");
  },
  created() {
    this.isFirst = false;
    this.showMessageLists();
  },
  methods: {
    ...mapDiscussMessageActions([
      "addMessageCard",
      "unreadcount",
      "modifyUrdCount",
      "nextPage",
      "readed",
      "send",
      "send_attachment",
      "readMessage",
      "deleteMessage",
      "deleteMessage"
    ]),
    getImgStyle(isSelf) {
      if (isSelf) {
        return "self";
      } else {
        return "noSelf";
      }
    },
    async subMessage() {
      if (this.messageData.content || this.messageData.file) {
        this.$emit("subMessage", {
          p: this.messageData
        });
        this.messageContent = "";
        this.messageFile = null;
        this.fileList = [];
      } else {
        Message.error("请勿发送空的消息");
      }
    },
    handleChange(file, fileList) {
      console.log(fileList);
      if (this.fileList.length > 0) {
        this.fileList = [];
        setTimeout(() => {
          this.fileList.push(file);
        }, 800);
      } else {
        this.fileList.push(file);
      }
      const isIMAGE = this.fileTypes.findIndex(v => v === file.raw.type) > -1;
      const isLt1M = file.raw.size / 1024 / 1024 < 2;
      console.log("isIMAGE", isIMAGE, "isLt1M", isLt1M);
      if (!isIMAGE) {
        Message.error("文件格式不正确,附件无法上传");
      } else if (!isLt1M) {
        Message.error("上传文件大小不能超过 2MB! 附件无法上传");
      } else {
        this.messageFile = file.raw;
      }
    },
    showMessageLists() {
      this.$emit("queryMessage", {
        p: this.group.group,
        pageIndex: this.group.pageIndex === 0 ? 1 : this.group.pageIndex,
        isAddList: false
      });
    },
    showMessageList(type) {
      this.$emit("queryMessage", {
        p: type,
        pageIndex: this.group.pageIndex === 0 ? 1 : this.group.pageIndex,
        isAddList: false
      });
    },

    loadMore() {
      if (this.canNextPage && this.pageCount > this.group.pageIndex) {
        this.canNextPage = false;
        this.nextPage(this.group.group);
        setTimeout(() => {
          this.$emit("queryMessage", {
            p: this.group.group,
            pageIndex: this.group.pageIndex,
            isAddList: true
          });
        }, 100);
      }
      this.canNextPage = true;
    },
    destroyed() {
      this.$refs.messageList.removeEventListener("scroll");
    },
    downLoadFile(url) {
      window.open(url, "_blank");
    },
    async doRead(id) {
      const a = await this.readed({
        id: id
      });
      if (a) {
        await this.readMessage({
          type: this.group.group,
          id: id
        });
        await this.unreadcount(this.grousString);
      }
    },
    async delMessage(id) {
      // console.log(id)
      const a = await this.$confirmAsync("提示", "是否确认删除该条消息");
      if (a) {
        await this.deleteMessage({
          type: this.group.group,
          id: id
        });
        await this.unreadcount(this.grousString);
      }
    }
  }
};
</script>
<style lang="scss" scoped>
.nametm {
  font-size: 14px;
  font-weight: 700;
  color: #303133;
  margin: 5px 0px;
  height: 19px;
}

.outContent {
  width: 100%;
  color: #606266;
  float: left;
  position: relative;
  min-height: 47px;
  font-size: 14px;
  white-space: normal;
}

.self {
  margin: 10px 0px;
}

.self .messImg {
  width: 50px;
  height: 50px;
  margin: 5px 5px 5px 20px;
  border-radius: 25px;
  float: right;
}

.self .name {
  color: #909399;
  float: right;
}

.self .time {
  color: #909399;
  margin-right: 10px;
  float: left;
  font-weight: 100 !important;
}

.self > p {
  color: #909399;
  margin-right: 10px;
  float: right;
}

.self .content {
  float: right;
  width: 390px;
  min-width: 100px;
  max-width: 390px;
  text-align: right;
  word-wrap: break-word;
  margin-bottom: 25px;
  display: block;
}

.self .isRead {
  position: absolute;
  left: 0px;
  bottom: 0px;
}

.self .delBtn {
  position: absolute;
  right: 0px;
  bottom: 0px;
}

.self .attact {
  position: absolute;
  left: 0px;
  margin-left: 54px;
  bottom: 0px;
}

.noSelf {
  margin: 10px 0px;
  border-bottom: 1px solid #eee;
}

.noSelf .messImg {
  width: 50px;
  height: 50px;
  margin: 5px 20px 5px 5px;
  border-radius: 25px;
  float: left;
}

.noSelf .name {
  color: #909399;
  float: left;
}

.noSelf .time {
  color: #909399;
  margin-right: 10px;
  float: right;
  font-weight: 100 !important;
}

.noSelf > p {
  color: #909399;
  margin-right: 10px;
  float: left;
}

.noSelf .content {
  float: left;
  width: 390px;
  min-width: 100px;
  max-width: 390px;
  text-align: left;

  margin-bottom: 25px;
  display: block;
}

.noSelf .isRead {
  position: absolute;
  right: 50px;
  bottom: 0px;
}

.noSelf .delBtn {
  position: absolute;
  right: 0px;
  bottom: 0px;
}

.noSelf .attact {
  position: absolute;
  right: 0px;
  margin-right: 54px;
  bottom: 0px;
}

.controls {
  margin-left: 5px;
  margin-bottom: 10px;
  font-size: 12px;
  padding: 4px 0;
  color: #1890ff;
}

.item {
  position: absolute;
  margin-left: -4px;
  top: -4px;
}
</style>
<style>
.el-upload-list {
  display: inline-block;
  position: absolute;
  margin-top: -2px !important;
  margin-left: 10px !important;
}

.el-upload-list li {
  margin-top: 0px !important;
}
</style>
