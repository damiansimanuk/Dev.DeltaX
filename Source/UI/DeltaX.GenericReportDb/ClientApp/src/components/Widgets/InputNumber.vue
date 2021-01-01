<template>
  <v-text-field
    v-model.number="valNumber"
    @input="updateNumber($event)"
    :label="label"
    :outlined="outlined"
    :dense="dense"
    :disabled="disabled"
    :readonly="readonly"
    :append-icon="!disabled && !readonly ? 'add' : ''"
    @click:append="increment(step || 1)"
    :prepend-inner-icon="!disabled && !readonly ? 'remove' : ''"
    @click:prepend-inner="increment(-(step || 1))"
    :rules="numberRules"
    v-bind="extraBind ? extraBind : {}"
    :class="extraClass ? extraClass : {}"
  />
</template>

<script lang="ts">
import { Vue, Component, Prop, Watch } from "vue-property-decorator";

@Component({ name: "InputNumber" })
export default class InputNumber extends Vue {
  @Prop({ default: 0 }) value?: number | string;
  @Prop({ default: "" }) label?: string;
  @Prop({ default: 1 }) step?: number;
  @Prop({ default: undefined }) min?: number;
  @Prop({ default: undefined }) max?: number;
  @Prop({ default: false }) required?: boolean;
  @Prop({ default: false }) outlined?: boolean;
  @Prop({ default: false }) dense?: boolean;
  @Prop({ default: false }) disabled?: boolean;
  @Prop({ default: false }) readonly?: boolean;
  @Prop({ default: undefined }) extraBind?: object;
  @Prop({ default: undefined }) extraClass?: object;

  valNumber?: number | string = 0;

  get propMin() {
    return this.min || 0;
  }

  get propMax() {
    return this.max || 0;
  }

  get numberRules() {
    const r: unknown[] = [];
    if (this.disabled == true || this.readonly == true) return r;

    r.push(
      (v: string | number) =>
        v == "" || !isNaN(v as number) || "Is not a number"
    );
    if (this.required)
      r.push((v: string | number) => v !== "" || "Value is required");
    if (this.min != null)
      r.push(
        (v: string | number) =>
          v == "" || v >= this.propMin || `Min ${this.propMin}`
      );
    if (this.max != null) {
      r.push(
        (v: string | number) =>
          v == "" || v <= this.propMax || `Max ${this.propMax}`
      );
    }
    return r;
  }

  @Watch("value", { immediate: true })
  protected onValueChanged(newValue: number | string) {
    // console.log("onValueChanged " + oldValue + " to " + newValue);
    if (newValue === "" || newValue == null || isNaN(newValue as number)  ) {
      this.valNumber = undefined;
    } else {
      this.valNumber = Number(newValue);
    }
  }

  round(num: number) {
    const offset: number = +(this.min || 0);
    const increment: number = +(this.step || 1);
    return Math.round((num - offset) / increment) * increment + offset;
  }

  updateNumber(newValue: string | number) {
    // console.log("updateNumber:", newValue, this.valNumber);
    const num = Number(newValue);
    if (!isNaN(num)) {
      this.valNumber = num;
      this.$emit("input", num);
    }
  }

  increment(step: number) {
    let num = +Number(this.valNumber) + +step;
    if (!isNaN(num)) {
      if (this.max != undefined && num > this.max) num = this.max;
      if (this.min != undefined && num < this.min) num = this.min;
      this.updateNumber(this.round(num));
    }
  }
}
</script>
