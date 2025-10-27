import gsap from "gsap";
import { CircleAlert, CircleQuestionMark, X } from "lucide-react";
import { useLayoutEffect, useReducer, useRef, useState } from "react";
import { useAppContext } from "../../Providers/AppContext";
import {
  getBase64FromPath,
  selectFileAbsolutePath,
} from "../../service/fileService";
import type { ApiResponse } from "../../models/ApiResponse";
import { ModernTextInput } from "../../ui/ModernTextInput";
import { ModernTextArea } from "../../ui/ModernTextArea";
import { ModernButton } from "../../ui/ModernButton";
import type { PackConfig } from "../../models/PackConfig";
import { createPack } from "../../service/packService";

interface PackFormState {
  PackName: string;
  Version: string;
  Author: string;
  Description: string;
  License: string;
}

type PackFormAction =
  | { type: "SET_FIELD"; field: keyof PackFormState; value: string }
  | { type: "RESET"; payload?: Partial<PackFormState> };

const initialState: PackFormState = {
  PackName: "",
  Version: "",
  Author: "",
  Description: "",
  License: "MIT",
};

function packFormReducer(
  state: PackFormState,
  action: PackFormAction
): PackFormState {
  switch (action.type) {
    case "SET_FIELD":
      return { ...state, [action.field]: action.value };

    case "RESET":
      return { ...initialState, ...action.payload };

    default:
      return state;
  }
}

function Createpackpopup() {
  const { setIsLoading, setShowCreatePackPopup } = useAppContext();
  const cardRef = useRef<HTMLDivElement>(null);
  const [packCoverBase64, setPackCoverBase64] = useState<string | null>(null);
  const [formState, dispatch] = useReducer(packFormReducer, initialState);
  const [packNameError, setPackNameError] = useState<string | null>(null);
  const [versionError, setVersionError] = useState<string | null>(null);

  useLayoutEffect(() => {
    if (cardRef.current) {
      gsap.fromTo(
        cardRef.current,
        { scale: 0, opacity: 0 },
        { scale: 1, opacity: 1, duration: 0.3 }
      );
    }
  }, []);

  function closePopup() {
    if (cardRef.current) {
      gsap.to(cardRef.current, {
        scale: 0.6,
        opacity: 0,
        duration: 0.3,
        onComplete: () => {
          setShowCreatePackPopup(false);
        },
      });
    }
  }

  async function selectPackCover() {
    try {
      setIsLoading(true);
      const filePath = await selectFileAbsolutePath([".png", "png"]);
      if (!filePath) return;

      const apiResponse: ApiResponse<string> = await getBase64FromPath(
        filePath
      );
      if (!apiResponse.Success) return;
      setPackCoverBase64(apiResponse.Data);
    } finally {
      setIsLoading(false);
    }
  }

  async function removeCover() {
    setPackCoverBase64(null);
  }

  async function handleSubmit() {
    try {
      setIsLoading(true);
      setPackNameError(null);
      setVersionError(null);
      let hasError = false;

      if (formState.PackName.trim() === "") {
        setPackNameError("Pack name is required.");
        hasError = true;
      }

      if (formState.Version.trim() === "") {
        setVersionError("Version is required.");
        hasError = true;
      }

      const normalizedVersion = formState.Version.startsWith("v")
        ? formState.Version.slice(1)
        : formState.Version;

      if (normalizedVersion.match(/^[0-9]+\.[0-9]+\.[0-9]+$/) === null) {
        setVersionError("Version must be in the format 1.0.0");
        hasError = true;
      }

      if (hasError) return;

      // @ts-ignore
      const packConfig: PackConfig = {
        PackName: formState.PackName,
        Version: normalizedVersion,
        Author: formState.Author,
        Description: formState.Description,
        License: formState.License,
      };
      if (packCoverBase64) {
        packConfig.CoverPngBase64 = packCoverBase64;
      }

      const apiResponse: ApiResponse<null> = await createPack(packConfig);

      console.log(apiResponse);

      if (!apiResponse.Success) {
        // show erro toast
        return;
      }

      closePopup();
    } finally {
      setIsLoading(false);
    }
  }

  return (
    <div className="w-full h-full absolute top-0 left-0 inset-0 z-100 overflow-y-auto">
      <div className="w-full h-full absolute top-0 left-0 inset-0 z-101 bg-black opacity-90 blur-xl" />
      <div
        ref={cardRef}
        className=" absolute top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2 z-102 w-96 h-fit p-3 bg-(--clr-surface-900) rounded-md border border-(--clr-surface-700) shadow-lg flex flex-col"
      >
        <div className="flex flex-row justify-between items-center">
          <span className="text-(--clr-text-secondary) font-bold ">
            Create Icon Pack
          </span>
          <button
            onMouseUp={() => closePopup()}
            className="cursor-pointer w-4 h-4 flex items-center justify-center text-(--clr-text-secondary) hover:text-(--clr-text-primary)"
          >
            <X />
          </button>
        </div>
        <div className="flex flex-col w-fit">
          <span className="text-(--clr-text-secondary) mt-4 text-center">
            Icon
          </span>
          {packCoverBase64 ? (
            <div className="relative select-none w-12 h-12 border-2   border-(--clr-surface-800) bg-(--clr-surface-950) rounded-md flex items-center justify-center text-center">
              <img
                src={packCoverBase64}
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
        <div className="mt-5 flex flex-col gap-1">
          <div className="flex flex-row items-center gap-2">
            <span className="text-(--clr-text-secondary) w-fit">Pack Name</span>
            {packNameError && (
              <span className="flex flex-row items-center gap-1 text-(--clr-warning-10)">
                <CircleAlert /> {packNameError}
              </span>
            )}
          </div>
          <ModernTextInput
            value={formState.PackName}
            onChange={(val) => {
              dispatch({ type: "SET_FIELD", field: "PackName", value: val });
            }}
            placeholder="Default pack"
            className="rounded-sm bg-(--clr-surface-900) "
          />
        </div>
        <div className="mt-5 flex flex-col gap-1">
          <div className="flex flex-row gap-2">
            <span className="text-(--clr-text-secondary) w-fit">Version</span>
            {versionError && (
              <span className="flex flex-row items-center gap-1 text-(--clr-warning-10)">
                <CircleAlert /> {versionError}
              </span>
            )}
          </div>
          <ModernTextInput
            value={formState.Version}
            onChange={(val) => {
              dispatch({ type: "SET_FIELD", field: "Version", value: val });
            }}
            placeholder="e.g., v1.0.0"
            className="rounded-sm bg-(--clr-surface-900) "
          />
        </div>
        <div className="mt-5 flex flex-col gap-1">
          <div className="flex flex-row">
            <span className="text-(--clr-text-secondary) w-fit">Author</span>
          </div>
          <ModernTextInput
            value={formState.Author}
            onChange={(val) => {
              dispatch({ type: "SET_FIELD", field: "Author", value: val });
            }}
            placeholder="Your name"
            className="rounded-sm bg-(--clr-surface-900) "
          />
        </div>
        <div className="mt-5 flex flex-col gap-1">
          <div className="flex flex-row">
            <span className="text-(--clr-text-secondary) w-fit">License</span>
          </div>
          <ModernTextInput
            value={formState.License}
            onChange={(val) => {
              dispatch({ type: "SET_FIELD", field: "License", value: val });
            }}
            placeholder="Enter license"
            className="rounded-sm bg-(--clr-surface-900) "
          />
        </div>
        <div className="mt-5 flex flex-col gap-1">
          <div className="flex flex-row">
            <span className="text-(--clr-text-secondary) w-fit">
              Description
            </span>
          </div>
          <ModernTextArea
            value={formState.Description}
            onChange={(val) => {
              dispatch({ type: "SET_FIELD", field: "Description", value: val });
            }}
            placeholder="Enter description"
            className="rounded-sm bg-(--clr-surface-900) "
          />
        </div>

        <ModernButton
          text="Create"
          type="info"
          className="mt-4 cursor-pointer"
          onMouseUp={() => handleSubmit()}
        />
      </div>
    </div>
  );
}

export { Createpackpopup };
