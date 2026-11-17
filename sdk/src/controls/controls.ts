import {
  createVNode,
  type VNode,
  type VNodeProps,
  type VNodeChild,
} from "../core/vnode";

export interface ContainerProps extends VNodeProps {
  children?: VNodeChild[];
}

export interface WindowProps extends ContainerProps {
  title?: string;
  width?: number;
  height?: number;
  background?: string;
  location?: "center-screen" | "manual";
  windowState?: "normal" | "minimized" | "maximized";
  windowStyle?: "none" | "single-border-window" | "three-d-border-window";
}

export interface StackProps extends ContainerProps {
  orientation?: "vertical" | "horizontal";
  margin?: string | number;
  padding?: string | number;
  background?: string;
  horizontalAlignment?: string;
  verticalAlignment?: string;
}

export interface TextProps extends VNodeProps {
  text?: string;
  fontSize?: number;
  fontWeight?: string;
  foreground?: string;
  margin?: string | number;
  textWrapping?: "wrap" | "no-wrap";
}

export interface ButtonProps extends VNodeProps {
  content?: string;
  onClick?: () => void;
  onPress?: () => void;
  padding?: string | number;
  margin?: string | number;
  fontSize?: number;
  isEnabled?: boolean;
}

export interface BorderProps extends ContainerProps {
  background?: string;
  brush?: string;
  thickness?: string | number;
  radius?: string | number;
  padding?: string | number;
  margin?: string | number;
}

export function Window(props: WindowProps): VNode {
  return createVNode("Window", props, props.children ?? []);
}

export function Stack(props: StackProps): VNode {
  return createVNode("Stack", props, props.children ?? []);
}

export function Text(props: TextProps): VNode {
  return createVNode("Text", props);
}

export function Button(props: ButtonProps): VNode {
  return createVNode("Button", props);
}

export function Border(props: BorderProps): VNode {
  return createVNode("Border", props, props.children ?? []);
}
