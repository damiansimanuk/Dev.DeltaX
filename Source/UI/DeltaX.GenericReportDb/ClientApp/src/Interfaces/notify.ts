export interface IButton {
  text: string;
  icon: string;
  bind: { color: string } | any | undefined;
}

export interface IMessage {
  title?: string;
  type?: string;
  text?: string;
  html?: string;
  bind?: { color: string } | any | undefined;
  timeout?: number;
  buttons?: Array<IButton>;
}

export interface IMessageAlert extends IMessage {
  id: string;
  color: string;
  timestart: number;
  resolve: (value: number) => void;
  reject: (reason?: number) => void;
}

export interface IMessageConfirm extends IMessage {
  id: string;
  color: string;
  resolve: (value: number) => void;
  reject: (reason?: number) => void;
}

export interface INotify {
  confirmations: IMessageConfirm[];
  alerts: IMessageAlert[];
  datetime: number;
  loading: boolean;
  getId(type: string): string;
  setLoading(name?: string, loading?: boolean): void;
  deleteItem(item: IMessage): void;
  notify(msg: IMessage): Promise<number>;
  confirm(msg: IMessage | any): Promise<number>;
  info(text: string, options?: IMessage): Promise<number>;
  success(text: string, options?: IMessage): Promise<number>;
  error(text: string, options?: IMessage): Promise<number>;
  warning(text: string, options?: IMessage): Promise<number>;
  deleteAll(): void;
}
