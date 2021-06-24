import {
  create,
  deleted,
  modify,
  query,
  AccountCreate,
  AccountUpdate,
  AccountDelete,
  AccountQuery,
  GetInvoiceByIdAsync,
} from "@/api/orgnization";

const state = {
  token: {}, //getToken(),
  name: "",
  avatar: "",
  introduction: "",
  roles: [],
};

const actions = {
  mquery({ commit }) {
    !!commit;
    return new Promise((resolve, reject) => {
      query()
        .then((response) => {
          resolve(response);
        })
        .catch((error) => {
          reject(error);
        });
    });
  },

  mcreate({ commit }, data) {
    !!commit;
    // console.log(data)
    return new Promise((resolve, reject) => {
      create(data)
        .then((response) => {
          resolve(response);
        })
        .catch((error) => {
          reject(error);
        });
    });
  },
  update({ commit }, data) {
    !!commit;
    return new Promise((resolve, reject) => {
      modify(data)
        .then((response) => {
          // console.log(response)
          resolve(response);
        })
        .catch((error) => {
          reject(error);
        });
    });
  },
  remove({ commit }, data) {
    !!commit;
    const { id } = data;
    return new Promise((resolve, reject) => {
      deleted({
        codeId: id,
      })
        .then((res) => {
          console.log(res);
          resolve(res);
        })
        .catch((error) => {
          reject(error);
        });
    });
  },
  async AccountCreateAsync({ commit }, data) {
    !!commit;
    var d = await AccountCreate(data);
    return d;
  },
  async AccountUpdateAsync({ commit }, data) {
    !!commit;
    var d = await AccountUpdate(data);
    return d;
  },
  async AccountDeleteAsync({ commit }, data) {
    !!commit;
    var d = await AccountDelete(data);
    return d;
  },
  async AccountQueryAsync({ commit }) {
    !!commit;
    var d = await AccountQuery();
    return d;
  },
  async GetInvoiceByIdAsync({ commit }, id) {
    !!commit;
    return await GetInvoiceByIdAsync(id);
  },
};

export default {
  namespaced: true,
  state,
  actions,
};
