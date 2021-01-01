<template>
  <div style="position:fixed; top:46px; right:0; z-index: 1000; ">
    <v-layout class="d-flex ma-0 pa-0" v-if="$Notify.alerts.length > 0">
      <v-scale-transition class="py-0" group>
        <template v-for="a in $Notify.alerts">
          <div :key="a.id" style="display: flex; flex-direction: row-reverse;">
            <v-alert
              max-width="700"
              min-width="10"
              class="ma-2 text-left"
              style="z-index: 1200;"
              :type="a.type"
              :color="a.color"
              elevation="2"
              light
              border="left"
              colored-border
              dense
              dismissible
              v-bind="a.bind ? a.bind : {}"
              @input="confirmOnPress(a, 0)"
            >
              <v-row>
                <v-col :class="a.type ? 'ma-0 pa-0 pr-2' : ''">
                  <h4 class="headline-2" v-if="a.title" v-text="a.title" />
                  <div v-if="a.html" v-html="a.html" />
                  <div v-else-if="a.text" v-text="a.text" />
                </v-col>
              </v-row>
              <v-progress-linear
                v-if="a.timeout && a.timeout > 1000"
                :color="a.color || a.type"
                height="2"
                :value="((datetime - a.timestart) / a.timeout) * 100"
              />
            </v-alert>
          </div>
        </template>
      </v-scale-transition>
    </v-layout>

    <v-dialog
      :value="$Notify.confirmations.length > 0"
      persistent
      max-width="700"
      class="elevation-0 pa-2 ma-2"
    >
      <v-slide-y-transition class="py-0" group>
        <template v-for="(a, i) in $Notify.confirmations">
          <v-alert
            :key="a.id"
            class="ma-0 text-center"
            :class="i > 0 ? 'mt-1' : ''"
            :type="a.type"
            :color="a.color"
            elevation="4"
            light
            border="bottom"
            colored-border
            v-bind="a.bind ? a.bind : {}"
          >
            <v-card elevation="0">
              <h3 class="headline" v-if="a.title" v-text="a.title" />
              <div v-if="a.html" v-html="a.html" />
              <div v-else-if="a.text" v-text="a.text" />

              <v-spacer class="ma-4"></v-spacer>
              <v-card-actions class="ma-0 pa-0">
                <v-spacer></v-spacer>

                <template v-if="a.buttons">
                  <v-btn
                    v-for="(b, i) in a.buttons"
                    :key="i"
                    @click="confirmOnPress(a, 1 + i)"
                    text
                    outlined
                    tile
                    v-bind="b.bind ? b.bind : {}"
                  >
                    {{ b.text }}
                    <v-icon b.icon right v-text="b.icon" />
                  </v-btn>
                </template>
                <v-btn @click="confirmOnPress(a, 0)" text outlined tile>
                  cancel
                  <v-icon right>mdi-close</v-icon>
                </v-btn>
              </v-card-actions>
            </v-card>
          </v-alert>
        </template>
      </v-slide-y-transition>
    </v-dialog>

    <v-overlay :value="$Notify.loading">
      <v-progress-circular indeterminate size="64"></v-progress-circular>
    </v-overlay>
  </div>
</template>

<script lang="ts">
/* eslint-disable @typescript-eslint/no-explicit-any */
import Vue from "vue";
import {
  IMessage,
  IMessageAlert,
  IMessageConfirm,
  INotify
} from "@/Interfaces/notify";

const loadingRutes = new Set();

const Notify = new Vue({
  data: () => ({
    confirmations: [] as Array<IMessageConfirm>,
    alerts: [] as Array<IMessageAlert>,
    datetime: Date.now(),
    interval: 0 as any,
    loading: false
  }),

  computed: {},

  beforeDestroy() {
    clearInterval(this.interval);
  },

  created() {
    this.interval = setInterval(() => {
      this.datetime = Date.now();
    }, 100);
  },

  methods: {
    getId(type: string): string {
      return `${Date.now()}${type}${this.alerts.length}`;
    },

    setLoading(name = "unknow", loading = true) {
      if (loading) {
        loadingRutes.add(name);
        this.loading = loadingRutes.size != 0;
      } else {
        this.clearLoading(name);
      }
    },

    clearLoading(name = "unknow") {
      loadingRutes.delete(name);
      setTimeout(() => {
        this.loading = loadingRutes.size != 0;
      }, 300);
    },

    confirm(msg: IMessage) {
      const c =
        msg.bind && msg.bind.color
          ? msg.bind.color
          : msg.type
          ? msg.type + " darken-2"
          : undefined;

      return new Promise<number>((resolve, reject) => {
        const m: IMessageConfirm = {
          id: this.getId(msg.type || ""),
          color: c,
          ...msg,
          resolve: resolve,
          reject: reject
        };
        this.confirmations.splice(0, 0, m);
      });
    },

    notify(msg: IMessage): Promise<number> {
      console.log("msg", msg);
      const c =
        msg.bind && msg.bind.color
          ? msg.bind.color
          : msg.type
          ? msg.type + " darken-2"
          : undefined;
      return new Promise<number>((resolve, reject) => {
        const m: IMessageAlert = {
          id: this.getId(msg.type || ""),
          color: c,
          ...msg,
          timeout: msg.timeout || 0,
          timestart: this.datetime,
          resolve: resolve,
          reject: reject
        };

        this.alerts.splice(0, 0, m);
        if (m.timeout || 0 > 200) {
          setTimeout(() => {
            m.resolve(-1);
            this.deleteItem(m);
          }, m.timeout);
        }
      });
    },

    info(text: string, options = {} as IMessage) {
      return this.notify({
        ...{
          type: "info",
          text: text
        },
        ...options
      });
    },

    success(text: string, options = {} as IMessage) {
      return this.notify({
        ...{
          type: "success",
          text: text
        },
        ...options
      });
    },

    error(text: string, options = {} as IMessage) {
      return this.notify({
        ...{
          type: "error",
          text: text
        },
        ...options
      });
    },

    warning(text: string, options = {} as IMessage) {
      return this.notify({
        ...{
          type: "warning",
          text: text
        },
        ...options
      });
    },

    deleteAll() {
      this.alerts = [];
      this.confirmations = [];
    },

    deleteItem(item: IMessage) {
      this.alerts = this.alerts.filter(i => i != item);
      this.confirmations = this.confirmations.filter(i => i != item);
    }
  }
});

window.Notify = Notify;

// EXPORT Vue.prototype.$Notify
declare module "vue/types/vue" {
  interface Vue {
    $Notify: INotify;
  }
}
Vue.prototype.$Notify = Notify;

export default Vue.extend({
  name: "Notify",
  computed: {
    datetime: function() {
      return this.$Notify.datetime;
    }
  },

  methods: {
    confirmOnPress(m: any, btn: number) {
      console.log("confirmOnPress", m, btn);
      m.resolve(btn);
      Notify.deleteItem(m);
    }
  }
});
</script>
