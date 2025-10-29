import { createBrowserRouter, Navigate } from "react-router";
import { PackPage } from "./Pages/PackPage";
import { PackLayout } from "./Layout/PackLayout";
import { DefaultLayout } from "./Layout/DefaultLayout";
import { SettingsLayout } from "./Layout/SettingsLayout";
import { GeneralSettingsPage } from "./Pages/GeneralSettingsPage";

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
