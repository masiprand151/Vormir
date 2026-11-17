export type VNodeChild = VNode | string | number | null | undefined | false;

export type VNodeProps = Record<string, unknown>;

export interface VNode {
  type: string;
  props: VNodeProps;
  children: VNodeChild[];
}

export function createVNode(
  type: string,
  props: VNodeProps = {},
  children: VNodeChild[] = [],
): VNode {
  return {
    type,
    props,
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
