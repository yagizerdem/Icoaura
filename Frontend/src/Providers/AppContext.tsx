import React, { createContext, useContext, useEffect, useState } from "react";
import type { AppConfig } from "../models/AppConfig";
import { useDebounce } from "../hook/useDebounce";
import { writeAppConfig } from "../service/appConfigService";
// import i18n from "../i18n";
import { Language } from "../enum/Language";

interface AppContextType {
  isLoading: boolean;
  setIsLoading: (value: boolean) => void;
  showCreatePackPopup: boolean;
  setShowCreatePackPopup: (value: boolean) => void;
  showDeletePackPopup: boolean;
  setShowDeletePackPopup: (value: boolean) => void;
  isAdmin: boolean;
  setIsAdmin: (value: boolean) => void;
  appConfig: AppConfig | null;
  setAppConfig: (config: AppConfig) => void;
}

const AppContext = createContext<AppContextType | undefined>(undefined);

export const AppProvider: React.FC<{ children: React.ReactNode }> = ({
  children,
}) => {
  const [isLoading, setIsLoading] = useState(false);
  const [showCreatePackPopup, setShowCreatePackPopup] = useState(false);
  const [showDeletePackPopup, setShowDeletePackPopup] = useState(false);
  const [isAdmin, setIsAdmin] = useState(false);
  const [appConfig, setAppConfig] = useState<AppConfig | null>(null);

  const debouncedConfig = useDebounce(appConfig, 500);

  useEffect(() => {
    if (debouncedConfig) {
      writeAppConfig(debouncedConfig);
    }
  }, [debouncedConfig]);

  // sync window scale
  useEffect(() => {
    if (debouncedConfig?.WindowRatio) {
      const scale = debouncedConfig.WindowRatio / 100;
      const baseFontSize = 16; // px
      document.documentElement.style.fontSize = `${baseFontSize * scale}px`;
    }

    if (debouncedConfig?.WindowOpacity) {
      const opacity = Math.max(
        0,
        Math.min(1, debouncedConfig.WindowOpacity / 100)
      );
      document.body.style.opacity = opacity.toString();
    }
  }, [debouncedConfig]);

  return (
    <AppContext.Provider
      value={{
        isLoading,
        setIsLoading,
        showCreatePackPopup,
        setShowCreatePackPopup,
        showDeletePackPopup,
        setShowDeletePackPopup,
        isAdmin,
        setIsAdmin,
        appConfig,
        setAppConfig,
      }}
    >
      {children}
    </AppContext.Provider>
  );
};

export const useAppContext = () => {
  const context = useContext(AppContext);
  if (!context)
    throw new Error("useAppContext must be used within an AppProvider");
  return context;
};
