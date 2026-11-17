import type { VNode, SpacerProps } from "../type";
import { createVNode } from "../core/vnode";

export function Spacer(props: SpacerProps = {}): VNode {
  return createVNode("Spacer", {
    ...props,
    flex: props.flex ?? 1,
  });
}
