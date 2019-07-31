using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Importer : AssetPostprocessor
{
	void OnPreprocessTexture()
	{
		if (assetPath.StartsWith("Assets/GameLogic/UI"))
		{
			var importer = assetImporter as TextureImporter;
			if (importer != null)
			{
				SetSpriteImportSettings(importer);
			}
		}
		if (assetPath.StartsWith("Assets/Icons"))
		{
			var importer = assetImporter as TextureImporter;
			if (importer != null)
			{
				SetSpriteImportSettings(importer);
			}
		}
	}

	void SetSpriteImportSettings(TextureImporter importer)
	{
		var settings = new TextureImporterSettings();
		importer.textureCompression = TextureImporterCompression.Uncompressed;
		importer.ReadTextureSettings(settings);
		settings.mipmapEnabled = false;
		settings.streamingMipmaps = false;
		settings.spriteMeshType = SpriteMeshType.FullRect;
		settings.wrapMode = TextureWrapMode.Repeat;
		settings.textureType = TextureImporterType.Sprite;
		settings.filterMode = FilterMode.Bilinear;
		settings.readable = false;
		settings.npotScale = TextureImporterNPOTScale.None;
		settings.textureType = TextureImporterType.Sprite;
		settings.spriteMode = 1; // Single
		settings.sRGBTexture = true;
		settings.alphaSource = TextureImporterAlphaSource.FromInput;
		settings.alphaIsTransparency = true;
		settings.spriteExtrude = 1;
		settings.spritePixelsPerUnit = 100f;
		importer.SetTextureSettings(settings);
	}
}
