import { createBrowserRouter, Navigate } from "react-router";
import { PackPage } from "./Pages/PackPage";
import { PackLayout } from "./Layout/PackLayout";
import { DefaultLayout } from "./Layout/DefaultLayout";
import { SettingsLayout } from "./Layout/SettingsLayout";
import { GeneralSettingsPage } from "./Pages/GeneralSettingsPage";
import { IconPackSettingsPage } from "./Pages/IconPackSettingsPage";
import { SystemSettingsPage } from "./Pages/SystemSettingsPage";

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
        element: <SettingsLayout />,
        children: [
          {
            path: "/settings",
            element: <Navigate to="/settings/general" replace />,
          },
          {
            path: "/settings/general",
            element: <GeneralSettingsPage />,
          },
          {
            path: "/settings/iconpack",
            element: <IconPackSettingsPage />,
          },
          {
            path: "/settings/system",
            element: <SystemSettingsPage />,
          },
        ],
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
