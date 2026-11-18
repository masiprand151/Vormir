import type { VNode, RadioButtonProps } from "../type";
import { createVNode } from "../core/vnode";

export function RadioButton(props: RadioButtonProps): VNode {
  return createVNode("RadioButton", props);
}
