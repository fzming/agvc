<template>
  <div :id="id"></div>
</template>
<script>
import { loadJsAsync } from "@/utils";
export default {
  name: "WxOpenQrCode",
  props: {
    appid: {
      type: String,
      default: "wxe2428ed94de8efd3", //微信开放平台AppId
    },
    redirectUri: String,
    state: String,
    origin: {
      type: String,
      default: "api.yy5156.com", //callback源域名，否则消息不会被处理
    },
    id: {
      type: String,
      default: "login_QRCode",
    },
  },
  data() {
    return {
      qr: null,
    };
  },
  async mounted() {
    await loadJsAsync("wxLogin");
    this.qr = new window.WxLogin(this.qrParam);
    window.addEventListener("message", this.onMessage, false);
    //this.tryEmit();
  },
  beforeDestroy() {
    window.removeEventListener("message", this.onMessage, false);
  },
  computed: {
    qrParam() {
      return {
        self_redirect: true, //true：手机点击确认登录后可以在 iframe 内跳转到 redirect_uri，false：手机点击确认登录后可以在 top window 跳转到 redirect_uri。默认为 false。
        id: this.id,
        appid: this.appid,
        scope: "snsapi_login",
        redirect_uri: encodeURIComponent(this.redirectUri || ""),
        state: this.state,
        style: "black",
        href: "https://image.yy5156.com/core/wxQrStyle.css",
      };
    },
  },
  methods: {
    onMessage(event) {
      // console.log("收到" + event.origin + "消息：", event.data);
      if (event.origin.includes(this.origin)) {
        var data = event.data;
        //alert(typeof event.data);
        if (!data.WxQrCode) return;
        this.$emit("on-scan", {
          code: data.Code,
          state: data.State,
          identify: data.AppOpenIdentify,
          user: data.User,
        });
      }
    },
    // tryEmit() {
    //   var data = {
    //     Code: "021uAe0S1Xwoy21iy80S1To10S1uAe00",
    //     State: "123",
    //     AppOpenIdentify: {
    //       AppId: "wxe2428ed94de8efd3",
    //       OpenId: "o8qxnwiq71lD7aCX9ips4cIcvaxs",
    //       UnionId: "oxV-av74HfsSlP0eovNwcosX6uVc",
    //     },
    //     User: {
    //       nickName: "Teddy",
    //       avatarUrl:
    //         "http://thirdwx.qlogo.cn/mmopen/vi_32/NvHwW0vpazE4p6LKvMG7aViaCsIUVfhKCBEibRZ9qd39LOXyZKG69FwoRBICrN2zY2ibuQCBpSEAQZwWnGFFY9EGw/132",
    //       gender: null,
    //       city: "汕头",
    //       province: "广东",
    //       country: "中国",
    //       unionId: "oxV-av74HfsSlP0eovNwcosX6uVc",
    //     },
    //     WxQrCode: true,
    //   };
    //   this.$emit("on-scan", {
    //     code: data.Code,
    //     state: data.State,
    //     identify: data.AppOpenIdentify,
    //     user: data.User,
    //   });
    // },
  },
};
</script>
