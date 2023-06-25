using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AutoPlayer.Utils
{
    public class LevelUtils
    {

        public static Vector2 GetCellSize()
        {
            return Vector2.one;
        }

        public static Vector2 GetWorldXYFromBeatmapCoords(int x, int y)
        {
            Vector2 cellSize = GetCellSize();
            return new Vector2(((x - 2) * cellSize.x) + 0.5f, (y * cellSize.y) + 0.5f);
        }

    }
}
