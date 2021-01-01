<template>
  <div id="generic-api">
    <div style="position: relative;">
      <v-flex class="ma-2" style="min-height:40px;">
        <label class="headline">{{ label }}</label>
      </v-flex>
      <DataTableApi :baseConfig="config" :height="'calc(100vh - 120px)'">
        <template slot="header-right-append" slot-scope="{ headers, rows }">
          <v-tooltip bottom>
            <template v-slot:activator="{ on }">
              <v-btn
                v-on="on"
                fab
                color="green darken-4"
                dark
                depressed
                small
                class="mx-2"
                @click="
                  onExportExcel(rows, headers, `export_${configName}.xlsx`)
                "
              >
                <v-icon>save_alt</v-icon>
              </v-btn>
            </template>
            <span>Export Excel</span>
          </v-tooltip>
        </template>
      </DataTableApi>
    </div>
  </div>
</template>

<script lang="ts">
import { Vue, Component, Watch } from "vue-property-decorator";
import XLSX from "xlsx";
import DataTableApi from "@/components/Widgets/DataTableApi.vue";
import crud from "@/api/crud";
import { EndpointConfiguration } from "@/Interfaces/crud";

@Component({
  name: "GenericTableApi",
  components: {
    DataTableApi
  }
})
export default class GenericTableApi extends Vue {
  config: EndpointConfiguration | null = null;
  label = "";

  get configName(): string {
    return this.$route.params.configName || "";
  }

  @Watch("configName", { immediate: true })
  protected onConfigNameChanged() {
    this.findConfig();
  }

  async findConfig() {
    this.config = await crud.GetConfig(this.configName);
    this.label = this.config.displayName || "";
    console.log("***** findConfig", this.config);
  }

  onExportExcel(rows = [], headers: any[] = [], fileName = "book.xlsx") {
    const headersFiltered = headers.filter(h => h.value != "actions");
    const headersText = headersFiltered.map(h => h.text);

    // Add only headers
    const rowsWs = XLSX.utils.json_to_sheet([], {
      header: headersText
    });

    // Map only need rows
    const data = rows.map(r => {
      const row = [];
      for (const h of headersFiltered) {
        row.push(r[h.value] !== undefined ? r[h.value] : null);
      }
      return row;
    });

    // Append rows
    XLSX.utils.sheet_add_json(rowsWs, data, {
      skipHeader: true,
      origin: "A2"
    });

    // Generate book
    const wb = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, rowsWs, "data");

    // And write
    XLSX.writeFile(wb, fileName);
  }
}
</script>

<style scoped>
.v-application--is-ltr .v-data-table--fixed-header .v-data-footer {
  margin-right: 1px;
}

.v-application--is-ltr .v-data-footer__select .v-select {
  margin: 3px 0 3px 10px;
}
</style>
