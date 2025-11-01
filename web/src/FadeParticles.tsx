import { useEffect, useMemo, useState } from "react";
import Particles, { initParticlesEngine } from "@tsparticles/react";
import { type Container, type ISourceOptions } from "@tsparticles/engine";
import { loadSlim } from "@tsparticles/slim";

export const FadeParticles = () => {
  const [init, setInit] = useState(false);

  useEffect(() => {
    initParticlesEngine(async (engine) => {
      await loadSlim(engine);
    }).then(() => {
      setInit(true);
    });
  }, []);

  const particlesLoaded = async (container?: Container): Promise<void> => {
    console.log(container);
  };

  const options: ISourceOptions = useMemo(
    () => ({
      fpsLimit: 60,
      background: {
        color: { value: "transparent" }, //
      },

      particles: {
        color: { value: "#ffffff" },
        move: {
          direction: "none",
          enable: true,
          outModes: "out",
          random: false,
          speed: 2,
          straight: false,
        },
        number: {
          density: { enable: false },
          value: 100,
        },

        opacity: {
          value: { min: 0, max: 0.8 },
          animation: { enable: true, speed: 0.8, sync: false },
        },
        life: {
          count: 0,
          duration: { min: 2, max: 5 },
          delay: { min: 0, max: 2 },
        },
        shape: { type: "square" },
        size: {
          value: { min: 4, max: 10 },
          animation: {
            enable: true,
            speed: 1,
            minimumValue: 0.1,
            sync: false,
            startValue: "random",
            destroy: "none",
          },
        },
      },
    }),
    []
  );

  if (!init) return null;

  return (
    <div
      style={{
        position: "absolute",
        inset: 0,
        zIndex: 99,
        pointerEvents: "none",
      }}
    >
      <Particles
        id="tsparticles"
        particlesLoaded={particlesLoaded}
        options={options}
      />
    </div>
  );
};
