import React, { createContext, useContext } from "react";
import { AppProvider } from "../Providers/AppContext";
import { PackProvider } from "./PackContext";
import { ThemeProvider } from "./ThemeProvider";
import { L10NProvider } from "./L10NContext";

interface BaseContextType {}

const BaseContext = createContext<BaseContextType | undefined>(undefined);

export const BaseProvider: React.FC<{ children: React.ReactNode }> = ({
  children,
}) => {
  const value = {} as BaseContextType;

  return (
    <AppProvider>
      <PackProvider>
        <ThemeProvider>
          <L10NProvider>
            <BaseContext.Provider value={value}>
              {children}
            </BaseContext.Provider>
          </L10NProvider>
        </ThemeProvider>
      </PackProvider>
    </AppProvider>
  );
};

export const useBaseContext = () => {
  const context = useContext(BaseContext);
  if (!context)
    throw new Error("useBaseContext must be used within a BaseProvider");
  return context;
};
