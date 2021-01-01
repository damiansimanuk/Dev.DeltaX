<template>
  <div id="input-table-list">
    <div :class="{ 'field-wrapper': !!label }" style="position: relative;">
      <v-card class="ma-0 pa-2 v-text-field" outlined>
        <label v-if="label" class="v-label v-label--active theme--light">{{
          label
        }}</label>
        <DataTableApi
          :baseConfig="baseConfigMerged"
          :itemParent="itemParentMerge"
          :searchItemsParams="searchItemsParamsMerge"
          :dense="dense"
          :height="height"
          :hide-footer="hideFooter"
          :extraBind="extraBind"
          :extraClass="extraClass"
        />
      </v-card>
    </div>
  </div>
</template>

<script lang="ts">
import crud from "@/api/crud";
import { EndpointConfiguration, TableLoad } from "@/Interfaces/crud";
import { Vue, Component, Prop, Watch } from "vue-property-decorator";
import DataTableApi from "./DataTableApi.vue";

@Component({ name: "InputDataTable", components: { DataTableApi } })
export default class InputDataTable extends Vue {
  @Prop({ default: "" }) label?: string;
  @Prop({ default: null }) baseConfig?: EndpointConfiguration;
  @Prop({ default: null }) loadItems?: TableLoad;
  @Prop({ default: null }) itemParent?: { [key: string]: object };
  @Prop({ default: null }) searchItemsParams?: { [key: string]: object };
  @Prop({ default: false }) btnFilter?: boolean;
  @Prop({ default: false }) dense?: boolean;
  @Prop({ default: false }) hideFooter?: boolean;
  @Prop({ default: null }) height?: number | string; // default: 'calc(100vh - 167px)'
  @Prop({ default: null }) extraBind?: object;
  @Prop({ default: null }) extraClass?: object;

  baseConfigMerged: EndpointConfiguration | null = null;

  get itemParentMerge() {
    return this.mergeItemParentProperty(this.itemParent || {});
  }

  get searchItemsParamsMerge() {
    return this.mergeItemParentProperty(this.searchItemsParams || {});
  }

  mergeItemParentProperty(item?: { [key: string]: object }) {
    const result = { ...item };
    if (this.loadItems != null) {
      const itemParent = this.itemParent || {};
      Object.keys(this.loadItems?.mapItemParent).forEach(k => {
        const v = this.loadItems?.mapItemParent[k] || "";
        result[k] = itemParent[v];
      });
    }
    return result;
  }

  @Watch("configName", { immediate: false })
  protected onConfigNameChanged() {
    this.loadBaseConfigMerged();
  }

  @Watch("baseConfig", { immediate: false })
  protected onBaseConfigChanged() {
    this.loadBaseConfigMerged();
  }

  @Watch("loadItems", { immediate: true, deep: true })
  protected onLoadItemsChanged() {
    this.loadBaseConfigMerged();
  }

  async loadBaseConfigMerged() {
    let configName = this.loadItems?.configName || null;

    let config = this.baseConfig;
    if (configName && configName != config?.name) {
      config = await crud.GetConfig(configName);
    } else {
      configName = this.baseConfig?.name || "";
    }

    this.baseConfigMerged = Object.assign(
      { name: configName },
      config,
      this.loadItems
    );

    console.log("InputDataTable loadBaseConfigMerged", configName, this.baseConfigMerged);
  }
}
</script>

<style scoped>
.field-wrapper {
  margin-top: 10px;
}

.field-wrapper__label {
  font-size: 12px;
  color: #777;
}

.v-application--is-ltr .v-data-table--fixed-header .v-data-footer {
  margin-right: 1px;
}

.v-application--is-ltr .v-data-footer__select .v-select {
  margin: 3px 0 3px 5px;
}
</style>
