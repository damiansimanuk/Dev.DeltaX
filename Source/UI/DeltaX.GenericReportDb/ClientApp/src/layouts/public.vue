<template>
  <v-app>
    <keep-alive>
      <Notify />
    </keep-alive>

    <v-main style="padding-top: 0px;">
      <transition name="slide" mode="out-in">
        <router-view />
      </transition>
    </v-main>
  </v-app>
</template>

<script>
import Notify from "@/components/Notify/Notify.vue";

export default {
  name: "PublicLayout",

  components: { Notify }
};
</script>
