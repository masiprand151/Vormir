import { Window, Stack, Text, Button, runApp, ScrollView } from "@vormir/sdk";

function Main() {
  return Window({
    title: "Vormir Stack",
    width: 800,
    height: 600,
    location: "center-screen",
    children: [
      Stack({
        children: [
          ScrollView({
            vertical: true,
            horizontal: false,
            height: 100,
            children: [
              Stack({
                spacing: 10,
                children: Array.from({ length: 100 }).map((_, i) =>
                  Text({ text: `Item ${i + 1}` }),
                ),
              }),
            ],
          }),
        ],
      }),
    ],
  });
}

runApp(Main);
