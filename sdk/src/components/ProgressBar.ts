import type { VNode, ProgressBarProps } from "../type";
import { createVNode } from "../core/vnode";

export function ProgressBar(props: ProgressBarProps): VNode {
  return createVNode("ProgressBar", props);
}
