using System.Collections.Generic;
using DMEL.pIO.pDatabase.pRecord;
using DMEL.pModel.pSongList;
using DMEL.pUtil;
using LiteDB;

namespace DMEL.pIO.pDatabase;

/// <summary>
/// DB入出力
/// LiteDatabaseを使用
/// </summary>
public static class DBIO
{
    /// <summary>
    /// データベースファイルパス
    /// </summary>
    private static readonly string _DatabasePath = $"{HelperAppDirectory.AppBaseDirectory}./data.db";

    private static readonly string _TableSongList = "SongList";

    /// <summary>
    /// SongList登録
    /// </summary>
    public static void MeargeSongList( List<GeneralPath> aSongList )
    {
        DeleteAllSongList();

        using var conn = new LiteDatabase( _DatabasePath );

        // DBへ接続
        var table = conn.GetCollection<RecordDmsFile>( _TableSongList );

        aSongList.ForEach
        (
            song =>
            {
                var aRecordDmsFile = new RecordDmsFile()
                {
                    FilePath        = song.AbsoluteFilePath,
                    BaseFolderPath  = song.BaseFolderPath,
                };

                // 作成
                table.Upsert( aRecordDmsFile );
            }
        );
    }

    /// <summary>
    /// SongList取得
    /// </summary>
    public static SongList SelectSongList()
    {
        var songList = new SongList();

        using var conn = new LiteDatabase( _DatabasePath );

        // DBへ接続
        var table = conn.GetCollection<RecordDmsFile>( _TableSongList );

        var result = table.FindAll();

        foreach ( var rec in result )
        {
            songList.Add( new( rec.FilePath, rec.BaseFolderPath ) );
        }

        return songList;
    }

    /// <summary>
    /// SongList削除
    /// </summary>
    public static void DeleteAllSongList()
    {
        using var conn = new LiteDatabase( _DatabasePath );

        // DBへ接続
        var table = conn.GetCollection<RecordDmsFile>( "SongList" );
        table.DeleteAll();
    }
}
