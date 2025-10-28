import React, { createContext, useContext, useState } from "react";
import type { PackConfig } from "../models/PackConfig";

interface PackContextType {
  packConfigs: PackConfig[];
  setPackConfigs: React.Dispatch<React.SetStateAction<PackConfig[]>>;
  editPackConfigMode: boolean;
  setEditPackConfigMode: React.Dispatch<React.SetStateAction<boolean>>;
}

const PackContext = createContext<PackContextType | undefined>(undefined);

export const PackProvider: React.FC<{ children: React.ReactNode }> = ({
  children,
}) => {
  const [packConfigs, setPackConfigs] = useState<PackConfig[]>([]);
  const [editPackConfigMode, setEditPackConfigMode] = useState<boolean>(false);

  return (
    <PackContext.Provider
      value={{
        packConfigs,
        setPackConfigs,
        editPackConfigMode,
        setEditPackConfigMode,
      }}
    >
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
