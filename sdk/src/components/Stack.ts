import { StackProps } from "../type";
import { createVNode } from "../core/vnode";

export function Stack(props: StackProps) {
  return createVNode("Stack", props);
}
