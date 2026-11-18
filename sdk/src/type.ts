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

export type Icons =
  // ==========================================
  // NAVIGATION
  // ==========================================
  | "home"
  | "house"
  | "menu"
  | "menu-2"
  | "grid"
  | "grid-2x2"
  | "layout"
  | "sidebar"
  | "panel-left"
  | "panel-right"
  | "maximize"
  | "minimize"
  | "expand"
  | "shrink"
  | "external-link"
  | "link"
  | "link-2"
  | "compass"
  | "map"

  // ==========================================
  // ARROWS
  // ==========================================
  | "arrow-up"
  | "arrow-down"
  | "arrow-left"
  | "arrow-right"
  | "arrow-up-left"
  | "arrow-up-right"
  | "arrow-down-left"
  | "arrow-down-right"
  | "arrow-left-right"
  | "arrow-up-down"
  | "arrow-up-circle"
  | "arrow-down-circle"
  | "arrow-left-circle"
  | "arrow-right-circle"
  | "chevron-up"
  | "chevron-down"
  | "chevron-left"
  | "chevron-right"
  | "chevrons-up"
  | "chevrons-down"
  | "chevrons-left"
  | "chevrons-right"
  | "corner-down-right"
  | "refresh-cw"
  | "refresh-ccw"
  | "repeat"

  // ==========================================
  // ACTIONS
  // ==========================================
  | "plus"
  | "minus"
  | "x"
  | "check"
  | "check-circle"
  | "x-circle"
  | "plus-circle"
  | "minus-circle"
  | "edit"
  | "edit-2"
  | "edit-3"
  | "pencil"
  | "trash"
  | "trash-2"
  | "copy"
  | "clipboard"
  | "clipboard-check"
  | "save"
  | "download"
  | "upload"
  | "share"
  | "filter"
  | "search"
  | "zoom-in"
  | "zoom-out"
  | "more-horizontal"
  | "more-vertical"
  | "sliders"
  | "settings"
  | "tool"
  | "wrench"

  // ==========================================
  // USER & PEOPLE
  // ==========================================
  | "user"
  | "user-round"
  | "users"
  | "user-plus"
  | "user-minus"
  | "user-check"
  | "user-x"
  | "contact"
  | "id-card"

  // ==========================================
  // COMMUNICATION
  // ==========================================
  | "mail"
  | "message-circle"
  | "message-square"
  | "messages-square"
  | "phone"
  | "phone-call"
  | "video"
  | "video-off"
  | "bell"
  | "bell-off"
  | "send"
  | "at-sign"
  | "wifi"
  | "wifi-off"

  // ==========================================
  // FILES & DOCUMENTS
  // ==========================================
  | "file"
  | "file-text"
  | "file-plus"
  | "file-minus"
  | "file-check"
  | "file-x"
  | "folder"
  | "folder-open"
  | "folder-plus"
  | "folder-check"
  | "archive"
  | "book"
  | "book-open"
  | "notebook"
  | "paperclip"
  | "printer"
  | "clipboard-check"

  // ==========================================
  // MEDIA & ENTERTAINMENT
  // ==========================================
  | "play"
  | "pause"
  | "stop"
  | "skip-back"
  | "skip-forward"
  | "rewind"
  | "fast-forward"
  | "volume"
  | "volume-1"
  | "volume-2"
  | "volume-x"
  | "music"
  | "music-2"
  | "headphones"
  | "mic"
  | "mic-off"
  | "radio"
  | "tv"
  | "monitor"
  | "smartphone"
  | "tablet"
  | "camera"
  | "image"
  | "images"
  | "film"
  | "clapperboard"
  | "radio-tower"

  // ==========================================
  // TIME & CALENDAR
  // ==========================================
  | "clock"
  | "watch"
  | "calendar"
  | "calendar-days"
  | "calendar-check"
  | "timer"
  | "alarm-clock"
  | "hourglass"

  // ==========================================
  // SHOPPING & FINANCE
  // ==========================================
  | "shopping-cart"
  | "shopping-bag"
  | "shopping-basket"
  | "wallet"
  | "credit-card"
  | "banknote"
  | "receipt"
  | "receipt-text"
  | "tag"
  | "tags"
  | "gift"
  | "coins"
  | "dollar-sign"
  | "percent"
  | "trending-up"
  | "trending-down"
  | "chart-line"
  | "chart-bar"
  | "chart-pie"
  | "calculator"

  // ==========================================
  // DEVICES & HARDWARE
  // ==========================================
  | "laptop"
  | "keyboard"
  | "mouse"
  | "printer-check"
  | "server"
  | "database"
  | "hard-drive"
  | "cpu"
  | "battery"
  | "battery-charging"
  | "bluetooth"
  | "power"

  // ==========================================
  // LOCATION & TRANSPORT
  // ==========================================
  | "map-pin"
  | "navigation"
  | "navigation-2"
  | "globe"
  | "car"
  | "bus"
  | "bike"
  | "plane"
  | "train"
  | "truck"

  // ==========================================
  // STATUS & ALERTS
  // ==========================================
  | "alert-circle"
  | "alert-triangle"
  | "alert-octagon"
  | "info"
  | "help-circle"
  | "shield"
  | "shield-check"
  | "shield-alert"
  | "lock"
  | "unlock"
  | "key"
  | "eye"
  | "eye-off"
  | "flag"

  // ==========================================
  // WEATHER & NATURE
  // ==========================================
  | "sun"
  | "moon"
  | "cloud"
  | "cloud-rain"
  | "cloud-sun"
  | "cloud-snow"
  | "umbrella"
  | "leaf"
  | "tree"
  | "flower"
  | "flame"

  // ==========================================
  // SHAPES & DESIGN
  // ==========================================
  | "circle"
  | "square"
  | "rectangle"
  | "triangle"
  | "diamond"
  | "heart"
  | "star"
  | "bookmark"
  | "palette"
  | "paintbrush"
  | "brush"
  | "pen-tool"
  | "scissors"
  | "ruler"
  | "crop"
  | "move"

  // ==========================================
  // MISCELLANEOUS
  // ==========================================
  | "activity"
  | "zap"
  | "award"
  | "trophy"
  | "crown"
  | "coffee"
  | "cup-soda"
  | "utensils"
  | "pizza"
  | "cookie"
  | "package"
  | "box"
  | "lamp"
  | "lightbulb"
  | "rocket"
  | "anchor"
  | "bug"
  | "puzzle"
  | "hand"
  | "thumbs-up"
  | "thumbs-down"
  | "smile"
  | "frown"
  | "meh"
  | "heart-handshake"
  | "sparkles";

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

export interface WindowPosition {
  x: number;
  y: number;
  offsetX?: number;
  offsetY?: number;
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
  fullscreen?: boolean;
  background?: Color;
  appBar?: "none" | "threedborderwindow" | "SingleBorderWindow";
  location?: "center-screen" | "center-owner" | WindowPosition;
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

export interface SpacerProps extends BaseProps {}
export interface DividerProps extends BaseProps {
  color?: Color;
  thickness?: number;
  orientation?: "horizontal" | "vertical";
}
// ScrollView
export interface ScrollViewProps extends BaseProps {
  vertical?: boolean;
  horizontal?: boolean;
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

// Image
export interface ImageProps extends BaseProps {
  source: string;
  fit?: "contain" | "cover" | "fill" | "none";
}

// Icon
export interface IconProps extends BaseProps {
  name: Icons;
  size?: number;
  color?: Color;
}

// Border
export interface BorderProps extends BaseProps {
  brush?: Color;
  thickness?: number | string;
  radius?: number | string;
  child?: VNodeChild;
  children?: VNodeChild[];
}

export interface TextInputProps extends BaseProps {
  value?: string;
  placeholder?: string;
  onChange?: (value: string) => void;

  fontSize?: number;
  color?: Color;

  borderColor?: Color;
  borderWidth?: number;
  borderRadius?: number;
}

export interface CheckBoxProps extends BaseProps {
  label?: string;
  checked?: boolean;
  onChange?: (value: boolean) => void;
  color?: Color;
  fontSize?: number;
}

export interface RadioButtonProps extends BaseProps {
  label?: string;
  group: string;
  value: string;
  checked?: boolean;
  onChange?: (value: string) => void;
  color?: Color;
  fontSize?: number;
}

export interface SwitchProps extends BaseProps {
  checked?: boolean;
  onChange?: (value: boolean) => void;
  color?: Color;
}

export interface SliderProps extends BaseProps {
  value?: number;
  min?: number;
  max?: number;
  onChange?: (value: number) => void;
  color?: Color;
}

export interface ProgressBarProps extends BaseProps {
  value?: number;
  min?: number;
  max?: number;
  color?: Color;
  trackColor?: Color;
  height?: number;
  cornerRadius?: number;
}

export interface GridProps extends BaseProps {
  columns?: number;
  rows?: number;
  spacing?: number;
  rowSpacing?: number;
  columnSpacing?: number;
  children?: VNodeChild[];
}

export interface DropdownOption {
  label: string;
  value: string;
}

export interface DropdownProps extends BaseProps {
  value?: string;
  options: DropdownOption[];
  placeholder?: string;
  onChange?: (value: string) => void;
}

export interface DatePickerProps extends BaseProps {
  value?: string;
  minDate?: string;
  maxDate?: string;
  placeholder?: string;
  onChange?: (value: string) => void;
}

export interface TimePickerProps extends BaseProps {
  value?: string;
  minTime?: string;
  maxTime?: string;
  placeholder?: string;
  visible?: boolean;
  onChange?: (value: string) => void;
}
