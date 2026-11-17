import { ButtonProps } from "../type";
import { createVNode } from "../core/vnode";

export function Button(props: ButtonProps) {
  return createVNode("Button", props);
}
