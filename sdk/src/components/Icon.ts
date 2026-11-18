import type { VNode, IconProps } from "../type";
import { createVNode } from "../core/vnode";

export function Icon(props: IconProps): VNode {
  return createVNode("Icon", props);
}
