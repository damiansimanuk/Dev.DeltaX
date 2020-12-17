import request from "./request"

interface User {
    id: number;
    name: string;
    fullName: string
}

function GetUser(id: number) {
    console.log("GetUser", id);
    return request.get<User>(`/user/${id}`);
}

function UpdateUser(user: User) {
    console.log("UpdateUser", user.id);
    return request.put<User>(`/alias/${user.id}`, user);
}

export {
    User,
    GetUser,
    UpdateUser
}
