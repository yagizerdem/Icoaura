import { CircleQuestionMark } from "lucide-react";
import type { PackConfig } from "../../models/PackConfig";
import { cn } from "../../util/twUtil";

interface PackConfigCardProps {
  packConfig: PackConfig;
  className?: string;
}

function PackConfigCard({ packConfig, className }: PackConfigCardProps) {
  return (
    <div
      className={cn(
        "w-full h-fit flex flex-row px-3 my-5 cursor-pointer hover:bg-(--clr-surface-800) rounded-md py-3 transition-colors duration-300",
        className
      )}
    >
      <div className="flex flex-row">
        {packConfig.CoverPngBase64 && (
          <img
            src={packConfig.CoverPngBase64}
            alt={packConfig.PackName}
            className="w-12 h-12 object-cover rounded-md"
          />
        )}
        {!packConfig.CoverPngBase64 && (
          <div className="w-12 h-12 bg-(--clr-surface-900) rounded-md flex items-center justify-center">
            <CircleQuestionMark className="w-full h-full text-(--clr-text-secondary)" />
          </div>
        )}

        <div className="flex flex-col ml-3">
          <span className="font-semibold text-lg">{packConfig.PackName}</span>
          <span className="text-sm text-gray-500">
            {packConfig.Version.startsWith("v")
              ? packConfig.Version
              : `v${packConfig.Version}`}
          </span>
        </div>
      </div>
    </div>
  );
}

export { PackConfigCard };
