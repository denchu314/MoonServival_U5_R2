using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 屋内空間(部屋)を表すデータクラス。
/// Flood Fillにより検出されたタイル群を保持し、酸素量などを管理する。
/// </summary>
public class RoomData
{
    public int roomID;                        // 一意の部屋ID (0,1,2…)
    public float oxygenLevel = 100f;          // この部屋の酸素量 (0~100% など想定)
    public List<Vector2Int> tilesInRoom;      // どのタイル座標がこの部屋に属するか

    // コンストラクタ
    public RoomData(int id)
    {
        roomID = id;
        tilesInRoom = new List<Vector2Int>();
    }
}
