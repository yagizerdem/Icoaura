import { useEffect, useRef, useState } from "react";
import type { PackItem } from "../../models/PackItem";
import { GetPackItemIconBase64 } from "../../service/packService";
import { getSelectedPackConfig } from "../../util/getSelectedPackConfig";
import {
  Check,
  ChevronUp,
  CircleQuestionMark,
  Folder,
  Trash,
  X,
} from "lucide-react";
import gsap from "gsap";
import { useAppContext } from "../../Providers/AppContext";
import {
  getBase64FromPath,
  IsFileSystemEntryExist,
  selectFileAbsolutePath,
  SelectFileRelativeFilePath,
  SelectRelativeDirectoryPath,
} from "../../service/fileService";
import { ModernTextInput } from "../../ui/ModernTextInput";
import { useDebounce } from "../../hook/useDebounce";
import { ModernTextArea } from "../../ui/ModernTextArea";

interface PackItemCardProps {
  packItems: PackItem[];
  selectedPackItem: PackItem;
  setPackItems: React.Dispatch<React.SetStateAction<PackItem[]>>;
  packItemIconMap: Record<string, string | null>;
  setPackItemIconMap: React.Dispatch<
    React.SetStateAction<Record<string, string | null>>
  >;
}

function PackItemCard({
  selectedPackItem,
  packItemIconMap,
  setPackItemIconMap,
  setPackItems,
}: PackItemCardProps) {
  const packConfig = getSelectedPackConfig();
  const iconBase64 = packItemIconMap[selectedPackItem.Uid];
  const [expanded, setExpanded] = useState(false);
  const cardBodyRef = useRef<HTMLDivElement>(null);
  const chevronRef = useRef<HTMLButtonElement>(null);
  const { setIsLoading } = useAppContext();
  const [taregetPath, setTargetPath] = useState<string>(
    selectedPackItem.TargetPath
  );
  const [targetUrl, setTargetUrl] = useState<string>(
    selectedPackItem.TargetUrl
  );
  const [targetExe, setTargetExe] = useState<string>(
    selectedPackItem.TargetExePath
  );
  const [name, setName] = useState<string>(selectedPackItem.Name);
  const [description, setDescription] = useState<string>(
    selectedPackItem.Description
  );

  const debouncedName = useDebounce(name, 200);
  const debouncedTargetPath = useDebounce(taregetPath, 200);
  const debouncedTargetUrl = useDebounce(targetUrl, 200);
  const debouncedTargetExe = useDebounce(targetExe, 200);
  const debouncedDescription = useDebounce(description, 200);
  const [isTargetPathExist, setIsTargetPathExist] = useState<boolean>(true);
  const [isTargetExeExist, setIsTargetExeExist] = useState<boolean>(true);

  useEffect(() => {
    setPackItems((items) => {
      return items.map((item) => {
        if (item.Uid === selectedPackItem.Uid) {
          return {
            ...item,
            Name: debouncedName,
            TargetExePath: debouncedTargetExe,
            TargetPath: debouncedTargetPath,
            TargetUrl: debouncedTargetUrl,
            Description: debouncedDescription,
          };
        }
        return item;
      });
    });
  }, [
    debouncedName,
    debouncedTargetExe,
    debouncedTargetPath,
    debouncedTargetUrl,
    debouncedDescription,
  ]);

  useEffect(() => {
    fetchIcon();
    async function fetchIcon() {
      if (!selectedPackItem || !selectedPackItem.Uid) return;
      if (!packConfig || !packConfig.Uid) return;

      const base64Icon: string =
        (await GetPackItemIconBase64(packConfig.Uid, selectedPackItem.Uid))
          .Data ?? "";

      if (base64Icon) {
        setPackItemIconMap((map) => {
          return { ...map, [selectedPackItem.Uid]: base64Icon };
        });
      }
    }
  }, [selectedPackItem]);

  function deletePackItem() {
    setPackItems((items) => {
      return items.filter((item) => item.Uid !== selectedPackItem.Uid);
    });
  }

  useEffect(() => {
    if (!cardBodyRef.current) return;

    cardBodyRef.current.style.height = "0px";
  }, [cardBodyRef]);

  useEffect(() => {
    if (!cardBodyRef.current) return;

    if (expanded) {
      gsap.to(cardBodyRef.current, {
        height: "550px",
        duration: 0.3,
        ease: "power2.out",
      });

      gsap.to(chevronRef.current, {
        rotate: -180,
        duration: 0.3,
        ease: "power2.out",
      });
    }

    if (!expanded) {
      gsap.to(cardBodyRef.current, {
        height: "0px",
        duration: 0.3,
        ease: "power2.out",
      });

      gsap.to(chevronRef.current, {
        rotate: 0,
        duration: 0.3,
        ease: "power2.out",
      });
    }
  }, [cardBodyRef, expanded]);

  useEffect(() => {
    helper();
    async function helper() {
      const flag = await IsFileSystemEntryExist(debouncedTargetPath);
      setIsTargetPathExist(flag);
    }
  }, [debouncedTargetPath]);

  useEffect(() => {
    helper();
    async function helper() {
      const flag = await IsFileSystemEntryExist(debouncedTargetExe);
      setIsTargetExeExist(flag);
    }
  }, [debouncedTargetExe]);

  function removePackItemIcon() {
    packItemIconMap[selectedPackItem.Uid] = null;
    setPackItemIconMap({ ...packItemIconMap });
  }

  async function selectPackItemIcon() {
    try {
      setIsLoading(true);
      const iconPath = await selectFileAbsolutePath([".png", "png"]);
      if (!iconPath) return;

      const base64 = (await getBase64FromPath(iconPath)).Data || null;

      if (base64) {
        packItemIconMap[selectedPackItem.Uid] = base64;
        setPackItemIconMap({ ...packItemIconMap });
      }
    } finally {
      setIsLoading(false);
    }
  }

  async function selecteTargetExePath() {
    try {
      setIsLoading(true);
      const exePath = await SelectFileRelativeFilePath([".exe", "exe"]);
      if (!exePath) return;
      setTargetExe(exePath);
    } finally {
      setIsLoading(false);
    }
  }

  async function selectTargetPath() {
    try {
      setIsLoading(true);
      let path = "";
      if (selectedPackItem.TargetPath.endsWith(".lnk")) {
        path = await SelectFileRelativeFilePath([".lnk", "lnk"]);
      } else if (selectedPackItem.TargetPath.endsWith(".url")) {
        path = await SelectFileRelativeFilePath([".url", "url"]);
      } else {
        path = await SelectRelativeDirectoryPath();
      }
      if (!path) return;
      setTargetPath(path);
    } finally {
      setIsLoading(false);
    }
  }

  return (
    <div className="flex flex-col py-1 px-3 border-b border-gray-300 items-center">
      {/* head  */}
      <div
        onMouseUp={() => setExpanded((prev) => !prev)}
        className="flex flex-row  w-full justify-between cursor-pointer"
      >
        <div className="flex flex-row items-center gap-4 ">
          {iconBase64 ? (
            <img src={iconBase64} alt="icon" className="w-12 h-12" />
          ) : (
            <div className="w-12 h-12 bg-(--clr-surface-950) rounded-sm">
              <CircleQuestionMark className="w-full h-full text-(--clr-text-primary)" />
            </div>
          )}
          <div className="text-(--clr-text-primary) font-bold ">
            {selectedPackItem.Name}
          </div>
        </div>
        <div className="flex flex-row gap-4 mx-5 items-center">
          <button
            onMouseUp={(e) => {
              e.stopPropagation();
              deletePackItem();
            }}
            className="text-(--clr-text-primary) cursor-pointer hover:text-(--clr-text-secondary) transition-colors duration-200"
          >
            <Trash />
          </button>
          <button
            ref={chevronRef}
            className="text-(--clr-text-primary) cursor-pointer hover:text-(--clr-text-secondary) transition-colors duration-200"
          >
            <span
              onMouseUp={(e) => {
                e.stopPropagation();
                setExpanded((prev) => !prev);
              }}
            >
              <ChevronUp />
            </span>
          </button>
        </div>
      </div>

      {/* card body */}
      <div
        ref={cardBodyRef}
        className="flex flex-col w-full mt-5 overflow-hidden"
      >
        <div className="flex flex-row  w-full p-3 items-center gap-4">
          {iconBase64 ? (
            <div className="w-12 h-12 bg-(--clr-surface-950) rounded-sm relative border border-(--clr-surface-500)">
              <div
                onMouseUp={() => removePackItemIcon()}
                className="absolute top-0 right-0 text-(--clr-text-primary) p-1 font-bold cursor-pointer  select-none w-4 h-4 flex items-center justify-center z-11 bg-(--clr-danger-20)  rounded-full"
              >
                <X />
              </div>
              <img
                src={iconBase64}
                alt="icon"
                className="w-12 h-12 relative z-10"
              />
            </div>
          ) : (
            <div
              className="w-12 h-12 bg-(--clr-surface-950) rounded-sm cursor-pointer"
              onMouseUp={() => selectPackItemIcon()}
            >
              <CircleQuestionMark className="w-full h-full text-(--clr-text-primary)" />
            </div>
          )}
          <ModernTextInput
            label="Name"
            value={name}
            onChange={(val) => {
              setName(val);
            }}
          />
        </div>

        <div className="flex flex-row  w-full p-3 items-center gap-4">
          <ModernTextInput
            label="Target path"
            value={taregetPath}
            onChange={(val) => {
              setTargetPath(val);
            }}
          />
          <button
            onMouseUp={() => selectTargetPath()}
            className="w-10 h-10 bg-(--clr-surface-950) mt-5 rounded-sm flex items-center justify-center cursor-pointer text-(--clr-text-primary) p-1"
          >
            <Folder />
          </button>
          <div className="flex flex-row items-center justify-center h-full align-middle mt-4">
            {!isTargetPathExist && <X className="text-(--clr-danger-10)" />}
            {isTargetPathExist && <Check className="text-(--clr-success-10)" />}
          </div>
        </div>

        <div className="flex flex-row  w-full p-3 items-center gap-4">
          <ModernTextInput
            label="Target exe path"
            value={targetExe}
            onChange={(val) => {
              setTargetExe(val);
            }}
          />
          <button
            onMouseUp={() => selecteTargetExePath()}
            className="w-10 h-10 bg-(--clr-surface-950) mt-5 rounded-sm flex items-center justify-center cursor-pointer text-(--clr-text-primary) p-1"
          >
            <Folder />
          </button>
          <div className="flex flex-row items-center justify-center h-full align-middle mt-4">
            {!isTargetExeExist && <X className="text-(--clr-danger-10)" />}
            {isTargetExeExist && <Check className="text-(--clr-success-10)" />}
          </div>
        </div>

        <div className="flex flex-row  w-full p-3 items-center gap-4">
          <ModernTextInput
            label="Target url path"
            value={targetUrl}
            onChange={(val) => {
              setTargetUrl(val);
            }}
          />
        </div>
        <ModernTextArea
          label="Description"
          value={description}
          onChange={(val) => {
            setDescription(val);
          }}
        />
      </div>
    </div>
  );
}

export { PackItemCard };
