<template>
  <div>
    <div
      style="
        text-align: right;
        font-size: 12px;
        color: gray;
        margin-top: -10px;
        padding-bottom: 10px;
      "
    >
      统计说明：首页数据均基于<a style="color: #1890ff;">"装货日期"</a>进行统计
    </div>
    <div class="flex">
      <div
        v-for="(o, index) in cards"
        :key="index"
        class="card flex1"
        :span="5"
        :offset="index > 0 ? 1 : 0"
      >
        <card
          :title="o.title"
          :count="o.count"
          :format="o.format"
          :unit="o.unit"
          :color="o.color"
          :icon="o.icon"
          :click-name="o.clickName"
          @clickThis="o.doClick"
        />
      </div>
    </div>

    <el-dialog
      title="企业列表"
      :visible.sync="dialogTableVisible"
      width="500px"
      :show-header="false"
    >
      <el-table :data="tableData" height="300">
        <el-table-column property="name" width="300" />
        <el-table-column width="100">
          <template slot-scope="scope">
            <el-button type="text" @click="goInfo(scope.row.name)">
              查看详情
            </el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-dialog>
    <div class="card-range" style="margin-top: 20px;">
      <MonthTeuAxis />
    </div>
    <div class="card2 flex-stretch" style="margin-top: 20px;">
      <MonthCostAxis class="card-range flex1" />
      <CompanyTeuAxis class="card-range flex1" />
      <UserTeuAxis class="card-range flex1" />
    </div>
  </div>
</template>
<script>
import dashboardApi from "@/api/dashboard";
import card from "../compoments/card";
import MonthTeuAxis from "./MonthTeuAxis";
import MonthCostAxis from "./MonthCostAxis";
import CompanyTeuAxis from "./CompanyTeuAxis";
import UserTeuAxis from "./UserTeuAxis";
export default {
  name: "AccountDashboard",
  components: {
    // card: () => import('../compoments/card'),
    //   MonthTeuAxis: () => import('./MonthTeuAxis'),
    //   MonthCostAxis: () => import('./MonthCostAxis'),
    //   CompanyTeuAxis: () => import('./CompanyTeuAxis'),
    //   UserTeuAxis: () => import('./UserTeuAxis')
    card,
    MonthTeuAxis,
    MonthCostAxis,
    CompanyTeuAxis,
    UserTeuAxis,
  },
  data() {
    const that = this;
    return {
      dialogTableVisible: false,
      tableData: [],
      cards: [
        {
          title: "超30天未发货客户",
          color: "#FA6E86",
          count: 0,
          format: false,
          icon: "card-4",
          unit: "家",
          clickName: "clickThis",
          data: [],
          doClick: function () {
            that.doOneCard();
          },
        },
        {
          title: "本月新增用户",
          color: "#19D4AE",
          count: 0,
          format: false,
          icon: "card-2",
          unit: "家",
          clickName: "clickThis",
          data: [],
          doClick: function () {
            that.doTwoCard();
          },
        },
        {
          title: "应收待对账",
          color: "#0080FF",
          count: 0,
          format: false,
          icon: "card-3",
          unit: "元",
          clickName: "clickThis",
          data: [],
          doClick: function () {
            that.doThrCard();
          },
        },
        {
          title: "总计未收运费",
          color: "#5AB1EF",
          count: 0,
          format: false,
          icon: "card-1",
          unit: "元",
          clickName: "clickThis",
          data: [],
          doClick: function () {
            that.doForCard();
          },
        },
      ],
    };
  },
  created() {
    window.eventBus.$on("$EventCollapseChange$",()=>{
      window.dispatchEvent(new Event("resize"));
    });
  },
  async mounted() {
    await this.countAsync();
  },
  async activated() {
    await this.countAsync();
  },

  methods: {
    async countAsync() {
      const a = await dashboardApi.counter();
      // console.log(a)
      if (a) {
        this.cards = a.map((item, index) => {
          var c = this.cards[index];
          return Object.assign(c, {
            data: item.extraCompanys,
            count: item.count,
            format: index > 1,
          });
        });
      }
    },
    doOneCard() {
      this.tableData = this.cards[0].data;
      this.dialogTableVisible = true;
    },
    doTwoCard() {
      this.tableData = this.cards[1].data;
      this.dialogTableVisible = true;
    },
    doThrCard() {
      this.$router.push({
        path: `/coststatistics/ageofAccount/1`,
        query: {
          aoa_key: "thisMonth",
        },
      });
    },
    doForCard() {
      this.$router.push({
        path: `/coststatistics/ageofAccount/1`,
        query: {
          aoa_key: "allCompanys",
        },
      });
    },
    goInfo(id) {
      this.dialogTableVisible = false;
      this.$router.push({
        path: `/client/index`,
        query: {
          key: id,
        },
      });
    },
  },
};
</script>
<style lang="scss" scoped>
.card-range {
  background: white;
  padding: 20px;
  border-radius: 2px;
  transition: all 0.2s ease;
  box-shadow: 0 1px 2px #c8d1d3;
}
.card2 {
  .card-range {
    margin-left: 10px;
    margin-right: 10px;
  }
  .card-range:first-child,
  .card-range:last-child {
    margin-left: 0px;
    margin-right: 0px;
  }
}
.card {
  margin-right: 20px;

  &:last-child {
    margin-right: 0;
  }
}
</style>
