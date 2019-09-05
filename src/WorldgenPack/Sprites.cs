using System.IO;
using UnityEngine;

namespace Utilities
{
    class Sprites
    {
        // Thanks Mayall mod for this
        public static Sprite CreateSpriteDXT5(Stream inputStream, int width, int height)
        {
            byte[] array = new byte[inputStream.Length - 128L];
            inputStream.Seek(128L, SeekOrigin.Current);
            inputStream.Read(array, 0, array.Length);
            Texture2D texture2D = new Texture2D(width, height, TextureFormat.DXT5, false);
            texture2D.LoadRawTextureData(array);
            texture2D.Apply(false, true);
            return Sprite.Create(texture2D, new Rect(0f, 0f, (float)width, (float)height), new Vector2((float)(width / 2), (float)(height / 2)));
        }
    }
}