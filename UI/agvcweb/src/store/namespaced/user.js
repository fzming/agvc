import { createNamespacedHelpers } from 'vuex'
export const {
  mapMutations: mapUserMutations,
  mapGetters: mapUserGetters,
  mapActions: mapUserActions,
  mapState: mapUserState
} = createNamespacedHelpers('user')
