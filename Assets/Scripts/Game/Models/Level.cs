using System;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Models
{
    [CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/Level")]
    public class Level : ScriptableObject
    {
        [SerializeField] private int _time;
        public int Time => _time;

        [SerializeField] private Board _board;

        public Board GetBoard()
        {
            var dupBoard = new Board();
            dupBoard.CreateBoard(_board.RowsCount, _board.ColumnsCount);
            for (var i = 0; i < _board.RowsCount; i++)
            {
                for (var j = 0; j < _board.ColumnsCount; j++)
                {
                    var dupCell = new BoardCell
                    {
                        StoneType = _board.Rows[i].Columns[j].StoneType
                    };
                    dupBoard.Rows[i].Columns[j] = dupCell;
                }
            }

            return dupBoard;
        }

        [CustomEditor(typeof(Level))]
        public class LevelEditor : Editor
        {
            private Level _level;
            private int _rows;
            private int _columns;

            private void Awake()
            {
                _level = serializedObject.targetObject as Level;
            }

            public override void OnInspectorGUI()
            {
                var time = EditorGUILayout.IntField("Time", _level._time);

                if (time != _level._time)
                {
                    _level._time = time;
                    SaveData();
                }
                
                var board = _level._board;
                if (board != null)
                {
                    EditorGUILayout.BeginHorizontal();
                    for (var i = 0; i < board.Rows.Count; i++)
                    {
                        var row = board.Rows[i];
                        EditorGUILayout.BeginVertical();
                        for (var j = 0; j < row.Columns.Count; j++)
                        {
                            var cell = row.Columns[j];
                            var newValue = (StoneType)EditorGUILayout.EnumPopup(cell.StoneType);
                            if (newValue != cell.StoneType)
                            {
                                cell.StoneType = newValue;
                                SaveData();
                            }
                        }

                        EditorGUILayout.EndVertical();
                    }

                    EditorGUILayout.EndHorizontal();
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
                    
                    SaveData();
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
                    SaveData();
                }
            }

            private void SaveData()
            {
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }
}