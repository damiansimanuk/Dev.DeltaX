<template>
  <v-app dark>
    <keep-alive>
      <Notify />
    </keep-alive>

    <v-app-bar app dark color="primary" dense>
      <div id="nav">
        <router-link class="white" to="/">Home</router-link> |
        <router-link class="white" to="/about">About</router-link>
      </div>
    </v-app-bar>

    <v-navigation-drawer
      v-model="drawer"
      app
      clipped
      dark
      color="grey darken-4"
    >
      <v-divider></v-divider>
      <v-list dense expand>
        <template v-for="(item, i) in items">
          <v-list-group
            v-if="item.items"
            :key="i"
            :group="item.group || null"
            :prepend-icon="item.icon"
            no-action="no-action"
          >
            <v-list-item dense slot="activator">
              <v-list-item-content>
                <v-list-item-title>{{ item.title }}</v-list-item-title>
              </v-list-item-content>
            </v-list-item>

            <template v-for="(subitem, si) in item.items">
              <v-list-item
                dense
                :key="`${subitem.title}-${si}`"
                :to="subitem.to"
                :href="subitem.href"
              >
                <v-list-item-action>
                  <v-icon>{{ subitem.icon }}</v-icon>
                </v-list-item-action>

                <v-list-item-content>
                  <v-list-item-title>{{ subitem.title }}</v-list-item-title>
                </v-list-item-content>
              </v-list-item>
            </template>
          </v-list-group>

          <v-subheader v-else-if="item.header" :key="i">
            {{ item.header }}
          </v-subheader>
          <v-divider v-else-if="item.divider" :key="i"></v-divider>

          <v-list-item v-else :key="i" :to="item.to" :href="item.href">
            <v-list-item-action>
              <v-icon>{{ item.icon }}</v-icon>
            </v-list-item-action>

            <v-list-item-content>
              <v-list-item-title>{{ item.title }}</v-list-item-title>
            </v-list-item-content>
          </v-list-item>
        </template>
      </v-list>
    </v-navigation-drawer>

    <v-main>
      <transition name="slide" mode="out-in">
        <pre>{{ items }}</pre>
        <router-view />
      </transition>
    </v-main>
  </v-app>
</template>

<script lang="ts">
import { Vue, Component } from "vue-property-decorator";
import Notify from "@/components/Notify/Notify.vue";
import crud from "@/api/crud";
import { GETTERS } from "@/components/auth/store";

interface MenuType {
  title?: string;
  header?: string;
  group?: string;
  icon?: string;
  to?: string;
  href?: string;
  divider?: boolean;
  loggedRequired?: boolean;
  roleRequired?: string | string[];
  items?: MenuType[];
}

const menu: MenuType[] = [
  {
    title: "Home",
    icon: "mdi-home-city",
    to: "/home",
    href: ""
  },
  {
    title: "My Account",
    icon: "mdi-account",
    to: "/profile",
    loggedRequired: true
  }
];

@Component({
  name: "DefaultLayout",
  components: {
    Notify
  }
})
export default class Home extends Vue {
  items: MenuType[] = [];
  drawer = true;

  siteTitle() {
    let title = this.$route.meta.title || this.$route.name || this.$route.path;
    if (!title) title = `${this.$route.name || this.$route.path}.title`;
    const t = this.$t(title);
    document.title = t.toString();
    return t;
  }

  get isLogged() {
    return GETTERS("isLogged");
  }

  get checkPermission() {
    return GETTERS("checkPermission");
  }

  created() {
    this.GenerateItems();
  }

  async GenerateItems() {
    const items = menu
      .filter(i => {
        if (i.loggedRequired && !this.isLogged) return false;
        if (i.roleRequired != null) return this.checkPermission(i.roleRequired);
        return true;
      })
      .map(i => {
        if (i.items) {
          const subitems = i.items.filter(si => {
            if (si.loggedRequired && !this.isLogged) return false;
            if (si.roleRequired != null)
              return this.checkPermission(si.roleRequired);
            return true;
          });
          i = { ...i, items: subitems };
        }
        return i;
      });

    const names = await crud.GetAllConfigNames();

    const addItems: MenuType[] = [
      { header: "CRUD API" },
      {
        title: "CRUD API",
        group: "crud",
        icon: "mdi-widgets",
        items: names.map(n => {
          return {
            title: `${n.displayName || n.name}`,
            icon: "mdi-information",
            to: `/crud/${n.name}`
          } as MenuType;
        })
      }
    ];

    items.push(...addItems);

    this.items = items;
  }
}
</script>
