import { useState } from "react";
import { ModernButton } from "../ui/ModernButton";
import { cn } from "../util/twUtil";
import { Outlet, useLocation, useNavigate } from "react-router";
import { useL10NContext } from "../Providers/L10NContext";

interface SettingsLayoutProps {}

function SettingsLayout({}: SettingsLayoutProps) {
  const navigate = useNavigate();
  const location = useLocation();
  const panel = location.pathname.split("/").pop() || "general";

  const { getLocalizedString } = useL10NContext();

  return (
    <div className="w-full h-full bg-(--clr-surface-800) overflow-y-auto flex flex-row ">
      <div className="w-48 h-full bg-(--clr-surface-700) flex flex-col gap-2 p-1">
        <ModernButton
          className={cn(
            "w-full cursor-pointer mt-1 border-none",
            panel === "general" && "bg-(--clr-surface-900)"
          )}
          onMouseUp={() => navigate("/settings/general")}
          text={getLocalizedString("Settings.SettingsMenu.General")}
          type="ghost"
        />
        <ModernButton
          className={cn(
            "w-full cursor-pointer border-none",
            panel === "application" && "bg-(--clr-surface-900)"
          )}
          onMouseUp={() => navigate("/settings/application")}
          text={getLocalizedString("Settings.SettingsMenu.Application")}
          type="ghost"
        />
        <ModernButton
          className={cn(
            "w-full cursor-pointer border-none",
            panel === "iconpack" && "bg-(--clr-surface-900)"
          )}
          onMouseUp={() => navigate("/settings/iconpack")}
          text={getLocalizedString("Settings.SettingsMenu.IconPack")}
          type="ghost"
        />
        <ModernButton
          className={cn(
            "w-full cursor-pointer border-none",
            panel === "system" && "bg-(--clr-surface-900)"
          )}
          onMouseUp={() => navigate("/settings/system")}
          text={getLocalizedString("Settings.SettingsMenu.System")}
          type="ghost"
        />
        <ModernButton
          className={cn(
            "w-full cursor-pointer border-none",
            panel === "advanced" && "bg-(--clr-surface-900)"
          )}
          onMouseUp={() => navigate("/settings/advanced")}
          text={getLocalizedString("Settings.SettingsMenu.Advanced")}
          type="ghost"
        />
        <ModernButton
          className={cn(
            "w-full cursor-pointer border-none",
            panel === "update" && "bg-(--clr-surface-900)"
          )}
          onMouseUp={() => navigate("/settings/update")}
          text={getLocalizedString("Settings.SettingsMenu.Update")}
          type="ghost"
        />
      </div>
      <div className="flex flex-1 min-h-0">{<Outlet />}</div>
    </div>
  );
}

export { SettingsLayout };
