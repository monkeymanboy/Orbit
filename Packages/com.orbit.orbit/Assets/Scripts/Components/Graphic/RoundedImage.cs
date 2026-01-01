using UnityEngine;

namespace Orbit.Components.Graphic
{
    public class RoundedImage : RoundedRect
    {
        [Header("Texture")]
        [SerializeField] protected Sprite sprite;
        [SerializeField] private bool preserveAspect;

        public Sprite Sprite
        {
            get => sprite;
            set
            {
                if (sprite == value)
                {
                    return;
                }

                sprite = value;
                SetVerticesDirty();
                SetMaterialDirty();
            }
        }
        public bool PreserveAspect
        {
            get => preserveAspect;
            set
            {
                if (preserveAspect == value)
                {
                    return;
                }

                preserveAspect = value;
                SetVerticesDirty();
            }
        }

        public override Texture mainTexture
        {
            get
            {
                if (sprite == null)
                {
                    if (material != null && material.mainTexture != null)
                    {
                        return material.mainTexture;
                    }
                    return s_WhiteTexture;
                }

                return sprite.texture;
            }
        }
        protected override void AdjustRect(ref Rect rect) {
            if (!PreserveAspect || Sprite == null) return;
            
            float spriteRatio = Sprite.rect.width / Sprite.rect.height;

            float currentWidth = rect.width;
            float currentHeight = rect.height;
            float currentRatio = currentWidth / currentHeight;

            if (currentRatio > spriteRatio) {
                float newWidth = currentHeight * spriteRatio;
                rect.x += (currentWidth - newWidth) * 0.5f;
                rect.width = newWidth;
            } else if (currentRatio < spriteRatio) {
                float newHeight = currentWidth / spriteRatio;
                rect.y += (currentHeight - newHeight) * 0.5f;
                rect.height = newHeight;
            }
        }

        protected override Vector2 GetUVForNormalizedPosition(Vector2 position) {
            if(!sprite)
                return position;
            
            Vector4 outerUV = UnityEngine.Sprites.DataUtility.GetOuterUV(sprite);
            return new Vector2(
                Mathf.Lerp(outerUV.x, outerUV.z, position.x),
                Mathf.Lerp(outerUV.y, outerUV.w, position.y)
            );
        }
    }
}
