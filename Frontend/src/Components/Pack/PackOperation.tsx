import { useEffect } from "react";
import { HasAdminPrivilege, rebootAsAdmin } from "../../service/windowService";
import { ModernButton } from "../../ui/ModernButton";
import { useAppContext } from "../../Providers/AppContext";
import {
  CircleQuestionMark,
  Folder,
  Image,
  Monitor,
  Pen,
  Upload,
} from "lucide-react";
import { ModernIconButton } from "../../ui/ModernIconButton";
import { getSelectedPackConfig } from "../../util/getSelectedPackConfig";
import type { ApiResponse } from "../../models/ApiResponse";
import type { PackItem } from "../../models/PackItem";
import {
  AddDesktopIcons,
  AppendPackItemFromPath,
  ApplyPackOperations,
  getAllPackConfigs,
} from "../../service/packService";
import { Toast } from "../../util/toast";
import type { PackConfig } from "../../models/PackConfig";
import { usePackContext } from "../../Providers/PackContext";
import { flash } from "../../util/cameraFlash";
import {
  ExportPack,
  selectDirectoryPath,
  selectFileAbsolutePath,
} from "../../service/fileService";
import { useL10NContext } from "../../Providers/L10NContext";

function PackOperation() {
  const { isAdmin, setIsAdmin, setIsLoading } = useAppContext();
  const { setPackConfigs, setEditPackItemMode } = usePackContext();
  const packConfig = getSelectedPackConfig();
  const { getLocalizedString } = useL10NContext();

  useEffect(() => {
    helper();
    async function helper() {
      const isAdmin = await HasAdminPrivilege();
      setIsAdmin(isAdmin);
    }
  }, []);

  async function handleAddDesktopIcons() {
    try {
      if (!packConfig?.Uid) return;
      setIsLoading(true);
      const response: ApiResponse<PackItem[]> = await AddDesktopIcons(
        packConfig.Uid
      );
      if (!response.Success) {
        Toast.error(response.ErrorMessage || "Failed to add desktop icons");
      }

      const configs: PackConfig[] = (await getAllPackConfigs()).Data;
      setPackConfigs(configs);
      Toast.success("Desktop icons added successfully");
      flash({});
    } finally {
      setIsLoading(false);
    }
  }

  async function handleAddIcon() {
    try {
      if (!packConfig?.Uid) return;

      setIsLoading(true);
      const filePath = await selectFileAbsolutePath([
        ".lnk",
        "lnk",
        "url",
        ".url",
      ]);
      if (!filePath) return;

      const apiResponse: ApiResponse<PackItem> = await AppendPackItemFromPath(
        packConfig.Uid,
        filePath
      );

      if (!apiResponse.Success) {
        Toast.error(apiResponse.ErrorMessage || "Failed to add icon");
        return;
      }

      const configs: PackConfig[] = (await getAllPackConfigs()).Data;
      setPackConfigs(configs);
      Toast.success("Icon added successfully");
      flash({});
    } finally {
      setIsLoading(false);
    }
  }

  async function handleAddDirIcon() {
    try {
      if (!packConfig?.Uid) return;

      setIsLoading(true);
      const dirPath = await selectDirectoryPath();

      if (!dirPath) return;

      const apiResponse: ApiResponse<PackItem> = await AppendPackItemFromPath(
        packConfig.Uid,
        dirPath
      );

      if (!apiResponse.Success) {
        Toast.error(apiResponse.ErrorMessage || "Failed to add icon");
        return;
      }

      const configs: PackConfig[] = (await getAllPackConfigs()).Data;
      setPackConfigs(configs);
      Toast.success("Icon added successfully");
      flash({});
    } finally {
      setIsLoading(false);
    }
  }

  async function handleApplyPackOperations() {
    try {
      if (!packConfig?.Uid) return;
      setIsLoading(true);
      const apiResponse: ApiResponse<null> = await ApplyPackOperations(
        packConfig.Uid
      );

      if (!apiResponse.Success) {
        Toast.error(apiResponse.ErrorMessage || "Failed to apply pack ops");
        return;
      }

      Toast.success("Pack operations applied successfully");
      flash({});
    } finally {
      setIsLoading(false);
    }
  }

  function handleEdit() {
    setEditPackItemMode(true);
  }

  async function handleExportPack() {
    try {
      if (!packConfig || !packConfig?.Uid) return;
      setIsLoading(true);

      const apiResponse: ApiResponse<string> = await ExportPack(packConfig.Uid);

      if (!apiResponse.Success) {
        Toast.error(apiResponse.ErrorMessage || "Failed to export pack");
        return;
      }
      Toast.success("Pack exported to downloads folder");
    } finally {
      setIsLoading(false);
    }
  }

  return (
    <div className="w-full h-fit bg-(--clr-surface-800) p-3 rounded-md">
      <div className="flex flex-row gap-4">
        <h1 className="text-xl text-(--clr-text-primary) font-medium">
          {getLocalizedString("Package.PackOperations.Title")}
        </h1>

        <ModernButton
          onMouseUp={() => rebootAsAdmin()}
          text={getLocalizedString("Package.PackOperations.RebootAsAdmin")}
          type="ghost"
          className="cursor-pointer bg-(--clr-surface-900)"
          disabled={isAdmin}
        />
        {!isAdmin && (
          <div className="flex flex-row items-center gap-1 text-(--clr-warning-10)">
            <CircleQuestionMark />
            <span className=" self-center ">
              {getLocalizedString("Package.PackOperations.AdminInfo")}
            </span>
          </div>
        )}
      </div>
      <hr className="my-2 border-(--clr-surface-500)" />

      <div className="flex flex-row gap-4 mt-5">
        <ModernButton
          text={getLocalizedString("Package.PackOperations.Apply")}
          type="success"
          className="cursor-pointer"
          onMouseUp={() => handleApplyPackOperations()}
        />
        <ModernIconButton
          icon={<Pen />}
          text={getLocalizedString("Package.PackOperations.Edit")}
          type="ghost"
          className="cursor-pointer"
          onMouseUp={() => handleEdit()}
        />
        <ModernIconButton
          onMouseUp={() => handleExportPack()}
          icon={<Upload />}
          text={getLocalizedString("Package.PackOperations.Export")}
          type="ghost"
          className="cursor-pointer"
        />
      </div>

      <div className="flex flex-row gap-4 mt-5">
        <ModernIconButton
          icon={<Monitor />}
          text={getLocalizedString("Package.PackOperations.AddDesktopIcons")}
          type="ghost"
          className="cursor-pointer"
          onMouseUp={() => handleAddDesktopIcons()}
        />
        <ModernIconButton
          icon={<Image />}
          text={getLocalizedString("Package.PackOperations.AddIcon")}
          type="ghost"
          className="cursor-pointer"
          onMouseUp={() => handleAddIcon()}
        />
        <ModernIconButton
          icon={<Folder />}
          text={getLocalizedString("Package.PackOperations.AddDirectoryIcon")}
          type="ghost"
          className="cursor-pointer"
          onMouseUp={() => handleAddDirIcon()}
        />
      </div>
    </div>
  );
}

export { PackOperation };
