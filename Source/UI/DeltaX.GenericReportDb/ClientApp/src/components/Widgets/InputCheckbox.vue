<template>
  <v-checkbox
    v-model="valueCheck"
    @change="onChange($event)"
    :label="label"
    :dense="dense"
    :disabled="disabled"
    :readonly="readonly"
    :indeterminate="value == null"
    :rules="rules"
    v-bind="extraBind ? extraBind : {}"
    :class="extraClass ? extraClass : {}"
  />
</template>

<script lang="ts">
import { Vue, Component, Prop, Watch } from "vue-property-decorator";

@Component({ name: "InputCheckbox" })
export default class InputCheckbox extends Vue {
  @Prop({ default: null }) value?: number | string | boolean;
  @Prop({ default: "" }) label?: string;
  @Prop({ default: undefined }) requiredValue?: boolean;
  @Prop({ default: false }) required?: boolean;
  @Prop({ default: false }) dense?: boolean;
  @Prop({ default: false }) disabled?: boolean;
  @Prop({ default: false }) readonly?: boolean;
  @Prop({ default: undefined }) extraBind?: object;
  @Prop({ default: undefined }) extraClass?: object;

  valueCheck: boolean | null = null;

  get rules() {
    const r: unknown[] = [];

    if (this.disabled == true || this.readonly == true) return r;

    if (this.required)
      r.push((v: boolean) => v === true || v === false || `Value is required`);
    if (this.requiredValue === true)
      r.push((v: boolean) => v == true || "Checked is required");
    if (this.requiredValue === false)
      r.push((v: boolean) => v == false || "Unchecked is required");

    return r;
  }

  onChange(val: boolean) {
    console.log("*** onChange", val, this.valueCheck);
    this.valueCheck = val;
    this.$emit("input", this.valueCheck);
  }

  @Watch("value", { immediate: true })
  private onValueChange(val: string | boolean | null) {
    console.log("--- onValueChange", val, this.valueCheck);
    this.valueCheck = val != null ? Boolean(val) : null;
  }
}
</script>
