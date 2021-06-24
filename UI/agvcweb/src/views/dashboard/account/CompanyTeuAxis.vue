<template>
  <div>
    <range-axis
      v-model="model.datas"
      :between="between"
      :limit-size="30"
      :caption="model.caption"
      :api="model.action"
    >
      <ve-bar
        v-if="model.datas"
        :data-zoom="dataZoom"
        :toolbox="toolbox"
        :data="chartData"
        :extend="extend"
        :settings="chartSettings"
      >
        <div v-if="chartData.rows.length == 0" class="data-empty">
          本月暂无箱量数据
        </div>
      </ve-bar>
      <template slot="filter">
        按月份统计：
        <el-date-picker
          v-model="dateValue"
          type="month"
          placeholder="选择月"
          size="small"
          style="width: 120px;"
        />
      </template>
    </range-axis>
  </div>
</template>
<script>
import rangeAxis from "../compoments/rangeAxis";
import VeBar from "v-charts/lib/bar.common";
import "echarts/lib/component/dataZoom";
import "echarts/lib/component/toolbox";
export default {
  name: "CompanyTeuAxis",
  components: {
    rangeAxis,
    VeBar,
  },
  data() {
    this.dataZoom = [
      {
        type: "slider",
        show: true,
        // filterMode: "none",
        zoomLock: false,
        filterMode: "empty",
        width: 15,
        // height: "80%",
        showDataShadow: false,
        showDetail: false,
        borderWidth: 0,
        yAxisIndex: 0,
        left: "97%",
        rangeMode: ["value", "value"],
        // handleIcon:
        //   "M10.7,11.9v-1.3H9.3v1.3c-4.9,0.3-8.8,4.4-8.8,9.4c0,5,3.9,9.1,8.8,9.4v1.3h1.3v-1.3c4.9-0.3,8.8-4.4,8.8-9.4C19.5,16.3,15.6,12.2,10.7,11.9z M13.3,24.4H6.7V23h6.6V24.4z M13.3,19.6H6.7v-1.4h6.6V19.6z",
        // handleSize: "80%",
        // handleStyle: {
        //   color: "#fff",
        //   shadowBlur: 3,
        //   shadowColor: "rgba(0, 0, 0, 0.6)",
        //   shadowOffsetX: 2,
        //   shadowOffsetY: 2,
        // },
        orient: "vertical",
        startValue: 0, //
        endValue: 12, //
      },
    ];
    this.toolbox = {
      feature: {
        restore: { show: true },
        mark: { show: true },
        dataView: { show: true, readOnly: true },
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
      //  xAxis: {
      //   type: 'value',
      //   inverse:true
      // },
      series: {
        label: {
          normal: {
            show: true,
          },
        },
      },
    };
    this.chartSettings = {
      metrics: ["total"],
      dimension: ["company"],
      labelMap: {
        total: "箱量",
      },
      
      dataOrder: {
        label: "total",
        order: "desc",
      },
    };
    return {
      dateValue: null,
      model: {
        action: "company_teu_axis",
        caption: "客户箱量排名",
        datas: [],
      },
    };
  },
  computed: {
    between() {
      if (!this.dateValue) return [];
      return [this.dateValue, this.dateValue];
    },
    chartData() {
      return {
        columns: ["company", "total"],
        rows: this.model.datas,
      };
    },
  },
  created() {
    this.dateValue = new Date();
  },
};
</script>
