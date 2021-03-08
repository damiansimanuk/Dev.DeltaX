import request from "@/api/request";
import { Role, User, UserInfo } from "@/Interfaces/users";
import { ActionContext, CommitOptions, Commit, Dispatch, DispatchOptions, ActionTree } from "vuex";
import { State } from './state'
import { Mutations } from './mutations'

export const COMMIT = function <P extends keyof Mutations>(
  commit: Commit,
  key: P,
  payload?: Parameters<Mutations[P]>[1], options?: CommitOptions) {
  return commit(key, payload, options)
}

let refreshTokenTimer: any = null;

const actions = {

  async Login(context: ActionContext<State, any>, credential: { username: string, password: string }) {
    console.log("Auth Action Login", credential.username);
    try {
      const response = await request.post<UserInfo>(`/Users/login`, credential);
      console.log("loged as FullName", response.data.fullName);

      COMMIT(context.commit, "LOGIN", {
        token: response.data.token || "",
        user: response.data
      });

      actions.cleanRefreshToken();
      actions.launchRefreshTokenTimer(context);
      return response.data;
    } catch (e) {
      COMMIT(context.commit, "LOGIN_FAIL");
      throw e;
    }
  },

  Logout(context: ActionContext<State, any>) {
    COMMIT(context.commit, "LOGOUT")
    actions.cleanRefreshToken();
    request.post(`/Users/logout`);
  },

  async GetCurrentUser(context: ActionContext<State, any>) {
    try {
      const response = await request.get<UserInfo>(`/Users/current`);
      console.log("Auth Action GetCurrentUser response ", response);

      COMMIT(context.commit, "SAVE_USER", {
        user: response.data
      });

      actions.launchRefreshTokenTimer(context);
      actions.RefreshToken(context);
    } catch (e) {
      console.log("Auth Action GetCurrentUser error", e.response, e);
      if (e.response && [400, 401, 403].includes(e.response.status)) {
        console.log(" getUser logout");
        actions.Logout(context);
      }
    }
  },

  cleanRefreshToken() {
    if (refreshTokenTimer) {
      clearInterval(refreshTokenTimer);
      refreshTokenTimer = null;
    }
  },

  launchRefreshTokenTimer(context: ActionContext<State, any>) {
    if (!refreshTokenTimer) {
      refreshTokenTimer = setInterval(() => {
        actions.RefreshToken(context);
      }, 5 * 60 * 1000);
    }
  },

  async RefreshToken(context: ActionContext<State, any>) {
    try {
      const response = await request.post<UserInfo>(`/Users/refresh-token`);
      COMMIT(context.commit, "SAVE_TOKEN", { token: response.data.token || "" });
    } catch (e) {
      console.log(" refreshToken error", e.response, e);
      if (e.response && [400, 401, 403].includes(e.response.status)) {
        console.log("Auth Action refreshToken logout");
        actions.Logout(context);
      }
    }
  },

  UpdateUser(context: ActionContext<State, any>, user: {
    Id: number,
    FullName: string,
    Email: string,
    Image: string,
    Password: string,
    ConfirmPassword: string
  }) {

    return request.put<UserInfo>(`/Users/${user.Id}`, user)
      .then((response) => {
        COMMIT(context.commit, "SAVE_USER", {
          user: response.data
        });
        return response.data;
      });
  },
}


export type Actions = typeof actions
const actionsTree: ActionTree<State, State> & Actions = { ...actions }
export default actionsTree;

