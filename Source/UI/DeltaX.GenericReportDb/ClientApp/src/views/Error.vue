<template>
  <v-app id="error">
    <v-container fluid fill-height>
      <v-layout align-center justify-center row>
        <div class="text-md-center">
          <h1 class="errorCode">{{ errorCode }}</h1>
          <h2 class="my-3 headline ">{{ reason }}</h2>
          <div>
            <v-btn color="secondary" @click="goHome()">
              Go To Home
            </v-btn>
            <v-btn text @click="goBack()">
              Go To Back
            </v-btn>
          </div>
        </div>
      </v-layout>
    </v-container>
  </v-app>
</template>

<script>
export default {
  computed: {
    errorCode() {
      return this.$route.params.errorCode;
    },
    reason() {
      return this.$route.query.reason;
    },
    redirectBack() {
      return this.$route.query.back || "/";
    }
  },
  methods: {
    goBack() {
      this.$router.push({ path: this.redirectBack });
    },
    goHome() {
      this.$router.push({ path: "/" });
    }
  }
};
</script>

<style>
.d {
  margin: 0 auto;
  width: 25% !important;
}
.errorCode {
  text-align: center;
  vertical-align: middle;
  font-size: 170px;
  line-height: 170px;
  font-weight: 800;
  color: #424242;
  text-shadow: rgba(61, 61, 61, 0.3) 1px 1px, rgba(61, 61, 61, 0.2) 2px 2px,
    rgba(61, 61, 61, 0.3) 3px 3px;
}
</style>
