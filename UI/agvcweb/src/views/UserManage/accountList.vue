<template>
  <div>
    <caption-container caption="机构账户管理">
      <el-button type="primary" @click="handleAddupdateUser">
        <i class="el-icon-plus" /> 新增账户
      </el-button>
    </caption-container>
    <region>
      <el-table :data="tableData" :height="tableHeight">
        <el-table-column label="抬头" prop="title"></el-table-column>
        <el-table-column label="开户行" prop="bank"></el-table-column>
        <el-table-column label="账号" prop="cardNo"></el-table-column>
        <el-table-column label="创建时间" prop="_c"></el-table-column>
        <el-table-column label="操作" width="200px">
          <template slot-scope="scope">
            <el-button type="infor" @click="handleAddupdateUser(scope.row)">
              修改
            </el-button>
            <el-button type="danger" @click="deleteAccount(scope)">
              删除
            </el-button>
          </template>
        </el-table-column>
      </el-table>
    </region>
  </div>
</template>
<script>
import { AccountDelete, AccountQuery } from "@/api/orgnization";
function initState() {
  return {
    invoiceId: "",
    title: "",
    tfn: "",
    bank: "",
    cardNo: "",
    tel: "",
    bankAddress: "",
    id: "",
  };
}
export default {
  name: "orgAccountList",
  data() {
    return {
      createData: initState(),
      tableData: [],
    };
  },
  computed: {
    tableHeight() {
      return window.innerHeight - 220;
    },
  },
  async created() {
    var d = await AccountQuery();
    this.tableData = d;

    var id = this.$route.query.id;
    if (id) {
      var a = this.tableData.find((x) => x.id === id);
      await this.handleAddupdateUser(a);
    }
  },

  methods: {
    // 新增用户/编辑用户
    async handleAddupdateUser(row) {
      this.createData = initState();
      if (row) {
        this.createData = {
          bank: row.bank,
          bankAddress: row.bankAddress,
          cardNo: row.cardNo,
          companyId: row.companyId,
          invoiceId: row.id,
          orgId: row.orgId,
          tel: row.tel,
          tfn: row.tfn,
          title: row.title,
          userId: row.userId,
          id: row.id,
          kdAccountId: row.kdAccountId,
        };
      }
      this.$openView(
        __dirname,
        "components/accountlistedit.vue",
        this.createData.id ? "修改机构账户" : "新增机构账户",
        {
          modeldata: this.createData,
          callback: this.modifyAccount,
        }
      );
    },

    async deleteAccount(scope) {
      const confirm = await this.$confirmAsync(
        "删除确认",
        "确定删除该账户吗？"
      );
      if (!confirm) {
        console.log("取消删除");
      } else {
        console.log("删除账户");
        await AccountDelete(scope.row.id);

        this.tableData.splice(scope.$index, 1);
      }
    },

    async modifyAccount() {
      var d = await AccountQuery();
      this.tableData = d;
    },
  },
};
</script>
