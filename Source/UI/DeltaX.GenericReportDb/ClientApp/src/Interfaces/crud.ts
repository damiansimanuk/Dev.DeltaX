
export interface ConfigNamesType {
    name: string;
    displayName: string;
}

export interface WidgetInputText {
    value?: number | string;
    label?: string;
    min?: number;
    max?: number;
    isAlphanumeric?: boolean;
    isEmail?: boolean;
    isWord?: boolean;
    trim?: boolean;
    regex?: string;
    required?: boolean;
    outlined?: boolean;
    dense?: boolean;
    disabled?: boolean;
    readonly?: boolean;
    extraBind?: object;
    extraClass?: object;
}

export interface WidgetInputNumber {
    value?: number | string;
    label?: string;
    step?: number;
    min?: number;
    max?: number;
    required?: boolean;
    outlined?: boolean;
    dense?: boolean;
    disabled?: boolean;
    readonly?: boolean;
    extraBind?: object;
    extraClass?: object;
}

export interface WidgetInputFile {
    value?: File;
    label?: string;
    accept?: string;
    min?: number;
    max?: number;
    required?: boolean;
    outlined?: boolean;
    dense?: boolean;
    disabled?: boolean;
    readonly?: boolean;
    extraBind?: object;
    extraClass?: object;
}

export interface WidgetInputCheckbox {
    value?: number | string | boolean;
    label?: string;
    requiredValue?: boolean;
    required?: boolean;
    dense?: boolean;
    disabled?: boolean;
    readonly?: boolean;
    extraBind?: object;
    extraClass?: object;
}

export interface WidgetInputCheckbox {
    value?: number | string | boolean;
    label?: string;
    requiredValue?: boolean;
    required?: boolean;
    dense?: boolean;
    disabled?: boolean;
    readonly?: boolean;
    extraBind?: object;
    extraClass?: object;
}


export interface WidgetInputSelect {
    value?: number | string | object;
    label?: string;
    items?: string | object[];
    itemText?: string | object[];
    itemValue?: string | object[];
    itemsApi?: SelectLoad;
    valuesApi?: SelectLoad;
    itemParent?: { [key: string]: object };
    required?: boolean;
    dense?: boolean;
    disabled?: boolean;
    readonly?: boolean;
    extraBind?: object;
    extraClass?: object;
}

export interface WidgetInputDataTable {
    label?: string;
    // baseConfig?: EndpointConfiguration;
    loadItems?: TableLoad;
    itemParent?: { [key: string]: object };
    searchItemsParams?: { [key: string]: object | null };
    btnFilter?: boolean;
    dense?: boolean;
    hideFooter?: boolean;
    height?: number | string; // default: 'calc(100vh - 167px)'
    extraBind?: object;
    extraClass?: object;
}


export interface BaseWidget {
    widgetName?: string;
    type?: string;
    field?: string;
    default?: object;
    bindFlex?: object;
}

export type Widget = BaseWidget & (
    WidgetInputText
    | WidgetInputNumber
    | WidgetInputFile
    | WidgetInputCheckbox
    | WidgetInputSelect
    | WidgetInputDataTable)


export interface EndpointQuery {
    primaryKeys?: string[];
    updateFields?: string[];
    getItem?: string[];
    updateItem?: string[];
    deleteItem?: string[];
    uploadFile?: string[];
    downloadFile?: string[];
    insertFields?: string[];
    insertList?: string[];
    countList?: string[];
    getList?: string[];
    searchFields?: string[];
    searchList?: string[];
}


export interface DataTableHeader {
    text: string;
    value: string;
    align?: 'start' | 'center' | 'end';
    sortable?: boolean;
    filterable?: boolean;
    groupable?: boolean;
    divider?: boolean;
    class?: string | string[];
    width?: string | number;
}


export interface EndpointConfiguration {
    displayName?: string;
    name: string;
    prefixList: string;
    prefixItem?: string;
    enable?: boolean;
    databaseType?: string;
    connectionString?: string;
    primaryKeysDelimiter?: string;
    permissionsRoles?: string[];
    widgets?: { [indexer: string]: Widget };
    widgetsOnCreate?: Widget[];
    widgetsOnEdit?: Widget[];
    listFields?: (string | DataTableHeader)[];
    endpoints?: { [indexer: string]: EndpointQuery };
}

export interface RequestConfig {
    name: string;
    prefixList: string;
    prefixItem: string | null;
    primaryKeysDelimiter: string | null;
    primaryKeys: string[] | null;
    // endpoint: EndpointQuery | null;
}

export interface SelectLoad {
    configName: string | null;
    prefixList: string;
    mapItemParent?: { [key: string]: string };
}

export interface TableLoad {
    configName: string;
    prefixList: string;
    prefixItem?: string;
    widgetsOnCreate?: Widget[];
    widgetsOnEdit?: Widget[];
    listFields?: (string | DataTableHeader)[];
    mapItemParent: { [key: string]: string };
}


