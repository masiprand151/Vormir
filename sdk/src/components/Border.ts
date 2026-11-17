import { BorderProps } from "../type";
import { createVNode } from "../core/vnode";

export function Border(props: BorderProps) {
  return createVNode("Border", props);
}
