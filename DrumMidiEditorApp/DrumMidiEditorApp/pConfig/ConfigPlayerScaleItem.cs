using System.Text.Json.Serialization;

namespace DrumMidiEditorApp.pConfig;

/// <summary>
/// 音階アイテム
/// </summary>
public class ConfigPlayerScaleItem
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ConfigPlayerScaleItem()
    {
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="aScaleKey">音階キー</param>
    /// <param name="aLabel">ラベル</param>
    /// <param name="aLineDrawFlag">ライン描画フラグ</param>
    public ConfigPlayerScaleItem( string aScaleKey, string aLabel, bool aLineDrawFlag )
    {
        ScaleKey        = aScaleKey;
        Label           = aLabel;
        LineDrawFlag    = aLineDrawFlag;
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="aScaleItem">コピー元</param>
    public ConfigPlayerScaleItem( ConfigPlayerScaleItem aScaleItem )
    {
        ScaleKey        = aScaleItem.ScaleKey;
        Label           = aScaleItem.Label;
        LineDrawFlag    = aScaleItem.LineDrawFlag;
    }

    #region member

    /// <summary>
    /// 音階キー
    /// </summary>
    [JsonInclude]
    public string ScaleKey { get; set; } = string.Empty;

    /// <summary>
    /// ラベル
    /// </summary>
    [JsonInclude]
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// ライン描画フラグ
    /// </summary>
    [JsonInclude]
    public bool LineDrawFlag { get; set; } = false;

    /// <summary>
    /// 音階キー＋ラベル
    /// </summary>
    [JsonIgnore]
    public string ScaleKeyLabel => $"{ScaleKey}-{Label}";

    #endregion
}
