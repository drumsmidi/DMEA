using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using DMEL.pConfig;
using DMEL.pUtil;
using Microsoft.UI.Xaml;
using Windows.Foundation;
using Windows.UI;
using static DMEL.pConfig.ConfigSystem;

namespace DMEA.pConfig;

/// <summary>
/// プレイヤー設定
/// </summary>
public class ConfigPlayer : IConfig
{
    public void CheckValidation()
    {
        PlayerSurfaceModeSelect         = (PlayerSurfaceMode)Math.Clamp( (int)PlayerSurfaceModeSelect, 0, 2 );
        PlayerSurfaceEffectModeSelect   = (PlayerSurfaceEffectMode)Math.Clamp( (int)PlayerSurfaceEffectModeSelect, 0, 61 );
        Fps                             = Math.Clamp( Fps, 30, 999 );
        ResolutionScreenIndex           = Math.Clamp( ResolutionScreenIndex, 0, ResolutionScreenList.Count - 1 );

        Score.CheckValidation();
        Sequence.CheckValidation();
        Simuration.CheckValidation();
    }

    #region 更新フラグ

    /// <summary>
    /// スコア更新フラグ
    /// </summary>
    [JsonIgnore]
    public bool FlagUpdateScore { get; set; } = false;

    /// <summary>
    /// プレイヤー描画モード更新フラグ
    /// </summary>
    [JsonIgnore]
    public bool FlagUpdateSurfaceModo { get; set; } = false;

    /// <summary>
    /// サイズ更新フラグ
    /// </summary>
    [JsonIgnore]
    public bool FlagUpdateSize { get; set; } = false;

    #endregion

    #region プレイヤー描画モード

    /// <summary>
    /// Player表示フラグ
    /// </summary>
    [JsonIgnore]
    public bool DisplayPlayer { get; set; } = false;

    /// <summary>
    /// Player表示フラグ
    /// </summary>
    [JsonIgnore]
    public Visibility DisplayPlayerVisibility
    {
        get => DisplayPlayer ? Visibility.Visible : Visibility.Collapsed;
        set => DisplayPlayer = value == Visibility.Visible;
    }

    /// <summary>
    /// 編集モード
    /// </summary>
    [JsonIgnore]
    public bool EditModeOn { get; set; } = false;

    /// <summary>
    /// プレイヤー描画モード
    /// </summary>
    [JsonInclude]
    public PlayerSurfaceMode PlayerSurfaceModeSelect = PlayerSurfaceMode.Score;

    /// <summary>
    /// プレイヤー描画モード
    /// </summary>
    [JsonIgnore]
    public int PlayerSurfaceModeSelectIndex
    {
        get => (int)PlayerSurfaceModeSelect;
        set => PlayerSurfaceModeSelect = (PlayerSurfaceMode)value;
    }

    /// <summary>
    /// プレイヤー描画エフェクトモード
    /// </summary>
    public enum PlayerSurfaceEffectMode : int
    {
        AlphaMaskEffect = 0,
        ArithmeticCompositeEffect,
        AtlasEffect,
        BlendEffect,
        BorderEffect,
        BrightnessEffect,
        ChromaKeyEffect,
        ColorManagementEffect,
        ColorManagementProfile,
        ColorMatrixEffect,
        ColorSourceEffect,
        CompositeEffect,
        ContrastEffect,
        ConvolveMatrixEffect,
        CropEffect,
        CrossFadeEffect,
        DirectionalBlurEffect,
        DiscreteTransferEffect,
        DisplacementMapEffect,
        DistantDiffuseEffect,
        DistantSpecularEffect,
        DpiCompensationEffect,
        EdgeDetectionEffect,
        EffectTransferTable3D,
        EmbossEffect,
        ExposureEffect,
        GammaTransferEffect,
        GaussianBlurEffect,
        GrayscaleEffect,
        HighlightsAndShadowsEffect,
        HueRotationEffect,
        HueToRgbEffect,
        InvertEffect,
        LinearTransferEffect,
        LuminanceToAlphaEffect,
        MorphologyEffect,
        OpacityEffect,
        OpacityMetadataEffect,
        PixelShaderEffect,
        PointDiffuseEffect,
        PointSpecularEffect,
        PosterizeEffect,
        PremultiplyEffect,
        RgbToHueEffect,
        SaturationEffect,
        ScaleEffect,
        SepiaEffect,
        ShadowEffect,
        SharpenEffect,
        SpotDiffuseEffect,
        SpotSpecularEffect,
        StraightenEffect,
        TableTransfer3DEffect,
        TableTransferEffect,
        TemperatureAndTintEffect,
        TileEffect,
        TintEffect,
        Transform2DEffect,
        Transform3DEffect,
        TurbulenceEffect,
        UnPremultiplyEffect,
        VignetteEffect,
    }

    /// <summary>
    /// プレイヤー描画エフェクトモード
    /// </summary>
    [JsonInclude]
    public PlayerSurfaceEffectMode PlayerSurfaceEffectModeSelect = PlayerSurfaceEffectMode.AtlasEffect;

    /// <summary>
    /// プレイヤー描画エフェクトモード
    /// </summary>
    [JsonIgnore]
    public int PlayerSurfaceEffectModeSelectIndex
    {
        get => (int)PlayerSurfaceEffectModeSelect;
        set => PlayerSurfaceEffectModeSelect = (PlayerSurfaceEffectMode)value;
    }

    #endregion

    #region 解像度/FPS

    /// <summary>
    /// FPS
    /// </summary>
    [JsonInclude]
    public float Fps { get; set; } = 60F;

    /// <summary>
    /// 解像度リスト
    /// </summary>
    [JsonIgnore]
    public readonly List<Size> ResolutionScreenList =
    [
        new(  480,  360 ),
        new(  640,  480 ),
        new(  640,  720 ),
        new(  640, 1024 ),
    ];

    /// <summary>
    /// 解像度リスト選択インデックス
    /// </summary>
    [JsonInclude]
    public int ResolutionScreenIndex { get; set; } = 2;

    /// <summary>
    /// 解像度：横幅
    /// </summary>
    [JsonIgnore]
    public float ResolutionScreenWidth => ResolutionScreenList [ ResolutionScreenIndex ]._width;

    /// <summary>
    /// 解像度：高さ
    /// </summary>
    [JsonIgnore]
    public float ResolutionScreenHeight => ResolutionScreenList [ ResolutionScreenIndex ]._height;

    #endregion

    #region Sheet

    /// <summary>
    /// 背景色
    /// </summary>
    [JsonIgnore]
    public FormatColor SheetColor { get; set; } = new()
    {
        Color = Color.FromArgb( 255, 0, 0, 0 ),
    };

    #endregion

    #region 音階

    /// <summary>
    /// 音階リスト
    /// </summary>
    [JsonInclude]
    public List<ConfigPlayerScaleItem> ScaleList =
    [
        new ( "DUMMY", "", true    ),
        new ( "CY"   , "", false   ),
        new ( "RD"   , "", true    ),
        new ( "HH"   , "", false   ),
        new ( "SD"   , "", false   ),
        new ( "TM"   , "", false   ),
      //new ( "HT"   , "", true    ),
      //new ( "MT"   , "", false   ),
      //new ( "LT"   , "", false   ),
      //new ( "FT1"  , "", false   ),
      //new ( "FT2"  , "", true    ),
        new ( "BD"   , "", true    ),
        new ( "PC"   , "", false   ),
    ];

    /// <summary>
    /// 音階リスト更新
    /// </summary>
    public void UpdateScaleList( List<ConfigPlayerScaleItem> aScaleList )
    {
        lock ( ScaleList )
        {
            ScaleList.Clear();
            aScaleList.ForEach( item => ScaleList.Add( new( item ) ) );
        }
    }

    /// <summary>
    /// 音階リストのインデックス番号取得
    /// </summary>
    /// <param name="aScaleKey">[音階キー] ( 例: "CY" )</param>
    /// <param name="aScaleKeyText">[音階テキスト] ( 例: "1" )</param>
    /// <returns>階リストのインデックス番号</returns>
    public (int, string) GetScaleListIndex( string aScaleKey, string aScaleKeyText )
    {
        if ( aScaleKey.Length == 0 )
        {
            return ( -1, string.Empty );
        }

        var index = -1;

        lock ( ScaleList )
        {
            foreach ( var item in ScaleList )
            {
                index++;

                if ( item.ScaleKey.Equals( aScaleKey ) )
                {
                    return ( index, aScaleKeyText );
                }
            }
        }

        return ( -1, string.Empty );
    }

    #endregion

    #region 個別設定

    /// <summary>
    /// プレイヤー描画モード別設定
    /// </summary>
    [JsonInclude]
    public ConfigPlayerScore Score { get; set; } = new();

    /// <summary>
    /// プレイヤー描画モード別設定
    /// </summary>
    [JsonInclude]
    public ConfigPlayerSequence Sequence { get; set; } = new();

    /// <summary>
    /// プレイヤー描画モード別設定
    /// </summary>
    [JsonInclude]
    public ConfigPlayerSimuration Simuration { get; set; } = new();

    #endregion
}
