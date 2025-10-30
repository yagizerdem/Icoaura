import { useEffect, useState } from "react";
import { useAppContext } from "../Providers/AppContext";
import { ModernSelectList } from "../ui/ModernSelectList";
import { Language } from "../enum/Language";
import { Filter } from "../enum/Filter";
import { Theme } from "../enum/Theme";
import { Monitor, Moon, Sun } from "lucide-react";
import { cn } from "../util/twUtil";
import { ModernSlider } from "../ui/ModernSlider";

function mapFilterToString(filter: Filter): string {
  if (filter === Filter.Default) return "Default";
  if (filter === Filter.Ash) return "Ash";
  if (filter === Filter.Mist) return "Mist";
  if (filter === Filter.Forest) return "Forest";
  if (filter === Filter.Ember) return "Ember";
  return "Default";
}

function mapStringToFilter(filter: string): Filter {
  if (filter === "Default") return Filter.Default;
  if (filter === "Ash") return Filter.Ash;
  if (filter === "Mist") return Filter.Mist;
  if (filter === "Forest") return Filter.Forest;
  if (filter === "Ember") return Filter.Ember;
  return Filter.Default;
}

function ApplicationSettingsPage() {
  const { appConfig, setAppConfig } = useAppContext();

  function onSelectFilter(option: string) {
    if (!appConfig) return;
    const filter = mapStringToFilter(option);
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
          <span className="text-(--clr-text-primary) font-bold">Theme</span>
          <span className="text-(--clr-text-secondary)">
            Choose your preferred theme
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
            Color Schema
          </span>
          <span className="text-(--clr-text-secondary)">
            Choose your preferred schema
          </span>
        </div>
        <ModernSelectList
          onSelectOption={onSelectFilter}
          options={[
            mapFilterToString(Filter.Default),
            mapFilterToString(Filter.Ash),
            mapFilterToString(Filter.Ember),
            mapFilterToString(Filter.Forest),
            mapFilterToString(Filter.Mist),
          ]}
          selectedOption={
            appConfig
              ? mapFilterToString(appConfig.Filter)
              : mapFilterToString(Filter.Default)
          }
        />
      </div>

      {/* screen ratio */}
      <div className="w-full h-fit p-2 flex flex-row justify-between  border-b border-(--clr-surface-600) pb-2 ">
        <div className="flex flex-col  ">
          <span className="text-(--clr-text-primary) font-bold">
            Window Scale
          </span>
          <span className="text-(--clr-text-secondary)">
            Adjust the window scaling.
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
            Window Opacity
          </span>
          <span className="text-(--clr-text-secondary)">
            Adjust the window opacity.
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
