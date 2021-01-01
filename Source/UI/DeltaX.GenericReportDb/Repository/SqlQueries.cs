

namespace DeltaX.GenericReportDb.Repository
{
    public class SqlQueries
    {
        public static readonly string GetUserDetail = "SELECT " +
            "Id, Username, FullName, Email, Image, Active, PasswordHash " +
            "FROM Users " +
            "WHERE Id = @Id ";

        public static readonly string GetUserDetailByName = "SELECT  " +
            "Id, Username, FullName, Email, Image, Active, PasswordHash " +
            "FROM Users " +
            "WHERE Username = @Username OR Email = @Username";

        public static readonly string GetRolesList = "SELECT " +
            "RolId, Name as RolName, ur.C, ur.R, ur.U, ur.D " +
            "FROM Roles r " +
            "JOIN UsersRoles ur on r.Id = ur.RolId " +
            "WHERE ur.UserId = @Id";

        public static readonly string InsertRole = "INSERT OR IGNORE INTO Roles(Name) VALUES(@roleName); ";

        public static readonly string InsertAdministrator = "INSERT OR IGNORE INTO Roles(Id, Name) VALUES(1, 'Administrator'); " +
          "INSERT OR IGNORE INTO Users(Id, Username, FullName) VALUES(1, 'admin', 'Administrator'); " +
          "INSERT OR IGNORE INTO UsersRoles(UserId, RolId, C, R, U, D ) VALUES(1,1,1,1,1,1); ";

        public static readonly string GetUserList = "SELECT " +
            "Id, Username, FullName, Email, Active " +
            "FROM Users ";

        public static readonly string UpdateUser = "UPDATE Users SET " +
           "FullName = @FullName,  " +
           "Email = @Email, " +
           "Image = @Image, " +
           "PasswordHash = @PasswordHash " +
           "where Id = @Id ";

        public static readonly string ScriptCreateTables = @"
PRAGMA foreign_keys = off;
BEGIN TRANSACTION;
 
CREATE TABLE IF NOT EXISTS Users(
    Id           INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    Username     TEXT    UNIQUE NOT NULL,
    FullName     TEXT,
    Email        TEXT    UNIQUE,
    Image        BLOB,
    Active       BOOLEAN DEFAULT (1) NOT NULL,
    PasswordHash TEXT,
    CreatedAt    DATE    DEFAULT (datetime('now', 'localtime') ) 
);

CREATE TABLE IF NOT EXISTS UsersRoles (
    UserId    INTEGER REFERENCES Users (Id) ON DELETE CASCADE,
    RolId     INTEGER REFERENCES Roles (Id) ON DELETE CASCADE,
    C         BOOLEAN DEFAULT (0),
    R         BOOLEAN DEFAULT (1),
    U         BOOLEAN DEFAULT (0),
    D         BOOLEAN DEFAULT (0),
    CreatedAt DATE    DEFAULT (datetime('now', 'localtime') ),
    UNIQUE (UserId, RolId)
);

CREATE TABLE IF NOT EXISTS Roles (
    Id        INTEGER PRIMARY KEY AUTOINCREMENT,
    Name      TEXT    UNIQUE NOT NULL,
    CreatedAt DATE    DEFAULT (datetime('now', 'localtime') ) 
);

COMMIT TRANSACTION;
PRAGMA foreign_keys = on;
";

    }
}
