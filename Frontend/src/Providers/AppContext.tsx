import React, { createContext, useContext, useState } from "react";

interface AppContextType {
  isLoading: boolean;
  setIsLoading: (value: boolean) => void;
  showCreatePackPopup: boolean;
  setShowCreatePackPopup: (value: boolean) => void;
  showDeletePackPopup: boolean;
  setShowDeletePackPopup: (value: boolean) => void;
  isAdmin: boolean;
  setIsAdmin: (value: boolean) => void;
}

const AppContext = createContext<AppContextType | undefined>(undefined);

export const AppProvider: React.FC<{ children: React.ReactNode }> = ({
  children,
}) => {
  const [isLoading, setIsLoading] = useState(false);
  const [showCreatePackPopup, setShowCreatePackPopup] = useState(false);
  const [showDeletePackPopup, setShowDeletePackPopup] = useState(false);
  const [isAdmin, setIsAdmin] = useState(false);

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
