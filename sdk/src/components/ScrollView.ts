import type { VNode, ScrollViewProps } from "../type";
import { createVNode } from "../core/vnode";

export function ScrollView(props: ScrollViewProps): VNode {
  return createVNode("ScrollView", props, props.children ?? []);
}
