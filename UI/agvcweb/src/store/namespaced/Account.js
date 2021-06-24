import { createNamespacedHelpers } from 'vuex'
export const {
  mapMutations: mapAccountMutations,
  mapGetters: mapAccountGetters,
  mapActions: mapAccountActions,
  mapState: mapAccountState
} = createNamespacedHelpers('Account')
