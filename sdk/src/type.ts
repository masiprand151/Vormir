export type Color = string;

export type Thickness =
  | number
  | string
  | [number, number]
  | [number, number, number, number];

export type FontWeight =
  | "thin"
  | "light"
  | "normal"
  | "medium"
  | "semi-bold"
  | "bold"
  | "extra-bold";

export type Alignment = "start" | "center" | "end" | "stretch";

export type Orientation = "vertical" | "horizontal";

// VNode
export type VNodeChild = VNode | string | number | boolean | null | undefined;

export interface VNodeProps {
  [key: string]: unknown;
}

export interface VNode {
  type: string;
  props: VNodeProps;
  children: VNodeChild[];
}

// Base Props
export interface BaseProps extends VNodeProps {
  key?: string;
  margin?: Thickness;
  padding?: Thickness;
  width?: number | string;
  height?: number | string;
  background?: Color;
  foreground?: Color;
  opacity?: number;
  isEnabled?: boolean;
  visible?: boolean;
}

// Window
export interface WindowProps extends VNodeProps {
  title?: string;
  width?: number;
  height?: number;
  minWidth?: number;
  minHeight?: number;
  maxWidth?: number;
  maxHeight?: number;
  background?: Color;
  appBar?: boolean;
  location?: "center-screen" | "center-owner" | "manual";
  children?: VNodeChild[];
}

// Stack
export interface StackProps extends BaseProps {
  orientation?: Orientation;
  spacing?: number;
  horizontalAlignment?: Alignment;
  verticalAlignment?: Alignment;
  children?: VNodeChild[];
}

// Text
export interface TextProps extends BaseProps {
  text: string;
  fontSize?: number;
  fontWeight?: FontWeight;
  textAlignment?: "left" | "center" | "right";
  fontFamily?: string;
  wrapping?: boolean;
}

// Button
export interface ButtonProps extends BaseProps {
  content: string;
  fontSize?: number;
  fontWeight?: FontWeight;
  borderRadius?: number;
  borderBrush?: Color;
  borderThickness?: number;
  onPress?: () => void;
}

// Border
export interface BorderProps extends BaseProps {
  brush?: Color;
  thickness?: number | string;
  radius?: number | string;
  child?: VNodeChild;
  children?: VNodeChild[];
}
