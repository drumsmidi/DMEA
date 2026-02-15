namespace DMEA.pResume.pItem;

/// <summary>
/// レジュームインターフェース
/// </summary>
public interface IResume
{
    /// <summary>
    /// Undo処理
    /// </summary>
    void Undo();

    /// <summary>
    /// Redo処理
    /// </summary>
    void Redo();
}
