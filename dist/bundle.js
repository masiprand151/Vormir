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

  // sdk/src/controls/controls.ts
  function Window(props) {
    return createVNode("Window", props, props.children ?? []);
  }
  function Stack(props) {
    return createVNode("Stack", props, props.children ?? []);
  }
  function Text(props) {
    return createVNode("Text", props);
  }
  function Button(props) {
    return createVNode("Button", props);
  }
  function Border(props) {
    return createVNode("Border", props, props.children ?? []);
  }
  var init_controls = __esm({
    "sdk/src/controls/controls.ts"() {
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
    if (node === null || node === void 0 || node === false) {
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

  // sdk/src/components/Window.ts
  var init_Window = __esm({
    "sdk/src/components/Window.ts"() {
      "use strict";
      init_vnode();
    }
  });

  // sdk/src/components/Stack.ts
  var init_Stack = __esm({
    "sdk/src/components/Stack.ts"() {
      "use strict";
      init_vnode();
    }
  });

  // sdk/src/components/Text.ts
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

  // sdk/src/index.ts
  var init_src = __esm({
    "sdk/src/index.ts"() {
      "use strict";
      init_controls();
      init_vnode();
      init_renderer();
      init_Window();
      init_Stack();
      init_Text();
      init_Button();
      init_Border();
    }
  });

  // example/app.ts
  var require_app = __commonJS({
    "example/app.ts"() {
      init_src();
      function Main() {
        return Window({
          title: "Vormir App",
          width: 800,
          height: 600,
          background: "#f5f7fa",
          location: "center-screen",
          appBar: false,
          children: [
            Stack({
              orientation: "vertical",
              margin: "20",
              children: [
                Text({
                  text: "Welcome to Vormir",
                  fontSize: 28,
                  fontWeight: "semi-bold",
                  foreground: "#222222",
                  margin: "0,0,0,12"
                }),
                Text({
                  text: "Native Desktop UI Framework",
                  fontSize: 15,
                  foreground: "#64748b",
                  margin: "0,0,0,20"
                }),
                Border({
                  background: "#ffffff",
                  brush: "#cbd5e1",
                  thickness: "1",
                  radius: "8",
                  padding: "16",
                  children: [
                    Stack({
                      orientation: "vertical",
                      children: [
                        Text({
                          text: "Vormir SDK",
                          fontSize: 20,
                          fontWeight: "semi-bold",
                          margin: "0,0,0,12"
                        }),
                        Button({
                          content: "Klik Saya",
                          padding: "16,8",
                          onPress: () => {
                            console.log("Tombol berhasil diklik!");
                          }
                        })
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
