import { INotify } from "./Interfaces/notify";

declare module "*.svg" {
  const content: any;
  export default content;
}

// EXPORT window.Notify
declare global {
  interface Window {
    Notify: INotify;
  }
}