
export interface User {
    id: number;
    name: string;
    fullName: string;
}

export interface Role {
    rolId: number;
    rolName: string;
    c: number;
    r: number;
    u: number;
    d: number;
}

export interface UserInfo {
    id: number;
    username: string;
    fullName?: string;
    email?: string;
    image?: string;
    roles?: Role[];
    token?: string;
}