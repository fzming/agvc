import { createNamespacedHelpers } from 'vuex';
export const {
  mapMutations: mapAppMutations,
  mapGetters: mapAppGetters,
  mapActions: mapAppActions,
  mapState: mapAppState
} = createNamespacedHelpers('app');
