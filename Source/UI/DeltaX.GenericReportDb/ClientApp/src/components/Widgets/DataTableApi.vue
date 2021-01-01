<template>
  <div id="table-list-api">
    <div style="position: absolute; top:5px; right:5px; z-index: 10;">
      <v-flex>
        <slot name="header-right" :headers="headers" :rows="rows"></slot>
        <v-tooltip bottom>
          <template v-slot:activator="{ on }">
            <v-btn
              v-on="on"
              fab
              color="info darken-1"
              dark
              depressed
              :x-small="dense"
              :small="!dense"
              @click="dialogCreateItem()"
            >
              <v-icon>add</v-icon>
            </v-btn>
          </template>
          <span>Create Item</span>
        </v-tooltip>
        <slot name="header-right-append" :headers="headers" :rows="rows"></slot>
      </v-flex>
    </div>

    <v-data-table
      :headers="headers"
      :items="rows"
      :options.sync="options"
      :server-items-length="totalItems || undefined"
      fix-header
      fixed-header
      :dense="dense"
      :height="height"
      v-bind="extraBind"
      :class="extraClass"
      :hide-default-footer="hideFooter"
    >
      <template v-slot:top>
        <v-container class="ma-0 pa-0" style="position: relative;">
          <v-row class="ma-0 pa-0">
            <v-col cols="12" sm="6" class="ma-0 pa-0">
              <v-text-field
                v-if="btnFilter"
                style="max-width:200px"
                class="ma-2"
                append-icon="mdi-filter"
                label="Filter"
                single-line
                hide-details
              />
            </v-col>
          </v-row>
        </v-container>
      </template>

      <template v-slot:item.actions="{ item }">
        <v-tooltip bottom>
          <template v-slot:activator="{ on }">
            <v-icon
              v-on="on"
              :small="dense"
              color="blue darknet-5"
              class="mr-2"
              @click="dialogEditItem(item)"
              >edit</v-icon
            >
          </template>
          <span>Edit Item</span>
        </v-tooltip>

        <v-tooltip bottom>
          <template v-slot:activator="{ on }">
            <v-icon
              v-on="on"
              :small="dense"
              color="red darknet-5"
              @click="dialogDeleteItem(item)"
              >delete</v-icon
            >
          </template>
          <span>Delete Item</span>
        </v-tooltip>
      </template>
    </v-data-table>

    <v-dialog
      :value="editItem != null || createItem"
      width="900"
      scrollable
      persistent
      :fullscreen="$vuetify.breakpoint.name == 'xs'"
    >
      <CardEditApi
        v-if="editItem != null || createItem"
        :config="baseConfig"
        :edit-item="editItem"
        :create-item="createItem"
        @update="onUpdateItem"
        @create="onCreateItem"
        @close="onCloseDialog"
      />
    </v-dialog>
  </div>
</template>

<script lang="ts">
import { Vue, Component, Prop, Watch } from "vue-property-decorator";
import crud from "@/api/crud";
import { EndpointConfiguration, RequestConfig } from "@/Interfaces/crud";

@Component({
  name: "DataTableApi",
  components: {
    CardEditApi: () => import("./CardEditApi.vue")
  }
})
export default class DataTableApi extends Vue {
  @Prop({ default: null }) baseConfig?: EndpointConfiguration;
  @Prop({ default: null }) itemParent?: { [key: string]: object };
  @Prop({ default: null }) searchItemsParams?: { [key: string]: object };
  @Prop({ default: false }) btnFilter?: boolean;
  @Prop({ default: false }) dense?: boolean;
  @Prop({ default: false }) hideFooter?: boolean;
  @Prop({ default: null }) height?: number | string; // default: 'calc(100vh - 167px)'
  @Prop({ default: null }) extraBind?: object;
  @Prop({ default: null }) extraClass?: object;

  headers: object[] = [];
  rows = [];
  totalItems?: number = 0;
  options = {
    page: 1,
    itemsPerPage: 10,
    sortBy: [],
    sortDesc: [],
    groupBy: [],
    groupDesc: [],
    mustSort: false,
    multiSort: false
  };
  editItem: { [key: string]: object | null } | null = null;
  createItem = false;

  get editItemBase() {
    const obj = this.itemParent || {};
    return { ...obj };
  }

  get requestConfig(): RequestConfig {
    const endpointName = this.baseConfig?.prefixItem || "UnknowEndpointName";
    const endpoints = this.baseConfig?.endpoints || {};
    const endpoint = endpoints[endpointName];
    return {
      name: this.baseConfig?.name || "",
      prefixList: this.baseConfig?.prefixList || "",
      prefixItem: this.baseConfig?.prefixItem || null,
      primaryKeysDelimiter: this.baseConfig?.primaryKeysDelimiter || null,
      primaryKeys: endpoint.primaryKeys || null
    };
  }

  @Watch("baseConfig", { immediate: true })
  protected onConfigChanged() {
    this.Initialize();
    this.onCloseDialog();
  }

  @Watch("options", { deep: true })
  protected onOptionsChanged() {
    if (this.totalItems) {
      this.getRows();
    }
  }

  async Initialize() {
    this.generateHeader();
    this.getRows();
    console.log("**** DataTableApi Initialize baseConfig", this.baseConfig);
    console.log("**** DataTableApi Initialize itemParent", this.itemParent);
    console.log("**** DataTableApi Initialize searchItemsParams", this.searchItemsParams);
    console.log("**** DataTableApi Initialize rows", this.rows);
  }

  async getRows() {
    if (this.baseConfig == null) return;

    if (this.searchItemsParams) {
      const list = await crud.SearchItems(
        this.requestConfig,
        this.searchItemsParams
      );
      this.rows = list.rows;
      this.totalItems = undefined;
    } else {
      const list = await crud.GetItems(
        this.requestConfig,
        this.options.itemsPerPage,
        this.options.page
      );
      this.rows = list.rows;
      this.totalItems = list.totalRows > 0 ? list.totalRows : null;
    }
  }

  generateHeader() {
    if (this.baseConfig == null) return;

    const wdef = this.baseConfig?.widgets || {};
    this.headers = [];

    this.baseConfig.listFields?.forEach(f => {
      if (typeof f === "string") {
        const w = Object.values(wdef).find(w => w.field == f || w.label == f);
        this.headers.push({
          text: w?.label || f,
          value: w?.field || f,
          filterable: false,
          sortable: false
        });
      } else {
        this.headers.push({ filterable: false, sortable: false, ...f });
      }
    });

    this.headers.push({
      text: "Actions",
      width: "100px",
      value: "actions",
      align: "center",
      sortable: false
    });
  }

  dialogCreateItem() {
    this.editItem = Object.assign({}, this.editItemBase);
    this.createItem = true;
  }

  dialogEditItem(item: { [key: string]: object }) {
    if (this.baseConfig == null) return;
    const idx = this.rows.indexOf(item as never);

    console.log("**** dialogEditItem", this.baseConfig, item, this.rows[idx]);

    crud.GetItem(this.requestConfig, item).then(i => {
      this.editItem = Object.assign({}, this.editItemBase, i);
    });
  }

  async dialogDeleteItem(item: { [key: string]: object }) {
    if (this.baseConfig == null) return;

    const deleteItem = { ...this.editItemBase, ...item };
    const pks = crud.GetPrimaryKeysValues(this.requestConfig, deleteItem);

    const msg = {
      title: "Delete Item",
      text: `Desea eliminar el item ${JSON.stringify(pks)}?`,
      buttons: [
        {
          text: "Eliminar",
          icon: "mdi-delete",
          bind: { color: "orange darken-1" }
        }
      ],
      bind: {
        icon: "mdi-delete",
        color: "orange darken-1",
        prominent: true
      }
    };
    const p = await this.$Notify.confirm(msg);

    if (p == 1) {
      this.onDeleteItem(deleteItem);
    }
  }

  onCreateItem(item: { [key: string]: object }) {
    if (this.baseConfig == null) return;

    crud.CreateItem(this.requestConfig, item).then(() => {
      this.onCloseDialog();
      this.getRows();
    });
  }

  onUpdateItem(item: { [key: string]: object }) {
    if (this.baseConfig == null) return;

    console.log("DataTableApi onUpdateItem", item);

    crud.UpdateItem(this.requestConfig, item).then(() => {
      this.onCloseDialog();
      this.getRows();
    });
  }

  onDeleteItem(item: { [key: string]: object }) {
    if (this.baseConfig == null) return;

    crud.DeleteItem(this.requestConfig, item).then(() => {
      this.onCloseDialog();
      this.getRows();
    });
  }

  onCloseDialog() {
    this.createItem = false;
    this.editItem = null;
  }
}
</script>

<style scoped>
#table-list-api .v-data-table__wrapper {
  /* height: calc(100vh - 167px); */
  min-height: 100px;
}
</style>
