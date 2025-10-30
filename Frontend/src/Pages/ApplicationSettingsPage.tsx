import { useEffect, useState } from "react";
import { useAppContext } from "../Providers/AppContext";
import { ModernSelectList } from "../ui/ModernSelectList";
import { Language } from "../enum/Language";
import { Filter } from "../enum/Filter";
import { Theme } from "../enum/Theme";
import { Monitor, Moon, Sun } from "lucide-react";
import { cn } from "../util/twUtil";
import { ModernSlider } from "../ui/ModernSlider";
import { useL10NContext } from "../Providers/L10NContext";

function mapFilterToString(
  filter: Filter,
  getLocalizedString: (key: string) => string
): string {
  if (filter === Filter.Default)
    return getLocalizedString("Settings.GeneralSettings.ColorSchema.Default");
  if (filter === Filter.Ash)
    return getLocalizedString("Settings.GeneralSettings.ColorSchema.Ash");
  if (filter === Filter.Mist)
    return getLocalizedString("Settings.GeneralSettings.ColorSchema.Mist");
  if (filter === Filter.Forest)
    return getLocalizedString("Settings.GeneralSettings.ColorSchema.Forest");
  if (filter === Filter.Ember)
    return getLocalizedString("Settings.GeneralSettings.ColorSchema.Ember");
  return getLocalizedString("Settings.GeneralSettings.ColorSchema.Default");
}

function mapStringToFilter(
  filter: string,
  getLocalizedString: (key: string) => string
): Filter {
  if (
    filter ===
    getLocalizedString("Settings.GeneralSettings.ColorSchema.Default")
  )
    return Filter.Default;
  if (filter === getLocalizedString("Settings.GeneralSettings.ColorSchema.Ash"))
    return Filter.Ash;
  if (
    filter === getLocalizedString("Settings.GeneralSettings.ColorSchema.Mist")
  )
    return Filter.Mist;
  if (
    filter === getLocalizedString("Settings.GeneralSettings.ColorSchema.Forest")
  )
    return Filter.Forest;
  if (
    filter === getLocalizedString("Settings.GeneralSettings.ColorSchema.Ember")
  )
    return Filter.Ember;
  return Filter.Default;
}

function ApplicationSettingsPage() {
  const { appConfig, setAppConfig } = useAppContext();
  const { getLocalizedString } = useL10NContext();

  function onSelectFilter(option: string) {
    if (!appConfig) return;
    const filter = mapStringToFilter(option, getLocalizedString);
    appConfig.Filter = filter;
    setAppConfig({ ...appConfig });
  }

  function onSelectTheme(theme: Theme) {
    if (!appConfig) return;
    appConfig.Theme = theme;
    setAppConfig({ ...appConfig });
  }

  function onSelectWindowRatio(ratio: number) {
    if (!appConfig) return;
    appConfig.WindowRatio = ratio;
    setAppConfig({ ...appConfig });
  }

  function onSelectWindowOpacity(opacity: number) {
    if (!appConfig) return;
    appConfig.WindowOpacity = opacity;
    setAppConfig({ ...appConfig });
  }

  return (
    <div className="flex flex-col w-full h-full ">
      {/* theme */}
      <div className="w-full h-fit p-2 flex flex-row justify-between  border-b border-(--clr-surface-600) pb-2 ">
        <div className="flex flex-col  ">
          <span className="text-(--clr-text-primary) font-bold">
            {getLocalizedString("Settings.ApplicationSettings.Theme.Title")}
          </span>
          <span className="text-(--clr-text-secondary)">
            {getLocalizedString(
              "Settings.ApplicationSettings.Theme.Description"
            )}
          </span>
        </div>
        <div className="flex flex-row  items-center text-(--clr-text-secondary) gap-2">
          <button
            onMouseUp={() => onSelectTheme(Theme.DefaultSystem)}
            className={cn(
              "w-10 h-10 p-2 flex flex-row items-center justify-center cursor-pointer hover:bg-(--clr-surface-700) rounded-md transition-colors duration-200",
              appConfig && appConfig.Theme === Theme.DefaultSystem
                ? "bg-(--clr-surface-600)"
                : ""
            )}
          >
            <Monitor />
          </button>
          <button
            onMouseUp={() => onSelectTheme(Theme.Light)}
            className={cn(
              "w-10 h-10 p-2 flex flex-row items-center justify-center cursor-pointer hover:bg-(--clr-surface-700) rounded-md transition-colors duration-200",
              appConfig && appConfig.Theme === Theme.Light
                ? "bg-(--clr-surface-600)"
                : ""
            )}
          >
            <Sun />
          </button>
          <button
            onMouseUp={() => onSelectTheme(Theme.Dark)}
            className={cn(
              "w-10 h-10 p-2 flex flex-row items-center justify-center cursor-pointer hover:bg-(--clr-surface-700) rounded-md transition-colors duration-200",
              appConfig && appConfig.Theme === Theme.Dark
                ? "bg-(--clr-surface-600)"
                : ""
            )}
          >
            <Moon />
          </button>
        </div>
      </div>

      {/* color schema */}
      <div className="w-full h-fit p-2 flex flex-row justify-between  border-b border-(--clr-surface-600) pb-2 ">
        <div className="flex flex-col  ">
          <span className="text-(--clr-text-primary) font-bold">
            {getLocalizedString(
              "Settings.ApplicationSettings.ColorSchema.Title"
            )}
          </span>
          <span className="text-(--clr-text-secondary)">
            {getLocalizedString(
              "Settings.ApplicationSettings.ColorSchema.Description"
            )}
          </span>
        </div>
        <ModernSelectList
          onSelectOption={onSelectFilter}
          options={[
            mapFilterToString(Filter.Default, getLocalizedString),
            mapFilterToString(Filter.Ash, getLocalizedString),
            mapFilterToString(Filter.Ember, getLocalizedString),
            mapFilterToString(Filter.Forest, getLocalizedString),
            mapFilterToString(Filter.Mist, getLocalizedString),
          ]}
          selectedOption={
            appConfig
              ? mapFilterToString(appConfig.Filter, getLocalizedString)
              : mapFilterToString(Filter.Default, getLocalizedString)
          }
        />
      </div>

      {/* screen ratio */}
      <div className="w-full h-fit p-2 flex flex-row justify-between  border-b border-(--clr-surface-600) pb-2 ">
        <div className="flex flex-col  ">
          <span className="text-(--clr-text-primary) font-bold">
            {getLocalizedString(
              "Settings.ApplicationSettings.WindowScale.Title"
            )}
          </span>
          <span className="text-(--clr-text-secondary)">
            {getLocalizedString(
              "Settings.ApplicationSettings.WindowScale.Description"
            )}
          </span>
        </div>
        <div className="flex flex-row  items-center text-(--clr-text-secondary) gap-2 w-fit">
          <ModernSlider
            className="w-64"
            showValue={false}
            min={50}
            max={150}
            step={1}
            value={appConfig?.WindowRatio ?? 100}
            onChange={(value) => {
              onSelectWindowRatio(value);
            }}
          />
          <span className="w-12 text-right font-bold text-(--clr-text-primary)">
            {`(${appConfig?.WindowRatio ?? 100}%)`}
          </span>
        </div>
      </div>

      {/* window opacity */}
      <div className="w-full h-fit p-2 flex flex-row justify-between  border-b border-(--clr-surface-600) pb-2 ">
        <div className="flex flex-col  ">
          <span className="text-(--clr-text-primary) font-bold">
            {getLocalizedString(
              "Settings.ApplicationSettings.WindowOpacity.Title"
            )}
          </span>
          <span className="text-(--clr-text-secondary)">
            {getLocalizedString(
              "Settings.ApplicationSettings.WindowOpacity.Description"
            )}
          </span>
        </div>
        <div className="flex flex-row  items-center text-(--clr-text-secondary) gap-2 w-fit">
          <ModernSlider
            className="w-64"
            showValue={false}
            min={50}
            max={100}
            step={1}
            value={appConfig?.WindowOpacity ?? 100}
            onChange={(value) => {
              onSelectWindowOpacity(value);
            }}
          />
          <span className="w-12 text-right font-bold text-(--clr-text-primary)">
            {`(${appConfig?.WindowOpacity ?? 100}%)`}
          </span>
        </div>
      </div>
    </div>
  );
}

export { ApplicationSettingsPage };
