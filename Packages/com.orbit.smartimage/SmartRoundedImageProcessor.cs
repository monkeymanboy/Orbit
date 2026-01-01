namespace Orbit.SmartImage {
    using ComponentProcessors;
    using Components.Graphic;
    using Cysharp.Threading.Tasks;
    using global::SmartImage;
    using System.Collections.Generic;
    using TypeSetters;

    public class SmartRoundedImageProcessor : ComponentProcessor<RoundedImage> {
        public override Dictionary<string, TypeSetter<RoundedImage>> Setters => new() {
            {"Source", new StringSetter<RoundedImage>((image, source) => LoadImage(image,source).Forget()) }
            
        };
        private async UniTaskVoid LoadImage(RoundedImage image, string source)
        {
            SmartSprite sprite = await OrbitSmartImageManager.Instance.LoadAsync(source);
            image.Sprite = sprite.Active.Sprite;

            void UpdateImageFrame(SmartSprite sprite, SmartFrame frame) {
                if(image == null) {
                    sprite.RemoveListener(UpdateImageFrame); //Handle case where image is destroyed
                    return;
                }
                image.Sprite = frame.Sprite;
            }

            sprite.AddListener(UpdateImageFrame);
        }
    }
}