
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AI
{
    /// <summary>
    /// Object that spawns AI in patrol areas.
    /// </summary>
    public class AISpawner : MonoBehaviour
    {
        #region Types
        /// <summary>
        /// An patrol area and the AI's that will spawn in it.
        /// </summary>
        [System.Serializable]
        public struct Area
        {
            public int fighterCount;
            public int archerCount;
            public int mageCount;
            public int blobCount;
            public AIBrain.PatrolArea area;

            public static implicit operator AIBrain.PatrolArea(Area a) => a.area;
        }
        #endregion

        #region Properties
        /// <summary> Assigned Areas. </summary>
        public Area[] areas;
        #endregion

        #region Unity Callbacks
        private void Start()
        {
            foreach (Area area in areas)
            {
                //Fighter
                for (int f = 0; f < area.fighterCount; f++)
                { GameManager.instance.SpawnCharacter(area.area.GetRandomInArea(), (int)CharacterType.fighter, false, area); }
                //Archer
                for (int a = 0; a < area.archerCount; a++)
                { GameManager.instance.SpawnCharacter(area.area.GetRandomInArea(), (int)CharacterType.archer, false, area); }
                //Mage
                for (int m = 0; m < area.mageCount; m++)
                { GameManager.instance.SpawnCharacter(area.area.GetRandomInArea(), (int)CharacterType.mage, false, area); }
                //Blob
                for (int b = 0; b < area.blobCount; b++)
                { GameManager.instance.SpawnCharacter(area.area.GetRandomInArea(), (int)CharacterType.blob, false, area); }
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            GUIStyle newStyle = new GUIStyle();
            newStyle.normal.textColor = Color.green;
            for (int i = 0; i < areas.Length; i++)
            {
                //Draw area
                if (areas[i].area.useCircle)
                {
                    Gizmos.DrawWireSphere(areas[i].area.center, areas[i].area.radius);
                    //Draw index number
                    Handles.Label(areas[i].area.center - new Vector3(1, 0, -1.5f), i.ToString(), newStyle);
                }
                else
                {
                    Gizmos.DrawWireCube(areas[i].area.center, areas[i].area.rect);
                    //Draw index number
                    float xPos = (areas[i].area.rect.x >= 2) ? (areas[i].area.rect.x / 2) - 0.5f : (areas[i].area.rect.x / 2);
                    float yPos = (areas[i].area.rect.y >= 2) ? -(areas[i].area.rect.y / 2) + 0.5f : -(areas[i].area.rect.y / 2);
                    Handles.Label(areas[i].area.center - new Vector3(xPos, 0, yPos), i.ToString(), newStyle);
                }

            }
        }
#endif
        #endregion
    }
}
