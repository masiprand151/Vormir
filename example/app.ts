import { Window, Stack, Text, Slider, runApp } from "@vormir/sdk";

function Main() {
  let volume = 0;

  return Window({
    title: "Slider Example",
    width: 500,
    height: 350,
    children: [
      Stack({
        orientation: "vertical",
        spacing: 15,
        padding: 20,
        children: [
          Text({
            text: "Volume",
            fontSize: 20,
          }),

          Slider({
            value: volume,
            min: 0,
            max: 100,
            width: 300,
            onChange: (value) => {
              volume = value;
              console.log("Volume:", value);
            },
          }),
        ],
      }),
    ],
  });
}

runApp(Main);
