import React, { createContext, useContext, useState } from "react";
import { AppProvider } from "../Providers/AppContext";

interface BaseContextType {
  isLoading: boolean;
  setIsLoading: (value: boolean) => void;
}

const BaseContext = createContext<BaseContextType | undefined>(undefined);

export const BaseProvider: React.FC<{ children: React.ReactNode }> = ({
  children,
}) => {
  const [isLoading, setIsLoading] = useState(false);

  return (
    <BaseContext.Provider value={{ isLoading, setIsLoading }}>
      <AppProvider>{children}</AppProvider>
    </BaseContext.Provider>
  );
};

export const useBaseContext = () => {
  const context = useContext(BaseContext);
  if (!context)
    throw new Error("useBaseContext must be used within a BaseProvider");
  return context;
};
