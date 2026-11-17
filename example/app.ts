import { Window, Stack, Text, Button, runApp, ScrollView } from "@vormir/sdk";

function Main() {
  const items = Array.from({ length: 100 }).map((_, i) =>
    Text({ text: `Item ${i + 1}` }),
  );
  return Window({
    title: "Vormir Stack",
    width: 800,
    height: 600,
    location: "center-screen",
    children: [
      Stack({
        orientation: "vertical",
        children: [
          Text({
            text: "Header",
            height: 50,
          }),

          ScrollView({
            flex: 1,
            vertical: true,
            children: [
              Stack({
                spacing: 10,
                children: items,
              }),
            ],
          }),

          Text({
            text: "Footer",
            height: 40,
          }),
        ],
      }),
    ],
  });
}

runApp(Main);
