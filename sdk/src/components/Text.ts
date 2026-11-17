import { TextProps } from "../type";
import { createVNode } from "../core/vnode";

export function Text(props: TextProps) {
  return createVNode("Text", props);
}
