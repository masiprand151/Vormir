import type { VNodeChild, WindowProps } from "../type";
import { createVNode } from "../core/vnode";

export function Window(props: WindowProps): VNodeChild {
  // fix x position
  if (typeof props.location === "object" && props.location.x === 0) {
    props.location.offsetX = -7;
  }

  return createVNode("Window", props, props.children ?? []);
}
