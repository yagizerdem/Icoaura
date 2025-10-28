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

function PackOperation() {
  const { isAdmin, setIsAdmin } = useAppContext();

  useEffect(() => {
    helper();
    async function helper() {
      const isAdmin = await HasAdminPrivilege();
      setIsAdmin(isAdmin);
    }
  }, []);
  return (
    <div className="w-full h-fit bg-(--clr-surface-800) p-3 rounded-md">
      <div className="flex flex-row gap-4">
        <h1 className="text-xl text-(--clr-text-primary) font-medium">
          Pack Operations
        </h1>

        <ModernButton
          onMouseUp={() => rebootAsAdmin()}
          text="Reboot as admin"
          type="ghost"
          className="cursor-pointer bg-(--clr-surface-900)"
          disabled={isAdmin}
        />
        {!isAdmin && (
          <div className="flex flex-row items-center gap-1 text-(--clr-warning-10)">
            <CircleQuestionMark />
            <span className=" self-center ">
              Some icons are only applied when the application is run as
              administrator
            </span>
          </div>
        )}
      </div>
      <hr className="my-2 border-(--clr-surface-500)" />

      <div className="flex flex-row gap-4 mt-5">
        <ModernButton text="Apply" type="success" className="cursor-pointer" />
        <ModernIconButton
          icon={<Pen />}
          text="Edit"
          type="ghost"
          className="cursor-pointer"
        />
        <ModernIconButton
          icon={<Upload />}
          text="Export"
          type="ghost"
          className="cursor-pointer"
        />
      </div>

      <div className="flex flex-row gap-4 mt-5">
        <ModernIconButton
          icon={<Monitor />}
          text="Add dekstop icons"
          type="ghost"
          className="cursor-pointer"
        />
        <ModernIconButton
          icon={<Image />}
          text="Add icon"
          type="ghost"
          className="cursor-pointer"
        />
        <ModernIconButton
          icon={<Folder />}
          text="Add directory icon"
          type="ghost"
          className="cursor-pointer"
        />
      </div>
    </div>
  );
}

export { PackOperation };
