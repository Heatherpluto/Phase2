import React, { createContext, useContext, useState, ReactNode } from "react";

interface SettingsContextProps {
  isDarkTheme: boolean;
  toggleDarkTheme: () => void;
}

const SettingsContext = createContext<SettingsContextProps | undefined>(
  undefined
);

export const SettingsProvider: React.FC<{ children: ReactNode }> = ({
  children,
}) => {
  const [isDarkTheme, setDarkTheme] = useState(false);

  const toggleDarkTheme = () => {
    setDarkTheme(!isDarkTheme);
  };

  return (
    <SettingsContext.Provider
      value={{
        isDarkTheme,
        toggleDarkTheme,
      }}
    >
      {children}
    </SettingsContext.Provider>
  );
};

export const useSettings = () => {
  const context = useContext(SettingsContext);
  if (context === undefined) {
    throw new Error("useSettings must be used within a SettingsProvider");
  }
  return context;
};
