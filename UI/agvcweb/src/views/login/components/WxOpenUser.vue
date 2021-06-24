<template>
  <div v-if="user.avatarUrl" class="flex openuser">
    <img
      v-src="avatar"
      default-img="http://www.yy5156.com/assets/images/avatar.png"
      class="user-avatar"
    />
    <div class="flex1" style="padding-left: 10px;">
      <div class="user-nick" v-if="nick">{{ nick }}</div>
      <div class="user-info">{{tip}}</div>
    </div>
  </div>
</template>
<script>
export default {
  name: "WxOpenUser",
  props: {
    user: Object,
    tip:{
      type:String,
      default:"登录以后将自动绑定微信"
    }
  },
  computed: {
    avatar() {
      return this.user.avatarUrl || "";
    },
    nick() {
      return this.user.nickName || "";
    },
  },
  data() {
    return {};
  },
};
</script>
<style lang="scss" scoped>
.openuser {
  border: 1px solid #dddddd;
  padding: 10px;
  margin-bottom: 20px;
  text-align: left;
}
.user-avatar {
  max-width: 60px;
  border-radius: 50%;
}
.user-nick {
  font-size: 20px;
  padding-bottom: 5px;
}
.user-info {
  color: gray;
}
</style>
