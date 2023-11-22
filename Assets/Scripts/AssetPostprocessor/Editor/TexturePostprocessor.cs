using UnityEngine;
using UnityEditor;

public class TexturePostprocessor : AssetPostprocessor
{
    private void OnPreprocessTexture()
    {
        //TextureImporter importer = assetImporter as TextureImporter;
        //if (importer == null) return;

        //importer.textureShape = TexturePreprocessSettings.TextureShape;
        //importer.sRGBTexture = TexturePreprocessSettings.SRGBTexture;
        //importer.alphaSource = TexturePreprocessSettings.AlphaSource;
        //importer.alphaIsTransparency = 
        //    TexturePreprocessSettings.AlphaIsTransparency;
        //importer.ignorePngGamma = 
        //    TexturePreprocessSettings.IgnorePNGFileGamma;
        //importer.npotScale = TexturePreprocessSettings.NonPowerOf2;
        //importer.isReadable = TexturePreprocessSettings.ReadWriteEnabled;
        //importer.streamingMipmaps = 
        //    TexturePreprocessSettings.StreamingMipmaps;
        //importer.vtOnly = TexturePreprocessSettings.VitrualTextureOnly;
        //importer.mipmapEnabled = TexturePreprocessSettings.GenerateMipMaps;
        //importer.borderMipmap = TexturePreprocessSettings.BorderMipMaps;
        //importer.mipmapFilter = TexturePreprocessSettings.MipmapFilter;
        //importer.mipMapsPreserveCoverage = 
        //    TexturePreprocessSettings.MipMapsPreserveCoverage;
        //importer.fadeout = TexturePreprocessSettings.FadeoutMipMaps;
        //importer.wrapMode = TexturePreprocessSettings.WrapMode;
        //importer.filterMode = TexturePreprocessSettings.FilterMode;
        //importer.anisoLevel = TexturePreprocessSettings.AnisoLevel;
        //importer.maxTextureSize = TexturePreprocessSettings.MaxSize;
        //importer.textureCompression = 
        //    TexturePreprocessSettings.Compression;
        //importer.crunchedCompression = 
        //    TexturePreprocessSettings.UseCrunchCompression;
        //importer.textureType = TexturePreprocessSettings.TextureType;
    }

    private void OnPostprocessGameObjectWithUserProperties(GameObject gameObject, string[] propNames, object[] values)
    {
        
    }

    private void OnPostprocessMeshHierarchy(GameObject root)
    {
        
    }
}