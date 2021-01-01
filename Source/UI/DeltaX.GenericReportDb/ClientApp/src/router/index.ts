import Vue from "vue";
import VueRouter, { RouteConfig } from "vue-router";
import auth, { GETTERS } from "@/components/auth/store"
import routes from "./routes"

Vue.use(VueRouter);

const router = new VueRouter({
  mode: "hash",
  base: process.env.BASE_URL,
  routes
});

function loginRequired() {
  return !GETTERS("isLogged");
}

function permissionRequired(roleRequired: string | string[]) {
  return !GETTERS("checkPermission")(roleRequired)
}

router.beforeEach(async (to, from, next) => {
  if (to.meta.isPublic == false) {
    if (loginRequired()) {
      return next({
        path: "/login",
        query: {
          redirect: to.path,
          back: from.path
        },
      });
    }

    if (to.meta.roleRequired && to.meta.roleRequired[0] && permissionRequired(to.meta.roleRequired)) {
      return next({
        path: "/error/403",
        query: {
          back: from.path,
          reason: "Role Required",
          roles: to.meta.roleRequired,
        },
      });
    }
  }
  next();
});


export default router;
