<template>
  <div class="home">
    <img alt="Vue logo" src="../assets/logo.png" />
    <HelloWorld msg="Welcome to Your Vue.js + TypeScript App" />
    <button @click="notify()">notify</button>
    <button @click="confirm()">confirm</button>
  </div>
</template>

<script lang="ts">
import { Vue, Component } from "vue-property-decorator";
import HelloWorld from "@/components/HelloWorld.vue";
import { IMessage } from "@/components/Notify/types";
import { GetUser } from "../api/user";

GetUser(1);

@Component({
  components: {
    HelloWorld
  }
})
export default class Home extends Vue {
  notify() {
    this.$Notify.notify({
      type: "error",
      text: "hola mundo error alto error es este..."
    } as IMessage);
    this.$Notify.notify({
      type: "info",
      text:
        "hola mundo nada importante, tenes que leer todo antes de que se termine el timepo",
      timeout: 3000
    } as IMessage);
    this.$Notify.notify({
      type: "warning",
      text: "hola mundo no importa tanto"
    } as IMessage);

    this.$Notify.error("Prueba con error");
    this.$Notify.error("Prueba con error", {
      timeout: 30000,
      title: "nada de titulo"
    } as IMessage);
  }

  confirm() {
    this.$Notify.confirm({
      type: "error",
      text: "hola mundo error alto error es este...",
      buttons: [{ text: "Aceptar", icon: "info", bind: { color: "info" } }]
    } as IMessage);
  }
}
</script>
