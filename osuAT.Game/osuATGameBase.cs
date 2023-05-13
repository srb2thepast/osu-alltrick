using System;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading.Tasks;
using NuGet.Protocol.Core.Types;
using osu.Framework;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Textures;
using osu.Framework.Input;
using osu.Framework.Input.Handlers.Mouse;
using osu.Framework.Input.Handlers.Tablet;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;
using osu.Framework.Screens;
using osu.Framework.Utils;
using osu.Game.Online;
using osuAT.Resources;
using osuTK;
using osuTK.Graphics.ES30;

namespace osuAT.Game
{
    public partial class osuATGameBase : osu.Framework.Game
    {
        // Anything in this class is shared between the test browser and the game implementation.
        // It allows for caching global dependencies that should be accessible to tests, or changing
        // the screen scaling for all components including the test browser and framework overlays.
        protected override Container<Drawable> Content { get; }

        public new static DependencyContainer Dependencies;
        public static readonly EndpointConfiguration ENDPOINT_CONFIGURATION = new ProductionEndpointConfiguration();

        protected osuATGameBase()
        {
            // Ensure game and tests scale with window size and screen DPI.
            base.Content.Add(Content = new DrawSizePreservingFillContainer
            {
                // You may want to change TargetDrawSize to your "default" resolution, which will decide how things scale and position when using absolute coordinates.
                TargetDrawSize = new Vector2(1366, 968),
            });
        }

        /*
        protected override bool OnExiting()
        {
            if (SaveStorage.IsSaving == false)
            {
                System.Console.WriteLine("exited, beginning autosave");
                SaveStorage.Save();
                Task.Delay(100);
            }
            System.Console.WriteLine("exited");
            return false;
        }
        */

        [BackgroundDependencyLoader]
        private void load()
        {
            Console.WriteLine(Host.AvailableInputHandlers);
            Window.WindowMode.Value = osu.Framework.Configuration.WindowMode.Windowed;
            Resources.AddStore(new DllResourceStore(typeof(osuATResources).Assembly));
            AddFont(Resources, "Fonts/osuFont");
            AddFont(Resources, "Fonts/VarelaRound");
            AddFont(Resources, "Fonts/ChivoBold");
            AddFont(Resources, "Fonts/Venera");
            AddFont(Resources, "Fonts/AveriaSansLibre");

            // [!] how to use turn local images outside game directory to texture help plz
            var largeStore = new LargeTextureStore(Host.Renderer, Host.CreateTextureLoaderStore(new NamespacedResourceStore<byte[]>(Resources, "Textures")));
            largeStore.AddTextureSource(Host.CreateTextureLoaderStore(new OnlineStore()));

            Dependencies.Cache(largeStore);
            Storage storage = (Updater.DevelopmentBuild) ? new NativeStorage("dev_savedata") : Host.Storage;
            Dependencies.CacheAs(storage);
            Window.Title = "osu!alltrick";
            Dependencies.CacheAs<osuATGameBase>(this);
            SaveStorage.Init(storage);
            ScoreImporter.Init();
            Texture pfptxt = largeStore.Get($"http://a.ppy.sh/{SaveStorage.SaveData.PlayerID}");
            Console.WriteLine($"https://a.ppy.sh/{SaveStorage.SaveData.PlayerID}");
            Dependencies.CacheAs(pfptxt ?? largeStore.Get("avatar-guest"));
            FixTabletCursorDrift();
        }

        public void FixTabletCursorDrift()
        {
            foreach (var handler in Host.AvailableInputHandlers)
            {
                Console.WriteLine(handler);
                if (handler is ITabletHandler tabhandler)
                {
                    Schedule(() => tabhandler.Enabled.Value = false);
                }
                if (handler is MouseHandler mousehandle)
                {
                    mousehandle.UseRelativeMode.Value = false;
                    Console.WriteLine("----------------- " + mousehandle.UseRelativeMode.Value);
                }
            }
        }

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) =>
            Dependencies = new DependencyContainer(base.CreateChildDependencies(parent));
    }
}
