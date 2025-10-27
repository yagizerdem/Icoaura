declare global {
  interface Window {
    chrome: {
      webview: {
        hostObjects: {
          windowBridge: windowBridge;
        };
      };
    };
  }
}

interface windowBridge {
  StartNativeDrag: () => void;
  Minimize: () => void;
  Maximize: () => void;
  Close: () => void;
}

interface PackBridge {
  GetAllPackConfigs: () => Promise<string>; // ApiResopnse<List<PackConfig>>
}

export {};
