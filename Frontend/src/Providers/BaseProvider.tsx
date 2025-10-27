import React, { createContext, useContext } from "react";
import { AppProvider } from "../Providers/AppContext";
import { PackProvider } from "./PackContext";

interface BaseContextType {}

const BaseContext = createContext<BaseContextType | undefined>(undefined);

export const BaseProvider: React.FC<{ children: React.ReactNode }> = ({
  children,
}) => {
  const value = {} as BaseContextType;

  return (
    <AppProvider>
      <PackProvider>
        <BaseContext.Provider value={value}>{children}</BaseContext.Provider>
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
