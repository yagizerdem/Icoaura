import React, { createContext, useContext, useEffect } from "react";
import { useAppContext } from "./AppContext";
import { Theme } from "../enum/Theme";
import { GetSystemDefaultTheme } from "../service/windowService";
import { Filter } from "../enum/Filter";

interface ThemeContextType {}

const ThemeContext = createContext<ThemeContextType | undefined>(undefined);

export const ThemeProvider: React.FC<{ children: React.ReactNode }> = ({
  children,
}) => {
  const value = {} as ThemeContextType;
  const { appConfig } = useAppContext();

  useEffect(() => {
    setTheme();
    async function setTheme() {
      // sync theme with config
      if (!appConfig) return;
      const root = document.documentElement;

      // apply theme
      if (appConfig.Theme === Theme.Dark) {
        cleanupThemeClasses();
        root.classList.add("dark-default");
      } else if (appConfig?.Theme === Theme.Light) {
        cleanupThemeClasses();
        root.classList.add("light-default");
      } else {
        // system default theme
        const theme: Theme = await GetSystemDefaultTheme();
        if (theme === Theme.Dark) {
          cleanupThemeClasses();
          root.classList.add("dark-default");
        } else {
          cleanupThemeClasses();
          root.classList.add("light-default");
        }
      }

      // apply filter
      if (appConfig.Filter == Filter.Default) {
        cleanUpFilterClasses();
      } else if (appConfig.Filter == Filter.Ash) {
        cleanUpFilterClasses();
        appConfig.Theme === Theme.Light
          ? root.classList.add("light-ash")
          : root.classList.add("dark-ash");
      } else if (appConfig.Filter == Filter.Mist) {
        cleanUpFilterClasses();
        appConfig.Theme === Theme.Light
          ? root.classList.add("light-mist")
          : root.classList.add("dark-mist");
      } else if (appConfig.Filter == Filter.Forest) {
        cleanUpFilterClasses();
        appConfig.Theme === Theme.Light
          ? root.classList.add("light-forest")
          : root.classList.add("dark-forest");
      } else if (appConfig.Filter == Filter.Ember) {
        cleanUpFilterClasses();
        appConfig.Theme === Theme.Light
          ? root.classList.add("light-ember")
          : root.classList.add("dark-ember");
      }
    }
  }, [appConfig]);

  function cleanupThemeClasses() {
    const root = document.documentElement;
    root.classList.remove("dark-default");
    root.classList.remove("light-default");
  }

  function cleanUpFilterClasses() {
    const root = document.documentElement;
    root.classList.remove("dark-ash");
    root.classList.remove("light-ash");
    root.classList.remove("dark-mist");
    root.classList.remove("light-mist");
    root.classList.remove("dark-forest");
    root.classList.remove("light-forest");
    root.classList.remove("dark-ember");
    root.classList.remove("light-ember");
  }

  return (
    <ThemeContext.Provider value={value}>{children}</ThemeContext.Provider>
  );
};

export const useThemeContext = () => {
  const context = useContext(ThemeContext);
  if (!context)
    throw new Error("useThemeContext must be used within a ThemeProvider");
  return context;
};
