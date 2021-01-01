<template>
  <v-text-field
    v-model="valueText"
    :label="label"
    :outlined="outlined"
    :dense="dense"
    :disabled="disabled"
    :readonly="readonly"
    :rules="rules"
    @input="updateValue()"
    v-bind="extraBind ? extraBind : {}"
    :class="extraClass ? extraClass : {}"
  />
</template>

<script lang="ts">
import { Vue, Component, Prop, Watch } from "vue-property-decorator";

@Component({ name: "InputText" })
export default class InputText extends Vue {
  @Prop({ default: 0 }) value?: number | string;
  @Prop({ default: "" }) label?: string;
  @Prop({ default: undefined }) min?: number;
  @Prop({ default: undefined }) max?: number;
  @Prop({ default: false }) isAlphanumeric?: boolean;
  @Prop({ default: false }) isEmail?: boolean;
  @Prop({ default: false }) isWord?: boolean;
  @Prop({ default: false }) trim?: boolean;
  @Prop({ default: undefined }) regex?: string;
  @Prop({ default: false }) required?: boolean;
  @Prop({ default: false }) outlined?: boolean;
  @Prop({ default: false }) dense?: boolean;
  @Prop({ default: false }) disabled?: boolean;
  @Prop({ default: false }) readonly?: boolean;
  @Prop({ default: undefined }) extraBind?: object;
  @Prop({ default: undefined }) extraClass?: object;

  valueText = "";
  wordsRegex = /^\w[\w\s,.]*[\w.]$/;
  alphanumericRegex = /^[a-zA-Z0-9]+$/;
  emailRegex = /^\w+([.-]?\w+)*@\w+([.-]?\w+)*(\.\w{2,3})+$/;
  trimRegex = /^[^ ]([\w\W ]*[^ ])?$/;

  get rules() {
    const r: unknown[] = [];

    if (this.disabled == true || this.readonly == true) return r;

    if (this.required) r.push((v: string) => v !== "" || "Value is required");
    if (this.min != undefined)
      r.push(
        (v: string) =>
          v == "" || !this.min || v.length >= this.min || `Min ${this.min}`
      );
    if (this.max != undefined)
      r.push(
        (v: string) =>
          v == "" || !this.max || v.length <= this.max || `Max ${this.max}`
      );
    if (this.isAlphanumeric)
      r.push(
        (v: string) =>
          v == "" || this.alphanumericRegex.test(v) || "Alphanumeric required"
      );
    if (this.isEmail)
      r.push(
        (v: string) => v == "" || this.emailRegex.test(v) || "Email required"
      );

    if (this.isWord)
      r.push(
        (v: string) =>
          v == "" || this.wordsRegex.test(v) || "Words in paragraph required"
      );

    if (this.trim)
      r.push(
        (v: string) =>
          v == "" ||
          this.trimRegex.test(v) ||
          "End and start without spaces required"
      );

    if (this.regex) {
      try {
        const rgx = new RegExp(this.regex);
        r.push(
          (v: string) =>
            v == "" || rgx.test(v) || `Regex required ${this.regex}`
        );
      } catch (e) {
        console.log("bad regex", e);
      }
    }
    return r;
  }

  updateValue() {
    this.$emit("input", this.valueText);
  }

  @Watch("value", { immediate: true })
  private onValueChange(val: string) {
    this.valueText = val || "";
  }
}
</script>
