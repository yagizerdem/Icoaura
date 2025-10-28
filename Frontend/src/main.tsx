import { createRoot } from "react-dom/client";
import "./flash.css";
import "./index.css";
import "toastify-js/src/toastify.css";
import "./modern.scrollbar.css";
import App from "./App.tsx";
import { BaseProvider } from "./Providers/BaseProvider.tsx";

createRoot(document.getElementById("root")!).render(
  <BaseProvider>
    <App />
  </BaseProvider>
);
