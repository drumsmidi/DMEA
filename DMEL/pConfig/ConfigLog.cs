using System.Text.Json.Serialization;
using DMEL.pLog;
using DMEL.pUtil;

namespace DMEL.pConfig;

/// <summary>
/// Log設定
/// </summary>
public class ConfigLog : IConfig
{
    public void CheckValidation()
    {
        // 何もしない
    }

    /// <summary>
    /// トレースログパス
    /// </summary>
    [JsonIgnore]
    public GeneralPath TraceLogPath { get; private set; } = new( $"{HelperAppDirectory.LocalFolder.Path}\\TraceLog.log" );

    /// <summary>
    /// トレースログ設定
    /// </summary>
    public void SetTraceLog() 
        => Log.SetLogFile( TraceLogPath );

    /// <summary>
    /// アプリ終了時にトレースログファイルを開く
    /// </summary>
    [JsonInclude]
    public bool OpenTraceLogFileWhenAppQuit { get; set; } = false;
}
