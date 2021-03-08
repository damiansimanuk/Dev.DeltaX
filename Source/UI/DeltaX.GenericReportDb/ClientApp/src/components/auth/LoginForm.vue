<template>
  <div class="login-form white" color="primary">
    <div class=" align-center justify-center">
      <v-alert class="px-5" type="error" :value="loginFailed" dismissible>
        {{ $t("global.login.failed") }}
      </v-alert>

      <v-progress-circular
        v-if="loginWait"
        :size="100"
        :width="5"
        class="login-form__loader"
        color="primary"
        indeterminate
      />

      <template v-else>
        <h1
          class="login-form__title text-center primary--text display-1 font-weight-bold mb-10"
        >
          {{ $t("login.title") }}
        </h1>

        <!-- locale select -->
        <!-- <v-menu v-if="localeSelectable">
          <template v-slot:activator="{ on }">
            <v-btn v-on="on" dark fab small color="secondary" class="mb-2">
              <v-icon>translate</v-icon>
            </v-btn>
          </template>
          <v-list>
            <v-list-item
              v-for="(locale, i) in locales"
              :key="i"
              @click="setLocale(locale.name)"
            >
              <v-list-item-title>{{ locale.text }}</v-list-item-title>
            </v-list-item>
          </v-list>
        </v-menu> -->

        <!-- login form -->
        <v-form
          v-model="valid"
          class="login-form__form"
          ref="form"
          lazy-validation
          @submit.prevent
        >
          <v-text-field
            :label="$t('login.username')"
            v-model="user"
            :rules="loginRules"
            required
          ></v-text-field>
          <v-text-field
            :label="$t('login.password')"
            v-model="password"
            :rules="passwordRules"
            :counter="30"
            required
            :append-icon="passAppendIcon"
            @click:append="() => (passwordHidden = !passwordHidden)"
            :type="passTextFieldType"
            class="mb-5"
            @keypress.enter="loginAttempt()"
          ></v-text-field>
          <v-row>
            <v-spacer />
            <v-btn
              v-if="redirectBack"
              text
              @click="goBack()"
              :disabled="!enableRedirect"
              >{{ $t("login.go_back") }}</v-btn
            >
            <v-btn
              @click="loginAttempt()"
              :disabled="!valid"
              class="primary white--text"
              >{{ $t("login.submit") }}</v-btn
            >
          </v-row>
        </v-form>
      </template>
    </div>
  </div>
</template>

<script lang="ts">
import { Component, Vue, Prop } from "vue-property-decorator";
import auth, { DISPATCH } from "./store";

@Component
export default class LoginForm extends Vue {
  @Prop({ default: true })
  enableRedirect?: boolean;

  @Prop({ default: true })
  showLogo?: boolean;

  @Prop({ default: "vue-crud-sm.png" })
  logo?: string;

  @Prop({ default: false })
  localeSelectable?: boolean;

  valid = false;
  password = "";
  user = "";
  passwordHidden = true;
  alphanumericRegex = /^[a-zA-Z0-9]+$/;
  emailRegex = /^\w+([.-]?\w+)*@\w+([.-]?\w+)*(\.\w{2,3})+$/;

  get loginWait() {
    return auth.state.loginWait;
  }

  get loginFailed() {
    return auth.state.loginFailed;
  }

  get loginRules() {
    return [
      (v: string) => !!v || this.$t("global.login.loginRequired"),
      (v: string) =>
        this.alphanumericRegex.test(v) || this.$t("global.login.incorrectLogin")
    ];
  }

  get passwordRules() {
    return [
      (v: string) => !!v || this.$t("global.login.passwordRequired"),
      (v: string) =>
        this.alphanumericRegex.test(v) ||
        this.$t("global.login.incorrectPassword")
    ];
  }
  get credential() {
    const credentials = { username: this.user, password: this.password };
    return credentials;
  }

  get passTextFieldType() {
    return this.passwordHidden ? "password" : "text";
  }

  get passAppendIcon() {
    return this.passwordHidden ? "visibility" : "visibility_off";
  }

  get redirect() {
    return (this.$route.query.redirect as string) || "/";
  }

  get redirectBack() {
    return (this.$route.query.back as string) || "";
  }

  goBack() {
    this.$router.push({ path: this.redirectBack });
  }

  loginAttempt() {
    if (!this.valid) {
      return;
    }
    DISPATCH("Login", this.credential).then(() => {
      if (this.enableRedirect) {
        this.$router.push({ path: this.redirect });
      }
    });
  }
}
</script>
