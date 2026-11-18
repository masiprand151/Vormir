import type { VNode, VNodeChild, VNodeProps } from "../type";

export function createVNode(
  type: string,
  props: VNodeProps = {},
  children: VNodeChild[] = [],
): VNode {
  return {
    type,
    props: { ...props },
    children: children.filter(
      (child) => child !== null && child !== undefined && child !== false,
    ),
  };
}

export function isVNode(value: unknown): value is VNode {
  return (
    typeof value === "object" &&
    value !== null &&
    "type" in value &&
    "props" in value &&
    "children" in value
  );
}
