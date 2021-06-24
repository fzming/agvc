import messageApi from "@/api/discussMessage";

const state = {
  navurdcount: "",
  navurdcountList: {},
  messagelist: [],
  pagetion: {
    $announce: {
      name: "系统公告",
      isAnncounce: true,
      buttonText: "发送公告",
      pageIndex: 1,
      pageSize: 20,
      datas: [],
      isFirst: true
    },
    $groupMessage: {
      name: "系统消息",
      isAnncounce: false,
      buttonText: "发送消息",
      pageIndex: 1,
      pageSize: 20,
      datas: [],
      isFirst: true
    },
    groupMessage: {
      name: "分组消息",
      isAnncounce: false,
      buttonText: "发送消息",
      pageIndex: 1,
      pageSize: 20,
      datas: [],
      isFirst: true
    }
  }
};

function refreshPagtion() {
  return {
    $announce: {
      name: "系统公告",
      isAnncounce: true,
      buttonText: "发送公告",
      pageIndex: 1,
      pageSize: 20,
      datas: [],
      isFirst: true
    },
    $groupMessage: {
      name: "系统消息",
      isAnncounce: false,
      buttonText: "发送消息",
      pageIndex: 1,
      pageSize: 20,
      datas: [],
      isFirst: true
    },
    groupMessage: {
      name: "分组消息",
      isAnncounce: false,
      buttonText: "发送消息",
      pageIndex: 1,
      pageSize: 20,
      datas: [],
      isFirst: true
    }
  };
}

function mapData(a) {
  if (a.datas) {
    const b = a.datas.map(x => {
      return {
        id: x.id,
        avatar: x.sender.avatar
          ? x.sender.avatar
          : "http://www.yy5156.com/assets/images/avatar.png",
        name: x.sender.name ? x.sender.name : "未设置名称",
        content: x.content,
        attachment: x.attachment,
        tm: x._c,
        isSelf: false,
        isRead: x.isRead
      };
    });
    return b;
  }
}

const mutations = {
  UNREAD_COUNT: (state, data) => {
    let a = 0;
    Object.values(data).forEach(function(value) {
      a += value * 1;
    });
    state.navurdcount = a;
  },
  UNREAD_LIST: (state, data) => {
    state.navurdcountList = data;
  },
  UPDATE_UNREAD: (state, data) => {
    state.navurdcount = data;
  },
  MESSAGE_LIST: (state, para) => {
    // console.log(data)
    const a = state.pagetion[para.groupType];
    console.log(a, para);
    if (a.pageIndex === 1) {
      state.pagetion[para.groupType].datas = mapData(para.data);
      state.pagetion[para.groupType].isFirst = false;
    } else {
      if (para.isAddList) {
        state.pagetion[para.groupType].datas = [
          ...state.pagetion[para.groupType].datas,
          ...mapData(para.data)
        ];
      }
    }
  },
  NEXT_PAGE: (state, data) => {
    state.pagetion[data].pageIndex += 1;
  },
  ADD_MESSAGE_CARD: (state, para) => {
    state.pagetion[para.type.group].datas.splice(0, 0, para.data);
  },
  READ_MESG: (state, data) => {
    const b = state.pagetion[data.type].datas.find(item => item.id === data.id);
    b.isRead = true;
  },
  CLEAR_MSGLIST: (state, data) => {
    !!data;
    state.pagetion = refreshPagtion();
  },
  DELETE_MESSAGECARD: (state, para) => {
    state.pagetion[para.type].datas.filter((item, index) => {
      if (item.id === para.id) {
        console.log(item.id, para.id);
        state.pagetion[para.type].datas.splice(index, 1);
      }
    });
  }
};
const getters = {
  get_groups: state => userType => {
    // console.log("userType", userType);
    const group = userType === "Admin" ? "$groupMessage" : "groupMessage";
    return [
      {
        name: state.pagetion[group].name,
        isAnncounce: state.pagetion[group].isAnncounce,
        group: group,
        buttonText: state.pagetion[group].buttonText,
        postMsg: true,
        pageIndex: state.pagetion[group].pageIndex,
        pageSize: state.pagetion[group].pageSize,
        datas: state.pagetion[group].datas,
        isFirst: state.pagetion[group].isFirst
      },
      {
        name: state.pagetion["$announce"].name,
        isAnncounce: state.pagetion["$announce"].isAnncounce,
        group: "$announce",
        buttonText: state.pagetion["$announce"].buttonText,
        postMsg: userType === "Admin",
        pageIndex: state.pagetion["$announce"].pageIndex,
        pageSize: state.pagetion["$announce"].pageSize,
        datas: state.pagetion["$announce"].datas,
        isFirst: state.pagetion["$announce"].isFirst
      }
    ];
  }
};
const actions = {
  async send({ commit }, data) {
    !!commit;
    var res = await messageApi["send"](data);
    return res;
  },
  async send_attachment({ commit }, data) {
    !!commit;
    var res = await messageApi["send_attachment"](data);
    return res;
  },
  async query({ commit }, data) {
    // console.log("query:", data);
    var res = await messageApi["query"](data);
    commit("MESSAGE_LIST", {
      data: res,
      groupType: data.group,
      isAddList: data.isAddList
    });
    return res;
  },
  async readed({ commit }, data) {
    !!commit;
    var res = await messageApi["readed"](data);
    return res;
  },
  async unreadcount({ commit }, data) {
    var res = await messageApi.unreadcount(data);
    commit("UNREAD_COUNT", res);
    commit("UNREAD_LIST", res);
    return res;
  },
  async modifyUrdCount({ commit }, data) {
    commit("UNREAD_COUNT", data);
    return data;
  },
  async saveUnreadCount({ commit }, data) {
    commit("UPDATE_UNREAD", data);
    return data;
  },
  async nextPage({ commit }, data) {
    commit("NEXT_PAGE", data);
  },
  async addMessageCard({ commit }, para) {
    commit("ADD_MESSAGE_CARD", para);
  },
  async deleteMessageCard({ commit }, data) {
    !!commit && !!data;
  },
  readMessage({ commit }, data) {
    commit("READ_MESG", data);
  },
  async ClearMessageList_Async({ commit }) {
    commit("CLEAR_MSGLIST");
  },
  async deleteMessage({ commit }, para) {
    const res = await messageApi["delMessage"](para);
    if (res) {
      commit("DELETE_MESSAGECARD", para);
    }
    return res;
  }
};

export default {
  namespaced: true,
  getters,
  mutations,
  state,
  actions
};
