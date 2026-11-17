import type { VNodeChild, WindowProps } from "../type";
import { createVNode } from "../core/vnode";

export function Window(props: WindowProps): VNodeChild {
  return createVNode("Window", props, props.children ?? []);
}
