
using System;
using System.Globalization;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shell;
using System.Linq;

namespace Vormir.UI;

public sealed class WpfNativeBridge
{
  private readonly Window _window;
  private readonly Action<string> _invokeCallback;

  public WpfNativeBridge(
      Window window,
      Action<string> invokeCallback)
  {
    _window = window;
    _invokeCallback = invokeCallback;
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

      _ => new TextBlock
      {
        Text = $"Unknown component: {type}",
        Foreground = Brushes.Red
      }
    };
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
