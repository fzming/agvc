<script>
export default {
  created() {
    console.log("redirect:created");
    const { params, query } = this.$route;
    const { path } = params;
    this.$router.replace({ path: "/" + path, query });
  },
  activated() {
    console.log("redirect:activated");
    // const { params, query } = this.$route;
    // const { path } = params;
    // this.$router.replace({ path: "/" + path, query });
  },
  render: function (h) {
    return h(); // avoid warning message
  },
};
</script>
