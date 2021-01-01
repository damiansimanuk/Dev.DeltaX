console.log("************** RPC *************");

interface IInner {
  Content: string;
  id: number;
}

interface IModal {
  content: string;
  id: number;
  inner: IInner;
}

console.log("IModal: ");

const str = JSON.stringify({
  content: "asdf",
  inner: { Content: "El contenido" }
});
const modal = JSON.parse(str) as IModal;

console.log("IModal: ", str, modal, modal.inner.Content);

// RPC

interface IRpcCommon {
  Concatenar(a: string, b: number): string;
  Sumar(a: number, b: number): Promise<number>;
}

interface IRpcConnection {
  Request(method: string, args: any): any;
  Send(method: string, args: any): void;
}

function GetServices<T>(connection: IRpcConnection, namePrefix = ""): T {
  const handler = {
    get<P extends keyof T>(target: any, propKey: P) {
      return function(...args: any) {
        const response = connection.Request(propKey as string, args);
        const res: any = args[0];
        const x = res as T[P];

        console.log(
          "get.function",
          target,
          namePrefix,
          propKey,
          "key:",
          typeof propKey,
          "result:",
          x,
          typeof x,
          typeof res,
          args,
          "response:",
          response
        );

        return res as T[P];
      };
    }
  };
  return new Proxy(connection, handler) as T;
}

const conn = { Request: (m, a) => a } as IRpcConnection;
var rpc = GetServices<IRpcCommon>(conn);
const r = rpc.Concatenar("asdf", 123) as string;
const r2 = rpc.Sumar(32, 34);
console.log("GetServices result", typeof r, r, typeof r2, r2);

console.log("-------------------");

class RpcCommon implements IRpcCommon {
  Concatenar(a: string, b: number): string {
    return a;
  }

  Sumar(a: number, b: number): Promise<number> {
    return Promise.resolve(a + b);
  }
}

const rpcCommon = new RpcCommon();
const r3 = rpcCommon.Sumar(23, 45);

console.log("rpcCommon Sumar result", typeof r3, r3);

r3.then(r => console.log("---- Sumar result", r));

console.log("-------------------");

const target = { a: 1 };
const handler = {
  get(target: any, propKey: string | number | symbol) {
    return function(...args: any) {
      console.log("get.function", target, propKey, args);
    };
  }
};

const p = new Proxy(target, handler) as IRpcCommon;
p.Concatenar("1, 2", 23);

var rpc = { Concatenar: (a, b) => `${a} ${b}` } as IRpcCommon;
// var res: string = rpc.Concatenar(2, "asdf");

console.log("rpc: ", Object.keys(rpc));

// some interfaces you expect httpCall to return
interface User {
  name: string;
  age: number;
}
interface Payment {
  id: string;
}

// a mapping of request paths to the function signatures
// you expect to return from createRequest
interface Requests {
  "/users": (clause: { createdAfter: Date }) => Promise<Array<User>>;
  "/payments": (id: string, clause: { createdAfter: Date }) => Promise<Payment>;
}

// a dummy httpCall function
declare function httpCall<R>(path: string, method: string, payload: any): R;

// for now only GET is supported, and the path must be one of keyof Requests
function createRequest<P extends keyof Requests>(method: "GET", path: P) {
  return (function resourceApiCall(
    ...args: Parameters<Requests[P]> // Parameters<F> is the arg tuple of function type F
  ): ReturnType<Requests[P]> {
    // ReturnType<F> is the return type of function type F
    return httpCall<ReturnType<Requests[P]>>(path, method, args);
  } as any) as Requests[P]; // assertion to clean up createRequest signature
}

async function foo() {
  const fetchUsers = createRequest("GET", "/users");
  const users = await fetchUsers({ createdAfter: new Date() }); // User[]
  const fetchPayment = createRequest("GET", "/payments");
  const payment = await fetchPayment("id", { createdAfter: new Date() }); // Payment
}

// you expect to return from createRequest
interface IRequests {}

interface Method {
  GET: {
    "/users": (clause: { createdAfter: Date }) => Promise<Array<User>>;
    "/payments": (
      id: string,
      clause: { createdAfter: Date }
    ) => Promise<Payment>;
  };
  Post: {
    "/users": (clause: { createdAfter: Date }) => Promise<Array<User>>;
    "/payments": (
      id: string,
      clause: { createdAfter: Date }
    ) => Promise<Payment>;
  };
}

// for now only GET is supported, and the path must be one of keyof Requests
const GetService = function<P extends keyof Method, TR extends Method[P]>(
  method: P
) {
  return function GetMethod<X extends keyof TR>(path: X) {
    return {} as TR[X];
  };
};

const x = GetService("GET")("/users")({ createdAfter: new Date() });

export default {
  GetService,
  GetServices
};
