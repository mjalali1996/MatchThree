using System;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Models
{
    [CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/Level")]
    public class Level : ScriptableObject
    {
        [SerializeField] private float _time;
        public float Time => _time;

        [SerializeField] private Board _board;
        public Board Board => _board;


        [CustomEditor(typeof(Level))]
        public class LevelEditor : Editor
        {
            private Level _level;
            private int _rows;
            private int _columns;

            private void Awake()
            {
                _level = target as Level;
            }

            public override void OnInspectorGUI()
            {
                _level._time = EditorGUILayout.FloatField("Time", _level._time);
                var board = _level._board;
                if (board != null)
                {
                    for (var i = 0; i < board.Rows.Count; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        var row = board.Rows[i];
                        for (var j = 0; j < row.Columns.Count; j++)
                        {
                            EditorGUILayout.BeginVertical();
                            var cell = row.Columns[j];
                            cell.StoneType = (StoneType)EditorGUILayout.EnumPopup(cell.StoneType);
                            EditorGUILayout.EndVertical();
                        }

                        EditorGUILayout.EndHorizontal();
                    }
                }


                EditorGUILayout.Separator();
                if (GUILayout.Button("Fill Board Randomly"))
                {
                    foreach (var row in board.Rows)
                    {
                        foreach (var cell in row.Columns)
                        {
                            cell.StoneType = (StoneType)Random.Range(1, Enum.GetValues(typeof(StoneType)).Length);
                        }
                    }
                }


                EditorGUILayout.Separator();
                EditorGUILayout.BeginHorizontal();
                _rows = EditorGUILayout.IntField("Rows", _rows);
                _columns = EditorGUILayout.IntField("Columns", _columns);
                EditorGUILayout.EndHorizontal();

                if (GUILayout.Button("Create Board"))
                {
                    _level._board = new Board();
                    _level._board.CreateBoard(_rows, _columns);
                }
            }
        }
    }
}