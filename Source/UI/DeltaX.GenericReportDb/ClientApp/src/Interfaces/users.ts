
export interface User {
    id: number;
    name: string;
    fullName: string;
}

export interface Role {
    RolId: number;
    RolName: string;
    C: number;
    R: number;
    U: number;
    D: number;
}

export interface UserInfo {
    Id: number;
    Username: string;
    FullName?: string;
    Email?: string;
    Image?: string;
    Roles?: Role[];
    Token?: string;
}