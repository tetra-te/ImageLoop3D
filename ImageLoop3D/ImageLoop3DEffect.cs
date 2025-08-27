using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin.Effects;

namespace ImageLoop3D
{
    [VideoEffect("3D画像ループ", ["配置"], ["3d image loop", "敷き詰め"], isAviUtlSupported: false, isEffectItemSupported: false)]
    internal class ImageLoop3DEffect : VideoEffectBase
    {
        public override string Label => "3D画像ループ";

        [Display(GroupName = "3D画像ループ", Name = "個数", Description = "個数")]
        [AnimationSlider("F0", "", 1, 5)]
        public Animation Count { get; } = new Animation(1, 1, 999);

        [Display(GroupName = "3D画像ループ", Name = "間隔X", Description = "間隔X")]
        [AnimationSlider("F1", "px", -100, 100)]
        public Animation X { get; } = new Animation(0, -99999, 99999);

        [Display(GroupName = "3D画像ループ", Name = "間隔Y", Description = "間隔Y")]
        [AnimationSlider("F1", "px", -100, 100)]
        public Animation Y { get; } = new Animation(0, -99999, 99999);

        [Display(GroupName = "3D画像ループ", Name = "間隔Z", Description = "間隔Z")]
        [AnimationSlider("F1", "px", -100, 100)]
        public Animation Z { get; } = new Animation(0, -99999, 99999);

        public override IEnumerable<string> CreateExoVideoFilters(int keyFrameIndex, ExoOutputDescription exoOutputDescription)
        {
            return [];
        }

        public override IVideoEffectProcessor CreateVideoEffect(IGraphicsDevicesAndContext devices)
        {
            return new ImageLoop3DEffectProcessor(devices, this);
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [Count, X, Y, Z];
    }
}
