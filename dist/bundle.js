(() => {
  var __getOwnPropNames = Object.getOwnPropertyNames;
  var __esm = (fn, res) => function __init() {
    return fn && (res = (0, fn[__getOwnPropNames(fn)[0]])(fn = 0)), res;
  };
  var __commonJS = (cb, mod) => function __require() {
    return mod || (0, cb[__getOwnPropNames(cb)[0]])((mod = { exports: {} }).exports, mod), mod.exports;
  };

  // sdk/src/core/vnode.ts
  function createVNode(type, props = {}, children = []) {
    return {
      type,
      props,
      children: children.filter(
        (child) => child !== null && child !== void 0 && child !== false
      )
    };
  }
  var init_vnode = __esm({
    "sdk/src/core/vnode.ts"() {
      "use strict";
    }
  });

  // sdk/src/components/Window.ts
  function Window(props) {
    if (typeof props.location === "object" && props.location.x === 0) {
      props.location.offsetX = -7;
    }
    return createVNode("Window", props, props.children ?? []);
  }
  var init_Window = __esm({
    "sdk/src/components/Window.ts"() {
      "use strict";
      init_vnode();
    }
  });

  // sdk/src/components/Stack.ts
  function Stack(props) {
    return createVNode("Stack", props, props.children ?? []);
  }
  var init_Stack = __esm({
    "sdk/src/components/Stack.ts"() {
      "use strict";
      init_vnode();
    }
  });

  // sdk/src/components/Text.ts
  function Text(props) {
    return createVNode("Text", props);
  }
  var init_Text = __esm({
    "sdk/src/components/Text.ts"() {
      "use strict";
      init_vnode();
    }
  });

  // sdk/src/components/Button.ts
  var init_Button = __esm({
    "sdk/src/components/Button.ts"() {
      "use strict";
      init_vnode();
    }
  });

  // sdk/src/components/Border.ts
  var init_Border = __esm({
    "sdk/src/components/Border.ts"() {
      "use strict";
      init_vnode();
    }
  });

  // sdk/src/components/ScrollView.ts
  function ScrollView(props) {
    return createVNode("ScrollView", props, props.children ?? []);
  }
  var init_ScrollView = __esm({
    "sdk/src/components/ScrollView.ts"() {
      "use strict";
      init_vnode();
    }
  });

  // sdk/src/components/Spacer.ts
  var init_Spacer = __esm({
    "sdk/src/components/Spacer.ts"() {
      "use strict";
      init_vnode();
    }
  });

  // sdk/src/components/Divider.ts
  function Divider(props = {}) {
    return createVNode("Divider", props);
  }
  var init_Divider = __esm({
    "sdk/src/components/Divider.ts"() {
      "use strict";
      init_vnode();
    }
  });

  // sdk/src/components/Image.ts
  var init_Image = __esm({
    "sdk/src/components/Image.ts"() {
      "use strict";
      init_vnode();
    }
  });

  // sdk/src/components/Icon.ts
  function Icon(props) {
    return createVNode("Icon", props);
  }
  var init_Icon = __esm({
    "sdk/src/components/Icon.ts"() {
      "use strict";
      init_vnode();
    }
  });

  // sdk/src/core/renderer.ts
  function getCallbacks() {
    if (!globalThis.__vormirCallbacks) {
      globalThis.__vormirCallbacks = {};
    }
    return globalThis.__vormirCallbacks;
  }
  function registerCallback(callback) {
    const id = `vormir_callback_${++callbackCounter}`;
    getCallbacks()[id] = callback;
    return id;
  }
  function serializeVNode(node) {
    if (node === null || node === void 0 || typeof node === "boolean") {
      return null;
    }
    if (typeof node === "string" || typeof node === "number") {
      return {
        type: "Text",
        props: {
          text: String(node)
        },
        children: []
      };
    }
    if (!("type" in node) || !("props" in node)) {
      return null;
    }
    const props = {};
    for (const [key, value] of Object.entries(node.props ?? {})) {
      if (typeof value === "function") {
        if (key === "onPress" || key === "onClick" || key === "onChange" || key === "onSubmit") {
          props[key] = registerCallback(value);
        }
      } else {
        props[key] = value;
      }
    }
    return {
      type: node.type,
      props,
      children: (node.children ?? []).map((child) => serializeVNode(child)).filter((child) => child !== null)
    };
  }
  function render(node) {
    if (typeof globalThis.__vormirRender !== "function") {
      throw new Error("Vormir Native Bridge belum tersedia.");
    }
    const tree = serializeVNode(node);
    globalThis.__vormirRender(JSON.stringify(tree));
  }
  function runApp(app) {
    render(app());
  }
  var callbackCounter;
  var init_renderer = __esm({
    "sdk/src/core/renderer.ts"() {
      "use strict";
      callbackCounter = 0;
    }
  });

  // sdk/src/index.ts
  var init_src = __esm({
    "sdk/src/index.ts"() {
      "use strict";
      init_Window();
      init_Stack();
      init_Text();
      init_Button();
      init_Border();
      init_ScrollView();
      init_Spacer();
      init_Divider();
      init_Image();
      init_Icon();
      init_vnode();
      init_renderer();
    }
  });

  // example/app.ts
  var require_app = __commonJS({
    "example/app.ts"() {
      init_src();
      function IconItem({ name }) {
        return Stack({
          orientation: "vertical",
          spacing: 8,
          width: 90,
          padding: 8,
          children: [
            Icon({
              name,
              size: 28,
              color: "#2563eb"
            }),
            Text({
              text: name,
              fontSize: 11
            })
          ]
        });
      }
      function IconSection({ title, icons }) {
        return Stack({
          orientation: "vertical",
          spacing: 12,
          children: [
            Text({
              text: title,
              fontSize: 18,
              fontWeight: "bold"
            }),
            Stack({
              orientation: "horizontal",
              spacing: 8,
              children: icons.map((name) => IconItem({ name }))
            }),
            Divider({
              color: "#dddddd",
              thickness: 1
            })
          ]
        });
      }
      function Main() {
        return Window({
          title: "Vormir - Icon Gallery",
          width: 900,
          height: 700,
          children: [
            ScrollView({
              vertical: true,
              padding: 20,
              children: [
                Stack({
                  orientation: "vertical",
                  spacing: 24,
                  children: [
                    Text({
                      text: "Vormir Icon Gallery",
                      fontSize: 28,
                      fontWeight: "bold"
                    }),
                    IconSection({
                      title: "Navigation",
                      icons: [
                        "home",
                        "house",
                        "menu",
                        "menu-2",
                        "grid",
                        "grid-2x2",
                        "layout",
                        "sidebar",
                        "panel-left",
                        "panel-right",
                        "maximize",
                        "minimize",
                        "expand",
                        "shrink",
                        "external-link",
                        "link",
                        "link-2",
                        "compass",
                        "map"
                      ]
                    }),
                    IconSection({
                      title: "Arrows",
                      icons: [
                        "arrow-up",
                        "arrow-down",
                        "arrow-left",
                        "arrow-right",
                        "arrow-up-left",
                        "arrow-up-right",
                        "arrow-down-left",
                        "arrow-down-right",
                        "arrow-left-right",
                        "arrow-up-down",
                        "arrow-up-circle",
                        "arrow-down-circle",
                        "arrow-left-circle",
                        "arrow-right-circle",
                        "chevron-up",
                        "chevron-down",
                        "chevron-left",
                        "chevron-right",
                        "chevrons-up",
                        "chevrons-down",
                        "chevrons-left",
                        "chevrons-right",
                        "corner-down-right",
                        "refresh-cw",
                        "refresh-ccw",
                        "repeat"
                      ]
                    }),
                    IconSection({
                      title: "Actions",
                      icons: [
                        "plus",
                        "minus",
                        "x",
                        "check",
                        "check-circle",
                        "x-circle",
                        "plus-circle",
                        "minus-circle",
                        "edit",
                        "edit-2",
                        "edit-3",
                        "pencil",
                        "trash",
                        "trash-2",
                        "copy",
                        "clipboard",
                        "clipboard-check",
                        "save",
                        "download",
                        "upload",
                        "share",
                        "filter",
                        "search",
                        "zoom-in",
                        "zoom-out",
                        "more-horizontal",
                        "more-vertical",
                        "sliders",
                        "settings",
                        "tool",
                        "wrench"
                      ]
                    }),
                    IconSection({
                      title: "User & People",
                      icons: [
                        "user",
                        "user-round",
                        "users",
                        "user-plus",
                        "user-minus",
                        "user-check",
                        "user-x",
                        "contact",
                        "id-card"
                      ]
                    }),
                    IconSection({
                      title: "Communication",
                      icons: [
                        "mail",
                        "message-circle",
                        "message-square",
                        "messages-square",
                        "phone",
                        "phone-call",
                        "video",
                        "video-off",
                        "bell",
                        "bell-off",
                        "send",
                        "at-sign",
                        "wifi",
                        "wifi-off"
                      ]
                    }),
                    IconSection({
                      title: "Files",
                      icons: [
                        "file",
                        "file-text",
                        "file-plus",
                        "file-minus",
                        "file-check",
                        "file-x",
                        "folder",
                        "folder-open",
                        "folder-plus",
                        "folder-check",
                        "archive",
                        "book",
                        "book-open",
                        "notebook",
                        "paperclip",
                        "printer"
                      ]
                    }),
                    IconSection({
                      title: "Media",
                      icons: [
                        "play",
                        "pause",
                        "stop",
                        "skip-back",
                        "skip-forward",
                        "rewind",
                        "fast-forward",
                        "volume",
                        "volume-1",
                        "volume-2",
                        "volume-x",
                        "music",
                        "music-2",
                        "headphones",
                        "mic",
                        "mic-off",
                        "radio",
                        "tv",
                        "monitor",
                        "smartphone",
                        "tablet",
                        "camera",
                        "image",
                        "images",
                        "film",
                        "clapperboard",
                        "radio-tower"
                      ]
                    }),
                    IconSection({
                      title: "Time & Calendar",
                      icons: [
                        "clock",
                        "watch",
                        "calendar",
                        "calendar-days",
                        "calendar-check",
                        "timer",
                        "alarm-clock",
                        "hourglass"
                      ]
                    }),
                    IconSection({
                      title: "Shopping & Finance",
                      icons: [
                        "shopping-cart",
                        "shopping-bag",
                        "shopping-basket",
                        "wallet",
                        "credit-card",
                        "banknote",
                        "receipt",
                        "receipt-text",
                        "tag",
                        "tags",
                        "gift",
                        "coins",
                        "dollar-sign",
                        "percent",
                        "trending-up",
                        "trending-down",
                        "chart-line",
                        "chart-bar",
                        "chart-pie",
                        "calculator"
                      ]
                    }),
                    IconSection({
                      title: "Devices",
                      icons: [
                        "laptop",
                        "keyboard",
                        "mouse",
                        "printer-check",
                        "server",
                        "database",
                        "hard-drive",
                        "cpu",
                        "battery",
                        "battery-charging",
                        "bluetooth",
                        "power"
                      ]
                    }),
                    IconSection({
                      title: "Location & Transport",
                      icons: [
                        "map-pin",
                        "navigation",
                        "navigation-2",
                        "globe",
                        "car",
                        "bus",
                        "bike",
                        "plane",
                        "train",
                        "truck"
                      ]
                    }),
                    IconSection({
                      title: "Status & Security",
                      icons: [
                        "alert-circle",
                        "alert-triangle",
                        "alert-octagon",
                        "info",
                        "help-circle",
                        "shield",
                        "shield-check",
                        "shield-alert",
                        "lock",
                        "unlock",
                        "key",
                        "eye",
                        "eye-off",
                        "flag"
                      ]
                    }),
                    IconSection({
                      title: "Weather & Nature",
                      icons: [
                        "sun",
                        "moon",
                        "cloud",
                        "cloud-rain",
                        "cloud-sun",
                        "cloud-snow",
                        "umbrella",
                        "leaf",
                        "tree",
                        "flower",
                        "flame"
                      ]
                    }),
                    IconSection({
                      title: "Shapes & Design",
                      icons: [
                        "circle",
                        "square",
                        "rectangle",
                        "triangle",
                        "diamond",
                        "heart",
                        "star",
                        "bookmark",
                        "palette",
                        "paintbrush",
                        "brush",
                        "pen-tool",
                        "scissors",
                        "ruler",
                        "crop",
                        "move"
                      ]
                    }),
                    IconSection({
                      title: "Miscellaneous",
                      icons: [
                        "activity",
                        "zap",
                        "award",
                        "trophy",
                        "crown",
                        "coffee",
                        "cup-soda",
                        "utensils",
                        "pizza",
                        "cookie",
                        "package",
                        "box",
                        "lamp",
                        "lightbulb",
                        "rocket",
                        "anchor",
                        "bug",
                        "puzzle",
                        "hand",
                        "thumbs-up",
                        "thumbs-down",
                        "smile",
                        "frown",
                        "meh",
                        "heart-handshake",
                        "sparkles"
                      ]
                    })
                  ]
                })
              ]
            })
          ]
        });
      }
      runApp(Main);
    }
  });
  require_app();
})();
//# sourceMappingURL=bundle.js.map
