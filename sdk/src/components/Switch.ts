import type { VNode, SwitchProps } from "../type";
import { createVNode } from "../core/vnode";

export function Switch(props: SwitchProps): VNode {
  return createVNode("Switch", props);
}
