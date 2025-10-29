import { RouterProvider } from "react-router/dom";
import { router } from "./routs";
import { useEffect } from "react";
import type { AppConfig } from "./models/AppConfig";
import { getAppConfig } from "./service/appConfigService";
import type { ApiResponse } from "./models/ApiResponse";
import { useAppContext } from "./Providers/AppContext";

function App() {
  const { setAppConfig } = useAppContext();

  useEffect(() => {
    initialize();
    async function initialize() {
      const response: ApiResponse<AppConfig> = await getAppConfig();
      if (response.Success && response.Data) {
        setAppConfig(response.Data);
      }
    }

    // disable zoom in and zoom out crtl + / ctrl -
    function handleWheel(event: WheelEvent) {
      if (event.ctrlKey) {
        event.preventDefault();
      }
    }
    window.addEventListener("wheel", handleWheel, { passive: false });
  }, []);

  return <RouterProvider router={router} />;
}

export default App;
