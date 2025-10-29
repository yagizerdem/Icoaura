import { useEffect, useState } from "react";
import { useAppContext } from "../Providers/AppContext";
import { ModernSelectList } from "../ui/ModernSelectList";
import { Language } from "../enum/Language";
import { Filter } from "../enum/Filter";
import { Theme } from "../enum/Theme";
import { Monitor, Moon, Sun } from "lucide-react";
import { cn } from "../util/twUtil";

function mapLanguageToString(lang: Language): string {
  if (lang === Language.En) return "English";
  if (lang === Language.Tr) return "Turkish";
  return "English";
}

function mapStringToLanguage(lang: string): Language {
  if (lang === "English") return Language.En;
  if (lang === "Turkish") return Language.Tr;
  return Language.En;
}

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

function GeneralSettingsPage() {
  const { appConfig, setAppConfig } = useAppContext();

  function onSelectLanguage(option: string) {
    if (!appConfig) return;
    const lang = mapStringToLanguage(option);
    appConfig.Language = lang;
    setAppConfig({ ...appConfig });
  }

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

  console.log(appConfig);

  return (
    <div className="flex flex-col w-full h-full ">
      {/* language */}
      <div className="w-full h-fit p-2 flex flex-row justify-between  border-b border-(--clr-surface-600) pb-2 ">
        <div className="flex flex-col  ">
          <span className="text-(--clr-text-primary) font-bold">Language</span>
          <span className="text-(--clr-text-secondary)">
            Choose your preferred language
          </span>
        </div>
        <ModernSelectList
          onSelectOption={onSelectLanguage}
          options={[
            mapLanguageToString(Language.En),
            mapLanguageToString(Language.Tr),
          ]}
          selectedOption={
            appConfig
              ? mapLanguageToString(appConfig.Language)
              : mapLanguageToString(Language.En)
          }
        />
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
    </div>
  );
}
export { GeneralSettingsPage };
