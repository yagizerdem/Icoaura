import { useLocation, useNavigate } from "react-router";
import { ModernButton } from "../../ui/ModernButton";
import { cn } from "../../util/twUtil";
import { useAppContext } from "../../Providers/AppContext";
import { Moon, Sun } from "lucide-react";
import { useEffect, useRef, useState } from "react";
import { Theme } from "../../enum/Theme";
import gsap from "gsap";
import { GetSystemDefaultTheme } from "../../service/windowService";
import { useL10NContext } from "../../Providers/L10NContext";

function DefaultNavBar() {
  const { appConfig, setAppConfig } = useAppContext();
  const [theme, setTheme] = useState(Theme.Light);
  const sunRef = useRef<HTMLElement>(null);
  const moonRef = useRef<HTMLElement>(null);

  const location = useLocation();
  const navigate = useNavigate();

  const { getLocalizedString } = useL10NContext();

  function navigateToPack() {
    navigate("/pack");
  }

  function navigateToSettings() {
    navigate("/settings");
  }

  useEffect(() => {
    fetch();
    async function fetch() {
      if (appConfig?.Theme == Theme.DefaultSystem) {
        const theme: Theme = await GetSystemDefaultTheme();
        setTheme(theme);
      } else {
        setTheme(appConfig?.Theme!);
      }
    }
  }, [appConfig?.Theme]);

  function switchTheme(newTheme: Theme) {
    if (!appConfig) return;

    setAppConfig({
      ...appConfig,
      Theme: newTheme,
    });
  }

  useEffect(() => {
    if (theme === Theme.Dark) {
      gsap.fromTo(
        moonRef.current,
        {
          rotation: 135,
        },
        {
          rotation: 0,
          duration: 0.5,
          ease: "power2.out",
        }
      );
    } else {
      gsap.fromTo(
        sunRef.current,
        {
          rotation: 135,
        },
        {
          rotation: 0,
          duration: 0.5,
          ease: "power2.out",
        }
      );
    }
  }, [theme]);

  return (
    <div className="w-full flex flex-row justify-between  bg-(--clr-surface-700) ">
      <div className=" h-12 flex items-center flex-row px-3 gap-3">
        <ModernButton
          text={getLocalizedString("Common.Pack")}
          type="ghost"
          className={cn(
            "cursor-pointer transition-colors duration-300 hover:bg-(--clr-surface-700) bg-(--clr-surface-800) border-none",
            location.pathname.startsWith("/pack") &&
              "font-bold bg-(--clr-surface-950) "
          )}
          onMouseUp={navigateToPack}
        />
        <ModernButton
          text={getLocalizedString("Common.Settings")}
          type="ghost"
          className={cn(
            "cursor-pointer transition-colors duration-300  hover:bg-(--clr-surface-700) bg-(--clr-surface-800) border-none",
            location.pathname.startsWith("/settings") &&
              "font-bold bg-(--clr-surface-950) "
          )}
          onMouseUp={navigateToSettings}
        />
      </div>

      <div className="flex flex-row items-center mx-5 text-(--clr-text-secondary)">
        {theme === Theme.Dark && (
          <span
            className="cursor-pointer"
            ref={moonRef}
            onMouseUp={() => switchTheme(Theme.Light)}
          >
            <Moon />
          </span>
        )}
        {theme === Theme.Light && (
          <span
            className="cursor-pointer"
            ref={sunRef}
            onMouseUp={() => switchTheme(Theme.Dark)}
          >
            <Sun />
          </span>
        )}
      </div>
    </div>
  );
}

export { DefaultNavBar };
