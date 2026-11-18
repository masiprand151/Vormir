import type { VNodeChild } from "../type";

type VormirCallback = (...args: any[]) => any;

declare global {
  var __vormirCallbacks: Record<string, VormirCallback> | undefined;

  var __vormirRender: ((json: string) => void) | undefined;

  var __vormirInvokeCallback:
    | ((id: string, ...args: any[]) => void)
    | undefined;
}

let callbackCounter = 0;

function getCallbacks(): Record<string, VormirCallback> {
  if (!globalThis.__vormirCallbacks) {
    globalThis.__vormirCallbacks = {};
  }

  return globalThis.__vormirCallbacks;
}

// Dipanggil oleh C# Runtime
globalThis.__vormirInvokeCallback = (id, ...args) => {
  const callback = globalThis.__vormirCallbacks?.[id];

  if (typeof callback === "function") {
    callback(...args);
  }
};

function registerCallback(callback: VormirCallback): string {
  const id = `vormir_callback_${++callbackCounter}`;

  getCallbacks()[id] = callback;

  return id;
}

function serializeVNode(node: VNodeChild): unknown {
  if (node === null || node === undefined || typeof node === "boolean") {
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

  if (!("type" in node) || !("props" in node)) {
    return null;
  }

  const props: Record<string, unknown> = {};

  for (const [key, value] of Object.entries(node.props ?? {})) {
    if (typeof value === "function") {
      if (
        key === "onPress" ||
        key === "onClick" ||
        key === "onChange" ||
        key === "onSubmit"
      ) {
        const callback = value as VormirCallback;

        if (node.type === "CheckBox" && key === "onChange") {
          props[key] = registerCallback((value: string) => {
            callback(value === "true");
          });
        } else {
          props[key] = registerCallback(callback);
        }
      }
    } else {
      // Properti biasa wajib diteruskan!
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

export function runApp(app: () => VNodeChild): void {
  render(app());
}
