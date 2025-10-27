declare global {
  interface Window {
    chrome: {
      webview: {
        hostObjects: {
          windowBridge: windowBridge;
          packBridge: PackBridge;
          fileBridge: FileBridge;
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

interface FileBridge {
  SelectFilePath(allowedExtensionsJsonArray: string): Promise<string>; // string
  GetBase64FromPath(filePath: string): Promise<string>; // ApiResponse<string>
}

export {};
