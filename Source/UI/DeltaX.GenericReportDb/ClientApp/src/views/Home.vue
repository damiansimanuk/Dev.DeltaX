<template>
  <v-container class="home">
    <img alt="Vue logo" src="../assets/logo.png" />
    <HelloWorld msg="Welcome to Your Vue.js + TypeScript App" />
    <v-row>
      <v-spacer />
      <v-btn class="mx-2" @click="notify()">notify</v-btn>
      <v-btn @click="confirm()">confirm</v-btn>
    </v-row>

    <pre style="wrap">accessToken:{{ accessToken }}</pre>
    <pre>userInfo:{{ userInfo }}</pre>

    <button @click="login()">login</button>
  </v-container>
</template>

<script lang="ts">
import { Vue, Component } from "vue-property-decorator";
import HelloWorld from "@/components/HelloWorld.vue";
import auth, { COMMIT, DISPATCH, GETTERS } from "@/components/auth/store";

@Component({
  components: {
    HelloWorld
  }
})
export default class Home extends Vue {
  get state() {
    return auth.state;
  }

  get userInfo() {
    return GETTERS("userInfo");
  }

  get accessToken() {
    return auth.state.accessToken;
  }

  login() {
    COMMIT("SAVE_TOKEN", { Token: "ältotoken" + Date.now() });
    DISPATCH("Login", { Username: "admin", Password: "admin " });
  }

  notify() {
    this.$Notify.notify({
      type: "error",
      text: "hola mundo error alto error es este..."
    });
    this.$Notify.notify({
      type: "info",
      text:
        "hola mundo nada importante, tenes que leer todo antes de que se termine el timepo",
      timeout: 3000
    });
    this.$Notify.notify({
      type: "warning",
      text: "hola mundo no importa tanto"
    });

    this.$Notify.error("Prueba con error");
    this.$Notify.error("Prueba con error", {
      timeout: 30000,
      title: "nada de titulo"
    });
  }

  confirm() {
    this.$Notify.confirm({
      type: "error",
      text: "hola mundo error alto error es este...",
      buttons: [{ text: "Aceptar", icon: "info", bind: { color: "info" } }]
    });
  }
}
</script>
