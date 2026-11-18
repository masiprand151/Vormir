import { TextProps } from "../type";
import { createVNode } from "../core/vnode";

export function Text(text: string, props: TextProps = {}) {
  props.text = text;
  return createVNode("Text", props);
}
