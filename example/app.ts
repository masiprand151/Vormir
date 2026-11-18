import { Window, Stack, Text, ProgressBar, runApp } from "@vormir/sdk";

function Main() {
  return Window({
    title: "ProgressBar Example",
    width: 500,
    height: 350,
    children: [
      Stack({
        orientation: "vertical",
        spacing: 15,
        padding: 20,
        children: [
          Text({
            text: "Download Progress",
            fontSize: 20,
          }),

          ProgressBar({
            value: 65,
            min: 0,
            max: 100,
            color: "#2563eb",
            trackColor: "#e5e7eb",
            height: 10,
          }),

          Text({
            text: "65%",
            fontSize: 14,
          }),
        ],
      }),
    ],
  });
}

runApp(Main);
