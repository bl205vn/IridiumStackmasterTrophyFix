//Using Nuget packages: Harmony and stardew
//Dùng Nutget packages: Harmony và stardew
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using HarmonyLib;

namespace IridiumStackmasterTrophyFix
{
    ///<summary>
    ///Mod allows the Iridium Trophy easter egg multilingual support
    ///Mod cho phép easter egg Iridium Trophy hỗ trợ đa ngôn ngữ
    ///</summary>
    public class ModEntry : Mod
    {
        //Helper for translation
        //Trợ giúp cho việc dịch
        private ITranslationHelper Translation => this.Helper.Translation;

        //Flag to check if Harmony patches have been applied
        //Cờ để kiểm tra xem các bản vá Harmony đã được áp dụng chưa
        private bool hasPatched = false;

        //Singleton instance of the mod
        //Đối tượng duy nhất của mod
        public static ModEntry? Instance { get; private set; }

        //Entry point of the mod
        //Điểm bắt đầu của mod
        public override void Entry(IModHelper helper)
        {
            //Set the singleton instance for static access
            //Gán đối tượng mod vào biến tĩnh để các phần khác có thể dùng
            Instance = this;
            //Subscribe to GameLoop.GameLaunched so we can patch after the game finishes loading
            //Đăng ký hàm OnGameLaunched để áp dụng Harmony patch khi game khởi động xong
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        }

        //Event handler for when the game is launched
        //Xử lý sự kiện khi trò chơi được khởi động
        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            //Only apply Harmony patches if not previously applied
            //Chỉ áp dụng Harmony patches nếu chưa được áp dụng trước đó
            if (!hasPatched)
            {
                //Apply Harmony patches to the game
                //Áp dụng các bản vá Harmony vào trò chơi
                this.ApplyHarmonyPatches();
            }
            else
            {
                //If hasPatched is true, meaning Harmony patches have been applied before, then notify
                //Nếu hasPatched là true, nghĩa là Harmony patches đã được áp dụng trước đó thì thông báo
                this.Monitor.Log("Harmony patches đã được áp dụng trước đó, bỏ qua.", LogLevel.Debug);
            }
            //Notification of successful installation
            //Thông báo đã cài thành công
            this.Monitor.Log("Iridium Stackmaster Trophy Mod Fix đã cài đặt thành công!", LogLevel.Info);
        }
        private void ApplyHarmonyPatches()
        {
            try
            {
                //Check if the mod has patched the original code
                //Kiểm tra xem mod đã vá code gốc chưa
                if (hasPatched)
                {
                    this.Monitor.Log("Bỏ qua việc vá bằng Harmony vì đã được áp dụng trước đó.", LogLevel.Info);
                    return;
                }
                //Create a Harmony instance scoped to this mod (using its UniqueID) for applying patches
                //Tạo Harmony instance có scope riêng cho mod này (dựa trên UniqueID) để áp dụng các bản vá
                var harmony = new Harmony(this.ModManifest.UniqueID);
                //Find the numbersEasterEgg method
                //Tìm phương thức numbersEasterEgg
                var originalMethod = AccessTools.Method(typeof(HUDMessage), nameof(HUDMessage.numbersEasterEgg));
                //If not found, report an error
                //Nếu không tìm thấy thì báo lỗi
                if (originalMethod == null)
                {
                    this.Monitor.Log("Không tìm thấy phương thức numbersEasterEgg để vá!", LogLevel.Error);
                    return;
                }
                //Find the numbersEasterEggfix method to replace the original code
                //Tìm phương thức numbersEasterEggfix để thay thế mã gốc
                var prefixMethod = AccessTools.Method(typeof(ModEntry), nameof(NumbersEasterEggPrefix));
                //If not found, report an error
                //Nếu không tìm thấy thì báo lỗi
                if (prefixMethod == null)
                {
                    this.Monitor.Log("Không tìm thấy phương thức NumbersEasterEggPrefix!", LogLevel.Error);
                    return;
                }
                //Apply harmony patch: Attach prefixMethod to originalMethod
                //Áp dụng bản vá harmony: Gắn prefixMethod vào originalMethod
                harmony.Patch(originalMethod, prefix: new HarmonyMethod(prefixMethod));
                //Set the check flag to true
                //Gắn cờ kiểm tra thành true
                hasPatched = true;
                this.Monitor.Log("Harmony đã vá xong", LogLevel.Debug);
            }
            catch (Exception ex)
            {
                this.Monitor.Log($"Lỗi khi áp dụng Harmony patches: {ex.Message}", LogLevel.Error);
                this.Monitor.Log($"Chi tiết lỗi: {ex.StackTrace}", LogLevel.Error);
            }
        }

        ///<summary>
        ///Harmony prefix to replace the original numbersEasterEgg method
        ///Harmony tiền tố để thay thế phương thức numbersEasterEgg gốc
        ///</summary>
        public static bool NumbersEasterEggPrefix(int number)
        {
            var mod = ModEntry.Instance;
            if (mod == null)
            {
                //Log warning when mod instance is not available
                //Ghi nhật ký cảnh báo khi đối tượng mod không khả dụng
                System.Diagnostics.Debug.WriteLine("IridiumStackmasterTrophyFix: Đối tượng mod không khả dụng, quay lại phương thức gốc");
                return true; //Fall back to original method if mod instance not available
                             //Quay lại phương thức gốc nếu đối tượng mod không khả dụng
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
                return false; //Skip original method
                              //Bỏ qua phương thức gốc
            }

            Game1.player.mailReceived.Add("numbersEgg7");
            Game1.chatBox.addMessage(mod.Translation.Get("mod.IridiumStackmasterTrophyFix.trophy.achieved"), new Color(104, 214, 255));
            Game1.playSound("discoverMineral");

            DelayedAction.functionAfterDelay(delegate
            {
                Game1.chatBox.addMessage(mod.Translation.Get("mod.IridiumStackmasterTrophyFix.qi.message"), new Color(100, 50, 255));
            }, 6000);

            return false; //Skip original method
                          //Bỏ qua phương thức gốc
        }
    }
}