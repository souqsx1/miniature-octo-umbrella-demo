using System;
using UnityEngine;
using Object = UnityEngine.Object;

public partial class test_balloons
{
    private const string CustomCategoryIconEmbeddedFileName = "Media/SIDico.png";

    // Optional fallback: paste a Base64-encoded image here
    private const string CustomCategoryIconBase64 = @"";

    private Sprite TryLoadCustomCategoryIconFromEmbeddedFile()
    {
        byte[] imageData;
        try
        {
            imageData = TryLoadEmbeddedFileBytes(CustomCategoryIconEmbeddedFileName);
        }
        catch (Exception exception)
        {
            Debug.LogWarning("Failed to read embedded custom category icon \"" + CustomCategoryIconEmbeddedFileName + "\".\n" + exception);
            return null;
        }

        if (imageData == null || imageData.Length == 0)
        {
            return null;
        }

        return CreateSpriteFromImageBytes(imageData, "CustomCategoryIcon_EmbeddedTexture", "CustomCategoryIcon_EmbeddedSprite");
    }

    private static Sprite TryLoadCustomCategoryIconFromBase64()
    {
        if (string.IsNullOrEmpty(CustomCategoryIconBase64))
        {
            return null;
        }

        byte[] imageData;
        try
        {
            imageData = Convert.FromBase64String(CustomCategoryIconBase64);
        }
        catch (Exception exception)
        {
            Debug.LogWarning("Failed to decode CustomCategoryIconBase64.\n" + exception);
            return null;
        }

        return CreateSpriteFromImageBytes(imageData, "CustomCategoryIcon_Base64Texture", "CustomCategoryIcon_Base64Sprite");
    }

    private static Sprite CreateSpriteFromImageBytes(byte[] imageData, string textureName, string spriteName)
    {
        Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        if (!texture.LoadImage(imageData))
        {
            Object.Destroy(texture);
            Debug.LogWarning("Could not decode image data for \"" + spriteName + "\".");
            return null;
        }

        texture.name = textureName;

        Sprite sprite = Sprite.Create(
            texture,
            new Rect(0f, 0f, texture.width, texture.height),
            new Vector2(0.5f, 0.5f),
            100f);
        sprite.name = spriteName;

        return sprite;
    }
}