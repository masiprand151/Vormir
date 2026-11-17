import type { VNode, DividerProps } from "../type";
import { createVNode } from "../core/vnode";

export function Divider(props: DividerProps = {}): VNode {
  return createVNode("Divider", props);
}
