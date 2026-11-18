import type { VNode, DatePickerProps } from "../type";
import { createVNode } from "../core/vnode";

export function DatePicker(props: DatePickerProps): VNode {
  return createVNode("DatePicker", props);
}
