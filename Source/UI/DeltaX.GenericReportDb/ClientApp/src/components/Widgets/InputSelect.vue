<template>
  <v-select
    v-model="valueSelected"
    :items="itemsSelect"
    :label="label"
    :dense="dense"
    :disabled="disabled"
    :readonly="readonly"
    :rules="rules"
    :item-text="itemText || undefined"
    :item-value="itemValue || undefined"
    :return-object="itemValue == 'Object'"
    @input="onChange()"
    v-bind="extraBind ? extraBind : {}"
    :class="extraClass ? extraClass : {}"
  />
</template>

<script lang="ts">
import { Vue, Component, Prop, Watch } from "vue-property-decorator";
import crud from "@/api/crud";
import { EndpointConfiguration, SelectLoad } from "@/Interfaces/crud";

@Component({ name: "InputSelect" })
export default class InputSelect extends Vue {
  @Prop({ default: null }) value?: string | object[];
  @Prop({ default: "" }) label?: string;
  @Prop({ default: null }) baseConfig?: EndpointConfiguration;
  @Prop({ default: null }) items?: string | object[];
  @Prop({ default: null }) itemText?: string | object[];
  @Prop({ default: null }) itemValue?: string | object[];
  @Prop({ default: null }) itemsApi?: SelectLoad;
  @Prop({ default: null }) valuesApi?: SelectLoad;
  @Prop({ default: null }) itemParent?: { [key: string]: object };
  @Prop({ default: false }) required?: boolean;
  @Prop({ default: true }) dense?: boolean;
  @Prop({ default: false }) disabled?: boolean;
  @Prop({ default: false }) readonly?: boolean;
  @Prop({ default: null }) extraBind?: object;
  @Prop({ default: null }) extraClass?: object;

  valueSelected: string | object[] = [];
  itemsSelect: string[] | object[] = [];

  get rules() {
    const r: unknown[] = [];
    if (this.disabled == true || this.readonly == true) return r;

    if (this.required) r.push((v: object) => v != null || "Value is required");
    return r;
  }

  onChange() {
    console.log("onChange", this.valueSelected);
    this.$emit("input", this.valueSelected);
  }

  async loadItemsFromApi(configLoad: SelectLoad) {
    this.itemsSelect = await this.loadFromApi(configLoad);
  }

  async loadValuesFromApi(configLoad: SelectLoad) {
    this.valueSelected = await this.loadFromApi(configLoad);
  }

  async loadFromApi(configLoad: SelectLoad) {
    const params: { [key: string]: object } = {};
    const itemParent = this.itemParent || {};
    const mapItemParent = configLoad.mapItemParent || {};

    Object.keys(mapItemParent).forEach(k => {
      const v = mapItemParent[k];
      params[k] = itemParent[v] || null;
    });

    console.log("loadFromApi----", configLoad, params);

    const list = await crud.SearchItems(
      {
        name: configLoad.configName || this.baseConfig?.name || "NotConfigName",
        prefixList: configLoad.prefixList,
        prefixItem: null,
        primaryKeysDelimiter: null,
        primaryKeys: null
      },
      params
    );
    return list.rows;
  }

  async loadItemsFromBinding(items: string | object[]) {
    if (typeof items === "string") {
      this.itemsSelect = items.trim().split(/\s*[;,]\s*/);
    } else {
      this.itemsSelect = items;
    }
  }

  @Watch("value", { immediate: true })
  protected onValueChanged(val: string | object[]) {
    this.valueSelected = val;
  }

  @Watch("items", { immediate: true })
  protected onItemsChanged(items: string | object[]) {
    if (!items) return;
    this.loadItemsFromBinding(items);
  }

  @Watch("itemsApi", { immediate: true })
  protected onItemsApiChanged(configLoad?: SelectLoad) {
    if (!configLoad) return;
    this.loadItemsFromApi(configLoad);
  }

  @Watch("valuesApi", { immediate: true })
  protected onValuesApiChanged(configLoad?: SelectLoad) {
    if (!configLoad) return;
    this.loadValuesFromApi(configLoad);
  }
}
</script>
