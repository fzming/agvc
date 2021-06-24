import { createNamespacedHelpers } from 'vuex'
export const {
  mapMutations: mapClientMutations,
  mapGetters: mapClientGetters,
  mapActions: mapClientActions,
  mapState: mapClientState
} = createNamespacedHelpers('client')
