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
  RebootAsAdmin: () => void;
  HasAdminPrivilege: () => Promise<boolean>;
}

interface PackBridge {
  GetAllPackConfigs: () => Promise<string>; // ApiResopnse<List<PackConfig>>
  CreatePack: (packConfigJson: string) => Promise<string>; // ApiResponse<null>
  DeletePack: (packId: string, deleteIcons: boolean) => Promise<string>; // ApiResponse<null>
  WritePackConfig: (packConfigJson: string) => Promise<string>; // ApiResponse<PackConfig>
  GetPackItems: (packId: string) => Promise<string>; // ApiResponse<List<PackItem>>
  GetPackItemIconBase64: (
    packId: string,
    packItemId: string
  ) => Promise<string>; // ApiResponse<string>
  AddDesktopIcons: (packId: string) => Promise<string>; // ApiResponse<PackItem[]>
  AppendPackItemFromPath: (packId: string, filePath: string) => Promise<string>; // ApiResponse<PackItem>
  ApplyPackOperations: (packId: string) => Promise<string>; // ApiResponse<null>
  WritePackItems: (
    packId: string,
    packItemsJson: string,
    mapJson: string
  ) => Promise<string>; // ApiResponse<PackItem[]>
}

interface FileBridge {
  SelectFilePath(allowedExtensionsJsonArray: string): Promise<string>; // string
  GetBase64FromPath(filePath: string): Promise<string>; // ApiResponse<string>
  SelectDirectoryPath(): Promise<string>; // string
  SelectFileRelativeFilePath(
    allowedExtensionsJsonArray: string
  ): Promise<string>; // string
  SelectRelativeDirectoryPath(): Promise<string>; // string
  IsFileSystemEntryExist: (filePath: string) => Promise<boolean>; // ApiResponse<boolean>
}

export {};
