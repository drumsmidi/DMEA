using System;
using System.Collections.Generic;
using System.Numerics;
using DrumMidiEditorApp.pConfig;
using DrumMidiLibrary.pAudio;
using DrumMidiLibrary.pControl;
using DrumMidiLibrary.pUtil;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Windows.Foundation;

namespace DrumMidiEditorApp.pView.pPlayer.pSurface.pSequence;

/// <summary>
/// プレイヤーサーフェイス
/// </summary>
public class PlayerSurface : PlayerSurfaceBase
{
    #region Member

    /// <summary>
    /// プレイヤー設定
    /// </summary>
    private static ConfigPlayerSequence DrawSet => Config.Player.Sequence;

    /// <summary>
    /// セクション範囲
    /// </summary>
	private Rect _SectionRange = new();

    /// <summary>
    /// 小節番号行ヘッダ範囲
    /// </summary>
    private Rect _MeasureNoHeadRange = new();

    /// <summary>
    /// 小節番号行ボディ範囲
    /// </summary>
	private Rect _MeasureNoBodyRange = new();

    /// <summary>
    /// ノート行ヘッダ範囲
    /// </summary>
    private Rect _ScoreHeadRange = new();

    /// <summary>
    /// ノート行ボディ範囲
    /// </summary>
    private Rect _ScoreBodyRange = new();

    /// <summary>
    /// 現在のBPM表示範囲
    /// </summary>
	private Rect _NowBpmRange = new();

    /// <summary>
    /// 小節分割線リスト
    /// </summary>
    private readonly List<ItemLine> _MeasureLineList = [];

    /// <summary>
    /// MidiMapGroupヘッダリスト（MidiMapGroupキー、横線描画アイテム）
    /// </summary>
    private readonly Dictionary<string, ItemLine> _HeaderList = [];

    /// <summary>
    /// NOTEリスト（小節番号、NOTE描画アイテム）
    /// </summary>
    private readonly Dictionary<int,List<ItemNote>> _NoteList = [];

    /// <summary>
    /// ノート背景色リスト＜MidiMapKey、背景色＞
    /// </summary>
    private readonly Dictionary<int, FormatRect> _MidiMapNoteFormatList = [];

    /// <summary>
    /// 小節番号
    /// </summary>
    private ItemMeasure? _MeasureNo = null;

    /// <summary>
    /// 現在のBPM
    /// </summary>
    private ItemLabel? _NowBpm = null;

    #endregion

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public PlayerSurface() : base() { }

    public override bool OnMove( double aFrameTime ) => base.OnMove( aFrameTime );

    protected override void UpdateScore()
    {
        base.UpdateScore();

        // screen
        if ( DrawSet.DrawDirectionModeSelect == ConfigPlayerSequence.DrawDirectionMode.Vertical )
        {
            var s = _ScreenSize;

            _ScreenSize.Height = s.Width;
            _ScreenSize.Width  = s.Height;
        }


        // bpm
        _NowBpmRange.X              = _ScreenSize.Width - 94;
        _NowBpmRange.Y              = 0;
        _NowBpmRange.Width          = DrawSet.BpmWidthSize;
        _NowBpmRange.Height         = DrawSet.BpmHeightSize;

        // measure no body
        _MeasureNoBodyRange.X       = DrawSet.HeaderWidthSize;
        _MeasureNoBodyRange.Y       = _NowBpmRange.Bottom;
        _MeasureNoBodyRange.Width   = (int)( ( _ScreenSize.Width - DrawSet.HeaderWidthSize ) / DrawSet.MeasureSize ) * DrawSet.MeasureSize;
        _MeasureNoBodyRange.Height  = DrawSet.MeasureNoHeightSize;

        // score body
        _ScoreBodyRange.X           = _MeasureNoBodyRange.Left;
        _ScoreBodyRange.Y           = _MeasureNoBodyRange.Bottom;
        _ScoreBodyRange.Width       = _MeasureNoBodyRange.Width;
        _ScoreBodyRange.Height      = DrawSet.ScoreMaxHeight;

        // 1小節分の範囲
        _SectionRange.X             = _MeasureNoBodyRange.X;
        _SectionRange.Y             = _MeasureNoBodyRange.Y;
        _SectionRange.Width         = _ScoreBodyRange.Width;
        _SectionRange.Height        = _ScoreBodyRange.Bottom - _MeasureNoBodyRange.Top;
    }

    protected override void UpdateScoreLine()
    {
        base.UpdateScoreLine();

        var body            = _ScoreBodyRange;
        var measure_size    = DrawSet.MeasureSize;

        #region Measure line
        {
            var pens = new FormatLine[]
                {
                    DrawSet.SheetMeasure001Line,
                    DrawSet.SheetMeasure004Line,
                    DrawSet.SheetMeasure008Line,
                    DrawSet.SheetMeasure016Line,
                    DrawSet.SheetMeasure032Line,
                    DrawSet.SheetMeasure064Line,
                    DrawSet.SheetMeasure128Line,
                };

            _MeasureLineList.Clear();

            var linesize =  pens[ 0 ].LineSize == 0 ? 0 : 1 ;

            if ( linesize == 0 )
            {
                for ( var i = 1; i < pens.Length; i++ )
                {
                    if ( pens [ i ].LineSize != 0 )
                    {
                        linesize = (int)Math.Pow( 2, i + 1 );
                        break;
                    }
                }
            }

            if ( linesize != 0 )
            {
                FormatLine? pen = null;

                var note_num   = ConfigSystem.MeasureNoteNumber;
                var note_width = DrawSet.NoteTermWidthSize;

                for ( var i = 0; i < note_num; i += linesize )
                {
                    for ( int x = 6, y = 1; x >= 0; x--, y *= 2 )
                    {
                        pen = pens [ x ];

                        if ( i % ( note_num / y ) == 0 && ( pen.LineSize != 0 ) )
                        {
                            break;
                        }
                    }

                    if ( pen == null )
                    {
                        continue;
                    }

                    _MeasureLineList.Add
                        (
                            new
                            (
                                (float)( body.X + ( note_width * i ) ),
                                body._y,
                                0,
                                body._height,
                                pen
                             )
                        );

                    pen = null;
                }
            }
        }
        #endregion

        #region MeasureNo
        {
            _MeasureNo = new
                (
                    _MeasureNoBodyRange._x,
                    _MeasureNoBodyRange._y,
                    measure_size,
                    _MeasureNoBodyRange._height,
                    DrawSet.MeasureNoRect
                );
        }
        #endregion
    }

    protected override void UpdateScoreHeader()
    {
        base.UpdateScoreHeader();

        _HeaderList.Clear();

        var body = _ScoreBodyRange;

        var x = body.X;
        var y = body.Y;
        var w = body.Width;
        var h = DrawSet.NoteTermHeightSize;

        #region MidiMapGroup
        lock ( DrawSet.ScaleList )
        {
            var cnt = 0;

            foreach ( var item in DrawSet.ScaleList )
            {
                if ( item != null )
                {
                    var obj = new ItemLine
                        (
                            (float)x,
                            (float)y,
                            (float)w,
                            0,
                            item.LineDrawFlag ? DrawSet.HeaderLineA : DrawSet.HeaderLineB
                        );

                    _HeaderList.TryAdd( item.ScaleKey, obj );
                }

                y += h;
                cnt++;
            }
        }
        #endregion

        #region MidiMap
        {
            foreach ( var midiMap in Score.EditMidiMapSet.DisplayMidiMaps )
            {
                // ノート描画用の書式を登録
                if ( !_MidiMapNoteFormatList.TryGetValue( midiMap.MidiMapKey, out var value ) )
                {
                    // TODO: 線の色とか情報追加が必要
                    var formatRect = new FormatRect
                    {
                        Background  = new( midiMap.Color ),
                        Text        = DrawSet.NoteRect.Text,
                    };

                    _MidiMapNoteFormatList.Add( midiMap.MidiMapKey, formatRect );
                }
                else
                {
                    value.Background = new( midiMap.Color );
                }
            }
        }
        #endregion

        #region Bpm now
        {
            _NowBpm = new ItemLabel
                (
                    _NowBpmRange._x,
                    _NowBpmRange._y,
                    _NowBpmRange._width,
                    _NowBpmRange._height,
                    string.Empty,
                    DrawSet.BpmNowRect
                );
        }
        #endregion
    }

    protected override void ClearMeasure()
    {
        base.ClearMeasure();

        foreach ( var nList in _NoteList )
        {
            nList.Value.Clear();
        }
        _NoteList.Clear();
    }

    protected override void UpdateScoreMeasure( int aMeasureNo )
    {
        base.UpdateScoreMeasure( aMeasureNo );

        #region Clear note
        {
            if ( _NoteList.TryGetValue( aMeasureNo, out var nList ) )
            {
                nList.Clear();
                _ = _NoteList.Remove( aMeasureNo );
            }
        }
        #endregion

        var measure = Score.EditChannel.GetMeasure( aMeasureNo );

        if ( measure == null )
        {
            return;
        }

        var body_s    = _ScoreBodyRange;
        var note_rect = new Rect( 0, 0, DrawSet.NoteWidthSize, DrawSet.NoteHeightSize );

        #region Set note
        {
            foreach ( var midiMap in Score.EditMidiMapSet.DisplayMidiMaps )
            {
                if ( midiMap.Group == null )
                {
                    continue;
                }

                if ( !measure.NoteLines.TryGetValue( midiMap.MidiMapKey, out var measure_line ) )
                {
                    continue;
                }

                var item = DrawSet.GetScaleListIndex( midiMap.Group.ScaleKey, midiMap.ScaleKeyText );

                if ( item.Item1 == -1 )
                {
                    continue;
                }

                foreach ( var info in measure_line.InfoStates.Values )
                {
                    if ( !info.NoteOn )
                    {
                        continue;
                    }

                    var volume = 1F;
                    if ( DrawSet.NoteVolumeSizeOn )
                    {
                        volume = ( info.Volume + midiMap.VolumeAddIncludeGroup ) / (float)MidiNet.MidiMaxVolume;

                        if ( volume > 1F )
                        {
                            volume = 1F;
                        }
                        else if ( volume < 0F )
                        {
                            volume = 0F;
                        }
                    }
                    if ( DrawSet.NoteVolumeZeroOn && volume <= 0F )
                    {
                        volume = 1F;
                    }

                    note_rect.Width     = DrawSet.NoteWidthSize  * volume;
                    note_rect.Height    = DrawSet.NoteHeightSize * volume;
                    note_rect.X         = body_s.X + ( info.NotePos * DrawSet.NoteTermWidthSize ); //- ( volume * DrawSet.NoteWidthSize / 2.0F );
                    note_rect.Y         = body_s.Y + ( item.Item1 * DrawSet.NoteTermHeightSize ) 
                                        + ( ( DrawSet.NoteTermHeightSize - note_rect.Height ) / 2.0F );

                    if ( volume != 0F )
                    {
                        if ( note_rect.Width <= 0F )
                        {
                            note_rect.Width = 0.1F;
                        }
                        if ( note_rect.Height <= 0F )
                        {
                            note_rect.Height = 0.1F;
                        }
                    }

                    var obj = new ItemNote
                        (
                            note_rect._x,
                            note_rect._y,
                            note_rect._width,
                            note_rect._height,
                            //DrawSet.NoteRect,
                            _MidiMapNoteFormatList[ midiMap.MidiMapKey ],
                            item.Item2
                        );

                    if ( !_NoteList.TryGetValue( aMeasureNo, out var lst ) )
                    {
                        lst = [];
                    }
                    lst.Add( obj );

                    _NoteList [ aMeasureNo ] = lst;
                }
            }
        }
        #endregion

        #region Sort note
        {
            if ( _NoteList.TryGetValue( aMeasureNo, out var lst ) )
            {
                lst.Sort();
            }
        }
        #endregion
    }

    public override bool OnDraw( CanvasDrawEventArgs args )
    {
        // 背景色
        args.DrawingSession.Clear( DrawSet.SheetColor.Color );

        if ( !base.OnDraw( args ) )
        {
            return false;
        }

        // 描画回転
        if ( DrawSet.DrawDirectionModeSelect == ConfigPlayerSequence.DrawDirectionMode.Vertical )
        {
            args.DrawingSession.Transform =
                Matrix3x2.CreateTranslation( -_ScreenSize._height, 0 ) *
                Matrix3x2.CreateRotation( (float)( Math.PI * -90 / 180.0 ) ) *
                Matrix3x2.CreateTranslation( 0, _ScreenSize._width - _ScreenSize._height );
        }

        //var section = _SectionRange;

        //if ( section.Width <= 0 || section.Height <= 0 )
        //{
        //    return true;
        //}

        var note_text_flag  = DrawSet.NoteTextOn;

        var head            = _ScoreHeadRange;
        var body            = _ScoreBodyRange;
        var note_pos        = _NotePositionX;
        var sheet_pos_x     = (float)Math.Round( _SheetPosX * DrawSet.NoteTermWidthSize, 0 );
        var measure_size    = DrawSet.MeasureSize;
        var measure_start   = (int)( ( note_pos - (head.Width / DrawSet.NoteTermWidthSize) ) / ConfigSystem.MeasureNoteNumber ) - 1;
        var measure_end     = (int)( ( note_pos + (body.Width / DrawSet.NoteTermWidthSize) ) / ConfigSystem.MeasureNoteNumber ) + 1;

        if ( measure_start < 0 )
        {
            measure_start = 0;
        }
        if ( measure_end > ConfigSystem.MeasureMaxNumber )
        {
            measure_end = ConfigSystem.MeasureMaxNumber;
        }

        #region Paint measure line
        {
            int     cnt;
            float   diff_x;

            for ( var measure_no = measure_start; measure_no <= measure_end; measure_no++ )
            {
                diff_x  = ( measure_size * measure_no ) - sheet_pos_x;
                cnt     = _MeasureLineList.Count;

                for ( var index = 0; index < cnt; index++ )
                {
                    _MeasureLineList [ index ].Draw( args.DrawingSession, diff_x, 0 );
                }
            }
        }
        #endregion

        #region Paint note
        {
            float diff_x;

            for ( var measure_no = measure_start; measure_no <= measure_end; measure_no++ )
            {
                if ( _NoteList.TryGetValue( measure_no, out var notes ) )
                {
                    diff_x = ( measure_size * measure_no ) - sheet_pos_x;

                    foreach ( var note in notes )
                    {
                        note.Draw( args.DrawingSession, diff_x, 0, note_text_flag );
                    }
                }
            }
        }
        #endregion

        #region Paint measure number
        {
            #region Paint body
            {
                float diff_x;

                for ( var measure_no = measure_start; measure_no <= measure_end; measure_no++ )
                {
                    diff_x = ( measure_size * measure_no ) - sheet_pos_x;

                    _MeasureNo?.Draw( args.DrawingSession, measure_no, diff_x, 0 );
                }
            }
            #endregion
        }
        #endregion

        #region Paint now bpm
        {
            if ( DrawSet.BpmNowDisplay && _NowBpm != null )
            {
                _NowBpm.Text = string.Format( "Bpm:{0, 6:##0.00}", DmsControl.GetBpm( _NotePositionX ) );
                _NowBpm.Draw( args.DrawingSession );
            }
        }
        #endregion

        return true;
    }
}
