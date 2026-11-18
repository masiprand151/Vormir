import type { VNode, TextInputProps } from "../type";
import { createVNode } from "../core/vnode";

export function TextInput(props: TextInputProps): VNode {
  return createVNode("TextInput", props);
}
