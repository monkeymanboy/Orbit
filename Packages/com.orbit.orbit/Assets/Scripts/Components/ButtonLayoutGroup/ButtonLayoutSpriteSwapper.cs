using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Orbit.Components.ButtonLayoutGroup {
    /// <summary>
    /// This component will create alternate sprites for a FullRect sprite in order to cut off edges while maintaining batching
    /// </summary>
    public class ButtonLayoutSpriteSwapper : MonoBehaviour, IButtonLayoutGroupElement {
        private class SpriteCache {
            public Sprite leftSprite;
            public Sprite horizontalCenterSprite;
            public Sprite rightSprite;
            public Sprite topSprite;
            public Sprite verticalCenterSprite;
            public Sprite bottomSprite;
        }

        private static Dictionary<Sprite, SpriteCache> modifiedSpriteCache = new();
        [SerializeField] private Image image;

        private Sprite singleSprite;
        private SpriteCache spriteCache;

        private void Awake() {
            singleSprite = image.sprite;
            if(!modifiedSpriteCache.TryGetValue(singleSprite, out spriteCache)) {
                spriteCache = new();
                Texture2D imageTexture = image.sprite.texture;
                Vector4 originalBorder = singleSprite.border;
                spriteCache.leftSprite = Sprite.Create(imageTexture,
                    new Rect(0, 0, imageTexture.width / 2, imageTexture.height),
                    new Vector2(0, 0.5f), singleSprite.pixelsPerUnit, 0, SpriteMeshType.FullRect,
                    new Vector4(originalBorder.x, originalBorder.y, 0, originalBorder.w), false);
                spriteCache.horizontalCenterSprite = Sprite.Create(imageTexture,
                    new Rect(imageTexture.width / 2, 0, 1, imageTexture.height),
                    new Vector2(0.5f, 0.5f), singleSprite.pixelsPerUnit, 0, SpriteMeshType.FullRect,
                    new Vector4(0, originalBorder.y, 0, originalBorder.w), false);
                spriteCache.rightSprite = Sprite.Create(imageTexture,
                    new Rect(imageTexture.width / 2, 0, imageTexture.width / 2, imageTexture.height),
                    new Vector2(0.5f, 0.5f), singleSprite.pixelsPerUnit, 0, SpriteMeshType.FullRect,
                    new Vector4(0, originalBorder.y, originalBorder.z, originalBorder.w), false);
                spriteCache.topSprite = Sprite.Create(imageTexture,
                    new Rect(0, 0, imageTexture.width, imageTexture.height / 2),
                    new Vector2(0.5f, 0), singleSprite.pixelsPerUnit, 0, SpriteMeshType.FullRect,
                    new Vector4(originalBorder.x, originalBorder.y, originalBorder.z), false);
                spriteCache.verticalCenterSprite = Sprite.Create(imageTexture,
                    new Rect(0, imageTexture.height / 2, imageTexture.width, 1),
                    new Vector2(0.5f, 0.5f), singleSprite.pixelsPerUnit, 0, SpriteMeshType.FullRect,
                    new Vector4(originalBorder.x, 0, originalBorder.z, 0), false);
                spriteCache.bottomSprite = Sprite.Create(imageTexture,
                    new Rect(0, imageTexture.height / 2, imageTexture.width, imageTexture.height/2),
                    new Vector2(0.5f, 0.5f), singleSprite.pixelsPerUnit, 0, SpriteMeshType.FullRect,
                    new Vector4(originalBorder.x, 0, originalBorder.z, originalBorder.w), false);
                modifiedSpriteCache[singleSprite] = spriteCache;
            }
        }

        public void SetTop() => image.sprite = spriteCache.topSprite;
        public void SetVerticalCenter() => image.sprite = spriteCache.verticalCenterSprite;
        public void SetBottom() => image.sprite = spriteCache.bottomSprite;
        public void SetLeft() => image.sprite = spriteCache.leftSprite;
        public void SetHorizontalCenter() => image.sprite = spriteCache.horizontalCenterSprite;
        public void SetRight() => image.sprite = spriteCache.rightSprite;
        public void SetSingle() => image.sprite = singleSprite;
    }
}
