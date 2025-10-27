import { createBrowserRouter, Navigate } from "react-router";
import { PackPage } from "./Pages/PackPage";
import { SettingsPage } from "./Pages/SettingsPage";
import { PackLayout } from "./Layout/PackLayout";
import { DefaultLayout } from "./Layout/DefaultLayout";

const router = createBrowserRouter([
  {
    path: "/",
    element: <DefaultLayout />,
    children: [
      {
        path: "/",
        element: <Navigate to="/pack" replace />,
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
    ],
  },
]);

export { router };
