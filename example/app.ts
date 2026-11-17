import { Window, Stack, Text, Button, Border, runApp } from "@vormir/sdk";

function Main() {
  return Window({
    title: "Vormir App",
    width: 800,
    height: 600,
    background: "#216de0",
    location: {
      x: 0,
      y: 0,
    },
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
            margin: "0,0,0,12",
          }),

          Text({
            text: "Native Desktop UI Framework",
            fontSize: 15,
            foreground: "#64748b",
            margin: "0,0,0,20",
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
                    margin: "0,0,0,12",
                  }),

                  Button({
                    content: "Klik Saya",
                    padding: "16,8",
                    onPress: () => {
                      console.log("Tombol berhasil diklik!");
                    },
                  }),
                ],
              }),
            ],
          }),
        ],
      }),
    ],
  });
}

runApp(Main);
