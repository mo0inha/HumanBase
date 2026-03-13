import { Card } from "./components/ui/card";
import { api } from "./lib/api";
import React from "react";

type WeatherForecast = {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
};

export function App() {
  const [data, setData] = React.useState<WeatherForecast[]>();

  React.useEffect(() => {
    async function fetchData() {
      const response = await api.get<WeatherForecast[]>("WeatherForecast");
      setData(response.data);
    }
    fetchData();
  }, []);

  return (
    <main className="dark max-w-7xl mx-auto flex flex-col gap-3">
      {data?.map((Weather) => (
        <WeatherForecastComponent
          item={Weather}
          key={Weather.summary}
          className="bg-yellow-500"
        />
      ))}
    </main>
  );
}

const WeatherForecastComponent = ({
  item,
  ...props
}: { item: WeatherForecast } & React.ComponentProps<"button">) => {
  return (
    <button
      className="p-3 border rounded-lg flex flex-col gap-3 bg-secondary"
      onClick={() => console.log("asdasd")}
      {...props}
    >
      <p>date: {item.date}</p>
      <p>temperatureC: {item.temperatureC}</p>
      <p>temperatureF: {item.temperatureF}</p>
      <p>summary: {item.summary}</p>
    </button>
  );
};
