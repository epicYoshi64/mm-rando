using MMR.Randomizer.Asm;
using MMR.Randomizer.Models.Colors;
using Newtonsoft.Json;
using System.Drawing;

namespace MMR.Randomizer.Models.Settings
{

    public class CosmeticSettings
    {
        /// <summary>
        /// Options for the Asm <see cref="Patcher"/>.
        /// </summary>
        [JsonIgnore]
        public AsmOptionsCosmetic AsmOptions { get; set; } = new AsmOptionsCosmetic();

        /// <summary>
        /// Hearts color selection used for HUD color override.
        /// </summary>
        public string HeartsSelection { get; set; }

        /// <summary>
        /// Magic color selection used for HUD color override.
        /// </summary>
        public string MagicSelection { get; set; }

        /// <summary>
        /// Randomize sound effects
        /// </summary>
        public bool RandomizeSounds { get; set; }

        /// <summary>
        /// The color of Link's tunic
        /// </summary>
        public Color TunicColor { get; set; } = Color.FromArgb(0x1E, 0x69, 0x1B);

        /// <summary>
        /// The color of Deku Link's tunic
        /// </summary>
        public Color DekuTunicColor { get; set; } = Color.FromArgb(0x1E, 0x69, 0x1B);

        /// <summary>
        /// The color of Goron Link's tunic
        /// </summary>
        public Color GoronTunicColor { get; set; } = Color.FromArgb(0x1E, 0x69, 0x1B);

        /// <summary>
        /// The color of Zora Link's tunic
        /// </summary>
        public Color ZoraTunicColor { get; set; } = Color.FromArgb(0x1E, 0x69, 0x1B);

        /// <summary>
        /// The color of Fierce Deity Link's tunic
        /// </summary>
        public Color DeityTunicColor { get; set; } = Color.FromArgb(0x1E, 0x69, 0x1B);

        /// <summary>
        /// Whether Link's tunic is a random color
        /// </summary>
        public bool ignoreTunicColor { get; set; } = false;

        /// <summary>
        /// Whether Deku Link's tunic is a random color
        /// </summary>
        public bool ignoreDekuTunicColor { get; set; } = false;

        /// <summary>
        /// Whether Goron Link's tunic is a random color
        /// </summary>
        public bool ignoreGoronTunicColor { get; set; } = false;

        /// <summary>
        /// The color of Zora Link's tunic is a random color
        /// </summary>
        public bool ignoreZoraTunicColor { get; set; } = false;

        /// <summary>
        /// Whether Fierce Deity Link's tunic is a random color
        /// </summary>
        public bool ignoreDeityTunicColor { get; set; } = false;

        /// <summary>
        /// Replaces Tatl's colors
        /// </summary>
        public TatlColorSchema TatlColorSchema { get; set; }

        /// <summary>
        /// Randomize background music (includes bgm from other video games)
        /// </summary>
        public Music Music { get; set; }

        /// <summary>
        /// Default Z-Targeting style to Hold
        /// </summary>
        public bool EnableHoldZTargeting { get; set; }

        #region Asm Getters / Setters

        /// <summary>
        /// D-Pad configuration.
        /// </summary>
        public DPadConfig DPad {
            get { return this.AsmOptions.DPadConfig; }
            set { this.AsmOptions.DPadConfig = value; }
        }

        /// <summary>
        /// HUD colors.
        /// </summary>
        public HudColors HudColors {
            get { return this.AsmOptions.HudColorsConfig.Colors; }
            set { this.AsmOptions.HudColorsConfig.Colors = value; }
        }

        #endregion
    }
}
