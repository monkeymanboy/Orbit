namespace Orbit.SmartImage {
    using ComponentProcessors;
    using Cysharp.Threading.Tasks;
    using global::SmartImage;
    using System.Collections.Generic;
    using TypeSetters;
    using UnityEngine.UI;

    public class SmartImageProcessor : ComponentProcessor<Image> {
        public override Dictionary<string, TypeSetter<Image>> Setters => new() {
            {"Source", new StringSetter<Image>((image, source) => LoadImage(image,source).Forget()) }
            
        };
        private async UniTaskVoid LoadImage(Image image, string source)
        {
            SmartSprite sprite = await OrbitSmartImageManager.Instance.LoadAsync(source);
            image.sprite = sprite.Active.Sprite;

            void UpdateImageFrame(SmartSprite sprite, SmartFrame frame) {
                if(image == null) {
                    sprite.RemoveListener(UpdateImageFrame); //Handle case where image is destroyed
                    return;
                }
                image.sprite = frame.Sprite;
            }

            sprite.AddListener(UpdateImageFrame);
        }
    }
}