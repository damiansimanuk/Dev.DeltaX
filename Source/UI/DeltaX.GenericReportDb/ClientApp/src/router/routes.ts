import { RouteConfig } from "vue-router";

const routes: Array<RouteConfig> = [
  {
    path: "/home",
    alias: "/",
    name: "Home",
    component: () =>
      import(/* webpackChunkName: "Home" */ "../views/Home.vue"),
    meta: {
      layout: "DefaultLayout",
      isPublic: true,
      roleRequired: []
    }
  },
  {
    path: "/about",
    name: "About",
    component: () =>
      import(/* webpackChunkName: "about" */ "../views/About.vue")
  },
  {
    path: "/help",
    name: "Help",
    component: () => import(/* webpackChunkName: "help" */ "../views/Help.vue")
  },
  {
    path: "/private",
    name: "Private",
    component: () => import(/* webpackChunkName: "help" */ "../views/Help.vue"),
    meta: {
      layout: "PublicLayout",
      isPublic: false,
      roleRequired: []
    }
  },
  {
    path: "/login",
    name: "Login",
    component: () => import(/* webpackChunkName: "login" */ "../views/Login.vue"),
    meta: {
      layout: "PublicLayout",
      isPublic: true,
      roleRequired: []
    }
  },
  {
    path: "/error/:errorCode",
    name: "error",
    component: () => import(/* webpackChunkName: "error" */"../views/Error.vue"),
    meta: {
      layout: "PublicLayout",
      isPublic: true,
      roleRequired: []
    }
  },
  {
    path: "/demo-widgets",
    name: "DemoWidgets",
    component: () => import(/* webpackChunkName: "DemoWidgets" */"../views/DemoWidgets.vue"),
    meta: {
      layout: "PublicLayout",
      isPublic: true,
      roleRequired: []
    }
  },
  {
    path: "/crud/:configName",
    name: "GenericTableApi",
    component: () => import(/* webpackChunkName: "DemoWidgets" */"../views/GenericTableApi.vue"),
    meta: {
      layout: "PublicLayout",
      isPublic: true,
      roleRequired: []
    }
  },
];

export default routes;
