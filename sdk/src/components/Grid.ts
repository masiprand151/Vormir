import type { VNode, GridProps } from "../type";
import { createVNode } from "../core/vnode";

export function Grid(props: GridProps): VNode {
  return createVNode("Grid", props, props.children ?? []);
}
