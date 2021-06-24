function initState() {
  return {
    sidebar: {
      opened: false,
      withoutAnimation: false,
    },
    device: "desktop",
    size: "medium",
    sleepSeconds: 0,
  };
}

const mutations = {
  TOGGLE_SIDEBAR: (state) => {
    state.sidebar.opened = !state.sidebar.opened;
    state.sidebar.withoutAnimation = false;
  },
  CLOSE_SIDEBAR: (state, withoutAnimation) => {
    state.sidebar.opened = false;
    state.sidebar.withoutAnimation = withoutAnimation;
  },
  TOGGLE_DEVICE: (state, device) => {
    state.device = device;
  },
  SET_SIZE: (state, size) => {
    state.size = size;
  },
  SET_SLEEP_SECONDS: (state, sleepSeconds) => {
    state.sleepSeconds = sleepSeconds;
  },
};

const actions = {
  toggleSideBar({ commit }) {
    commit("TOGGLE_SIDEBAR");
  },
  closeSideBar({ commit }, { withoutAnimation }) {
    commit("CLOSE_SIDEBAR", withoutAnimation);
  },
  toggleDevice({ commit }, device) {
    commit("TOGGLE_DEVICE", device);
  },
  setSize({ commit }, size) {
    commit("SET_SIZE", size);
  },
  setSleepSeconds({ commit }, sleepSeconds) {
    commit("SET_SLEEP_SECONDS", sleepSeconds);
  },
};

export default {
  namespaced: true,
  state: initState,
  mutations,
  actions,
};
