import { createNamespacedHelpers } from 'vuex'
export const {
  mapMutations: mapDashboardMutations,
  mapGetters: mapDashboardGetters,
  mapActions: mapDashboardActions,
  mapState: mapDashboardState
} = createNamespacedHelpers('dashboard')
