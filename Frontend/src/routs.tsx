import { createBrowserRouter, Navigate } from "react-router";
import { PackPage } from "./Pages/PackPage";
import { SettingsPage } from "./Pages/SettingsPage";
import { PackLayout } from "./Layout/PackLayout";

const router = createBrowserRouter([
  {
    path: "/",
    element: <Navigate to="/pack" />,
  },
  {
    path: "/settings",
    element: <SettingsPage />,
  },
  {
    path: "/pack",
    element: <PackLayout />,
    children: [
      {
        path: ":packId",
        element: <PackPage />,
      },
    ],
  },
]);

export { router };
