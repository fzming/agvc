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
        :toolbox="toolbox"
        :colors="colors"
        :extend="extend"
        :data="chartData"
        :settings="chartSettings"
      >
        <div v-if="chartData.rows.length == 0" class="data-empty">暂无数据</div>
      </ve-histogram>
      <template slot="filter"> 最近一年数据</template>
    </range-axis>
  </div>
</template>
<script>
import rangeAxis from "../compoments/rangeAxis";
import VeHistogram from "v-charts/lib/histogram.common";
import "echarts/lib/component/toolbox";
export default {
  name: "MonthCostAxis",
  components: {
    rangeAxis,
    VeHistogram,
  },
  data() {
    this.colors = ["#5AB1EF", "#FA6E86"];
    this.toolbox = {
      feature: {
        magicType: { type: ["line", "bar"] },
        saveAsImage: {
          show: true,
          excludeComponents: ["toolbox"],
          pixelRatio: 2,
        },
      },
    };
    this.extend = {
      tooltip: {
        trigger: "axis",
        axisPointer: {
          type: "shadow",
        },
      },
      series: {
        label: { show: false, position: "top" },
      },
    };
    this.chartSettings = {
      metrics: ["total", "unTotal"],
      dimension: ["month"],
      labelMap: {
        total: "应收总额",
        unTotal: "未销帐总额",
      },
      // itemStyle
    };
    return {
      model: {
        action: "month_cost_axis",
        caption: "应收款/未销帐总额合计",
        datas: [],
      },
    };
  },
  computed: {
    between() {
      const end = new Date();
      const start = new Date();
      start.setMonth(start.getMonth() - 12);
      return [start, end];
    },
    chartData() {
      return {
        columns: ["month", "total", "unTotal"],
        rows: this.model.datas,
      };
    },
  },
};
</script>
