import { ButtonProps } from "../type";
import { createVNode } from "../core/vnode";

export function Button(text: string, props: ButtonProps = {}) {
  props.text = text;
  return createVNode("Button", props);
}
