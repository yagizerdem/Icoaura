import { useEffect, useState } from "react";
import { getSelectedPackConfig } from "../../util/getSelectedPackConfig";
import type { PackConfig } from "../../models/PackConfig";
import { CircleAlert, CircleQuestionMark, X } from "lucide-react";
import { useAppContext } from "../../Providers/AppContext";
import {
  getBase64FromPath,
  selectFileAbsolutePath,
} from "../../service/fileService";
import type { ApiResponse } from "../../models/ApiResponse";
import { ModernTextInput } from "../../ui/ModernTextInput";
import { ModernTextArea } from "../../ui/ModernTextArea";
import { ModernButton } from "../../ui/ModernButton";
import { usePackContext } from "../../Providers/PackContext";
import { flash } from "../../util/cameraFlash";
import { WritePackConfig } from "../../service/packService";
import { Toast } from "../../util/toast";
import { useL10NContext } from "../../Providers/L10NContext";

function EditPackConfig() {
  const packConfig = getSelectedPackConfig();
  const [packConfigDeepCopy, setPackConfigDeepCopy] =
    useState<PackConfig | null>({ ...packConfig } as PackConfig);
  const { setIsLoading } = useAppContext();
  const { setEditPackConfigMode, setPackConfigs } = usePackContext();
  const [packNameError, setPackNameError] = useState<string | null>(null);
  const [versionError, setVersionError] = useState<string | null>(null);

  const { getLocalizedString } = useL10NContext();

  // sync pack config deep copy
  useEffect(() => {
    setPackConfigDeepCopy({ ...packConfig } as PackConfig);
  }, [packConfig]);

  function removeCover() {
    if (!packConfigDeepCopy) return;
    const updatedPackConfig = { ...packConfigDeepCopy, CoverPngBase64: "" };
    setPackConfigDeepCopy(updatedPackConfig);
  }

  async function selectPackCover() {
    try {
      setIsLoading(true);
      const filePath = await selectFileAbsolutePath([".png", "png"]);
      if (!filePath) return;

      const apiResponse: ApiResponse<string> = await getBase64FromPath(
        filePath
      );
      if (!apiResponse.Success || !packConfigDeepCopy) return;
      const updatedPackConfig = {
        ...packConfigDeepCopy,
        CoverPngBase64: apiResponse.Data,
      };
      setPackConfigDeepCopy(updatedPackConfig);
    } finally {
      setIsLoading(false);
    }
  }

  async function cancel() {
    setEditPackConfigMode(false);
  }

  async function save() {
    try {
      setIsLoading(true);
      setPackNameError(null);
      setVersionError(null);
      if (!packConfigDeepCopy) return;

      let hasError = false;

      if (packConfigDeepCopy.PackName.trim() === "") {
        setPackNameError("Pack name is required.");
        hasError = true;
      }

      if (packConfigDeepCopy.Version.trim() === "") {
        setVersionError("Version is required.");
        hasError = true;
      }

      const normalizedVersion = packConfigDeepCopy.Version.startsWith("v")
        ? packConfigDeepCopy.Version.slice(1)
        : packConfigDeepCopy.Version;

      if (normalizedVersion.match(/^[0-9]+\.[0-9]+\.[0-9]+$/) === null) {
        setVersionError("Version must be in the format 1.0.0");
        hasError = true;
      }

      if (hasError) return;

      const apiResponse: ApiResponse<PackConfig> = await WritePackConfig(
        packConfigDeepCopy
      );

      if (!apiResponse.Success) {
        // show error toast
        Toast.error(apiResponse.ErrorMessage || "Failed to create package.");
        return;
      }

      const updatedPackConfig = apiResponse.Data;
      setPackConfigs((prevConfigs) => {
        const otherConfigs = prevConfigs.filter(
          (pc) => pc.Uid !== updatedPackConfig.Uid
        );
        return [...otherConfigs, updatedPackConfig];
      });

      Toast.success("Pack information updated successfully.");
      flash({});
      setEditPackConfigMode(false);
    } finally {
      setIsLoading(false);
    }
  }

  return (
    <div className="w-full h-fit bg-(--clr-surface-800) p-3 rounded-md">
      <h1 className="text-xl text-(--clr-text-primary) font-medium">
        {getLocalizedString("Package.EditPackConfig.Title")}
      </h1>
      <hr className="my-2 border-(--clr-surface-500)" />
      <div className="flex flex-row">
        {/* pack cover */}
        <div className="flex flex-col gap-1">
          <span className="text-(--clr-text-primary)  text-left font-bold">
            {getLocalizedString("Package.EditPackConfig.Icon")}
          </span>
          {packConfigDeepCopy?.CoverPngBase64 &&
          packConfigDeepCopy?.CoverPngBase64.trim().length > 0 ? (
            <div className="relative select-none w-12 h-12 border-2   border-(--clr-surface-800) bg-(--clr-surface-950) rounded-md flex items-center justify-center text-center">
              <img
                src={packConfigDeepCopy?.CoverPngBase64}
                alt="Pack Cover"
                className="w-full h-full object-cover rounded-md  text-center"
              />
              <div
                onMouseUp={() => removeCover()}
                className="flex items-center justify-center text-(--clr-text-primary) cursor-pointer w-3 h-3 rounded-full bg-(--clr-danger-20) absolute  top-0 right-0  "
              >
                <X />
              </div>
            </div>
          ) : (
            <div
              onMouseUp={() => selectPackCover()}
              className="cursor-pointer select-none w-12 h-12 border-2   border-(--clr-surface-800) bg-(--clr-surface-950) rounded-md flex items-center justify-center text-center"
            >
              <CircleQuestionMark className="text-(--clr-text-secondary) w-full h-full" />
            </div>
          )}
        </div>

        <div className="flex flex-wrap flex-row ml-10 w-full h-fit gap-2">
          <div className="flex flex-row w-full  gap-2">
            {/* pack name */}
            <div className="flex flex-col gap-1 flex-1">
              <div className="flex flex-row items-center">
                <span className="text-(--clr-text-primary)  text-left font-bold">
                  {getLocalizedString("Package.EditPackConfig.Name")}
                </span>
                {packNameError && (
                  <span className="text-(--clr-warning-10) ml-2 text-sm">
                    <CircleAlert className="inline w-4 h-4 mr-1" />
                    {packNameError}
                  </span>
                )}
              </div>
              <ModernTextInput
                value={packConfigDeepCopy?.PackName ?? ""}
                onChange={(val) => {
                  if (!packConfigDeepCopy) return;
                  setPackConfigDeepCopy({
                    ...packConfigDeepCopy,
                    PackName: val,
                  });
                }}
              />
            </div>

            {/* version  */}

            <div className="flex flex-col gap-1 flex-1">
              <div className="flex flex-row ">
                <span className="text-(--clr-text-primary)  text-left font-bold">
                  {getLocalizedString("Package.EditPackConfig.Version")}
                </span>
                {versionError && (
                  <span className="text-(--clr-warning-10) ml-2 text-sm">
                    <CircleAlert className="inline w-4 h-4 mr-1" />{" "}
                    {versionError}
                  </span>
                )}
              </div>

              <ModernTextInput
                value={packConfigDeepCopy?.Version ?? ""}
                onChange={(val) => {
                  if (!packConfigDeepCopy) return;
                  setPackConfigDeepCopy({
                    ...packConfigDeepCopy,
                    Version: val,
                  });
                }}
              />
            </div>
          </div>

          <div className="flex flex-row w-full  gap-2">
            {/* author */}
            <div className="flex flex-col gap-1 flex-1">
              <span className="text-(--clr-text-primary)  text-left font-bold">
                {getLocalizedString("Package.EditPackConfig.Author")}
              </span>
              <ModernTextInput
                value={packConfigDeepCopy?.Author ?? ""}
                onChange={(val) => {
                  if (!packConfigDeepCopy) return;
                  setPackConfigDeepCopy({
                    ...packConfigDeepCopy,
                    Author: val,
                  });
                }}
              />
            </div>

            {/* license  */}

            <div className="flex flex-col gap-1 flex-1">
              <span className="text-(--clr-text-primary)  text-left font-bold">
                {getLocalizedString("Package.EditPackConfig.License")}
              </span>
              <ModernTextInput
                value={packConfigDeepCopy?.License ?? ""}
                onChange={(val) => {
                  if (!packConfigDeepCopy) return;
                  setPackConfigDeepCopy({
                    ...packConfigDeepCopy,
                    License: val,
                  });
                }}
              />
            </div>
          </div>

          <div className="flex flex-row w-full  gap-2">
            <div className="flex flex-col gap-1 flex-1">
              <span className="text-(--clr-text-primary)  text-left font-bold">
                {getLocalizedString("Package.EditPackConfig.Description")}
              </span>
              <ModernTextArea
                value={packConfigDeepCopy?.Description ?? ""}
                onChange={(val) => {
                  if (!packConfigDeepCopy) return;
                  setPackConfigDeepCopy({
                    ...packConfigDeepCopy,
                    Description: val,
                  });
                }}
              />
            </div>
          </div>
        </div>
      </div>
      <div className="flex flex-row justify-end mt-4 gap-2">
        <ModernButton
          onMouseUp={() => save()}
          text={getLocalizedString("Package.EditPackConfig.Save")}
          type="success"
          className="cursor-pointer"
        />
        <ModernButton
          onMouseUp={() => cancel()}
          text={getLocalizedString("Package.EditPackConfig.Cancel")}
          type="warning"
          className="cursor-pointer"
        />
      </div>
    </div>
  );
}

export { EditPackConfig };
