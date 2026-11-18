
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Text.Json;
using System.Windows.Media;
using System.Windows.Shell;
using System.Globalization;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media.Imaging;


namespace Vormir.UI;

public sealed class WpfNativeBridge
{
  private readonly Window _window;
  private readonly Action<string> _invokeCallback;
  private readonly Action<string, string>? _invokeCallbackWithValue;

  public WpfNativeBridge(
    Window window,
    Action<string> invokeCallback,
    Action<string, string>? invokeCallbackWithValue = null)
  {
    _window = window;
    _invokeCallback = invokeCallback;
    _invokeCallbackWithValue = invokeCallbackWithValue;
  }

  public void Render(string json)
  {
    using var document = JsonDocument.Parse(json);

    JsonElement root = document.RootElement;

    if (GetString(root, "type") == "Window")
    {
      RenderWindow(root);
      return;
    }

    _window.Content = BuildElement(root);
  }

  private void RenderWindow(JsonElement node)
  {
    JsonElement props = GetProperty(node, "props");
    JsonElement children = GetProperty(node, "children");

    string? title = GetString(props, "title");
    if (title != null)
      _window.Title = title;

    double? width = GetNumber(props, "width");
    if (width.HasValue)
      _window.Width = width.Value;

    double? height = GetNumber(props, "height");
    if (height.HasValue)
      _window.Height = height.Value;

    string? background = GetString(props, "background");
    if (background != null)
      _window.Background = ParseBrush(background);

    // Window Location
    JsonElement location = GetProperty(props, "location");

    if (location.ValueKind == JsonValueKind.String)
    {
      string? position = location.GetString();

      _window.WindowStartupLocation = Normalize(position ?? "") switch
      {
        "centerscreen" => WindowStartupLocation.CenterScreen,
        "centerowner" => WindowStartupLocation.CenterOwner,
        _ => WindowStartupLocation.Manual
      };
    }
    else if (location.ValueKind == JsonValueKind.Object)
    {
      double x = GetNumber(location, "x") ?? 0;
      double y = GetNumber(location, "y") ?? 0;

      double offsetX = GetNumber(location, "offsetX") ?? 0;
      double offsetY = GetNumber(location, "offsetY") ?? 0;

      _window.WindowStartupLocation = WindowStartupLocation.Manual;

      _window.Left = x + offsetX;
      _window.Top = y + offsetY;
    }

    // Fullscreen
    bool? fullscreen = GetBoolean(props, "fullscreen");

    if (fullscreen == true)
    {
      _window.WindowStyle = WindowStyle.None;
      _window.WindowState = WindowState.Maximized;
      _window.ResizeMode = ResizeMode.NoResize;
    }
    else if (fullscreen == false)
    {
      _window.ResizeMode = ResizeMode.CanResize;
    }

    string? windowState = GetString(props, "windowState");
    if (windowState != null)
    {
      _window.WindowState = Normalize(windowState) switch
      {
        "maximized" => WindowState.Maximized,
        "minimized" => WindowState.Minimized,
        _ => WindowState.Normal
      };
    }

    string? windowStyle = GetString(props, "appBar");
    if (windowStyle != null)
    {
      _window.WindowStyle = Normalize(windowStyle) switch
      {
        "none" => WindowStyle.None,
        "threedborderwindow" => WindowStyle.ThreeDBorderWindow,
        _ => WindowStyle.SingleBorderWindow
      };
    }

    if (children.ValueKind == JsonValueKind.Array &&
        children.GetArrayLength() > 0)
    {
      _window.Content = BuildElement(children[0]);
    }
    else
    {
      _window.Content = null;
    }
  }

  private FrameworkElement BuildElement(JsonElement node)
  {
    string type = GetString(node, "type") ?? "Text";

    JsonElement props = GetProperty(node, "props");
    JsonElement children = GetProperty(node, "children");

    return type switch
    {
      "Stack" => BuildStack(props, children),
      "View" => BuildStack(props, children),
      "Text" => BuildText(props),
      "Button" => BuildButton(props),
      "Border" => BuildBorder(props, children),
      "Spacer" => BuildSpacer(props),
      "ScrollView" => BuildScrollView(props, children),
      "Divider" => BuildDivider(props),
      "Image" => BuildImage(props),
      "Icon" => BuildIcon(props),
      "TextInput" => BuildTextInput(props),
      "CheckBox" => BuildCheckBox(props),
      "RadioButton" => BuildRadioButton(props),
      _ => new TextBlock
      {
        Text = $"Unknown component: {type}",
        Foreground = Brushes.Red
      }
    };
  }

  // Radio button
  private FrameworkElement BuildRadioButton(JsonElement props)
  {
    string label = GetString(props, "label") ?? "";
    string group = GetString(props, "group") ?? "default";
    string value = GetString(props, "value") ?? "";
    string color = GetString(props, "color") ?? "#222222";

    double fontSize = GetNumber(props, "fontSize") ?? 14;

    bool checkedValue = GetBoolean(props, "checked") ?? false;

    string? callbackId = GetString(props, "onChange");

    var labelText = new TextBlock
    {
      Text = label,
      Foreground = ParseBrush(color),
      FontSize = fontSize,
      VerticalAlignment = VerticalAlignment.Center
    };

    var radioButton = new System.Windows.Controls.RadioButton
    {
      Content = labelText,
      GroupName = group,
      IsChecked = checkedValue,
      VerticalAlignment = VerticalAlignment.Center,
      HorizontalAlignment = HorizontalAlignment.Left,
      Margin = ParseOptionalThickness(props, "margin") ?? new Thickness(0)
    };

    radioButton.Checked += (sender, args) =>
    {
      if (!string.IsNullOrEmpty(callbackId))
      {
        _invokeCallbackWithValue?.Invoke(callbackId, value);
      }
    };

    double? width = GetNumber(props, "width");
    if (width.HasValue)
      radioButton.Width = width.Value;

    double? height = GetNumber(props, "height");
    if (height.HasValue)
      radioButton.Height = height.Value;

    bool? isEnabled = GetBoolean(props, "isEnabled");
    if (isEnabled.HasValue)
      radioButton.IsEnabled = isEnabled.Value;

    bool? visible = GetBoolean(props, "visible");
    if (visible.HasValue)
    {
      radioButton.Visibility = visible.Value
          ? Visibility.Visible
          : Visibility.Collapsed;
    }

    return radioButton;
  }



  // checkbok
  private FrameworkElement BuildCheckBox(JsonElement props)
  {
    string label = GetString(props, "label") ?? "";
    string color = GetString(props, "color") ?? "#222222";
    double fontSize = GetNumber(props, "fontSize") ?? 14;

    bool checkedValue = GetBoolean(props, "checked") ?? false;

    string? callbackId = GetString(props, "onChange");
    Console.Write(label);

    var labelText = new TextBlock
    {
      Text = label,
      Foreground = ParseBrush(color),
      FontSize = fontSize,
      VerticalAlignment = VerticalAlignment.Center
    };

    var checkBox = new System.Windows.Controls.CheckBox
    {
      Content = labelText,
      IsChecked = checkedValue,
      IsThreeState = false,
      VerticalContentAlignment = VerticalAlignment.Center,
      HorizontalAlignment = HorizontalAlignment.Left,
      VerticalAlignment = VerticalAlignment.Center,
      Margin = ParseOptionalThickness(props, "margin") ?? new Thickness(0)
    };

    checkBox.Checked += (sender, args) =>
    {
      if (!string.IsNullOrEmpty(callbackId))
        _invokeCallbackWithValue?.Invoke(callbackId, "true");
    };

    checkBox.Unchecked += (sender, args) =>
    {
      if (!string.IsNullOrEmpty(callbackId))
        _invokeCallbackWithValue?.Invoke(callbackId, "false");
    };

    double? width = GetNumber(props, "width");
    if (width.HasValue)
      checkBox.Width = width.Value;

    double? height = GetNumber(props, "height");
    if (height.HasValue)
      checkBox.Height = height.Value;

    bool? isEnabled = GetBoolean(props, "isEnabled");
    if (isEnabled.HasValue)
      checkBox.IsEnabled = isEnabled.Value;

    bool? visible = GetBoolean(props, "visible");
    if (visible.HasValue)
    {
      checkBox.Visibility = visible.Value
          ? Visibility.Visible
          : Visibility.Collapsed;
    }

    return checkBox;
  }


  // Input
  private FrameworkElement BuildTextInput(JsonElement props)
  {
    string value = GetString(props, "value") ?? "";
    string placeholder = GetString(props, "placeholder") ?? "";

    double fontSize = GetNumber(props, "fontSize") ?? 14;
    double borderWidth = GetNumber(props, "borderWidth") ?? 1;
    double borderRadius = GetNumber(props, "borderRadius") ?? 4;

    string color = GetString(props, "color") ?? "#222222";
    string background = GetString(props, "background") ?? "#ffffff";
    string borderColor = GetString(props, "borderColor") ?? "#cccccc";

    string? callbackId = GetString(props, "onChange");

    var textBox = new System.Windows.Controls.TextBox
    {
      Text = value,
      FontSize = fontSize,
      Foreground = ParseBrush(color),
      Background = Brushes.Transparent,
      BorderThickness = new Thickness(0),
      VerticalContentAlignment = VerticalAlignment.Center,
      HorizontalContentAlignment = HorizontalAlignment.Stretch,
      Padding = new Thickness(0)
    };

    var placeholderText = new TextBlock
    {
      Text = placeholder,
      Foreground = ParseBrush("#999999"),
      FontSize = fontSize,
      VerticalAlignment = VerticalAlignment.Center,
      IsHitTestVisible = false
    };

    var content = new Grid();

    content.Children.Add(textBox);
    content.Children.Add(placeholderText);

    placeholderText.Visibility =
        string.IsNullOrEmpty(value)
            ? Visibility.Visible
            : Visibility.Collapsed;

    textBox.TextChanged += (sender, args) =>
    {
      placeholderText.Visibility =
          string.IsNullOrEmpty(textBox.Text)
              ? Visibility.Visible
              : Visibility.Collapsed;

      if (!string.IsNullOrEmpty(callbackId))
      {
        _invokeCallbackWithValue?.Invoke(
            callbackId,
            textBox.Text
        );
      }
    };

    var container = new Border
    {
      Child = content,
      Background = ParseBrush(background),
      BorderBrush = ParseBrush(borderColor),
      BorderThickness = new Thickness(borderWidth),
      CornerRadius = new CornerRadius(borderRadius),
      Padding = ParseOptionalThickness(props, "padding") ?? new Thickness(8),
      Margin = ParseOptionalThickness(props, "margin") ?? new Thickness(0)
    };

    double? width = GetNumber(props, "width");
    if (width.HasValue)
      container.Width = width.Value;

    double? height = GetNumber(props, "height");
    if (height.HasValue)
      container.Height = height.Value;

    bool? isEnabled = GetBoolean(props, "isEnabled");
    if (isEnabled.HasValue)
      container.IsEnabled = isEnabled.Value;

    bool? visible = GetBoolean(props, "visible");
    if (visible.HasValue)
    {
      container.Visibility = visible.Value
          ? Visibility.Visible
          : Visibility.Collapsed;
    }

    return container;
  }

  // Icon
  private FrameworkElement BuildIcon(JsonElement props)
  {
    string name = GetString(props, "name") ?? "";

    double size = GetNumber(props, "size") ?? 24;

    string color = GetString(props, "color") ?? "#333333";

    string? pathData = name.ToLowerInvariant() switch
    {
      // ==========================================
      // NAVIGATION
      // ==========================================
      "home" => "M3 10 L12 3 L21 10 V21 H14 V14 H10 V21 H3 Z",
      "house" => "M3 10 L12 3 L21 10 V21 H14 V14 H10 V21 H3 Z",
      "menu" => "M3 6 H21 M3 12 H21 M3 18 H21",
      "menu-2" => "M4 6 H20 M4 12 H20 M4 18 H20",
      "grid" => "M3 3 H10 V10 H3 Z M14 3 H21 V10 H14 Z M3 14 H10 V21 H3 Z M14 14 H21 V21 H14 Z",
      "grid-2x2" => "M3 3 H10 V10 H3 Z M14 3 H21 V10 H14 Z M3 14 H10 V21 H3 Z M14 14 H21 V21 H14 Z",
      "layout" => "M3 3 H21 V21 H3 Z M9 3 V21 M9 12 H21",
      "sidebar" => "M3 3 H21 V21 H3 Z M9 3 V21",
      "panel-left" => "M3 3 H21 V21 H3 Z M9 3 V21",
      "panel-right" => "M3 3 H21 V21 H3 Z M15 3 V21",
      "maximize" => "M8 3 H3 V8 M16 3 H21 V8 M3 16 V21 H8 M21 16 V21 H16",
      "minimize" => "M5 12 H19",
      "expand" => "M8 3 H3 V8 M16 3 H21 V8 M3 16 V21 H8 M21 16 V21 H16",
      "shrink" => "M3 8 H8 V3 M16 3 V8 H21 M3 16 H8 V21 M16 21 V16 H21",
      "external-link" => "M13 5 H19 V11 M19 5 L10 14 M19 13 V19 H5 V5 H11",
      "link" => "M10 13 A5 5 0 0 0 17 13 L20 10 A5 5 0 0 0 13 3 L11 5 M14 11 A5 5 0 0 0 7 11 L4 14 A5 5 0 0 0 11 21 L13 19",
      "link-2" => "M10 13 A5 5 0 0 0 17 13 L20 10 A5 5 0 0 0 13 3 M14 11 A5 5 0 0 0 7 11 L4 14 A5 5 0 0 0 11 21",
      "compass" => "M12 2 A10 10 0 1 0 12 22 A10 10 0 1 0 12 2 M16 8 L14 14 L8 16 L10 10 Z",
      "map" => "M3 6 L9 3 L15 6 L21 3 V18 L15 21 L9 18 L3 21 Z M9 3 V18 M15 6 V21",

      // ==========================================
      // ARROWS
      // ==========================================
      "arrow-up" => "M12 19 V5 M5 12 L12 5 L19 12",
      "arrow-down" => "M12 5 V19 M5 12 L12 19 L19 12",
      "arrow-left" => "M19 12 H5 M12 5 L5 12 L12 19",
      "arrow-right" => "M5 12 H19 M12 5 L19 12 L12 19",
      "arrow-up-left" => "M17 17 L7 7 M7 17 V7 H17",
      "arrow-up-right" => "M7 17 L17 7 M7 7 H17 V17",
      "arrow-down-left" => "M17 7 L7 17 M17 17 H7 V7",
      "arrow-down-right" => "M7 7 L17 17 M17 7 V17 H7",
      "arrow-left-right" => "M17 8 L21 12 L17 16 M3 12 H21 M7 8 L3 12 L7 16",
      "arrow-up-down" => "M8 17 L12 21 L16 17 M12 3 V21 M8 7 L12 3 L16 7",
      "arrow-up-circle" => "M12 22 A10 10 0 1 0 12 2 A10 10 0 1 0 12 22 M12 16 V8 M8 12 L12 8 L16 12",
      "arrow-down-circle" => "M12 2 A10 10 0 1 0 12 22 A10 10 0 1 0 12 2 M12 8 V16 M8 12 L12 16 L16 12",
      "arrow-left-circle" => "M22 12 A10 10 0 1 0 2 12 A10 10 0 1 0 22 12 M16 12 H8 M12 8 L8 12 L12 16",
      "arrow-right-circle" => "M2 12 A10 10 0 1 0 22 12 A10 10 0 1 0 2 12 M8 12 H16 M12 8 L16 12 L12 16",
      "chevron-up" => "M18 15 L12 9 L6 15",
      "chevron-down" => "M6 9 L12 15 L18 9",
      "chevron-left" => "M15 18 L9 12 L15 6",
      "chevron-right" => "M9 18 L15 12 L9 6",
      "chevrons-up" => "M7 11 L12 6 L17 11 M7 18 L12 13 L17 18",
      "chevrons-down" => "M7 6 L12 11 L17 6 M7 13 L12 18 L17 13",
      "chevrons-left" => "M11 7 L6 12 L11 17 M18 7 L13 12 L18 17",
      "chevrons-right" => "M6 7 L11 12 L6 17 M13 7 L18 12 L13 17",
      "corner-down-right" => "M5 5 V12 Q5 19 12 19 H19 M14 14 L19 19 L14 24",
      "refresh-cw" => "M20 7 V3 L16 7 M4 17 V21 L8 17 M5 9 A8 8 0 0 1 19 6 L20 7 M4 17 L5 18 A8 8 0 0 0 19 15",
      "refresh-ccw" => "M4 7 V3 L8 7 M20 17 V21 L16 17 M19 9 A8 8 0 0 0 5 6 L4 7 M20 17 L19 18 A8 8 0 0 1 5 15",
      "repeat" => "M17 2 L21 6 L17 10 M3 11 V9 Q3 6 6 6 H21 M7 22 L3 18 L7 14 M21 13 V15 Q21 18 18 18 H3",

      // ==========================================
      // ACTIONS
      // ==========================================
      "plus" => "M12 4 V20 M4 12 H20",
      "minus" => "M4 12 H20",
      "x" => "M5 5 L19 19 M19 5 L5 19",
      "check" => "M4 12 L9 17 L20 6",
      "check-circle" => "M22 12 A10 10 0 1 0 2 12 A10 10 0 1 0 22 12 M7 12 L10 15 L17 8",
      "x-circle" => "M22 12 A10 10 0 1 0 2 12 A10 10 0 1 0 22 12 M9 9 L15 15 M15 9 L9 15",
      "plus-circle" => "M22 12 A10 10 0 1 0 2 12 A10 10 0 1 0 22 12 M12 7 V17 M7 12 H17",
      "minus-circle" => "M22 12 A10 10 0 1 0 2 12 A10 10 0 1 0 22 12 M7 12 H17",
      "edit" => "M12 20 H21 M16.5 3.5 A2.1 2.1 0 0 1 19.5 6.5 L8 18 L4 20 L6 16 Z",
      "edit-2" => "M15 5 L19 9 M4 20 L8 19 L19 8 A2.8 2.8 0 0 0 15 4 L4 15 Z",
      "edit-3" => "M12 20 H21 M16 4 L20 8 M4 20 L8 19 L19 8 A3 3 0 0 0 15 4 L4 15 Z",
      "pencil" => "M12 20 H21 M16 4 L20 8 M4 20 L8 19 L19 8 A3 3 0 0 0 15 4 L4 15 Z",
      "trash" => "M3 6 H21 M8 6 V4 H16 V6 M19 6 L18 21 H6 L5 6 M10 11 V17 M14 11 V17",
      "trash-2" => "M3 6 H21 M8 6 V4 H16 V6 M19 6 L18 21 H6 L5 6 M10 11 V17 M14 11 V17",
      "copy" => "M8 8 H20 V21 H8 Z M4 16 H3 V3 H16 V4",
      "clipboard" => "M9 4 H15 V7 H9 Z M8 5 H5 V21 H19 V5 H16 M9 14 L11 16 L16 11",
      "save" => "M5 3 H17 L21 7 V21 H3 V3 Z M7 3 V9 H17 V3 M7 21 V14 H17 V21",
      "download" => "M12 3 V15 M7 10 L12 15 L17 10 M4 19 H20",
      "upload" => "M12 15 V3 M7 8 L12 3 L17 8 M4 19 H20",
      "share" => "M18 8 A3 3 0 1 0 18 2 A3 3 0 1 0 18 8 M6 15 A3 3 0 1 0 6 9 A3 3 0 1 0 6 15 M18 22 A3 3 0 1 0 18 16 A3 3 0 1 0 18 22 M8.5 10.5 L15.5 6.5 M8.5 13.5 L15.5 17.5",
      "filter" => "M4 4 H20 L14 12 V19 L10 21 V12 Z",
      "search" => "M11 3 A8 8 0 1 0 11 19 A8 8 0 1 0 11 3 M17 17 L22 22",
      "zoom-in" => "M11 3 A8 8 0 1 0 11 19 A8 8 0 1 0 11 3 M17 17 L22 22 M11 7 V15 M7 11 H15",
      "zoom-out" => "M11 3 A8 8 0 1 0 11 19 A8 8 0 1 0 11 3 M17 17 L22 22 M7 11 H15",
      "more-horizontal" => "M5 12 A1 1 0 1 0 5 12.1 M12 12 A1 1 0 1 0 12 12.1 M19 12 A1 1 0 1 0 19 12.1",
      "more-vertical" => "M12 5 A1 1 0 1 0 12 5.1 M12 12 A1 1 0 1 0 12 12.1 M12 19 A1 1 0 1 0 12 19.1",
      "sliders" => "M4 21 V14 M4 10 V3 M12 21 V12 M12 8 V3 M20 21 V16 M20 12 V3 M2 14 H6 M10 8 H14 M18 16 H22",
      "settings" => "M12 8 A4 4 0 1 0 12 16 A4 4 0 1 0 12 8 M12 2 V5 M12 19 V22 M2 12 H5 M19 12 H22 M4.9 4.9 L7 7 M17 17 L19.1 19.1 M19.1 4.9 L17 7 M7 17 L4.9 19.1",
      "tool" => "M14 7 A5 5 0 0 0 7 14 L3 18 L6 21 L10 17 A5 5 0 0 0 17 10 L21 6 L18 3 Z",
      "wrench" => "M14 7 A5 5 0 0 0 7 14 L3 18 L6 21 L10 17 A5 5 0 0 0 17 10 L21 6 L18 3 Z",

      // ==========================================
      // USER & PEOPLE
      // ==========================================
      "user" => "M12 12 A4 4 0 1 0 12 4 A4 4 0 1 0 12 12 M4 22 C4 17 8 15 12 15 C16 15 20 17 20 22",
      "user-round" => "M12 12 A4 4 0 1 0 12 4 A4 4 0 1 0 12 12 M4 22 C4 17 8 15 12 15 C16 15 20 17 20 22",
      "users" => "M16 21 V19 C16 16 14 14 11 14 H7 C4 14 2 16 2 19 V21 M9 11 A4 4 0 1 0 9 3 A4 4 0 1 0 9 11 M16 3 A4 4 0 0 1 16 11 M18 14 C21 15 22 17 22 20",
      "user-plus" => "M15 21 V19 C15 16 13 14 10 14 H6 C3 14 1 16 1 19 V21 M8 11 A4 4 0 1 0 8 3 A4 4 0 1 0 8 11 M19 8 V14 M16 11 H22",
      "user-minus" => "M15 21 V19 C15 16 13 14 10 14 H6 C3 14 1 16 1 19 V21 M8 11 A4 4 0 1 0 8 3 A4 4 0 1 0 8 11 M16 11 H22",
      "user-check" => "M15 21 V19 C15 16 13 14 10 14 H6 C3 14 1 16 1 19 V21 M8 11 A4 4 0 1 0 8 3 A4 4 0 1 0 8 11 M16 11 L18 13 L22 8",
      "user-x" => "M15 21 V19 C15 16 13 14 10 14 H6 C3 14 1 16 1 19 V21 M8 11 A4 4 0 1 0 8 3 A4 4 0 1 0 8 11 M17 9 L22 14 M22 9 L17 14",
      "contact" => "M16 2 H8 V6 H16 Z M6 4 H4 V22 H20 V4 H18 M8 12 H16 M8 16 H13",
      "id-card" => "M3 4 H21 V20 H3 Z M8 10 A2 2 0 1 0 8 6 A2 2 0 1 0 8 10 M5 16 C5 13 11 13 11 16 M14 9 H18 M14 13 H18",

      // ==========================================
      // COMMUNICATION
      // ==========================================
      "mail" => "M3 5 H21 V19 H3 Z M3 6 L12 13 L21 6",
      "message-circle" => "M21 11.5 A8.5 8.5 0 0 1 8.5 20 L3 21 L4 15.5 A8.5 8.5 0 1 1 21 11.5",
      "message-square" => "M3 3 H21 V17 H8 L3 21 Z",
      "messages-square" => "M21 11 H8 L3 16 V3 H21 Z M8 16 V21 H18 L21 18",
      "phone" => "M5 3 H9 L11 8 L8 10 C9 13 11 15 14 16 L16 13 L21 15 V19 C21 21 19 22 17 21 C9 19 4 14 2 6 C1 4 3 3 5 3 Z",
      "phone-call" => "M5 3 H9 L11 8 L8 10 C9 13 11 15 14 16 L16 13 L21 15 V19 C21 21 19 22 17 21 C9 19 4 14 2 6 C1 4 3 3 5 3 Z M15 3 Q20 4 21 9 M15 7 Q17 7 17 9",
      "video" => "M3 6 H15 V18 H3 Z M15 10 L21 6 V18 L15 14 Z",
      "video-off" => "M3 6 H15 V18 H3 Z M15 10 L21 6 V18 L15 14 Z M3 3 L21 21",
      "bell" => "M18 8 A6 6 0 0 0 6 8 C6 15 3 16 3 18 H21 C21 16 18 15 18 8 M10 21 H14",
      "bell-off" => "M3 3 L21 21 M18 8 A6 6 0 0 0 8 5 M6 8 C6 15 3 16 3 18 H17 M10 21 H14",
      "send" => "M22 2 L11 13 M22 2 L15 22 L11 13 L2 9 Z",
      "at-sign" => "M16 8 A4 4 0 1 0 16 16 A4 4 0 1 0 16 8 M16 8 V14 A3 3 0 0 0 22 14 V12 A10 10 0 1 0 18 20",
      "wifi" => "M2 8 A16 16 0 0 1 22 8 M5 12 A11 11 0 0 1 19 12 M8.5 16 A5 5 0 0 1 15.5 16 M12 20 H12",
      "wifi-off" => "M3 3 L21 21 M2 8 A16 16 0 0 1 18 7 M5 12 A11 11 0 0 1 14 11 M8.5 16 A5 5 0 0 1 11 15",

      // ==========================================
      // FILES & DOCUMENTS
      // ==========================================
      "file" => "M5 2 H14 L20 8 V22 H5 Z M14 2 V8 H20",
      "file-text" => "M5 2 H14 L20 8 V22 H5 Z M14 2 V8 H20 M8 13 H16 M8 17 H16",
      "file-plus" => "M5 2 H14 L20 8 V22 H5 Z M14 2 V8 H20 M12 12 V18 M9 15 H15",
      "file-minus" => "M5 2 H14 L20 8 V22 H5 Z M14 2 V8 H20 M9 15 H15",
      "file-check" => "M5 2 H14 L20 8 V22 H5 Z M14 2 V8 H20 M8 15 L11 18 L17 12",
      "file-x" => "M5 2 H14 L20 8 V22 H5 Z M14 2 V8 H20 M9 13 L15 19 M15 13 L9 19",
      "folder" => "M3 5 H10 L12 8 H21 V20 H3 Z",
      "folder-open" => "M3 5 H10 L12 8 H21 L18 19 H3 Z",
      "folder-plus" => "M3 5 H10 L12 8 H21 V20 H3 Z M12 11 V17 M9 14 H15",
      "folder-check" => "M3 5 H10 L12 8 H21 V20 H3 Z M8 14 L11 17 L16 12",
      "archive" => "M3 4 H21 V8 H3 Z M5 8 V20 H19 V8 M10 12 H14",
      "book" => "M4 3 H20 V21 H4 Z M8 3 V21 M12 8 H17 M12 12 H17",
      "book-open" => "M2 4 H9 C11 4 12 6 12 8 V21 C12 19 10 18 8 18 H2 Z M22 4 H15 C13 4 12 6 12 8 V21 C12 19 14 18 16 18 H22 Z",
      "notebook" => "M5 3 H20 V21 H5 Z M3 7 H5 M3 12 H5 M3 17 H5 M9 8 H16 M9 12 H16",
      "paperclip" => "M21 11 L11 21 A6 6 0 0 1 3 13 L13 3 A4 4 0 0 1 19 9 L9 19 A2 2 0 0 1 6 16 L15 7",
      "printer" => "M6 9 V3 H18 V9 M6 18 H4 V9 H20 V18 H18 M6 14 H18 V21 H6 Z M17 12 H17.1",
      "clipboard-check" => "M9 4 H15 V7 H9 Z M8 5 H5 V21 H19 V5 H16 M8 14 L11 17 L16 12",

      // ==========================================
      // MEDIA & ENTERTAINMENT
      // ==========================================
      "play" => "M7 3 L20 12 L7 21 Z",
      "pause" => "M7 4 H10 V20 H7 Z M14 4 H17 V20 H14 Z",
      "stop" => "M5 5 H19 V19 H5 Z",
      "skip-back" => "M6 5 V19 M19 5 L9 12 L19 19 Z",
      "skip-forward" => "M18 5 V19 M5 5 L15 12 L5 19 Z",
      "rewind" => "M11 5 L2 12 L11 19 Z M22 5 L13 12 L22 19 Z",
      "fast-forward" => "M2 5 L11 12 L2 19 Z M13 5 L22 12 L13 19 Z",
      "volume" => "M3 9 V15 H7 L12 19 V5 L7 9 Z M16 9 Q20 12 16 15",
      "volume-1" => "M3 9 V15 H7 L12 19 V5 L7 9 Z M16 9 Q19 12 16 15",
      "volume-2" => "M3 9 V15 H7 L12 19 V5 L7 9 Z M16 9 Q20 12 16 15 M18 5 Q24 12 18 19",
      "volume-x" => "M3 9 V15 H7 L12 19 V5 L7 9 Z M16 9 L21 15 M21 9 L16 15",
      "music" => "M9 18 V5 L21 3 V16 M9 18 A3 3 0 1 0 9 18.1 M21 16 A3 3 0 1 0 21 16.1",
      "music-2" => "M9 18 V5 L21 3 V16 M9 18 A3 3 0 1 0 9 18.1 M21 16 A3 3 0 1 0 21 16.1",
      "headphones" => "M3 13 V11 A9 9 0 0 1 21 11 V13 M3 13 H7 V20 H5 Q3 20 3 18 Z M21 13 H17 V20 H19 Q21 20 21 18 Z",
      "mic" => "M12 2 A3 3 0 0 0 9 5 V12 A3 3 0 0 0 15 12 V5 A3 3 0 0 0 12 2 M5 11 V12 A7 7 0 0 0 19 12 V11 M12 19 V22 M8 22 H16",
      "mic-off" => "M3 3 L21 21 M9 9 V12 A3 3 0 0 0 15 12 V11 M9 5 A3 3 0 0 1 15 5 V8 M5 11 V12 A7 7 0 0 0 17 18 M12 19 V22 M8 22 H16",
      "radio" => "M3 7 L21 3 M3 7 V21 H21 V7 Z M7 12 A2 2 0 1 0 7 16 A2 2 0 1 0 7 12 M13 13 H18 M13 17 H18",
      "tv" => "M3 5 H21 V18 H3 Z M8 22 H16 M12 18 V22 M8 2 L12 5 L16 2",
      "monitor" => "M3 3 H21 V17 H3 Z M8 21 H16 M12 17 V21",
      "smartphone" => "M6 2 H18 V22 H6 Z M10 18 H14",
      "tablet" => "M4 2 H20 V22 H4 Z M11 18 H13",
      "camera" => "M3 7 H7 L9 4 H15 L17 7 H21 V20 H3 Z M12 16 A4 4 0 1 0 12 8 A4 4 0 1 0 12 16",
      "image" => "M3 3 H21 V21 H3 Z M8 9 A2 2 0 1 0 8 5 A2 2 0 1 0 8 9 M3 17 L9 11 L13 15 L16 12 L21 17",
      "images" => "M5 5 H19 V19 H5 Z M2 9 V2 H16 M8 15 L11 12 L14 15 L16 13 L19 16",
      "film" => "M4 3 H20 V21 H4 Z M4 7 H20 M4 17 H20 M8 3 V7 M16 3 V7 M8 17 V21 M16 17 V21",
      "clapperboard" => "M3 7 H21 V21 H3 Z M3 7 L5 3 L10 7 L13 3 L18 7 L21 3",
      "radio-tower" => "M12 2 L8 22 M12 2 L16 22 M5 8 A10 10 0 0 1 19 8 M2 5 A14 14 0 0 1 22 5 M12 12 V12.1",

      // ==========================================
      // TIME & CALENDAR
      // ==========================================
      "clock" => "M12 2 A10 10 0 1 0 12 22 A10 10 0 1 0 12 2 M12 6 V12 L16 14",
      "watch" => "M8 2 H16 V5 H8 Z M8 19 H16 V22 H8 Z M5 5 H19 V19 H5 Z M12 8 V12 L15 14",
      "calendar" => "M3 5 H21 V21 H3 Z M16 3 V7 M8 3 V7 M3 10 H21",
      "calendar-days" => "M3 5 H21 V21 H3 Z M16 3 V7 M8 3 V7 M3 10 H21 M8 14 H8.1 M12 14 H12.1 M16 14 H16.1 M8 17 H8.1 M12 17 H12.1",
      "calendar-check" => "M3 5 H21 V21 H3 Z M16 3 V7 M8 3 V7 M3 10 H21 M8 15 L11 18 L17 12",
      "timer" => "M10 2 H14 M12 14 L16 10 M12 6 A8 8 0 1 0 12 22 A8 8 0 1 0 12 6",
      "alarm-clock" => "M12 8 V12 L15 14 M12 22 A9 9 0 1 0 12 4 A9 9 0 1 0 12 22 M5 3 L2 6 M19 3 L22 6",
      "hourglass" => "M5 2 H19 M5 22 H19 M6 2 C6 8 9 10 12 12 C15 14 18 16 18 22 M18 2 C18 8 15 10 12 12 C9 14 6 16 6 22",

      // ==========================================
      // SHOPPING & FINANCE
      // ==========================================
      "shopping-cart" => "M3 3 H5 L8 16 H19 L22 6 H7 M10 21 A1 1 0 1 0 10 21.1 M18 21 A1 1 0 1 0 18 21.1",
      "shopping-bag" => "M4 8 H20 L21 21 H3 Z M8 8 V6 A4 4 0 0 1 16 6 V8",
      "shopping-basket" => "M3 10 H21 L19 21 H5 Z M8 10 L12 3 M16 10 L12 3 M8 14 V18 M16 14 V18",
      "wallet" => "M3 5 H20 V21 H3 Z M3 5 L17 2 V5 M15 11 H21 V17 H15 A3 3 0 1 1 15 11",
      "credit-card" => "M2 5 H22 V19 H2 Z M2 10 H22 M6 15 H10",
      "banknote" => "M2 6 H22 V18 H2 Z M12 9 A3 3 0 1 0 12 15 A3 3 0 1 0 12 9 M5 9 H5.1 M19 15 H19.1",
      "receipt" => "M4 2 L7 4 L10 2 L13 4 L16 2 L20 4 V22 L16 20 L13 22 L10 20 L7 22 L4 20 Z M8 9 H16 M8 13 H16 M8 17 H13",
      "receipt-text" => "M4 2 L7 4 L10 2 L13 4 L16 2 L20 4 V22 L16 20 L13 22 L10 20 L7 22 L4 20 Z M8 9 H16 M8 13 H16 M8 17 H13",
      "tag" => "M20 13 L13 20 L3 10 V3 H10 Z M7 7 A1 1 0 1 0 7 7.1",
      "tags" => "M20 13 L13 20 L3 10 V3 H10 Z M7 7 A1 1 0 1 0 7 7.1 M14 3 H20 V9",
      "gift" => "M3 8 H21 V21 H3 Z M2 8 H22 V12 H2 Z M12 8 V21 M12 8 C5 8 4 3 8 3 C11 3 12 8 12 8 M12 8 C19 8 20 3 16 3 C13 3 12 8 12 8",
      "coins" => "M12 8 A8 3 0 1 0 12 14 A8 3 0 1 0 12 8 M4 11 V15 A8 3 0 0 0 20 15 V11 M7 4 A8 3 0 1 1 20 6 M7 4 V8",
      "dollar-sign" => "M12 2 V22 M17 5 H9 A4 4 0 0 0 9 13 H15 A4 4 0 0 1 15 21 H7",
      "percent" => "M19 5 L5 19 M7 7 A2 2 0 1 0 7 7.1 M17 17 A2 2 0 1 0 17 17.1",
      "trending-up" => "M3 17 L9 11 L13 15 L21 7 M15 7 H21 V13",
      "trending-down" => "M3 7 L9 13 L13 9 L21 17 M15 17 H21 V11",
      "chart-line" => "M3 3 V21 H21 M5 16 L10 11 L14 14 L21 6",
      "chart-bar" => "M3 3 V21 H21 M7 17 V12 M12 17 V6 M17 17 V9",
      "chart-pie" => "M12 3 A9 9 0 1 0 21 12 H12 Z M12 3 V12 H21",
      "calculator" => "M4 2 H20 V22 H4 Z M7 6 H17 V10 H7 Z M8 14 H8.1 M12 14 H12.1 M16 14 H16.1 M8 18 H8.1 M12 18 H12.1 M16 18 H16.1",

      // ==========================================
      // DEVICES & HARDWARE
      // ==========================================
      "laptop" => "M4 4 H20 V16 H4 Z M2 20 H22 L20 16 H4 Z",
      "keyboard" => "M2 5 H22 V19 H2 Z M6 9 H6.1 M10 9 H10.1 M14 9 H14.1 M18 9 H18.1 M6 13 H6.1 M10 13 H10.1 M14 13 H14.1 M18 13 H18.1 M8 16 H16",
      "mouse" => "M12 2 A8 8 0 0 0 4 10 V14 A8 8 0 0 0 20 14 V10 A8 8 0 0 0 12 2 M12 2 V10",
      "printer-check" => "M6 9 V3 H18 V9 M6 18 H4 V9 H20 V18 H18 M6 14 H18 V21 H6 Z M9 16 L11 18 L15 14",
      "server" => "M3 3 H21 V9 H3 Z M3 15 H21 V21 H3 Z M7 6 H7.1 M7 18 H7.1 M11 6 H17 M11 18 H17",
      "database" => "M3 5 A9 3 0 1 0 21 5 A9 3 0 1 0 3 5 M3 5 V19 A9 3 0 0 0 21 19 V5 M3 12 A9 3 0 0 0 21 12",
      "hard-drive" => "M3 6 H21 L23 18 H1 Z M6 14 H6.1 M10 14 H10.1 M15 10 H18",
      "cpu" => "M5 5 H19 V19 H5 Z M9 9 H15 V15 H9 Z M9 1 V5 M15 1 V5 M9 19 V23 M15 19 V23 M1 9 H5 M1 15 H5 M19 9 H23 M19 15 H23",
      "battery" => "M3 7 H20 V17 H3 Z M20 10 H22 V14 H20 M6 10 H6.1 M10 10 H10.1 M14 10 H14.1",
      "battery-charging" => "M3 7 H20 V17 H3 Z M20 10 H22 V14 H20 M12 8 L9 13 H13 L11 17",
      "bluetooth" => "M7 7 L17 17 L12 21 V3 L17 7 L7 17 M7 7 L17 17",
      "power" => "M12 2 V12 M5.6 5.6 A9 9 0 1 0 18.4 5.6",

      // ==========================================
      // LOCATION & TRANSPORT
      // ==========================================
      "map-pin" => "M20 10 C20 16 12 22 12 22 C12 22 4 16 4 10 A8 8 0 1 1 20 10 M12 10 A2 2 0 1 0 12 10.1",
      "navigation" => "M12 2 L21 22 L12 17 L3 22 Z",
      "navigation-2" => "M12 2 L21 22 L12 17 L3 22 Z",
      "globe" => "M12 2 A10 10 0 1 0 12 22 A10 10 0 1 0 12 2 M2 12 H22 M12 2 C7 7 7 17 12 22 M12 2 C17 7 17 17 12 22",
      "car" => "M5 11 L7 5 H17 L19 11 L21 13 V19 H3 V13 Z M5 11 H19 M6 16 H6.1 M18 16 H18.1",
      "bus" => "M4 5 Q4 3 7 3 H17 Q20 3 20 5 V18 H4 Z M4 9 H20 M7 18 V21 M17 18 V21 M7 14 H7.1 M17 14 H17.1",
      "bike" => "M5 18 A4 4 0 1 0 5 18.1 M19 18 A4 4 0 1 0 19 18.1 M5 18 L9 10 L14 18 H5 L12 7 H16 M14 7 H17",
      "plane" => "M22 2 L15 22 L11 13 L2 9 Z M22 2 L11 13",
      "train" => "M5 3 H19 V18 H5 Z M5 9 H19 M8 21 L10 18 M16 18 L18 21 M8 13 H8.1 M16 13 H16.1",
      "truck" => "M2 5 H15 V17 H2 Z M15 9 H19 L22 13 V17 H15 Z M6 20 A2 2 0 1 0 6 20.1 M18 20 A2 2 0 1 0 18 20.1",

      // ==========================================
      // STATUS & ALERTS
      // ==========================================
      "alert-circle" => "M22 12 A10 10 0 1 0 2 12 A10 10 0 1 0 22 12 M12 7 V13 M12 17 H12.1",
      "alert-triangle" => "M10 3 L2 19 Q1 21 4 21 H20 Q23 21 22 19 L14 3 Q12 0 10 3 Z M12 9 V14 M12 17 H12.1",
      "alert-octagon" => "M8 2 H16 L22 8 V16 L16 22 H8 L2 16 V8 Z M12 7 V13 M12 17 H12.1",
      "info" => "M12 2 A10 10 0 1 0 12 22 A10 10 0 1 0 12 2 M12 11 V17 M12 7 H12.1",
      "help-circle" => "M22 12 A10 10 0 1 0 2 12 A10 10 0 1 0 22 12 M9 9 A3 3 0 1 1 14 11 C12 12 12 13 12 14 M12 17 H12.1",
      "shield" => "M12 2 L21 6 V12 C21 17 17 20 12 22 C7 20 3 17 3 12 V6 Z",
      "shield-check" => "M12 2 L21 6 V12 C21 17 17 20 12 22 C7 20 3 17 3 12 V6 Z M8 12 L11 15 L16 9",
      "shield-alert" => "M12 2 L21 6 V12 C21 17 17 20 12 22 C7 20 3 17 3 12 V6 Z M12 8 V13 M12 17 H12.1",
      "lock" => "M5 10 H19 V21 H5 Z M8 10 V7 A4 4 0 0 1 16 7 V10",
      "unlock" => "M5 10 H19 V21 H5 Z M8 10 V7 A4 4 0 0 1 16 7",
      "key" => "M15 8 A5 5 0 1 0 15 8.1 M14 11 L21 18 M18 15 L20 13 M20 17 L22 15",
      "eye" => "M2 12 Q12 -1 22 12 Q12 25 2 12 Z M12 9 A3 3 0 1 0 12 15 A3 3 0 1 0 12 9",
      "eye-off" => "M3 3 L21 21 M10 10 A3 3 0 0 0 14 14 M9 5 Q12 4 15 5 Q19 7 22 12 Q20 16 16 18 M6 7 Q3 9 2 12 Q4 16 8 18 Q11 20 14 19",
      "flag" => "M4 22 V3 M4 4 H19 L16 9 L19 14 H4",

      // ==========================================
      // WEATHER & NATURE
      // ==========================================
      "sun" => "M12 8 A4 4 0 1 0 12 16 A4 4 0 1 0 12 8 M12 2 V4 M12 20 V22 M4.9 4.9 L6.3 6.3 M17.7 17.7 L19.1 19.1 M2 12 H4 M20 12 H22 M4.9 19.1 L6.3 17.7 M17.7 6.3 L19.1 4.9",
      "moon" => "M20 15 A8 8 0 0 1 9 4 A9 9 0 1 0 20 15",
      "cloud" => "M5 19 H18 A4 4 0 0 0 18 11 A7 7 0 0 0 5 10 A4.5 4.5 0 0 0 5 19",
      "cloud-rain" => "M5 15 H18 A4 4 0 0 0 18 7 A7 7 0 0 0 5 6 A4.5 4.5 0 0 0 5 15 M8 18 L7 21 M13 18 L12 21 M18 18 L17 21",
      "cloud-sun" => "M8 5 V2 M3 10 H1 M4 4 L2 2 M14 5 A4 4 0 0 0 6 5 M5 19 H18 A4 4 0 0 0 18 11 A7 7 0 0 0 5 10 A4.5 4.5 0 0 0 5 19",
      "cloud-snow" => "M5 15 H18 A4 4 0 0 0 18 7 A7 7 0 0 0 5 6 A4.5 4.5 0 0 0 5 15 M8 18 H8.1 M12 20 H12.1 M16 18 H16.1",
      "umbrella" => "M12 22 V12 M12 2 A10 10 0 0 0 2 12 Q7 9 12 12 Q17 9 22 12 A10 10 0 0 0 12 2 Z M12 22 A3 3 0 0 0 15 19",
      "leaf" => "M20 4 C8 3 3 9 3 16 C3 20 6 21 9 20 C16 18 20 11 20 4 Z M4 20 L14 10",
      "tree" => "M12 22 V14 M5 14 L12 3 L19 14 H5 Z M3 19 L8 10 L13 19 H3 Z M11 19 L16 10 L21 19 H11 Z",
      "flower" => "M12 12 C7 12 7 5 12 5 C17 5 17 12 12 12 C12 7 19 7 19 12 C19 17 12 17 12 12 C17 12 17 19 12 19 C7 19 7 12 12 12 C12 17 5 17 5 12 C5 7 12 7 12 12 Z M12 19 V23",
      "flame" => "M12 22 C4 22 3 15 7 10 C7 14 10 14 10 9 C10 5 13 3 14 2 C14 8 21 11 19 17 C18 20 15 22 12 22 Z",

      // ==========================================
      // SHAPES & DESIGN
      // ==========================================
      "circle" => "M12 2 A10 10 0 1 0 12 22 A10 10 0 1 0 12 2",
      "square" => "M4 4 H20 V20 H4 Z",
      "rectangle" => "M2 5 H22 V19 H2 Z",
      "triangle" => "M12 3 L22 21 H2 Z",
      "diamond" => "M12 2 L22 12 L12 22 L2 12 Z",
      "heart" => "M12 21 C10 19 3 14 3 9 A5 5 0 0 1 12 6 A5 5 0 0 1 21 9 C21 14 14 19 12 21 Z",
      "star" => "M12 2 L15 9 H22 L16 14 L18 22 L12 17 L6 22 L8 14 L2 9 H9 Z",
      "bookmark" => "M5 3 H19 V22 L12 17 L5 22 Z",
      "palette" => "M12 3 A9 9 0 1 0 12 21 H15 A3 3 0 0 0 15 15 H14 A2 2 0 0 1 14 11 H18 A4 4 0 0 0 22 7 A9 9 0 0 0 12 3 Z M7 10 H7.1 M10 7 H10.1 M15 7 H15.1",
      "paintbrush" => "M14 4 L20 10 L10 20 L4 14 Z M4 14 C2 17 2 20 2 22 C5 22 8 22 10 20",
      "brush" => "M14 4 L20 10 L10 20 L4 14 Z M4 14 C2 17 2 20 2 22 C5 22 8 22 10 20",
      "pen-tool" => "M12 22 L3 13 L14 2 L22 10 Z M14 2 L22 10 M3 13 L9 13 L11 19",
      "scissors" => "M6 6 A3 3 0 1 0 6 6.1 M6 18 A3 3 0 1 0 6 18.1 M8 8 L20 20 M8 16 L20 4",
      "ruler" => "M3 17 L17 3 L22 8 L8 22 Z M7 13 L10 16 M11 9 L14 12 M15 5 L18 8",
      "crop" => "M6 2 V18 H22 M2 6 H18 V22",
      "move" => "M12 2 L8 6 M12 2 L16 6 M12 2 V22 M12 22 L8 18 M12 22 L16 18 M2 12 H22 M2 12 L6 8 M2 12 L6 16 M22 12 L18 8 M22 12 L18 16",

      // ==========================================
      // MISCELLANEOUS
      // ==========================================
      "activity" => "M22 12 H18 L15 21 L9 3 L6 12 H2",
      "zap" => "M13 2 L3 14 H11 L10 22 L21 9 H13 Z",
      "award" => "M12 15 A6 6 0 1 0 12 3 A6 6 0 1 0 12 15 M8 14 L6 22 L12 19 L18 22 L16 14",
      "trophy" => "M8 21 H16 M12 17 V21 M5 3 H19 V9 A7 7 0 0 1 5 9 Z M5 6 H2 V9 Q2 14 7 14 M19 6 H22 V9 Q22 14 17 14",
      "crown" => "M2 7 L7 12 L12 3 L17 12 L22 7 L20 20 H4 Z",
      "coffee" => "M4 8 H18 V17 A4 4 0 0 1 14 21 H8 A4 4 0 0 1 4 17 Z M18 10 H20 A3 3 0 0 1 20 16 H18 M8 2 V5 M12 2 V5",
      "cup-soda" => "M5 3 H19 L17 21 H7 Z M7 8 H17 M12 3 V1 M15 3 L18 1",
      "utensils" => "M4 2 V10 A3 3 0 0 0 10 10 V2 M7 2 V22 M17 2 V22 M17 2 C22 5 22 12 17 14",
      "pizza" => "M3 3 Q15 -1 22 5 L12 22 Z M7 7 A1.5 1.5 0 1 0 7 7.1 M13 6 A1.5 1.5 0 1 0 13 6.1 M12 14 A1.5 1.5 0 1 0 12 14.1",
      "cookie" => "M20 13 A8 8 0 1 1 11 4 A4 4 0 0 0 20 13 Z M8 11 H8.1 M13 15 H13.1 M9 17 H9.1",
      "package" => "M3 7 L12 2 L21 7 V17 L12 22 L3 17 Z M3 7 L12 12 L21 7 M12 12 V22 M7 4.5 L16 9.5",
      "box" => "M3 7 L12 2 L21 7 V17 L12 22 L3 17 Z M3 7 L12 12 L21 7 M12 12 V22",
      "lamp" => "M9 18 H15 M10 22 H14 M12 2 A7 7 0 0 0 8 15 C9 16 9 17 9 18 H15 C15 17 15 16 16 15 A7 7 0 0 0 12 2",
      "lightbulb" => "M9 18 H15 M10 22 H14 M12 2 A7 7 0 0 0 8 15 C9 16 9 17 9 18 H15 C15 17 15 16 16 15 A7 7 0 0 0 12 2",
      "rocket" => "M12 2 C17 3 20 7 20 13 L14 19 H10 L4 13 C4 7 7 3 12 2 Z M9 19 L7 22 M15 19 L17 22 M12 7 A2 2 0 1 0 12 7.1 M4 13 L2 17 L7 17 M20 13 L22 17 L17 17",
      "anchor" => "M12 2 V15 M8 6 H16 M12 2 A2 2 0 1 0 12 2.1 M3 12 A9 9 0 0 0 21 12 M3 12 H7 M17 12 H21",
      "bug" => "M8 8 H16 V16 A4 4 0 0 1 8 16 Z M12 8 V16 M3 9 H8 M16 9 H21 M3 15 H8 M16 15 H21 M7 3 L10 8 M17 3 L14 8",
      "puzzle" => "M3 3 H9 A3 3 0 1 1 15 3 H21 V9 A3 3 0 1 0 21 15 V21 H15 A3 3 0 1 0 9 21 H3 V15 A3 3 0 1 0 3 9 Z",
      "hand" => "M7 11 V5 A2 2 0 0 1 11 5 V10 V4 A2 2 0 0 1 15 4 V10 V6 A2 2 0 0 1 19 6 V12 L20 11 A2 2 0 0 1 23 13 L21 19 Q20 22 17 22 H11 Q8 22 7 19 L3 14 A2 2 0 0 1 6 11 Z",
      "thumbs-up" => "M7 10 H3 V21 H7 Z M7 10 L12 3 Q15 3 14 7 L13 10 H19 A2 2 0 0 1 21 12 L19 20 H7",
      "thumbs-down" => "M7 14 H3 V3 H7 Z M7 14 L12 21 Q15 21 14 17 L13 14 H19 A2 2 0 0 0 21 12 L19 4 H7",
      "smile" => "M12 2 A10 10 0 1 0 12 22 A10 10 0 1 0 12 2 M8 14 Q12 19 16 14 M9 9 H9.1 M15 9 H15.1",
      "frown" => "M12 2 A10 10 0 1 0 12 22 A10 10 0 1 0 12 2 M8 17 Q12 12 16 17 M9 9 H9.1 M15 9 H15.1",
      "meh" => "M12 2 A10 10 0 1 0 12 22 A10 10 0 1 0 12 2 M8 15 H16 M9 9 H9.1 M15 9 H15.1",
      "heart-handshake" => "M12 21 L4 14 A5 5 0 0 1 11 7 L12 8 L13 7 A5 5 0 0 1 20 14 Z M3 12 L7 16 L10 13 L14 17 L18 13",
      "sparkles" => "M12 3 L14 9 L20 11 L14 13 L12 19 L10 13 L4 11 L10 9 Z M19 2 L20 5 L23 6 L20 7 L19 10 L18 7 L15 6 L18 5 Z M5 16 L6 19 L9 20 L6 21 L5 24 L4 21 L1 20 L4 19 Z",

      // ==========================================
      // FALLBACK
      // ==========================================
      _ => null
    };

    if (string.IsNullOrEmpty(pathData))
    {
      return new TextBlock
      {
        Text = $"? {name}",
        Foreground = ParseBrush(color),
        FontSize = size,
        Margin = ParseOptionalThickness(props, "margin")
              ?? new Thickness(0)
      };
    }

    var geometry = Geometry.Parse(pathData);

    var iconPath = new System.Windows.Shapes.Path
    {
      Data = geometry,
      Stroke = ParseBrush(color),
      StrokeThickness = Math.Max(1, size / 12),
      StrokeStartLineCap = PenLineCap.Round,
      StrokeEndLineCap = PenLineCap.Round,
      StrokeLineJoin = PenLineJoin.Round,
      Stretch = Stretch.Uniform,
      Width = size,
      Height = size,
      Margin = ParseOptionalThickness(props, "margin")
            ?? new Thickness(0)
    };

    return iconPath;
  }


  // Image
  private FrameworkElement BuildImage(JsonElement props)
  {
    string source = GetString(props, "source") ?? "";

    string fit = GetString(props, "fit") ?? "contain";

    var image = new System.Windows.Controls.Image
    {
      Stretch = Normalize(fit) switch
      {
        "contain" => Stretch.Uniform,
        "cover" => Stretch.UniformToFill,
        "fill" => Stretch.Fill,
        "none" => Stretch.None,
        _ => Stretch.Uniform
      },

      Margin =
            ParseOptionalThickness(props, "margin")
            ?? new Thickness(0)
    };

    // Width
    double? width = GetNumber(props, "width");

    if (width.HasValue)
      image.Width = width.Value;

    // Height
    double? height = GetNumber(props, "height");

    if (height.HasValue)
      image.Height = height.Value;

    // Load local image
    if (!string.IsNullOrWhiteSpace(source))
    {
      string imagePath = System.IO.Path.IsPathRooted(source)
          ? source
          : System.IO.Path.Combine(
              Environment.CurrentDirectory,
              source
          );

      imagePath = System.IO.Path.GetFullPath(imagePath);

      if (File.Exists(imagePath))
      {
        var bitmap = new BitmapImage();

        bitmap.BeginInit();
        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        bitmap.UriSource = new Uri(
            imagePath,
            UriKind.Absolute
        );
        bitmap.EndInit();
        bitmap.Freeze();

        image.Source = bitmap;
      }
    }

    return image;
  }

  // Divider Handler
  private FrameworkElement BuildDivider(JsonElement props)
  {
    string orientation =
        GetString(props, "orientation") ?? "horizontal";

    double thickness =
        GetNumber(props, "thickness") ?? 1;

    bool isHorizontal =
        Normalize(orientation) == "horizontal";

    var divider = new Border
    {
      Background = ParseBrush(
            GetString(props, "color") ?? "#cccccc"
        ),

      Margin =
            ParseOptionalThickness(props, "margin")
            ?? new Thickness(0),

      HorizontalAlignment = isHorizontal
            ? HorizontalAlignment.Stretch
            : HorizontalAlignment.Left,

      VerticalAlignment = isHorizontal
            ? VerticalAlignment.Top
            : VerticalAlignment.Stretch
    };

    if (isHorizontal)
    {
      divider.Height = thickness;
    }
    else
    {
      divider.Width = thickness;
    }

    return divider;
  }

  // Spacer Handler
  private FrameworkElement BuildSpacer(JsonElement props)
  {
    var spacer = new Border
    {
      Background = Brushes.Transparent
    };

    double? width = GetNumber(props, "width");
    if (width.HasValue)
      spacer.Width = width.Value;

    double? height = GetNumber(props, "height");
    if (height.HasValue)
      spacer.Height = height.Value;

    return spacer;
  }

  // ScrollView Handler
  private FrameworkElement BuildScrollView(
    JsonElement props,
    JsonElement children)
  {
    bool vertical =
        GetBoolean(props, "vertical") ?? false;

    bool horizontal =
        GetBoolean(props, "horizontal") ?? false;

    // ScrollViewer hanya menerima satu Content.
    // Jika ada beberapa children, bungkus dalam StackPanel.
    FrameworkElement content;

    if (children.ValueKind == JsonValueKind.Array &&
        children.GetArrayLength() == 1)
    {
      content = BuildElement(children[0]);
    }
    else
    {
      var stack = new StackPanel
      {
        Orientation = Orientation.Vertical
      };

      if (children.ValueKind == JsonValueKind.Array)
      {
        foreach (JsonElement child in children.EnumerateArray())
        {
          stack.Children.Add(BuildElement(child));
        }
      }

      content = stack;
    }

    var scrollViewer = new ScrollViewer
    {
      Content = content,

      VerticalScrollBarVisibility = vertical
            ? ScrollBarVisibility.Auto
            : ScrollBarVisibility.Disabled,

      HorizontalScrollBarVisibility = horizontal
            ? ScrollBarVisibility.Auto
            : ScrollBarVisibility.Disabled,

      CanContentScroll = false,

      Padding =
            ParseOptionalThickness(props, "padding")
            ?? new Thickness(0),

      Margin =
            ParseOptionalThickness(props, "margin")
            ?? new Thickness(0),

      Background = ParseBrush(
            GetString(props, "background") ?? "Transparent"
        )
    };

    // Width
    double? width = GetNumber(props, "width");

    if (width.HasValue)
      scrollViewer.Width = width.Value;

    // Height
    double? height = GetNumber(props, "height");

    if (height.HasValue)
      scrollViewer.Height = height.Value;

    // Enabled
    bool? isEnabled = GetBoolean(props, "isEnabled");

    if (isEnabled.HasValue)
      scrollViewer.IsEnabled = isEnabled.Value;

    // Visibility
    bool? visible = GetBoolean(props, "visible");

    if (visible.HasValue)
    {
      scrollViewer.Visibility = visible.Value
          ? Visibility.Visible
          : Visibility.Collapsed;
    }

    return scrollViewer;
  }

  private FrameworkElement BuildStack(
      JsonElement props,
      JsonElement children)
  {
    string orientation =
        GetString(props, "orientation") ?? "vertical";

    double spacing =
        GetNumber(props, "spacing") ?? 0;

    bool isHorizontal =
        Normalize(orientation) == "horizontal";

    // Panel utama
    Panel panel = new StackPanel
    {
      Orientation = isHorizontal
            ? Orientation.Horizontal
            : Orientation.Vertical
    };

    // Baca semua children
    var childElements = new List<FrameworkElement>();
    var flexValues = new List<double>();

    if (children.ValueKind == JsonValueKind.Array)
    {
      foreach (JsonElement item in children.EnumerateArray())
      {
        FrameworkElement child = BuildElement(item);

        double flex = 0;

        if (item.TryGetProperty("props", out JsonElement childProps))
        {
          flex = GetNumber(childProps, "flex") ?? 0;
        }

        childElements.Add(child);
        flexValues.Add(Math.Max(0, flex));
      }
    }

    // Periksa apakah ada child menggunakan flex
    bool hasFlex = flexValues.Any(flex => flex > 0);

    if (hasFlex)
    {
      var grid = new Grid();

      // Buat kolom atau baris berdasarkan orientation
      for (int i = 0; i < childElements.Count; i++)
      {
        double flex = flexValues[i];

        if (isHorizontal)
        {
          grid.ColumnDefinitions.Add(
              new ColumnDefinition
              {
                Width = flex > 0
                      ? new GridLength(flex, GridUnitType.Star)
                      : GridLength.Auto
              }
          );
        }
        else
        {
          grid.RowDefinitions.Add(
              new RowDefinition
              {
                Height = flex > 0
                      ? new GridLength(flex, GridUnitType.Star)
                      : GridLength.Auto
              }
          );
        }
      }

      // Masukkan children ke Grid
      for (int i = 0; i < childElements.Count; i++)
      {
        FrameworkElement child = childElements[i];

        if (isHorizontal)
        {
          Grid.SetColumn(child, i);
        }
        else
        {
          Grid.SetRow(child, i);
        }

        // Spacing antar item
        if (i > 0 && spacing > 0)
        {
          Thickness currentMargin = child.Margin;

          child.Margin = isHorizontal
              ? new Thickness(
                  currentMargin.Left + spacing,
                  currentMargin.Top,
                  currentMargin.Right,
                  currentMargin.Bottom)
              : new Thickness(
                  currentMargin.Left,
                  currentMargin.Top + spacing,
                  currentMargin.Right,
                  currentMargin.Bottom);
        }

        grid.Children.Add(child);
      }

      panel = grid;
    }
    else
    {
      // Layout biasa menggunakan StackPanel
      var stackPanel = (StackPanel)panel;

      for (int i = 0; i < childElements.Count; i++)
      {
        FrameworkElement child = childElements[i];

        // Spacing antar item
        if (i > 0 && spacing > 0)
        {
          Thickness currentMargin = child.Margin;

          child.Margin = isHorizontal
              ? new Thickness(
                  currentMargin.Left + spacing,
                  currentMargin.Top,
                  currentMargin.Right,
                  currentMargin.Bottom)
              : new Thickness(
                  currentMargin.Left,
                  currentMargin.Top + spacing,
                  currentMargin.Right,
                  currentMargin.Bottom);
        }

        stackPanel.Children.Add(child);
      }
    }

    // Container untuk padding, margin, dan background
    var container = new Border
    {
      Child = panel,

      Padding =
            ParseOptionalThickness(props, "padding")
            ?? new Thickness(0),

      Margin =
            ParseOptionalThickness(props, "margin")
            ?? new Thickness(0),

      Background = ParseBrush(
            GetString(props, "background") ?? "Transparent"
        )
    };

    // Width
    double? width = GetNumber(props, "width");

    if (width.HasValue)
      container.Width = width.Value;

    // Height
    double? height = GetNumber(props, "height");

    if (height.HasValue)
      container.Height = height.Value;

    // Enabled
    bool? isEnabled = GetBoolean(props, "isEnabled");

    if (isEnabled.HasValue)
      container.IsEnabled = isEnabled.Value;

    // Visibility
    bool? visible = GetBoolean(props, "visible");

    if (visible.HasValue)
    {
      container.Visibility = visible.Value
          ? Visibility.Visible
          : Visibility.Collapsed;
    }

    return container;
  }


  private FrameworkElement BuildText(JsonElement props)
  {
    string value =
        GetString(props, "text") ??
        GetString(props, "content") ??
        "";

    var text = new TextBlock
    {
      Text = value,
      FontSize = GetNumber(props, "fontSize") ?? 16,
      TextWrapping = Normalize(
            GetString(props, "textWrapping") ?? "wrap"
        ) == "nowrap"
            ? TextWrapping.NoWrap
            : TextWrapping.Wrap
    };

    string? foreground =
        GetString(props, "foreground") ??
        GetString(props, "color");

    if (foreground != null)
      text.Foreground = ParseBrush(foreground);

    string? fontWeight = GetString(props, "fontWeight");
    if (fontWeight != null)
      text.FontWeight = ParseFontWeight(fontWeight);

    string? margin = GetStringOrNumber(props, "margin");
    if (margin != null)
      text.Margin = ParseThickness(margin);

    return text;
  }

  private FrameworkElement BuildButton(JsonElement props)
  {
    string content =
        GetString(props, "content") ??
        GetString(props, "title") ??
        "Button";

    string? callbackId = GetString(props, "onPress");

    var button = new Button
    {
      Content = content,
      FontSize = GetNumber(props, "fontSize") ?? 14,
      Padding = ParseOptionalThickness(props, "padding")
            ?? new Thickness(12, 7, 12, 7),
      IsEnabled = GetBoolean(props, "isEnabled") ?? true
    };

    string? margin = GetStringOrNumber(props, "margin");
    if (margin != null)
      button.Margin = ParseThickness(margin);

    if (!string.IsNullOrWhiteSpace(callbackId))
    {
      button.Click += (_, _) =>
      {
        _invokeCallback(callbackId);
      };
    }

    return button;
  }

  private FrameworkElement BuildBorder(
      JsonElement props,
      JsonElement children)
  {
    var border = new Border();

    string? background = GetString(props, "background");
    if (background != null)
      border.Background = ParseBrush(background);

    string? brush = GetString(props, "brush");
    if (brush != null)
      border.BorderBrush = ParseBrush(brush);

    string? thickness = GetStringOrNumber(props, "thickness");
    if (thickness != null)
      border.BorderThickness = ParseThickness(thickness);

    string? radius = GetStringOrNumber(props, "radius");
    if (radius != null)
      border.CornerRadius = ParseCornerRadius(radius);

    string? padding = GetStringOrNumber(props, "padding");
    if (padding != null)
      border.Padding = ParseThickness(padding);

    string? margin = GetStringOrNumber(props, "margin");
    if (margin != null)
      border.Margin = ParseThickness(margin);

    if (children.ValueKind == JsonValueKind.Array &&
        children.GetArrayLength() > 0)
    {
      border.Child = BuildElement(children[0]);
    }

    return border;
  }

  // ========================================================
  // VALUE CONVERTERS
  // ========================================================

  private static Thickness ParseThickness(string value)
  {
    return (Thickness)new ThicknessConverter()
        .ConvertFromString(null, CultureInfo.InvariantCulture, value)!;
  }

  private static CornerRadius ParseCornerRadius(string value)
  {
    return (CornerRadius)new CornerRadiusConverter()
        .ConvertFromString(null, CultureInfo.InvariantCulture, value)!;
  }

  private static Brush ParseBrush(string value)
  {
    return (Brush)new BrushConverter()
        .ConvertFromString(value)!;
  }

  private static FontWeight ParseFontWeight(string value)
  {
    return Normalize(value) switch
    {
      "thin" => FontWeights.Thin,
      "extralight" => FontWeights.ExtraLight,
      "light" => FontWeights.Light,
      "medium" => FontWeights.Medium,
      "semibold" => FontWeights.SemiBold,
      "bold" => FontWeights.Bold,
      "extrabold" => FontWeights.ExtraBold,
      "black" => FontWeights.Black,
      _ => FontWeights.Normal
    };
  }

  private static string Normalize(string value)
  {
    return value
        .Replace("-", "")
        .Replace("_", "")
        .Replace(" ", "")
        .ToLowerInvariant();
  }

  private static Thickness? ParseOptionalThickness(
      JsonElement props,
      string name)
  {
    string? value = GetStringOrNumber(props, name);

    return value == null
        ? null
        : ParseThickness(value);
  }

  // ========================================================
  // JSON HELPERS
  // ========================================================

  private static JsonElement GetProperty(
      JsonElement element,
      string name)
  {
    if (element.ValueKind == JsonValueKind.Object &&
        element.TryGetProperty(name, out JsonElement value))
    {
      return value;
    }

    return default;
  }

  private static string? GetString(
      JsonElement element,
      string name)
  {
    JsonElement value = GetProperty(element, name);

    return value.ValueKind == JsonValueKind.String
        ? value.GetString()
        : null;
  }

  private static string? GetStringOrNumber(
      JsonElement element,
      string name)
  {
    JsonElement value = GetProperty(element, name);

    return value.ValueKind switch
    {
      JsonValueKind.String => value.GetString(),
      JsonValueKind.Number => value.GetRawText(),
      _ => null
    };
  }

  private static double? GetNumber(
      JsonElement element,
      string name)
  {
    JsonElement value = GetProperty(element, name);

    return value.ValueKind == JsonValueKind.Number &&
           value.TryGetDouble(out double result)
        ? result
        : null;
  }

  private static bool? GetBoolean(
      JsonElement element,
      string name)
  {
    JsonElement value = GetProperty(element, name);

    return value.ValueKind switch
    {
      JsonValueKind.True => true,
      JsonValueKind.False => false,
      _ => null
    };
  }
}
