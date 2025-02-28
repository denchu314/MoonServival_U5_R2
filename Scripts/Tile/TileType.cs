/// <summary>
/// タイルが壁なのか空気なのかパイプなのかなどを区別するための列挙型
/// </summary>
public enum TileType
{
    Empty,    // 何もない
    Wall,     // 壁 (建材)
    Pipe,     // パイプ
    Other     // その他必要に応じて
}

