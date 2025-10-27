export interface AppConfig {
  MatchLnkByTargetExe: boolean;
  MatchUrlByTargetUrl: boolean;
  ChangeDescriptionOfMatchedLnkFiles: boolean;
  PackOpacity: number;
  PackCornerRadius: number;
  WindowOpacity: number;
  ForceExplorerRefreshAfterIcoChange: boolean;
  EnableLogging: boolean;
  MaxLogCount: number;
  EnableTraceLogging: boolean;
  EnableDebugLogging: boolean;
  EnableInfoLogging: boolean;
  EnableWarningLogging: boolean;
  EnableErrorLogging: boolean;
  EnableFatalLogging: boolean;
  Theme: string;
  Language: string;
}
