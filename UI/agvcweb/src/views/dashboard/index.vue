<template>
  <div class="dashboard-container">
    <component :is="currentDashboard" />
  </div>
</template>

<script>
import { mapGetters } from "vuex";
import adminDashboard from "./admin";
import hmcIndex from "./hmc";

export default {
  name: "Dashboard",
  components: { adminDashboard, hmcIndex },

  computed: {
    ...mapGetters(["roles"]),
    currentDashboard() {
      const a = this.$store.getters.token.userType;
      const arr = ["adminDashboard", "hmcIndex"];
      const module = this.$store.getters.module;
      return a === "Admin" ? arr[0] : `${arr[module || 0]}`;
    }
  },
  created() {},
  methods: {}
};
</script>
