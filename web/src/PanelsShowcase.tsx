import { motion, easeOut } from "framer-motion";
import packPanel from "./assets/Ekran görüntüsü 2025-10-31 105009.png";
import editPackPanel from "./assets/Ekran görüntüsü 2025-10-31 105247.png";
import settingsPanel from "./assets/Ekran görüntüsü 2025-10-31 105052.png";

function PanelsShowcase() {
  const fadeIn = (direction: "left" | "right", delay: number) => ({
    hidden: {
      opacity: 0,
      x: direction === "left" ? -80 : 80,
    },
    visible: {
      opacity: 1,
      x: 0,
      transition: {
        duration: 0.8,
        delay: delay * 0.3,
        ease: easeOut,
      },
    },
  });

  const cards = [
    {
      direction: "left",
      image: packPanel,
      title: "Pack Management",
      desc: "Organize, preview, and export your custom icon packs with just a few clicks.",
      align: "left",
    },
    {
      direction: "right",
      image: editPackPanel,
      title: "Icon Editor",
      desc: "Edit, recolor, and fine-tune your icons with built-in advanced editing tools.",
      align: "right",
    },
    {
      direction: "left",
      image: settingsPanel,
      title: "Smart Settings",
      desc: "Configure global themes, localization, and automation directly within Icoaura.",
      align: "left",
    },
  ];

  return (
    <div className="flex flex-col items-center gap-32 mt-20">
      {cards.map((card, i) => (
        <motion.div
          key={i}
          variants={fadeIn(card.direction as "left" | "right", i)}
          initial="hidden"
          whileInView="visible"
          viewport={{ once: true, amount: 0.3 }}
          className={`flex flex-col md:flex-row ${
            card.align === "right" ? "md:flex-row-reverse" : ""
          } items-center gap-10 w-[90%] md:w-[80%]`}
        >
          {/* Image */}
          <img
            src={card.image}
            className="w-[450px] rounded-xl shadow-[0_0_30px_rgba(100,100,255,0.3)]"
            alt={card.title}
          />

          {/* Text Section */}
          <div className="flex flex-col text-center md:text-left gap-4 max-w-[500px]">
            <h2 className="text-3xl font-bold text-white drop-shadow-[0_0_10px_rgba(180,180,255,0.3)]">
              {card.title}
            </h2>
            <p className="text-gray-300 text-lg leading-relaxed">{card.desc}</p>
          </div>
        </motion.div>
      ))}
    </div>
  );
}

export { PanelsShowcase };
