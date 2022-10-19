using System;
using System.Threading.Tasks;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Screens;
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics.Containers;
using osu.Framework.IO.Stores;
using osuTK;
using osuAT.Resources;
using osuTK.Graphics.ES30;

namespace osuAT.Game
{
    public class osuATGameBase : osu.Framework.Game
    {
        // Anything in this class is shared between the test browser and the game implementation.
        // It allows for caching global dependencies that should be accessible to tests, or changing
        // the screen scaling for all components including the test browser and framework overlays.
        protected override Container<Drawable> Content { get; }
        private DependencyContainer dependencies;

        protected osuATGameBase()
        {
            // Ensure game and tests scale with window size and screen DPI.
            base.Content.Add(Content = new DrawSizePreservingFillContainer
            {
                // You may want to change TargetDrawSize to your "default" resolution, which will decide how things scale and position when using absolute coordinates.
                TargetDrawSize = new Vector2(1312, 738)
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
            Resources.AddStore(new DllResourceStore(typeof(osuATResources).Assembly));
            AddFont(Resources, @"Fonts/osuFont");
            AddFont(Resources, @"Fonts/VarelaRound");
            AddFont(Resources, @"Fonts/ChivoBold");
            AddFont(Resources, @"Fonts/Venera");
            AddFont(Resources, @"Fonts/AveriaSansLibre");

            var largeStore = new LargeTextureStore(Host.Renderer, Host.CreateTextureLoaderStore(new NamespacedResourceStore<byte[]>(Resources, @"Textures")));
            largeStore.AddTextureSource(Host.CreateTextureLoaderStore(new OnlineStore()));
            dependencies.Cache(largeStore);

            dependencies.CacheAs(this);

            this.Window.Title = "osu!alltrick";
            ScoreImporter.Init();
        }
            
        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) =>
            dependencies = new DependencyContainer(base.CreateChildDependencies(parent));
    }
}
