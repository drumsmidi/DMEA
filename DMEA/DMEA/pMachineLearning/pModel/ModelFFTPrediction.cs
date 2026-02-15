using Microsoft.ML.Data;

namespace DMEA.pMachineLearning.pModel;

/// <summary>
/// ドラム予測
/// </summary>
internal class ModelFFTPrediction
{
    /// <summary>
    /// ドラムパターン予測結果
    /// </summary>
    [ColumnName( "PredictedLabel" )]
    public string DrumPattern { get; set; } = string.Empty;
}
