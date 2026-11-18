import type { VNode, ImageProps } from "../type";
import { createVNode } from "../core/vnode";

export function Image(props: ImageProps): VNode {
  return createVNode("Image", props);
}
