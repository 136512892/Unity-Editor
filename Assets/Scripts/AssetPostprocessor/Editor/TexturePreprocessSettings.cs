using System.IO;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu]
public class TexturePreprocessSettings : ScriptableObject
{
    [SerializeField] private TextureImporterType textureType 
        = TextureImporterType.Default;
    [SerializeField] private TextureImporterShape textureShape 
        = TextureImporterShape.Texture2D;
    [SerializeField] private bool sRGBTexture = true;
    [SerializeField] private TextureImporterAlphaSource alphaSource 
        = TextureImporterAlphaSource.FromInput;
    [SerializeField] private bool alphaIsTransparency;
    [SerializeField] private bool ignorePNGFileGamma;

    [Header("Advanced")]
    [SerializeField] private TextureImporterNPOTScale nonPowerOf2
        = TextureImporterNPOTScale.ToNearest;
    [SerializeField] private bool readWriteEnabled;
    [SerializeField] private bool streamingMipmaps;
    [SerializeField] private bool vitrualTextureOnly;
    [SerializeField] private bool generateMipMaps = true;
    [SerializeField] private bool borderMipMaps;
    [SerializeField] private TextureImporterMipFilter mipmapFilter
        = TextureImporterMipFilter.BoxFilter;
    [SerializeField] private bool mipMapsPreserveCoverage;
    [SerializeField] private bool fadeoutMipMaps;

    [SerializeField] private TextureWrapMode wrapMode
        = TextureWrapMode.Repeat;
    [SerializeField] private FilterMode filterMode 
        = FilterMode.Bilinear;
    [SerializeField, Range(0, 16)] private int anisoLevel = 1;

    [SerializeField] private int maxSize = 2048;
    [SerializeField] private TextureImporterFormat format
        = TextureImporterFormat.Automatic;
    [SerializeField] private TextureImporterCompression compression
        = TextureImporterCompression.Compressed;
    [SerializeField] private bool useCrunchCompression;

    private static TexturePreprocessSettings m_Settings;
    private static TexturePreprocessSettings Settings
    {
        get
        {
            if (m_Settings == null)
            {
                var path = "Assets/Settings/" +
                    "Texture Preprocess Settings.asset";
                m_Settings = AssetDatabase
                    .LoadAssetAtPath<TexturePreprocessSettings>(path);
                if (m_Settings == null)
                {
                    m_Settings = CreateInstance<TexturePreprocessSettings>();
                    var directory = Application.dataPath + "/Settings";
                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(
                            Application.dataPath + "/Settings");
                    AssetDatabase.CreateAsset(m_Settings, path);
                    AssetDatabase.Refresh();
                }
            }
            return m_Settings;
        }
    }

    public static TextureImporterType TextureType { 
        get { return Settings.textureType; } }
    public static TextureImporterShape TextureShape { 
        get { return Settings.textureShape; } }
    public static bool SRGBTexture { 
        get { return Settings.sRGBTexture; } }
    public static TextureImporterAlphaSource AlphaSource { 
        get { return Settings.alphaSource; } }
    public static bool AlphaIsTransparency { 
        get { return Settings.alphaIsTransparency; } }
    public static bool IgnorePNGFileGamma { 
        get { return Settings.ignorePNGFileGamma; } }

    public static TextureImporterNPOTScale NonPowerOf2 { 
        get { return Settings.nonPowerOf2; } }
    public static bool ReadWriteEnabled { 
        get { return Settings.readWriteEnabled; } }
    public static bool StreamingMipmaps { 
        get { return Settings.streamingMipmaps; } }
    public static bool VitrualTextureOnly { 
        get { return Settings.vitrualTextureOnly; } }
    public static bool GenerateMipMaps { 
        get { return Settings.generateMipMaps; } }
    public static bool BorderMipMaps { 
        get { return Settings.borderMipMaps; } }
    public static TextureImporterMipFilter MipmapFilter { 
        get { return Settings.mipmapFilter; } }
    public static bool MipMapsPreserveCoverage { 
        get { return Settings.mipMapsPreserveCoverage; } }
    public static bool FadeoutMipMaps { 
        get { return Settings.fadeoutMipMaps; } }

    public static TextureWrapMode WrapMode { 
        get { return Settings.wrapMode; } }
    public static FilterMode FilterMode { 
        get { return Settings.filterMode; } }
    public static int AnisoLevel { 
        get { return Settings.anisoLevel; } }

    public static int MaxSize { 
        get { return Settings.maxSize; } }
    public static TextureImporterFormat Format { 
        get { return Settings.format; } }
    public static TextureImporterCompression Compression { 
        get { return Settings.compression; } }
    public static bool UseCrunchCompression { 
        get { return Settings.useCrunchCompression; } }
}