<template>
  <div class="app-container">
    <caption-container :caption="titlename" />
    <region>
      <el-form v-if="isbdview" label-width="80px">
        <el-form-item label="手机号">
          <el-input placeholder="输入手机号码" style="width: 430px">
            <el-button slot="append" :disabled="isDisabled" @click="GetVerification()">{{ buttonName }}</el-button>
          </el-input>
        </el-form-item>
        <el-form-item label="验证码">
          <el-input
            placeholder="请输验证码"
            clearable
            style="width: 230px"
          />
        </el-form-item>
      </el-form>
      <el-form v-else>
        <el-form-item label="手机号:">
          {{ mobile |replacePhone }}
        </el-form-item>
        <el-form-item label="验证码:">
          <el-input placeholder="请输入验证码" style="width: 430px">
            <el-button slot="append" :disabled="isDisabled" @click="GetVerification()">{{ buttonName }}</el-button>
          </el-input>
        </el-form-item>
      </el-form>
    </region>
  </div>
</template>

<script>
import { mapAccountActions } from '@/store/namespaced/Account'
import { mapGetters } from 'vuex'
export default {
  name: 'BindingMoblie',
  data() {
    return {
      titlename: '', // 标题名称
      isbdview: true, // 是否是绑定页面
      buttonName: '发送短信',
      isDisabled: false,
      time: 60
    }
  },
  computed: {
    ...mapGetters([
      'mobile'
    ])

  },
  created() {
    if (this.mobile) {
      this.titlename = '解除绑定'
      this.isbdview = false
    } else {
      this.titlename = '绑定手机'
      this.isbdview = true
    }
  },
  methods: {
    ...mapAccountActions(['getfetchSmsSecurity']),
    // 获取短信验证码
    async  GetVerification() {
      const me = this;
      me.isDisabled = true;
      const interval = window.setInterval(function() {
        me.buttonName = '（' + me.time + '秒）后重新发送';
        --me.time;
        if (me.time < 0) {
          me.buttonName = '重新发送';
          me.time = 60;
          me.isDisabled = false;
          window.clearInterval(interval);
        }
      }, 1000);

      const data = { mobile: '', key: '', securityKey: '' }
      data.mobile = '18862385601'
      data.key = 'fetchSmsSecurity'

      var md = Math.floor((Math.random() * 32));
      console.log('验证码', md)
      var d = this.getSecurityKey(md);
      data.securityKey = d
    //   var a = await this.getfetchSmsSecurity(data)
    //   console.log('返回', a)
    },
    // 生产fetchSmsSecurity

    getSecurityKey(length) {
      let i = 0;
      const words = [];
      const timestamp = new Date().valueOf() + ''; // 毫秒级时间戳
      console.log(timestamp);

      if (!length || length > timestamp.length) {
        length = timestamp.length;
      }
      const tsArray = timestamp.split('').reverse();
      const endprex = Math.floor(Math.random() * 10);
      9
      const start = 'A'.charCodeAt(0);
      while (i < length) {
        var n = Math.floor(Math.random() * 26); // 20
        const word = String.fromCharCode(n + start); // 20+65 U
        let ts = '';
        if (tsArray.length) {
          ts = String.fromCharCode(parseInt(tsArray.shift()) + start); // ? + 65
        }
        var ns = n + endprex; // 20+9
        words.push(ns + '' + word + '' + ts); // 29 U ?
        i++;
      }
      return words.join('') + endprex;
    }
  }

}
</script>
