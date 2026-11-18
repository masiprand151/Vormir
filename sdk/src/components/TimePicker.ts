import type { VNode, TimePickerProps } from "../type";
import { createVNode } from "../core/vnode";

export function TimePicker(props: TimePickerProps): VNode {
  return createVNode("TimePicker", props);
}
