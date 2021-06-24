<template>
  <div>
    <el-form
      :model="createData"
      :inline="false"
      :rules="rules"
      label-width="100px"
      ref="formData"
    >
      <el-form-item label="抬头" prop="title">
        <el-autocomplete
          v-if="createData"
          :trigger-on-focus="false"
          :fetch-suggestions="selectCompany"
          v-model="createData.title"
          placeholder="请输入单位名称"
          style="width: 100%"
          @select="handleSelect"
        >
          <template slot-scope="{ item }">
            <div class="name">{{ item.name }}</div>
          </template>
          <el-button slot="append" type="primary" size="mini" @click="getTfnNo">
            获取税号
          </el-button>
        </el-autocomplete>
      </el-form-item>
      <el-form-item label="税号" prop="tfn">
        <el-input v-model="createData.tfn" placeholder=""></el-input>
      </el-form-item>
      <el-form-item label="开户行" prop="bank">
        <el-input v-model="createData.bank" placeholder=""></el-input>
      </el-form-item>
      <el-form-item label="账号" prop="cardNo">
        <el-input v-model="createData.cardNo" placeholder=""></el-input>
      </el-form-item>
      <el-form-item label="联系电话" prop="tel">
        <el-input v-model="createData.tel" placeholder=""></el-input>
      </el-form-item>
      <el-form-item label="联系地址" prop="bankAddress">
        <el-input v-model="createData.bankAddress" placeholder=""></el-input>
      </el-form-item>
    </el-form>
    <div style="text-align: center">
      <el-button type="success" @click="modifyAccount">保存</el-button>
      <!-- <el-button type="danger" @click="cancelModify">取消</el-button> -->
    </div>
  </div>
</template>

<script>
import { mapClientActions } from "@/store/namespaced/client";
import { AccountCreate, AccountUpdate } from "@/api/orgnization";
import { mapOrgActions } from "@/store/namespaced/orgnization";

export default {
  name: "accountlistedit",

  props: {
    ws: Object,
  },
  data() {
    return {
      createData: {
        kdAccountId: "",
      },
      rules: {
        title: [
          {
            required: true,
            message: "请输入抬头",
            trigger: "blur",
          },
        ],
        bank: [
          {
            required: true,
            message: "请输入开户行",
            trigger: "blur",
          },
        ],
        cardNo: [
          {
            required: true,
            message: "请输入账号",
            trigger: "blur",
          },
        ],
      },
      accountList: [],
    };
  },
  async created() {
    if (this.ws.invoiceId) {
      this.createData = await this.GetInvoiceByIdAsync(this.ws.invoiceId);
      this.createData["invoiceId"] = this.ws.invoiceId;
      console.log(this.createData);
    } else {
      this.createData = this.ws.modeldata;
    }
    var list = await this.AccountsAsync();
    this.accountList = this.getChildren(list, 0);
  },
  computed: {},
  methods: {
    ...mapOrgActions(["GetInvoiceByIdAsync"]),
    ...mapClientActions(["search", "getTfn"]),
    modifyAccount() {
      this.$refs["formData"].validate(async (valid) => {
        if (valid) {
          var res = null;
          if (!this.createData.id) {
            res = await AccountCreate(this.createData);
          } else {
            res = await AccountUpdate(this.createData);
          }
          if (res) {
            this.$message({
              type: "success",
              message: "操作成功!",
            });
            if (this.ws.callback) await this.ws.callback();
            this.$close();
          }
        } else {
          this.$message({
            type: "error",
            message: "操作失败!",
          });
          return false;
        }
      });
    },

    handleSelect(item) {
      this.createData.title = item.name;
    },
    async selectCompany(queryString, cb) {
      if (queryString) {
        var res = await this.search({ Keyword: queryString });
        cb(res);
      }
    },
    async getTfnNo() {
      if (!this.createData.title) {
        this.$message({
          type: "error",
          message: "请选择公司!",
        });
        return;
      }
      const a = await this.getTfn({ id: this.createData.title });
      if (a) {
        this.createData.tfn = a || "";
      } else {
        this.createData.tfn = "";
        this.$alert("未查询到税号，请自行填写", "提示");
      }
    },
    getChildren(list, id) {
      var a = list.filter((x) => x.fParentID === id);
      if (a.length > 0) {
        a.forEach((x) => {
          var l = this.getChildren(list, x.fAccountID);
          if (l.length > 0) {
            x.children = l;
          } else {
            x.children = null;
          }
        });
      }
      return a;
    },
  },
};
</script>
