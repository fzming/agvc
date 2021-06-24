<template>
  <div class="rangeAxis">
    <div class="title flex-start">
      <div class="flex1 ca">{{ caption }}</div>
      <div class="filter">
        <slot name="filter" />
      </div>
    </div>
    <slot />
  </div>
</template>
<script>
import { mapDashboardActions } from "@/store/namespaced/dashboard";
import dayjs from "dayjs";
export default {
  name: "RangeAxis",
  model: {
    prop: "rangeData",
    event: "updateRangeData",
  },
  props: {
    caption: {
      type: String,
      default: "",
    },
    api: {
      type: String,
      default: "",
    },
    between: {
      type: Array,
      default: () => {
        return [];
      },
    },
    limitSize: {
      type: Number,
      default: 0,
    },
    // model props
    rangeData: {
      type: Array,
      default: () => {
        return [];
      },
    },
  },
  data() {
    return {};
  },
  computed: {
    range() {
        if(!this.between) return;
      if (this.between.length !== 2) return {};
      return {
        btm: dayjs(this.between[0])
          .startOf("month")
          .startOf("day")
          .format("YYYY-MM-DD HH:mm:ss"),
        etm: dayjs(this.between[1])
          .endOf("month")
          .endOf("day")
          .format("YYYY-MM-DD HH:mm:ss"),
        limitSize: this.limitSize || 12,
      };
    },
    query() {
       if(!this.range) return;
      if (this.range.btm && this.range.etm && this.api)
        return { api: this.api, param: this.range };
      return null;
    },
  },
  watch: {
    between: {
      handler(v) {
        if(!v) return;
        console.log(v);
        this.btm = v[0];
        this.etm = v[1];
      },
      deep: true,
    },
    query: {
      async handler(v) {
        // console.log('query..change');
        if (v) {
          await this.fetchAsync();
        }
      },

      immediate: true,
    },
  },
  async activated() {
    // console.log(this.caption + ' activated');
    await this.fetchAsync();
  },
  methods: {
    ...mapDashboardActions(["axisFetchAsync"]),
    async fetchAsync() {
      // console.log('fetchAsync....', this.query);
      if (this.query == null) return;
      var rangeData = await this.axisFetchAsync(this.query);
      this.$emit("updateRangeData", rangeData);
      this.$emit("update:rangeData", rangeData); // 兼容:open.sync语法调用
    },
  },
};
</script>
<style lang="scss" scoped>
.rangeAxis {
  .title {
    margin-bottom: 20px;
    padding-left: 15px;
    position: relative;
    height: 32px;
    line-height: 32px;
    &::before {
      content: " ";
      position: absolute;
      width: 7px;
      height: 20px;
      background: $light-blue;
      border-radius: 5px;
      left: -5px;
      transform: translateY(-50%);
      top: 50%;
      box-shadow: 2px 2px 4px 2px rgba($light-blue, 0.2);
    }
  }
  .ca {
    font-size: 15px;
    color: #333333;
    font-weight: bold;
  }
  .filter {
    color: #888;
    font-size: 14px;
  }
}
</style>
