using UnityEngine;
using PhysicsMaster.UI;

namespace PhysicsMaster.Gameplay
{
    public static class Effects
    {
        private static Sprite particleSprite;

        private static Sprite ParticleSprite
        {
            get
            {
                if (particleSprite != null)
                {
                    return particleSprite;
                }

                Texture2D texture = Texture2D.whiteTexture;
                particleSprite = Sprite.Create(
                    texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f),
                    1f);
                return particleSprite;
            }
        }

        public static void Burst(Vector2 position, Color color, int count = 18)
        {
            GameObject root = new GameObject("Burst");
            root.transform.position = position;

            for (int i = 0; i < count; i++)
            {
                GameObject particle = new GameObject("Particle");
                particle.transform.SetParent(root.transform, false);
                particle.transform.localScale = Vector3.one * 0.08f;

                SpriteRenderer renderer = particle.AddComponent<SpriteRenderer>();
                renderer.sprite = ParticleSprite;
                renderer.color = color;

                Rigidbody2D body = particle.AddComponent<Rigidbody2D>();
                float angle = i * Mathf.PI * 2f / count;
                body.linearVelocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle))
                    * Random.Range(1.5f, 4f);
                body.gravityScale = 0.5f;

                UiFactory.SafeDestroy(particle, 1.2f);
            }

            UiFactory.SafeDestroy(root, 1.3f);
        }
    }
}
