import { Window, Stack, Text, Button, useState, runApp } from "@vormir/sdk";

function App() {
  const [count, setCount] = useState(0);

  return Window({
    title: "Vormir State Management",
    width: 500,
    height: 350,
    children: [
      Stack({
        orientation: "vertical",
        spacing: 12,
        padding: 20,
        children: [
          Text({
            text: `Counter: ${count}`,
            fontSize: 24,
          }),

          Button({
            content: "Tambah",
            onPress: () => setCount((previous) => previous + 1),
          }),

          Button({
            content: "Reset",
            onPress: () => setCount(0),
          }),
        ],
      }),
    ],
  });
}

runApp(App);
