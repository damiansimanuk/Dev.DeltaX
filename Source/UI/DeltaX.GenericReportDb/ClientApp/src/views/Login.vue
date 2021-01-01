<template>
  <v-layout id="login" align-center justify-center>
    <v-flex xs12 sm8 md4 lg4>
      <v-card class="elevation-1 pa-3">
        <v-card-text>
          <LoginForm :enableRedirect="true" />
        </v-card-text>
      </v-card>
    </v-flex>
  </v-layout>
</template>

<script>
import LoginForm from "@/components/auth/LoginForm.vue";

export default {
  name: "Login",

  components: {
    LoginForm
  }
};
</script>
