<template>
  <div>
    <range-axis
      v-model="model.datas"
      :between="between"
      :caption="model.caption"
      :api="model.action"
    >
      <ve-histogram
        v-if="model.datas"
        :data="chartData"
        :extend="extend"
        :settings="chartSettings"
        
      >
        <div v-if="chartData.rows.length == 0" class="data-empty">
          当前时间段暂无箱量数据
        </div>
      </ve-histogram>
      <div slot="filter">
        时间范围：
        <el-date-picker
          v-model="between"
          type="monthrange"
          align="right"
          size="mini"
          unlink-panels
          range-separator="至"
          start-placeholder="开始月份"
          end-placeholder="结束月份"
          :picker-options="pickerOptions"
        />
      </div>
    </range-axis>
  </div>
</template>
<script>
import rangeAxis from "../compoments/rangeAxis";
import VeHistogram from "v-charts/lib/histogram.common";
 
export default {
  name: "MonthTeuAxis",
  components: {
    rangeAxis,
    VeHistogram,
  },
  data() {
    this.extend = {
      series: {
        label: {
          normal: {
            show: true,
          },
        },
      },
    };
    this.chartSettings = {
      axisSite: { right: ["profitRate"] },
      showLine: ["profitRate"],
      yAxisType: ["KMB", "percent"],
      yAxisName: ["箱量", "毛利率"],
      metrics: ["total",'profitRate'],
      dimension: ["month"],
      labelMap: {
        total: "月箱量",
        month: "月份",
        profitRate: "毛利率",
      },
    };
    return {
      pickerOptions: {
        shortcuts: [
          {
            text: "今年至今",
            onClick(picker) {
              const end = new Date();
              const start = new Date(new Date().getFullYear(), 0);
              picker.$emit("pick", [start, end]);
            },
          },
          {
            text: "最近六个月",
            onClick(picker) {
              const end = new Date();
              const start = new Date();
              start.setMonth(start.getMonth() - 6);
              picker.$emit("pick", [start, end]);
            },
          },
          {
            text: "最近一年",
            onClick(picker) {
              const end = new Date();
              const start = new Date();
              start.setMonth(start.getMonth() - 12);
              picker.$emit("pick", [start, end]);
            },
          },
          {
            text: "最近两年",
            onClick(picker) {
              const end = new Date();
              const start = new Date();
              start.setMonth(start.getMonth() - 24);
              picker.$emit("pick", [start, end]);
            },
          },
        ],
      },
      between: [],
      model: {
        action: "month_teu_axis",
        caption: "订单箱量及毛利率合计",
        datas: [],
      },
    };
  },
  computed: {
    chartData() {
      return {
        columns: ["month", "total", "profitRate"],
        rows: this.model.datas,
      };
    },
  },
  created() {
    const end = new Date();
    const start = new Date();
    // 近半年
    start.setMonth(start.getMonth() - 6);
    this.between = [start, end];
  },
};
</script>
