import { createVNode } from "../core/vnode";

export function Text(props: any) {
  return createVNode("Text", props);
}
