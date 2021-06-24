<template>
  <region>
    <div :class="{ hidden: hidden }" class="flexbox pagination-container">
      <div class="flex1">
        <el-pagination
          :background="background"
          :current-page.sync="pageIndex"
          :page-size.sync="pageSize"
          :layout="layout"
          :page-sizes="pageSizes"
          :total="total"
          v-bind="$attrs"
          @size-change="handleSizeChange"
          @current-change="handleCurrentChange"
        />
      </div>
      <div>
        <slot></slot>
      </div>
    </div>
  </region>
</template>

<script>
import { scrollTo } from "@/utils/scroll-to";

export default {
  name: "Pagination",
  props: {
    total: {
      required: true,
      type: Number,
    },
    page: {
      type: Number,
      default: 1,
    },
    limit: {
      type: Number,
      default: 20,
    },
    pageSizes: {
      type: Array,
      default() {
        return [10, 20, 30, 50];
      },
    },
    layout: {
      type: String,
      default: "total, sizes, prev, pager, next, jumper",
    },
    background: {
      type: Boolean,
      default: true,
    },
    autoScroll: {
      type: Boolean,
      default: true,
    },
    hidden: {
      type: Boolean,
      default: false,
    },
  },
  data() {
    return {
      pageIndex: this.page,
      pageSize: this.limit,
    };
  },
  watch: {
    pageIndex(value) {
      this.$emit("update:page", value);
    },
    pageSize(value) {
      this.$emit("update:limit", value);
    },
    //---------prop---------------
    page(val) {
      this.pageIndex = val;
    },
    limit(val) {
      this.pageSize = val;
    },
  },
  created() {
    this.pageIndex = this.page;
    this.pageSize = this.limit;
  },
  methods: {
    handleSizeChange(val) {
      console.log("handleSizeChange", val);
      this.$emit("pagination", { page: 1, limit: val });
      if (this.autoScroll) {
        scrollTo(0, 800);
      }
    },
    handleCurrentChange(val) {
      console.log("handleCurrentChange", val);
      this.$emit("pagination", { page: val, limit: this.pageSize });
      if (this.autoScroll) {
        scrollTo(0, 800);
      }
    },
  },
};
</script>

<style lang="scss" scoped>
.pagination-container {
  background: #fff;
}

.pagination-container.hidden {
  display: none;
}
</style>
