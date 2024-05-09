using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace LevelGeneration
{
    /// <summary>
    /// Custom rule tile that allows for interaction with other tile types.
    /// </summary>
    [CreateAssetMenu(menuName = "2D/Tiles/Custom/ConnectRuleTile")]
    public class ConnectRuleTile : RuleTile<ConnectRuleTile.Neighbor>
    {
        public bool customField;

        /// <summary>
        /// Other tile type this tile will interact with.
        /// </summary>
        public TileBase[] connectTiles;

        /// <summary>
        /// Custom neighbor tile rules.
        /// </summary>
        public class Neighbor : RuleTile.TilingRule.Neighbor
        {
            /// <summary> Empty tile. </summary>
            public const int Null = 3;
            /// <summary> Any tile type even if they are not listed in ConnectRuleTile.connectTiles. </summary>
            public const int Any = 4;
            /// <summary> A tile type in ConnectRuleTile.connectTiles or this tile type. </summary>
            public const int Connect = 5;
            /// <summary> A tile type in ConnectRuleTile.connectTiles. </summary>
            public const int ConnectOnly = 6;
        }

        public override bool RuleMatch(int neighbor, TileBase tile)
        {
            switch (neighbor)
            {
                case Neighbor.Null: return tile == null;
                case Neighbor.Any: return tile != null;
                case Neighbor.Connect: return connectTiles.Contains(tile) || tile == this;
                case Neighbor.ConnectOnly: return connectTiles.Contains(tile);
            }
            return base.RuleMatch(neighbor, tile);
        }
    }
}
