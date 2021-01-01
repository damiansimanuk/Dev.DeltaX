/* eslint-disable */
const config = {
    "displayName": "Usuarios",
    "name": "Users",
    "prefixItem": "user",
    "prefixList": "users",
    "enable": true,
    "databaseType": "Sqlite",
    "connectionString": "Filename=GenericReportDb.sqlite3",
    "primaryKeysDelimiter": ";",
    "permissionsRoles": [
        "UsersAdmin"
    ],
    "widgets": {
        "Id": {
            "disabled": true,
            "type": "Number",
            "bindFlex": {
                "md2": true,
                "sm2": true,
                "xs4": true
            }
        },
        "UserName": {
            "field": "UserName",
            "type": "Text",
            "readonly": true,
            "label": "Usuario",
            "min": 4,
            "max": 20,
            "required": true,
            "bindFlex": {
                "md5": true,
                "sm5": true,
                "xs8": true
            }
        },
        "FullName": {
            "field": "FullName",
            "type": "Text",
            "label": "Nombre Completo",
            "min": 4,
            "max": 40,
            "bindFlex": {
                "md6": true,
                "sm6": true,
                "xs12": true
            }
        },
        "Email": {
            "field": "Email",
            "type": "Text",
            "label": "Correo Electrónico",
            "min": 2,
            "max": 20,
            "required": false,
            "isEmail": true,
            "trim": true,
            "bindFlex": {
                "md6": true,
                "sm6": true,
                "xs12": true
            }
        },
        "Image": {
            "field": "Image",
            "type": "File",
            "outlined": true,
            "required": false,
            "base64": true,
            "size": 80,
            "imageResize": 300
        },
        "Active": {
            "field": "Active",
            "label": "Usuario Activo",
            "disabled": false,
            "type": "Checkbox",
            "required": false,
            "min": 0,
            "max": 1
        }
    },
    "widgetsOnCreate": [
        {
            "widgetName": "UserName",
            "readonly": false
        },
        {
            "widgetName": "Email",
            "bindFlex": {
                "md6": true,
                "xs12": true
            }
        },
        {
            "label": "Permiso de usuario",
            "type": "Select",
            "field": "permissionId",
            "disabled": false,
            "required": true,
            "itemText": "name",
            "itemValue": "id",
            "items": [
                { name: "FullAdmin 10", id: 10 },
                { name: "UserAdmin 11", id: 11 }
            ],
            "bindFlex": {
                "md6": true,
                "xs12": true
            }
        }
    ],
    "widgetsOnEdit": [
        {
            "widgetName": "Id",
            "dense": true
        },
        {
            "widgetName": "UserName",
            "readonly": true,
            "dense": true
        },
        {
            "widgetName": "FullName",
            "dense": true
        },
        {
            "widgetName": "Email",
            "dense": true
        },
        {
            "type": "File",
            "label": "Permisos de usuario",
            "configName": "UsersRoles",
            "prefixItem": "usersrole",
            "prefixList": "usersroles",
            "mapParent": {
                "UserId": "Id",
                "UserName": "UserName"
            },
            "bindFlex": {
                "md12": true,
                "xs12": true
            }
        },
        // {
        //     "type": "TableList",
        //     "dense": true,
        //     "hideFooter": true,
        //     "label": "Permisos de usuario",
        //     "configName": "UsersRoles",
        //     "prefixItem": "usersrole",
        //     "prefixList": "usersroles",
        //     "mapParent": {
        //         "UserId": "Id",
        //         "UserName": "UserName"
        //     },
        //     "listFields": [
        //         "RoleName",
        //         "C",
        //         "R",
        //         "U",
        //         "D"
        //     ],
        //     "widgetsCreate": [
        //         {
        //             "field": "UserName",
        //             "readonly": true,
        //             "dense": true,
        //             "bindFlex": {
        //                 "md6": true,
        //                 "xs12": true
        //             }
        //         },
        //         {
        //             "widgetName": "SelectRol",
        //             "dense": true
        //         },
        //         {
        //             "widgetName": "Create",
        //             "dense": true
        //         },
        //         {
        //             "widgetName": "Read",
        //             "dense": true
        //         },
        //         {
        //             "widgetName": "Update",
        //             "dense": true
        //         },
        //         {
        //             "widgetName": "Delete",
        //             "dense": true
        //         }
        //     ],
        //     "height": 150,
        //     "bindFlex": {
        //         "md12": true,
        //         "xs12": true
        //     }
        // }
    ],
    "listFields": [
        "Id",
        "UserName",
        "FullName",
        {
            "text": "Correo",
            "value": "Email",
            "sortable": true
        }
    ],
    "endpoints": {
        "user": {
            "primaryKeys": [
                "Id"
            ],
            "updateFields": [
                "FullName",
                "Email",
                "Image"
            ],
            "getItem": [
                "select ",
                "Id, UserName, FullName, Email, Image, Active",
                "from users where Id = @Id "
            ],
            "updateItem": [
                "update users set ",
                "FullName=@FullName, ",
                "Email=@Email ",
                "where Id = @Id "
            ],
            "deleteItem": [
                "delete from users where Id = @Id "
            ],
            "uploadFile": [],
            "downloadFile": [],
            "insertFields": [],
            "insertList": [],
            "countList": [],
            "getList": [],
            "searchFields": [],
            "searchList": []
        },
        "users": {
            "primaryKeys": [],
            "updateFields": [],
            "getItem": [],
            "updateItem": [],
            "deleteItem": [],
            "uploadFile": [],
            "downloadFile": [],
            "insertFields": [
                "UserName",
                "FullName",
                "Email",
                "Image"
            ],
            "insertList": [
                "insert into users ",
                "(UserName, FullName, Email, Image, Active)",
                "values (@UserName, @FullName, @Email, @Image, 1)"
            ],
            "countList": [
                "select count() from users "
            ],
            "getList": [
                "select ",
                "Id, UserName, FullName, Email ",
                "from users ",
                "LIMIT @PerPage OFFSET @Offset"
            ],
            "searchFields": [
                "Id",
                "UserName",
                "FullName",
                "Email"
            ],
            "searchList": [
                "select ",
                "Id, UserName, FullName, Email ",
                "from users ",
                "WHERE (Id = @Id or @Id is null) ",
                "AND (UserName like @UserName or @UserName is null) ",
                "AND (FullName like @FullName or @FullName is null) ",
                "AND (Email like @Email or @Email is null) "
            ]
        }
    }
}
/* eslint-enable */
export default config;

