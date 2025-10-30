import React, { createContext, useContext, useEffect, useState } from "react";
import { useAppContext } from "./AppContext";
import { Language } from "../enum/Language";

interface L10NContextType {
  getLocalizedString: (key: string) => string;
}

const L10NContext = createContext<L10NContextType | undefined>(undefined);

export const L10NProvider: React.FC<{ children: React.ReactNode }> = ({
  children,
}) => {
  const { appConfig } = useAppContext();
  const [enDic, setEnDic] = useState<Record<string, unknown>>({});
  const [trDic, setTrDic] = useState<Record<string, unknown>>({});

  useEffect(() => {
    loadLangPacks();
    async function loadLangPacks() {
      const res = await fetch("/locales/en/translation.json");
      const enJsonData = await res.json();
      setEnDic(enJsonData);

      const res2 = await fetch("/locales/tr/translation.json");
      const trJsonData = await res2.json();
      setTrDic(trJsonData);
    }
  }, []);

  function getLocalizedString(key: string): string {
    let parts = key.split(".");
    let result: any = appConfig?.Language === Language.Tr ? trDic : enDic;
    for (let i = 0; i < parts.length; i++) {
      const part = parts[i];
      if (!result || !(part in result)) {
        return key; // return the key itself if not found
      }
      result = result[part] as string;
    }
    return result;
  }

  return (
    <L10NContext.Provider value={{ getLocalizedString }}>
      {children}
    </L10NContext.Provider>
  );
};

export const useL10NContext = () => {
  const context = useContext(L10NContext);
  if (!context)
    throw new Error("useL10NContext must be used within a L10NProvider");
  return context;
};
