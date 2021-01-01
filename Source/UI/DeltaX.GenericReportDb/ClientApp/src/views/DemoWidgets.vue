<template>
  <v-form ref="form">
    <v-container fluid class="d-flex flex">
      <v-row>
        <v-col col="12" md="6" sm="12">
          <v-card class="pa-5">
            <v-card-title>
              InputNumber value: {{ inputNumberValue1 }}
            </v-card-title>
            <InputNumber
              :min="0"
              :max="100"
              v-model="inputNumberValue1"
              step="0.5"
            />
          </v-card>
        </v-col>

        <v-col col="12" md="6" sm="12">
          <v-card class="pa-5">
            <v-card-title>
              InputText value: {{ inputTextValue1 }}
            </v-card-title>
            <InputText :max="10" v-model="inputTextValue1" />
          </v-card>
        </v-col>

        <v-col col="12" md="6" sm="12">
          <v-card class="pa-5">
            <v-card-title>
              InputCheckbox value: '{{
                inputCheckboxValue1 == null ? "NULL" : inputCheckboxValue1
              }}'
            </v-card-title>
            <InputCheckbox
              v-model="inputCheckboxValue1"
              label="No requerido"
              :dense="true"
            />
            <InputCheckbox
              v-model="inputCheckboxValue1"
              :required="true"
              :dense="true"
              label="Requerido"
            />
            <InputCheckbox
              :requiredValue="true"
              :dense="true"
              v-model="inputCheckboxValue1"
              label="Requiere Checked"
            />
            <InputCheckbox
              :requiredValue="false"
              :dense="true"
              v-model="inputCheckboxValue1"
              label="Requiere Unchecked"
            />
          </v-card>
        </v-col>

        <v-col col="12" md="6" sm="12">
          <v-card class="pa-5">
            <v-text-field
              readonly
              :value="
                `InputFile value: ${
                  inputFileValue1 ? inputFileValue1.name : '----'
                }`
              "
            />
            <InputFile v-model="inputFileValue1" @input="previewFile($event)" />
            previewSrc:
            <pre
              style="max-height: 200px; max-width: 500px; overflow: scroll;"
              >{{ previewSrc }}</pre
            >
            <v-img :src="previewSrc" width="200" height="200" />
          </v-card>
        </v-col>

        <v-col col="12" md="6" sm="12">
          <v-card class="pa-5">
            <v-text-field
              readonly
              :value="`InputNumber value: ${inputNumberValue1}`"
            />
            <InputNumber
              :min="-3"
              :max="100"
              v-model="inputNumberValue1"
              step="0.5"
            />
          </v-card>
        </v-col>

        <v-col col="12" md="6" sm="12">
          <v-container>
            <h1>Demo Form</h1>

            <v-dialog
              width="900"
              scrollable
              :fullscreen="$vuetify.breakpoint.name == 'xs'"
            >
              <template v-slot:activator="{ on }">
                <v-btn color="red lighten-2" dark v-on="on">Click Me</v-btn>
              </template>
              <CardEditApi :config="config" :createItem="true" />
            </v-dialog>
          </v-container>
        </v-col>

        <v-col col="12" md="6" sm="12">
          <v-card class="pa-5">
            <v-card-title> InputSelect {{ inputSelectValue1 }} </v-card-title>
            <InputSelect v-model="inputSelectValue1" :items="'hola;nada'" />
          </v-card>
        </v-col>

        <v-col col="12" md="6" sm="12">
          <v-card class="pa-5">
            <v-card-title> InputSelect2 {{ inputSelectValue2 }} </v-card-title>
            <InputSelect
              v-model="inputSelectValue2"
              :items="inputSelectItems2"
              :itemText="'name'"
              :itemValue="'id'"
            />
          </v-card>
        </v-col>

        <v-col col="12" md="12" sm="12">
          <v-card class="pa-5">
            <v-card-title> CRUD TABLE {{ inputSelectValue2 }} </v-card-title>
            <InputDataTable :configName="'Users'" />
          </v-card>
        </v-col>
      </v-row>
    </v-container>
  </v-form>
</template>

<script lang="ts">
import { Vue, Component } from "vue-property-decorator";
import InputNumber from "@/components/Widgets/InputNumber.vue";
import InputText from "@/components/Widgets/InputText.vue";
import InputCheckbox from "@/components/Widgets/InputCheckbox.vue";
import InputFile from "@/components/Widgets/InputFile.vue";
import InputSelect from "@/components/Widgets/InputSelect.vue";
import CardEditApi from "@/components/Widgets/CardEditApi.vue";
import DataTableApi from "@/components/Widgets/DataTableApi.vue";
import InputDataTable from "@/components/Widgets/InputDataTable.vue";
import config from "./DemoConfig";

@Component({
  components: {
    InputNumber,
    InputText,
    InputCheckbox,
    InputFile,
    InputSelect,
    CardEditApi,
    DataTableApi,
    InputDataTable
  }
})
export default class DemoWidgets extends Vue {
  inputNumberValue1 = "1";
  inputTextValue1 = "hola mundo";
  inputCheckboxValue1?: boolean | null = null;
  inputFileValue1: File | null = null;
  inputSelectValue1 = "";
  inputSelectValue2 = "";
  inputSelectItems2 = [
    { name: "Hola Texto 2", id: 2 },
    { name: "Hola Texto 3", id: 3 }
  ];
  previewSrc: string | ArrayBuffer | null = null;
  config = config;

  mounted() {
    this.form.validate();
  }

  get form(): Vue & { validate: () => boolean } {
    return this.$refs.form as Vue & { validate: () => boolean };
  }

  async FileReadAsDataURL(file: File) {
    return new Promise<string | ArrayBuffer | null>((resolve, reject) => {
      const reader = new FileReader();
      reader.onload = (ev: ProgressEvent<FileReader>) => {
        const reader = ev.target as FileReader;
        resolve(reader.result);
      };
      reader.onerror = function() {
        reject();
      };
      reader.readAsDataURL(file);
    });
  }

  async previewFile(file: File | null) {
    console.log("previewFile", file);
    if (file) {
      this.previewSrc = await this.FileReadAsDataURL(file);
    } else {
      this.previewSrc = "";
    }
    console.log("previewSrc", this.previewSrc);
  }
}
</script>
