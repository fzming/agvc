<template>
  <div>
    <range-axis
      v-model="model.datas"
      :between="between"
      :caption="model.caption"
      :api="model.action"
    >
      <ve-bar
        v-if="model.datas"
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
          style="width: 120px"
        />
      </template>
    </range-axis>
  </div>
</template>
<script>
import rangeAxis from "../compoments/rangeAxis";
import VeBar from "v-charts/lib/bar.common";
import "echarts/lib/component/toolbox";
export default {
  name: "UserTeuAxis",
  components: {
    rangeAxis,
    VeBar
  },
  data() {
    this.toolbox = {
      feature: {
        saveAsImage: {
          show: true,
          excludeComponents: ["toolbox"],
          pixelRatio: 2
        }
      }
    };
    this.extend = {
       tooltip: {
        trigger: "axis",
        axisPointer: {
          type: "shadow",
        },
      },
      series: {
        label: {
          normal: {
            show: true
          }
        }
      }
    };
    this.chartSettings = {
      metrics: ["total"],
      dimension: ["user"],
      labelMap: {
        total: "箱量"
      },
      dataOrder: {
        label: "total",
        order: "desc"
      }
    };
    return {
      dateValue: null,
      model: {
        action: "user_teu_axis",
        caption: "业务员箱量排名",
        datas: []
      }
    };
  },
  computed: {
    between() {
      if (!this.dateValue) return [];
      return [this.dateValue, this.dateValue];
    },
    chartData() {
      return {
        columns: ["user", "total"],
        rows: this.model.datas
      };
    }
  },
  created() {
    this.dateValue = new Date();
  }
};
</script>
