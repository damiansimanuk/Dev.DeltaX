<template>
  <v-file-input
    :accept="accept"
    v-model="fileupload"
    :prepend-icon="null"
    prepend-inner-icon="mdi-paperclip"
    :label="label"
    :outlined="outlined"
    :dense="dense"
    :disabled="disabled"
    :readonly="readonly"
    :rules="rules"
    v-bind="extraBind ? extraBind : {}"
    :class="extraClass ? extraClass : {}"
    :show-size="1000"
    :truncate-length="100"
    @change="onChange"
  />
</template>

<script lang="ts">
import { Vue, Component, Prop, Watch } from "vue-property-decorator";

@Component({ name: "InputFile" })
export default class InputFile extends Vue {
  @Prop({ default: null }) value?: File;
  @Prop({ default: "" }) label?: string;
  @Prop({ default: "*" }) accept?: string;
  @Prop({ default: null }) min?: number;
  @Prop({ default: null }) max?: number;
  @Prop({ default: false }) required?: boolean;
  @Prop({ default: true }) outlined?: boolean;
  @Prop({ default: true }) dense?: boolean;
  @Prop({ default: false }) disabled?: boolean;
  @Prop({ default: false }) readonly?: boolean;
  @Prop({ default: null }) extraBind?: object;
  @Prop({ default: null }) extraClass?: object;

  fileupload: File | null = null;
  fileName?: string;

  get propMin() {
    return this.min || 0;
  }

  get propMax() {
    return this.max || 0;
  }

  get rules() {
    const r: unknown[] = [];
    if (this.disabled == true || this.readonly == true) return r;

    if (this.required) r.push((v?: File) => !!v || "Value is required");
    if (this.min)
      r.push(
        (v?: File) => !v || v.size >= this.propMin || `Min ${this.propMin}`
      );
    if (this.max)
      r.push(
        (v?: File) => !v || v.size <= this.propMax || `Max ${this.propMax}`
      );

    return r;
  }

  onChange(file: File | null) {
    this.fileName = file ? file.name : undefined;
    this.fileupload = file;
    console.log("onChange", this.$data);
    this.$emit("input", this.fileupload);
  }

  @Watch("value", { immediate: true })
  protected onValueChanged(file: File | null) {
    this.fileupload = file;
    this.fileName = file ? file.name : undefined;
  }
}
</script>
