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

export {};
