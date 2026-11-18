import type { VNode, SliderProps } from "../type";
import { createVNode } from "../core/vnode";

export function Slider(props: SliderProps): VNode {
  return createVNode("Slider", props);
}
