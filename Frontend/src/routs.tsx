import { createBrowserRouter, Navigate } from "react-router";
import { PackPage } from "./Pages/PackPage";
import { PackLayout } from "./Layout/PackLayout";
import { DefaultLayout } from "./Layout/DefaultLayout";
import { SettingsLayout } from "./Layout/SettingsLayout";
import { GeneralSettingsPage } from "./Pages/GeneralSettingsPage";
import { IconPackSettingsPage } from "./Pages/IconPackSettingsPage";
import { SystemSettingsPage } from "./Pages/SystemSettingsPage";
import { AdvancedSettingsPage } from "./Pages/AdvancedSettingsPage";
import { ApplicationSettingsPage } from "./Pages/ApplicationSettingsPage";
import { UpdateSettingsPage } from "./Pages/UpdateSettingsPage";

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
          {
            path: "/settings/advanced",
            element: <AdvancedSettingsPage />,
          },
          {
            path: "/settings/application",
            element: <ApplicationSettingsPage />,
          },
          {
            path: "/settings/update",
            element: <UpdateSettingsPage />,
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
