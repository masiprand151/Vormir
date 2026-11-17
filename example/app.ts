const callbacks = (globalThis as any).__vormirCallbacks;

callbacks["hello"] = () => {
  console.log("Tombol berhasil diklik!");
};

const tree = {
  type: "View",
  props: {
    style: {
      flexDirection: "column",
      padding: 24,
      gap: 12,
    },
  },
  children: [
    {
      type: "Text",
      props: {
        text: "Selamat datang di Vormir!",
        style: {
          fontSize: 28,
          color: "#222222",
        },
      },
      children: [],
    },
    {
      type: "Text",
      props: {
        text: "Native UI menggunakan WPF Renderer.",
      },
      children: [],
    },
    {
      type: "Button",
      props: {
        title: "Klik Saya",
        onPress: "hello",
      },
      children: [],
    },
  ],
};

(globalThis as any).__vormirRender(JSON.stringify(tree));
