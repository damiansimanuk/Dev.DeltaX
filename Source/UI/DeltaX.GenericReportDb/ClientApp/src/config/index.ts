function parse<T>(value: any, fallback: T): T {
  if (typeof value === "undefined") {
    return fallback;
  }
  switch (typeof fallback) {
    case "boolean":
      return (!!JSON.parse(value) as unknown) as T;
    case "number":
      return JSON.parse(value);
    default:
      return value;
  }
}

const API_URL = parse(process.env.VUE_APP_API_URL, "https://127.0.0.1:5051/api/");
const API_TIMEOUT = parse(process.env.VUE_APP_API_TIMEOUT, 5000);
console.log("API_URL", API_URL, "API_TIMEOUT", API_TIMEOUT);

export { parse, API_URL, API_TIMEOUT };
