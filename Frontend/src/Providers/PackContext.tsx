import React, { createContext, useContext, useState } from "react";
import type { PackConfig } from "../models/PackConfig";

interface PackContextType {
  packConfigs: PackConfig[];
  setPackConfigs: React.Dispatch<React.SetStateAction<PackConfig[]>>;
}

const PackContext = createContext<PackContextType | undefined>(undefined);

export const PackProvider: React.FC<{ children: React.ReactNode }> = ({
  children,
}) => {
  const [packConfigs, setPackConfigs] = useState<PackConfig[]>([]);

  return (
    <PackContext.Provider value={{ packConfigs, setPackConfigs }}>
      {children}
    </PackContext.Provider>
  );
};

export const usePackContext = () => {
  const context = useContext(PackContext);
  if (!context)
    throw new Error("usePackContext must be used within a PackProvider");
  return context;
};
