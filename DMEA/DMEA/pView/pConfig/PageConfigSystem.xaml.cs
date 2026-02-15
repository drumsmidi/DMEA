using DMEA.pConfig;
using DMEL.pConfig;
using Microsoft.UI.Xaml.Controls;

namespace DMEA.pView.pConfig;

public sealed partial class PageConfigSystem : Page
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public PageConfigSystem()
    {
        InitializeComponent();
    }

    #region Member

    /// <summary>
    /// システム設定
    /// </summary>
    private ConfigSystem ConfigSystem => Config.System;

    /// <summary>
    /// ログ設定
    /// </summary>
    private ConfigLog ConfigLog => Config.Log;

    #endregion
}
