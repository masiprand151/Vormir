import { Window, Stack, Text, Button, runApp } from "@vormir/sdk";

function Main() {
  return Window({
    title: "Vormir Stack",
    width: 800,
    height: 600,
    location: "center-screen",

    children: [
      Stack({
        orientation: "vertical",
        spacing: 16,
        padding: 16,

        children: [
          Text({ text: "Item Pertama", fontSize: 24 }),
          Button({ content: "Item Kedua" }),
          Text({ text: "Item Ketiga", fontSize: 18 }),
        ],
      }),
    ],
  });
}

runApp(Main);
