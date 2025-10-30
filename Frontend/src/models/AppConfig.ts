import type { Filter } from "../enum/Filter";
import type { Language } from "../enum/Language";
import type { Theme } from "../enum/Theme";

export interface AppConfig {
  MatchLnkByTargetExe: boolean;
  MatchUrlByTargetUrl: boolean;
  ChangeDescriptionOfMatchedLnkFiles: boolean;
  PackOpacity: number;
  PackCornerRadius: number;
  WindowOpacity: number;
  WindowRatio: number;
  ForceExplorerRefreshAfterIcoChange: boolean;
  EnableLogging: boolean;
  MaxLogCount: number;
  EnableTraceLogging: boolean;
  EnableDebugLogging: boolean;
  EnableInfoLogging: boolean;
  EnableWarningLogging: boolean;
  EnableErrorLogging: boolean;
  EnableFatalLogging: boolean;
  Theme: Theme;
  Language: Language;
  Filter: Filter;
  RecursiveScanningDepth: number;
}
