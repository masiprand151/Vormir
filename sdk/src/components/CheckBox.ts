import type { VNode, CheckBoxProps } from "../type";
import { createVNode } from "../core/vnode";

export function CheckBox(props: CheckBoxProps): VNode {
  return createVNode("CheckBox", props);
}
