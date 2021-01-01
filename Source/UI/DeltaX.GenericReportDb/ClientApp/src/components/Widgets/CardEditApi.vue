<template>
  <v-card id="carditemedit">
    <v-card-title primary-title class="headline info darkent-2">
      <span style="color:white">
        {{ createItem ? "Create New Item" : "Edit Item" }}
      </span>
      <v-spacer />
      <v-tooltip top>
        <template v-slot:activator="{ on }">
          <v-icon
            color="orange lighten-2"
            v-on="on"
            dark
            left
            @click="showConfig = 1"
          >
            mdi-eye
          </v-icon>
        </template>
        <span>
          Show config of widgets
        </span>
      </v-tooltip>
      <v-tooltip top>
        <template v-slot:activator="{ on }">
          <v-icon
            color="grey lighten-1"
            v-on="on"
            dark
            right
            @click="showConfig = 2"
          >
            mdi-eye
          </v-icon>
        </template>
        <span>
          Show values
        </span>
      </v-tooltip>
    </v-card-title>
    <v-card-text>
      <v-form ref="form" v-model="valid">
        <v-container ma-0 pa-0 pt-8>
          <v-layout
            ma-0
            pa-0
            class="d-flex align-content-start flex-wrap"
            justify-center
          >
            <v-flex
              v-for="(item, idx) in editableFields"
              v-bind="item.bindFlex"
              :key="`${item.field}-${idx}`"
            >
              <component
                class="mx-3 my-2"
                :is="getComponent(item)"
                v-model="editableValues[item.field || '']"
                :label="item.label || item.field"
                v-bind="item"
                :item-parent="editableValues"
                :base-config="config"
              />
            </v-flex>
          </v-layout>
        </v-container>
      </v-form>

      <!-- <pre>{{ config }} </pre> -->
      <!-- <pre>{{ editableValues }} </pre> -->
      <!-- <pre>{{ editableFields }} </pre> -->
    </v-card-text>

    <v-card-actions>
      <v-spacer></v-spacer>
      <v-btn
        v-if="createItem"
        :disabled="!valid"
        color="info darken-1"
        outlined
        tile
        @click="onCreate()"
      >
        Create <v-icon right>mdi-content-save</v-icon>
      </v-btn>
      <v-btn
        v-else
        :disabled="!valid"
        color="info darken-1"
        outlined
        tile
        @click="onUpdate()"
      >
        Update <v-icon>mdi-content-save</v-icon>
      </v-btn>
      <v-btn color="darken-1" outlined tile @click="onClose()">
        Cancel <v-icon right>mdi-close</v-icon>
      </v-btn>
    </v-card-actions>

    <v-dialog
      :value="showConfig != null"
      max-width="700"
      scrollable
      @input="showConfig = null"
    >
      <v-card v-if="showConfig != null">
        <v-card-title>
          {{ showConfig == 1 ? "Widgets Configuration" : "Item Values" }}
        </v-card-title>
        <v-card-text>
          <pre>{{ showConfig == 1 ? editableFields : editableValues }}</pre>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="darken-1" text @click="showConfig = null">
            close <v-icon>mdi-close</v-icon>
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-card>
</template>

<script lang="ts">
import { Vue, Component, Prop, Watch } from "vue-property-decorator";
import InputNumber from "./InputNumber.vue";
import InputText from "./InputText.vue";
import InputFile from "./InputFile.vue";
import InputCheckbox from "./InputCheckbox.vue";
import InputSelect from "./InputSelect.vue";
import InputDataTable from "./InputDataTable.vue";
import { Widget, EndpointConfiguration } from "@/Interfaces/crud";

const FIELDS = ["Number", "Text", "File", "Checkbox", "Select", "DataTable"];

@Component({
  name: "CardEditApi",
  components: {
    InputNumber,
    InputText,
    InputFile,
    InputCheckbox,
    InputSelect,
    InputDataTable
  }
})
export default class CardEditApi extends Vue {
  @Prop({ default: null }) config?: EndpointConfiguration;
  @Prop({ default: null }) editItem?: { [key: string]: object };
  @Prop({ default: null }) widgetsConfig?: Widget[];
  @Prop({ default: false }) createItem?: boolean;

  valid = false;
  editableFields: Widget[] = [];
  editableValues: { [key: string]: object | null } = {};
  showConfig: number | null = null;

  get form(): Vue & { validate: () => boolean } {
    return this.$refs.form as Vue & { validate: () => boolean };
  }

  getComponent(item: Widget) {
    let type = item.type || "Text";
    if (!FIELDS.includes(type)) type = "Text";
    return `Input${type}`;
  }

  updateValues() {
    this.$Notify.setLoading("CardEditApi");

    this.editableFields = [];
    const editItem = this.editItem || {};
    this.editableValues = { ...editItem };

    let wdef = this.widgetsConfig;
    if (wdef == null) {
      wdef = this.createItem
        ? this.config?.widgetsOnCreate
        : this.config?.widgetsOnEdit;
    }

    let i = 1;
    wdef?.forEach(f => {
      let w = this.config?.widgets
        ? this.config?.widgets[f.widgetName || ""] || {}
        : ({} as Widget);
      w = Object.assign({}, w, f);
      const fName = w.field || w.widgetName || `unknow-${i++}`;

      this.editableValues[fName] =
        editItem[fName] != null
          ? editItem[fName]
          : w.default != null
          ? w.default
          : null;

      this.editableFields.push(Object.assign({}, w, { field: fName }));
    });

    this.editableValues = Object.assign({}, this.editableValues);

    setTimeout(() => {
      this.form.validate();
      this.$Notify.setLoading("CardEditApi", false);
    }, 200);
  }

  mounted() {
    this.updateValues();
  }

  @Watch("config", { immediate: true })
  protected onConfigChanged() {
    this.updateValues();
  }

  @Watch("widgetsConfig", { immediate: true })
  protected onWidgetConfigChanged() {
    this.updateValues();
  }

  @Watch("editItem", { immediate: true })
  protected onEditItemChanged() {
    this.updateValues();
  }

  onUpdate() {
    console.log("onUpdate", this.editableValues);
    this.$emit("update", this.editableValues, this.editableFields);
  }

  onCreate() {
    console.log("onCreate", this.editableValues);
    this.$emit("create", this.editableValues, this.editableFields);
  }

  onClose() {
    this.$emit("close");
  }
}
</script>
