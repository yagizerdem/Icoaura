import { RouterProvider } from "react-router/dom";
import { router } from "./routs";
import { useEffect } from "react";

function App() {
  useEffect(() => {
    let rootElement = document.documentElement;
    rootElement.classList = "";

    rootElement.classList.add("dark-default");
  }, []);

  return <RouterProvider router={router} />;
}

export default App;
