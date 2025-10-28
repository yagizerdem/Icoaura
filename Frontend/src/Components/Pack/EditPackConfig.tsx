import { useEffect, useState } from "react";
import { getSelectedPackConfig } from "../../util/getSelectedPackConfig";
import type { PackConfig } from "../../models/PackConfig";
import { CircleQuestionMark, X } from "lucide-react";
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

function EditPackConfig() {
  const packConfig = getSelectedPackConfig();
  const [packConfigDeepCopy, setPackConfigDeepCopy] =
    useState<PackConfig | null>({ ...packConfig } as PackConfig);
  const { setIsLoading } = useAppContext();
  const { setEditPackConfigMode } = usePackContext();

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

      flash({});
      setEditPackConfigMode(false);
    } finally {
      setIsLoading(false);
    }
  }

  return (
    <div className="w-full h-fit bg-(--clr-surface-800) p-3 rounded-md">
      <h1 className="text-xl text-(--clr-text-primary) font-medium">
        Pack Information
      </h1>
      <hr className="my-2 border-(--clr-surface-500)" />
      <div className="flex flex-row">
        {/* pack cover */}
        <div className="flex flex-col gap-1">
          <span className="text-(--clr-text-primary)  text-left font-bold">
            Icon
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
              <span className="text-(--clr-text-primary)  text-left font-bold">
                Name
              </span>
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
              <span className="text-(--clr-text-primary)  text-left font-bold">
                Version
              </span>
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
                Author
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
                License
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
                Description
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
          text="Save"
          type="success"
          className="cursor-pointer"
        />
        <ModernButton
          onMouseUp={() => cancel()}
          text="Cancel"
          type="warning"
          className="cursor-pointer"
        />
      </div>
    </div>
  );
}

export { EditPackConfig };
