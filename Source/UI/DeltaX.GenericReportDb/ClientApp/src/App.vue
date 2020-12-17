<template>
  <v-app dark>
    <keep-alive>
      <Notify />
    </keep-alive>
    <v-app-bar app color="primary" dark>
      <div id="nav">
        <router-link to="/">Home</router-link> |
        <router-link class="color-red" to="/about">About</router-link>
      </div>
    </v-app-bar>

    <v-main>
      <router-view />
    </v-main>
  </v-app>
</template>

<script lang="ts">
import Vue from "vue";
import Notify from "./components/Notify/Notify.vue";

export default Vue.extend({
  name: "App",
  components: { Notify },
  data: () => ({})
});
</script>
