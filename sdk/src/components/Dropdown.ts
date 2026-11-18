import type { VNode, DropdownProps } from "../type";
import { createVNode } from "../core/vnode";

export function Dropdown(props: DropdownProps): VNode {
  return createVNode("Dropdown", props);
}
