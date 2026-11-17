import { Window, Stack, Text, Button, runApp } from "@vormir/sdk";

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
            children: [Text({ text: "Sidebar" })],
          }),

          Stack({
            flex: 2,
            background: "#eeeeee",
            children: [Text({ text: "Konten utama" })],
          }),

          Stack({
            width: 10,
            background: "#dddddd",
            children: [Text({ text: "Konten kedua" })],
          }),
        ],
      }),
    ],
  });
}

runApp(Main);
