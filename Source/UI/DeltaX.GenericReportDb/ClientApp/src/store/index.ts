import Vue from "vue";
import Vuex from "vuex";

Vue.use(Vuex);

import "@/components/auth/store";

const store = new Vuex.Store({
  state: {},
  mutations: {},
  actions: {}
})

export type StoreType = typeof store
export default store;