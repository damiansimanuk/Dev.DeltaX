import { State } from './state'
import { UserInfo } from '@/Interfaces/users'
import { MutationTree } from "vuex";


const mutations = {
  LOGIN(state: State, payload: { Token: string, user: UserInfo }) {
    state.logged = true;
    state.loginFailed = false;
    state.loginWait = false;

    mutations.SAVE_USER(state, payload);
    mutations.SAVE_TOKEN(state, payload);
  },

  LOGIN_WAIT(state: State) {
    state.loginWait = true;
    state.loginFailed = false;
  },

  LOGIN_FAIL(state: State) {
    state.loginWait = false;
    state.loginFailed = true;
  },

  LOGOUT(state: State) {
    state.loginWait = false;
    state.loginFailed = false;
    state.logged = false;
    state.user = {} as UserInfo;
    state.accessToken = null;
    state.roles = [];
    localStorage.removeItem("accessToken");
    localStorage.removeItem("user");
  },

  SAVE_USER(state: State, payload: { user: UserInfo }) {
    state.user = payload.user;
    state.roles = payload.user.Roles;
    localStorage.setItem("user", JSON.stringify(state.user));

    // FIXME reemplazar por un evento
    state.userUpdated = true;
    setTimeout(() => {
      state.userUpdated = false;
    }, 1000);
  },

  SAVE_TOKEN(state: State, payload: { Token: string }) {
    state.logged = true;
    localStorage.setItem("accessToken", payload.Token);
    state.accessToken = payload.Token;
  },
};

export type Mutations = typeof mutations
const mutationTree: MutationTree<State> & Mutations = { ...mutations }
export default mutationTree;