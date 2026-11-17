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
      init_vnode();
      init_renderer();
    }
  });

  // example/app.ts
  var require_app = __commonJS({
    "example/app.ts"() {
      init_src();
      function Main() {
        return Window({
          title: "Vormir Stack",
          width: 800,
          height: 600,
          location: "center-screen",
          children: [
            Stack({
              orientation: "horizontal",
              children: [
                Stack({
                  width: 200,
                  children: [Text({ text: "Sidebar" })]
                }),
                Stack({
                  flex: 2,
                  background: "#eeeeee",
                  children: [Text({ text: "Konten utama" })]
                }),
                Stack({
                  width: 10,
                  background: "#dddddd",
                  children: [Text({ text: "Konten kedua" })]
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
