using DMEA.pView.pEditer;
using DMEA.pView.pMenuBar;
using DMEA.pView.pMidiMap;
using DMEA.pView.pML;
using DMEA.pView.pMusic;
using DMEA.pView.pPlayer;
using DMEA.pView.pScore;
using DMEA.pView.pStatusBar;

namespace DMEA.pView;

public static class ControlAccess
{
    public static WindowEditer? MainWindow { get; set; } = null;

    public static PageEditerMain? PageEditerMain { get; set; } = null;

    public static PageMenuBar? PageMenuBar { get; set; } = null;

    public static PageMusic? PageMusic { get; set; } = null;

    public static PageEdit? PageEdit { get; set; } = null;

    public static PageML? PageML { get; set; } = null;

    public static PageStatusBar? PageStatusBar { get; set; } = null;

    public static PagePlayer? PagePlayer { get; set; } = null;

    public static UserControlEqualizer? UCEqualizer { get; set; } = null;

    public static UserControlMidiMapPanel? UCMidiMapPanel { get; set; } = null;

    public static UserControlKeyChangePanel? UCKeyChangePanel { get; set; } = null;

    public static UserControlScore? UCScore { get; set; } = null;

    public static UserControlPlayerPanel? UCPlayerPanel { get; set; } = null;
}
