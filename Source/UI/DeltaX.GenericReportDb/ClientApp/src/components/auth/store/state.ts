import { UserInfo, Role } from "@/Interfaces/users";

const token = localStorage.getItem("accessToken");
const usrJson = localStorage.getItem("user");
const user = (usrJson ? JSON.parse(usrJson) : {}) as UserInfo;

const state = {
  logged: token != null,
  user: user as (UserInfo | undefined),
  accessToken: token,
  roles: [] as Role[] | undefined,
  userUpdated: false,
  userPasswordUpdated: false,
  userPasswordUpdateError: false,
  userPasswordUpdateErrorMsg: "",
  loginWait: false,
  loginFailed: false,
};

export type State = typeof state
export default state;
