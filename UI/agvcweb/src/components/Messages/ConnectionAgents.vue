<template>
  <div id="connagents">
    <div class="des">
      当前展示以相同账号登录的设备列表，您可以进行对设备进行管理。
    </div>
    <el-popover
      ref="popover"
      placement="top-start"
      title="解除登录设备"
      width="200"
      trigger="hover"
      content="解除当前使用您的账号登录的其他设备"
    />
    <el-dialog
      title="解除设备登录"
      :visible.sync="dialogFormVisible"
      width="420px"
      :append-to-body="true"
      :destroy-on-close="true"
    >
      <el-form label-width="100px" label-suffix="：">
        <el-form-item label="手机号">
          {{ mobile }}
        </el-form-item>
        <el-form-item label="验证码">
          <SmsCodeInput
            v-model="smsCode.code"
            :sms-code-key="smsCode.key"
            :mobile="mobile"
          />
        </el-form-item>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click="dialogFormVisible = false">取 消</el-button>
        <el-button type="primary" @click="handleKickOffDevice">确 定</el-button>
      </div>
    </el-dialog>
    <div class="agents">
      <div v-for="ag in agents" :key="ag.connectionId" class="flex-start agent">
        <div class="ico">
          <i class="el-icon-monitor" />
        </div>
        <div class="flex-1">
          <div style="width: 250px;">
            <div class="name">
              {{ ag.agent }}
              <el-tag
                v-if="ag.connectionId == connectionId"
                type="success"
                size="mini"
                effect="dark"
              >
                当前登录设备
              </el-tag>
            </div>
            <div class="gray">登录地点：{{ ag.ipAddr }}</div>
            <div class="gray">登录时间：{{ ag.time }}</div>
          </div>
        </div>
        <div v-if="ag.connectionId != connectionId" class="but">
          <el-button
            v-if="!kickkingConnections.includes(ag.connectionId)"
            v-popover:popover
            type="danger"
            icon="el-icon-delete"
            circle
            @click="handleRemove(ag.connectionId)"
          />
          <el-button v-else v-loading="true" icon="el-icon-delete" circle />
        </div>
      </div>
    </div>
  </div>
</template>
<script>
import { mapUserState, mapUserActions } from "@/store/namespaced/user";
import SmsCodeInput from "@/components/Messages/SmsCodeInput";
export default {
  name: "ConnectionAgents",
  components: {
    SmsCodeInput,
  },
  props: {
    agents: {
      type: Array,
      default: () => [],
    },
  },
  data() {
    return {
      forDeleteConnectionId: "",
      kickkingConnections: [], // 正在踢出的ID
      dialogFormVisible: false,
      smsCode: {
        key: "ConnectionAgents",
        code: "",
      },
    };
  },
  computed: {
    ...mapUserState(["connectionId", "mobile"]),
  },
  methods: {
    ...mapUserActions(["kickOffDeviceAsync"]),
    handleRemove(connectionId) {
      this.dialogFormVisible = true;
      this.forDeleteConnectionId = connectionId;
    },
    async handleKickOffDevice() {
      if (this.smsCode.code.length < 4) {
        this.$alert("请输入验证码", "系统提示");
        return;
      }
      var ret = await this.kickOffDeviceAsync({
        connectionId: this.forDeleteConnectionId,
        smsCode: this.smsCode,
      });
      if (ret) {
        this.kickkingConnections.push(this.forDeleteConnectionId);
      }
      this.dialogFormVisible = false;
    },
  },
};
</script>
<style lang="scss" scoped>
#connagents {
  margin: 0 20px;
  .des {
    font-size: 14px;
    color: gray;
  }
  .agents {
    margin: 16px;
    border: 1px solid #eee;
  }
  .agent {
    padding: 20px 16px;
    border-bottom: 1px solid #eee;
    align-items: center;
    &:hover {
      background: #eee;
    }
    &:last-child {
      border-bottom: none;
    }
    .ico {
      font-size: 38px;
      margin-right: 16px;
      color: $primary-color;
    }
    .but {
      margin-left: 16px;
    }
    .name {
      font-size: 14px;
    }
    .gray {
      font-size: 12px;
      padding-top: 4px;
      color: #999;
    }
  }
}
</style>
