import { EndpointConfiguration, ConfigNamesType, RequestConfig } from "@/Interfaces/crud";
import request from "./request";

function setLoading(name: string, loading = true) {
  window.Notify.setLoading(`crudApi.${name}`, loading);
}

function notyError(title: string, exeption: any) {
  const error =
    exeption?.response?.data?.error ||
    exeption?.response?.data?.message ||
    null;
  const msg = error ? `<br/><b>Message:</b> ${error}` : "";

  window.Notify.notify({
    type: "error",
    title: title,
    html: `${exeption}${msg}`,
    timeout: 30000
  });
}

function notySuccess(title: string, message: string) {
  window.Notify.success(message, {
    title: title,
    timeout: 4000,
  });
}


function GetPrimaryKeysValues(config: RequestConfig, item: { [indexer: string]: any }) {
  console.log("GetPrimaryKeysValues", config, item)
  const pks = config.primaryKeys;
  const result: { [indexer: string]: any } = {};
  pks?.forEach((pk: string) => {
    result[pk] = item[pk] || null;
  });

  return result;
}

function GetPrimaryKeyJoined(config: RequestConfig, item: { [indexer: string]: any }) {
  console.log("GetPrimaryKeyJoined", config, item)
  const pks = GetPrimaryKeysValues(config, item);
  return Object.values(pks).join(config.primaryKeysDelimiter || ";");
}


async function GetAllConfigNames() {
  setLoading("GetAllConfigNames", true)
  try {
    const result = await request.get<ConfigNamesType[]>(`/Crud/_all_config_name`);
    return result.data;
  }
  catch (error) {
    notyError(`GetAllConfigNames`, error);
    return Promise.reject(error);
  }
  finally {
    setLoading("GetAllConfigNames", false);
  }
}

async function GetConfig(configName: string) {
  setLoading("GetConfig", true)
  try {
    const result = await request.get<EndpointConfiguration>(`/Crud/_config/${configName}`);
    return result.data;
  }
  finally {
    setLoading("GetConfig", false);
  }
}


async function GetItem(config: RequestConfig, item: { [indexer: string]: any }) {
  const pks = GetPrimaryKeysValues(config, item);
  const pk = Object.values(pks).join(config.primaryKeysDelimiter || ";");

  setLoading("GetItem", true);
  try {
    const result = await request.get(`/Crud/${config.name}/${config.prefixItem}/${pk}`)
    return result.data;
  }
  catch (error) {
    notyError(`Get Item ${JSON.stringify(pks)}`, error);
    return Promise.reject(error);
  }
  finally {
    setLoading("GetItem", false);
  }
}

async function UpdateItem(config: RequestConfig, item: { [indexer: string]: any }) {
  const pks = GetPrimaryKeysValues(config, item);
  const pk = Object.values(pks).join(config.primaryKeysDelimiter || ";");

  setLoading("UpdateItem", true);
  try {
    const result = await request.put(`/Crud/${config.name}/${config.prefixItem}/${pk}`, item);
    notySuccess(
      "Update Item",
      `Update item ${JSON.stringify(pks)} successfully`
    );
    return result.data;
  }
  catch (error) {
    notyError(`Update Item ${JSON.stringify(pks)}`, error);
    return Promise.reject(error);
  }
  finally {
    setLoading("UpdateItem", false);
  }
}


async function DeleteItem(config: RequestConfig, item: { [indexer: string]: any }) {
  const pks = GetPrimaryKeysValues(config, item);
  const pk = Object.values(pks).join(config.primaryKeysDelimiter || ";");

  setLoading("DeleteItem", true);
  try {
    const result = await request.delete(`/Crud/${config.name}/${config.prefixItem}/${pk}`)
    notySuccess(
      "Delete Item",
      `Delete item ${JSON.stringify(pks)} successfully`
    );
    return result.data;
  }
  catch (error) {
    notyError(`Delete Item ${JSON.stringify(pks)}`, error);
    return Promise.reject(error);
  }
  finally {
    setLoading("DeleteItem", false);
  }
}

async function CreateItem(config: RequestConfig, item: { [indexer: string]: any }) {
  setLoading("CreateItem", true);
  try {
    const result = await request.post(`/Crud/${config.name}/${config.prefixList}`, item)
    notySuccess(
      "Create Item",
      `Create item successfully`
    );
    return result.data;
  }
  catch (error) {
    notyError(`Create Item ${JSON.stringify(item)}`, error);
    return Promise.reject(error);
  }
  finally {
    setLoading("CreateItem", false);
  }
}

async function GetItems(config: RequestConfig, perPage = 10, page = 1) {
  setLoading("GetItems", true);
  try {
    const result = await request.get(`/Crud/${config.name}/${config.prefixList}?perPage=${perPage}&page=${page}`)
    return result.data;
  }
  finally {
    setLoading("GetItems", false);
  }
}

async function SearchItems(config: RequestConfig, params: { [indexer: string]: any }) {
  setLoading("SearchItems", true);
  try {
    const result = await request
      // .post(`/Crud/_search/${config.name}/${config.prefixList}`, params)
      .get(
        `/Crud/_search/${config.name}/${config.prefixList}?q=${JSON.stringify(
          params
        )}`)
    return result.data;
  }
  finally {
    setLoading("SearchItems", false);
  }
}

export default {
  GetPrimaryKeysValues,
  GetPrimaryKeyJoined,
  GetAllConfigNames,
  GetConfig,
  GetItem,
  UpdateItem,
  DeleteItem,
  CreateItem,
  GetItems,
  SearchItems
}
