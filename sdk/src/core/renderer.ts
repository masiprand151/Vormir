import type { VNode, VNodeChild } from "./vnode";

type VormirCallback = (...args: any[]) => any;

declare global {
  var __vormirCallbacks: Record<string, VormirCallback> | undefined;

  var __vormirRender: ((json: string) => void) | undefined;
}

let callbackCounter = 0;

function getCallbacks(): Record<string, VormirCallback> {
  if (!globalThis.__vormirCallbacks) {
    globalThis.__vormirCallbacks = {};
  }

  return globalThis.__vormirCallbacks;
}

function registerCallback(callback: VormirCallback): string {
  const id = `vormir_callback_${++callbackCounter}`;

  getCallbacks()[id] = callback;

  return id;
}

function serializeVNode(node: VNodeChild): any {
  if (node === null || node === undefined || node === false) {
    return null;
  }

  if (typeof node === "string" || typeof node === "number") {
    return {
      type: "Text",
      props: {
        text: String(node),
      },
      children: [],
    };
  }

  const props: Record<string, any> = {};

  for (const [key, value] of Object.entries(node.props ?? {})) {
    if (typeof value === "function") {
      if (
        key === "onPress" ||
        key === "onClick" ||
        key === "onChange" ||
        key === "onSubmit"
      ) {
        props[key] = registerCallback(value as VormirCallback);
      }
    } else {
      props[key] = value;
    }
  }

  return {
    type: node.type,
    props,
    children: (node.children ?? [])
      .map((child) => serializeVNode(child))
      .filter((child) => child !== null),
  };
}

export function render(node: VNodeChild): void {
  if (typeof globalThis.__vormirRender !== "function") {
    throw new Error("Vormir Native Bridge belum tersedia.");
  }

  const tree = serializeVNode(node);

  globalThis.__vormirRender(JSON.stringify(tree));
}

export function runApp(app: () => VNode): void {
  render(app());
}
