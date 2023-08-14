using System;
using System.Threading.Tasks;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using OsuApiHelper;
using osuAT.Game.API;
using osuTK;

namespace osuAT.Game.Objects
{
    public partial class PlayCheckButton : ClickableContainer
    {
        private SpriteIcon iconDisplay;
        private SpriteIcon iconDisplayShad;
        protected IconUsage ButtonIcon;
        protected int Cooldown = 4000;
        protected bool CheckingPlay = false;

        public PlayCheckButton()
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            Size = new Vector2(100, 100);
            ButtonIcon = FontAwesome.Solid.Crosshairs;
            Action = ButtonClicked;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Children = new Drawable[]
            {
                iconDisplayShad = new SpriteIcon {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Y = 5,
                    RelativeSizeAxes = Axes.Both,
                    Colour = Colour4.Black,
                    Icon = ButtonIcon,
                },
                iconDisplay = new SpriteIcon {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both,
                    Colour = Colour4.PeachPuff,
                    Icon = ButtonIcon,
                },
            };
        }

        protected async void ButtonClicked()
        {
            if (CheckingPlay) return;
            if (!SaveStorage.OsuPathIsValid() || OsuApiKey.Key == default || !(ApiScoreProcessor.ApiKeyValid))
            {
                OnCheckFailed();
                return;
            }

            CheckingPlay = true;

            iconDisplay.RotateTo(Rotation + 360, 1000, Easing.OutCubic);
            iconDisplayShad.RotateTo(Rotation + 360, 1000, Easing.OutCubic);
            await Task.Delay(1000);
            iconDisplay.Loop(b => b.RotateTo(0).RotateTo(360, 1000, Easing.InOutCubic));
            iconDisplayShad.Loop(b => b.RotateTo(0).RotateTo(360, 1000, Easing.InOutCubic));

            await CheckRecent();
            iconDisplay.RotateTo(0, 1000, Easing.OutSine);
            iconDisplayShad.RotateTo(0, 1000, Easing.OutSine);

            await Task.Delay(Cooldown);
            CheckingPlay = false;
        }

        protected void OnCheckFailed()
        {
            iconDisplay.FlashColour(Colour4.Red, 5000, Easing.Out);
        }

        protected void OnCheckSuccess()
        {
            iconDisplay.FlashColour(Colour4.LightGreen, 5000, Easing.Out);
        }

        protected async Task CheckRecent()
        {
            ApiScoreProcessor.ApiReqs += 1;
            var recent = OsuApi.GetUserRecent(SaveStorage.SaveData.PlayerID.ToString());
            if (recent == null)
            {
                OnCheckFailed();
                return;
            }

            var osuScore = recent[0];
            async Task<OsuApiBeatmap> mapRet() => await ApiScoreProcessor.OsuGetBeatmap(osuScore.MapID, osuScore.Mods, osuScore.Mode);

            ProcessResult result = await ApiScoreProcessor.SaveToStorageIfValid(osuScore, mapRet);
            Console.WriteLine(result);
            if (result == ProcessResult.Okay)
            {
                OnCheckSuccess();
                return;
            }
            OnCheckFailed();
        }

        protected override bool OnHover(HoverEvent e)
        {
            this.ScaleTo(1.07f, 100, Easing.Out);
            return true;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            this.ScaleTo(1f, 100, Easing.Out);
        }
    }
}
