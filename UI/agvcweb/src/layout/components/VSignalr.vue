<template>
  <div style="display: none;" />
</template>

<script>
import $ from "jquery";
import "signalr";
import { WEBAPI_BASE_URL } from "@/global/const";
import { mapUserActions } from "@/store/namespaced/user";
export default {
  name: "VSignalr",
  props: {
    persistentConnectionName: {
      type: String,
      default: "sg",
    },
    clientId: {
      type: String,
      default: "",
    },
    autoreconnected: Boolean,
  },
  data() {
    return {
      connection: null,
      connectionState: $.signalR.connectionState.disconnected,
    };
  },
  computed: {},
  watch: {
    clientId(value) {
      if (!value) {
        this.disconnect();
        return;
      }
      // 变更了clientId 需要重新登录
      this.connect();
    },
    connectionState(value) {
      switch (value) {
        // 连接超时判断disconnectTimeout=30000，其中signalr会自动重连每5秒，超过10次后才会触发disconnected
        // 注意：服务端失去链接30秒后才会触发disconnected
        case $.signalR.connectionState.disconnected: {
          const noretry = !this.clientId || !this.autoreconnected;
          console.warn("disconnected,retry connect:" + !noretry);
          if (noretry) return;
          setTimeout(() => {
            this.connect();
          }, 3000);
          break;
        }
      }
    },
  },
  created() {
    window.addEventListener("beforeunload", () => {
      this.disconnect();
    });
  },
  mounted() {
    if ($.signalR && this.clientId) {
      this.connect();
    }
    //0: 'connecting', 1: 'connected', 2: 'reconnecting', 4: 'disconnected'
    // https://github.com/SignalR/SignalR/wiki/SignalR-JS-Client
    // 连接状态
    /*
                      Connected	The connection state is connected.
                      Connecting	The connection state is connecting.
                      Disconnected	The connection state is disconnected.
                      Reconnecting	The connection state is reconnecting.
                      */
  },
  beforeDestroy() {
    this.disconnect();
  },
  methods: {
    ...mapUserActions(["setConnectionId"]),
    connect() {
      if (!this.clientId) return;
      if (
        this.connection &&
        this.connectionState === $.signalR.connectionState.connected
      )
        return; // connected
      const url = `${WEBAPI_BASE_URL}/${this.persistentConnectionName}`;
      console.log(url);
      this.connection = $.connection(url, {
        qs: `client_id=${this.clientId}&client_no_kickoff=true&source=wms`,
      });

      this.connection.received((data) => {
        // console.log(data);
        this.$emit("on-received", data);
      });
      this.connection.stateChanged((state) => {
        var stateConversion = {
          0: "connecting",
          1: "connected",
          2: "reconnecting",
          4: "disconnected",
        };
        console.log(
          "SignalR state changed from: " +
            stateConversion[state.oldState] +
            " to: " +
            stateConversion[state.newState]
        );
        this.connectionState = state.newState;
      });
      const that = this;
      this.connection
        .start()
        .done((data) => {
          console.log(data);
          that.setConnectionId(data.id);
        })
        .fail((e) => {
          console.log(e);
        });
    },
    send(toClientId, jsonData) {
      const message = Object.assign({}, jsonData, {
        fromId: this.clientId,
        toId: toClientId,
      });
      return this.connection.send(message);
    },
    disconnect() {
      if (this.connection) {
        this.connection.stop();
      }
    },
  },
};
</script>
