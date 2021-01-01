<template>
  <div>
    <component :is="componentLayout">
      <slot />
    </component>
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import { DISPATCH } from "@/components/auth/store";
import PublicLayout from "@/layouts/public.vue";
import DefaultLayout from "@/layouts/default.vue";

export default Vue.extend({
  name: "App",
  components: { PublicLayout, DefaultLayout },
  data: () => ({}),
  created() {
    //  DISPATCH("GetCurrentUser");
  },
  computed: {
    componentLayout(): string {
      const meta = this.$route.meta;
      return meta && meta.layout ? meta.layout : "PublicLayout";
    }
  }
});
</script>
