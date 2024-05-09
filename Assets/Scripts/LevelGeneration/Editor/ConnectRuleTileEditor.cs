using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LevelGeneration.Editor
{
    /// <summary>
    /// ConnectRuleTile editor for custom rule icons.
    /// </summary>
    [CustomEditor(typeof(ConnectRuleTile), true), CanEditMultipleObjects]
    public class ConnectRuleTileEditor : RuleTileEditor
    {
        public Texture2D Null;
        public Texture2D Any;
        public Texture2D Connect;
        public Texture2D ConnectOny;

        public override void RuleOnGUI(Rect rect, Vector3Int position, int neighbor)
        {
            switch (neighbor)
            {
                case 3:
                    GUI.DrawTexture(rect, Null);
                    return;
                case 4:
                    GUI.DrawTexture(rect, Any);
                    return;
                case 5:
                    GUI.DrawTexture(rect, Connect);
                    return;
                case 6:
                    GUI.DrawTexture(rect, ConnectOny);
                    return;
            }
            base.RuleOnGUI(rect, position, neighbor);
        }
    }
}
