import { UserInfo } from '@/Interfaces/users';
import { State } from './state'

const getters = {
  isLogged: function (state: State) {
    return !!state.logged;
  },

  userPermission: function (state: State): string[] {
    const rls = state.roles || [];
    return rls.map((r) => r.rolName.toUpperCase());
  },

  checkPermission: function (state: State) {
    return (roleCode: string | string[]) => {
      if (!roleCode)
        return true;

      let rls: string[];
      if (!(roleCode instanceof Array))
        rls = [roleCode.toUpperCase()];
      else
        rls = roleCode.map((r) => r.toUpperCase());

      const permissions = getters.userPermission(state);

      if (permissions.includes("ADMINISTRATOR")) {
        return true;
      }

      return permissions.some((role) => rls.includes(role));
    };
  },

  userInfo: function (state: State): UserInfo {
    return state.user || {
      id: 0,
      username: "Anonimous"
    };
  }
}

export type Getters = typeof getters
export default getters;