using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using HarmonyLib;

namespace IridiumStackmasterTrophyFix
{
    /// <summary>
    /// Mod allows the Iridium Trophy easter egg multilingual support
    /// Mod cho phép easter egg Iridium Trophy hỗ trợ đa ngôn ngữ
    /// </summary>
    public class ModEntry : Mod
    {
        // Helper for translation
        // Trợ giúp cho việc dịch
        private ITranslationHelper Translation => this.Helper.Translation;

        // Flag to check if Harmony patches have been applied
        // Cờ để kiểm tra xem các bản vá Harmony đã được áp dụng chưa
        private bool hasPatched = false;

        // Singleton instance of the mod
        // Đối tượng duy nhất của mod
        public static ModEntry? Instance { get; private set; }

        // Entry point of the mod
        // Điểm bắt đầu của mod
        public override void Entry(IModHelper helper)
        {
            Instance = this;
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        }

        // Event handler for when the game is launched
        // Xử lý sự kiện khi trò chơi được khởi động
        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            this.ApplyHarmonyPatches();
            this.Monitor.Log("Iridium Stackmaster Trophy Mod Fix đã cài đặt thành công!", LogLevel.Info);
        }

        // Apply Harmony patches to the game
        // Áp dụng các bản vá Harmony vào trò chơi
        private void ApplyHarmonyPatches()
        {
            try
            {
                var harmony = new Harmony(this.ModManifest.UniqueID);
                var originalMethod = AccessTools.Method(typeof(HUDMessage), nameof(HUDMessage.numbersEasterEgg));

                if (originalMethod == null)
                {
                    this.Monitor.Log("Không tìm thấy phương thức numbersEasterEgg để vá!", LogLevel.Error);
                    return;
                }

                var prefixMethod = AccessTools.Method(typeof(ModEntry), nameof(NumbersEasterEggPrefix));

                if (prefixMethod == null)
                {
                    this.Monitor.Log("Không tìm thấy phương thức NumbersEasterEggPrefix!", LogLevel.Error);
                    return;
                }

                harmony.Patch(originalMethod, prefix: new HarmonyMethod(prefixMethod));
                hasPatched = true;
                this.Monitor.Log("Harmony đã vá xong", LogLevel.Debug);
            }
            catch (Exception ex)
            {
                this.Monitor.Log($"Lỗi khi áp dụng Harmony patches: {ex.Message}", LogLevel.Error);
                this.Monitor.Log($"Chi tiết lỗi: {ex.StackTrace}", LogLevel.Error);
            }
        }

        /// <summary>
        /// Harmony prefix to replace the original numbersEasterEgg method
        /// Harmony tiền tố để thay thế phương thức numbersEasterEgg gốc
        /// </summary>
        public static bool NumbersEasterEggPrefix(int number)
        {
            var mod = ModEntry.Instance;
            if (mod == null)
            {
                // Log warning when mod instance is not available
                // Ghi nhật ký cảnh báo khi đối tượng mod không khả dụng
                System.Diagnostics.Debug.WriteLine("IridiumStackmasterTrophyFix: Đối tượng mod không khả dụng, quay lại phương thức gốc");
                return true; // Fall back to original method if mod instance not available
                             // Quay lại phương thức gốc nếu đối tượng mod không khả dụng
            }

            if (number > 100000 && !Game1.player.mailReceived.Contains("numbersEgg1"))
            {
                Game1.player.mailReceived.Add("numbersEgg1");
                Game1.chatBox.addMessage(mod.Translation.Get("mod.IridiumStackmasterTrophyFix.egg1.message"), new Color(255, 255, 255));
            }

            if (number > 200000 && !Game1.player.mailReceived.Contains("numbersEgg2"))
            {
                Game1.player.mailReceived.Add("numbersEgg2");
                Game1.chatBox.addMessage(mod.Translation.Get("mod.IridiumStackmasterTrophyFix.egg2.message"), new Color(255, 255, 255));
            }

            if (number > 250000 && !Game1.player.mailReceived.Contains("numbersEgg3"))
            {
                Game1.player.mailReceived.Add("numbersEgg3");
                Game1.chatBox.addMessage(mod.Translation.Get("mod.IridiumStackmasterTrophyFix.egg3.message"), new Color(255, 255, 255));
            }

            if (number > 500000 && !Game1.player.mailReceived.Contains("numbersEgg1.5"))
            {
                Game1.player.mailReceived.Add("numbersEgg1.5");
                Game1.chatBox.addMessage(mod.Translation.Get("mod.IridiumStackmasterTrophyFix.egg4.message"), new Color(255, 255, 255));
            }

            if (number <= 1000000 || Game1.player.mailReceived.Contains("numbersEgg7"))
            {
                return false; // Skip original method
                              // Bỏ qua phương thức gốc
            }

            Game1.player.mailReceived.Add("numbersEgg7");
            Game1.chatBox.addMessage(mod.Translation.Get("mod.IridiumStackmasterTrophyFix.trophy.achieved"), new Color(104, 214, 255));
            Game1.playSound("discoverMineral");

            DelayedAction.functionAfterDelay(delegate
            {
                Game1.chatBox.addMessage(mod.Translation.Get("mod.IridiumStackmasterTrophyFix.qi.message"), new Color(100, 50, 255));
            }, 6000);

            return false; // Skip original method
                          // Bỏ qua phương thức gốc
        }
    }
}