import state from './state'
import mutations, { Mutations } from './mutations'
import actions, { Actions } from './actions'
import getters, { Getters } from './getters'
import { CommitOptions, Commit, Dispatch, DispatchOptions } from "vuex";
import Vuex from "vuex";
import Vue from "vue";
Vue.use(Vuex);

const auth = new Vuex.Store({
  state,
  mutations,
  actions,
  getters
});

export const COMMIT = function <P extends keyof Mutations>(
  key: P,
  payload?: Parameters<Mutations[P]>[1],
  options?: CommitOptions) {
  return auth.commit(key, payload, options)
}

export const DISPATCH = async function <K extends keyof Actions>(
  key: K,
  payload?: Parameters<Actions[K]>[1],
  options?: DispatchOptions) {
  return await auth.dispatch(key, payload, options) as ReturnType<Actions[K]>;
}

export const GETTERS = function <K extends keyof Getters>(key: K) {
  return auth.getters[key] as ReturnType<Getters[K]>;
}

export default auth 
