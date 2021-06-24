<template>
  <div>
    <VSignalr
      :autoreconnected="true"
      :client-id="clientId"
      @on-received="on_received"
    />
    <MobileMain />
  </div>
</template>
<script>
import { MobileMain } from "./components";
import VSignalr from "./components/VSignalr";
import { SIG_EVENT } from "@/global/const";
export default {
  name: "MobileLayout",
  components: {
    MobileMain,
    VSignalr,
  },
  computed: {
    clientId() {
      return this.$store.getters.id;
    },
  },

  methods: {
    on_received(data) {
      window.eventBus.$emit(SIG_EVENT, data);
    },
  },
};
</script>
