using UnityEngine;

namespace Orbit.Components.Graphic
{
    public class RoundedRawImage : RoundedRect
    {
        [Header("Texture")]
        [SerializeField] protected Texture texture;
        [SerializeField] protected Rect uvRect = new Rect(0, 0, 1, 1);

        public Texture Texture
        {
            get => texture;
            set
            {
                if (texture == value)
                {
                    return;
                }

                texture = value;
                SetVerticesDirty();
                SetMaterialDirty();
            }
        }

        public Rect UVRect
        {
            get => uvRect;
            set
            {
                if (uvRect == value)
                {
                    return;
                }

                uvRect = value;
                SetVerticesDirty();
            }
        }

        public override Texture mainTexture
        {
            get
            {
                if (texture == null)
                {
                    if (material != null && material.mainTexture != null)
                    {
                        return material.mainTexture;
                    }
                    return s_WhiteTexture;
                }

                return texture;
            }
        }

        protected override Vector2 GetUVForNormalizedPosition(Vector2 position) => Rect.NormalizedToPoint(uvRect, position);
    }
}
