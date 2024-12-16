using System;
using UnityEditor;
using UnityEngine;

[Serializable]
public class LevelData
{
    public string level_tag;
    public string scene;
    // public SceneAsset scene; <- for debug
}

[CreateAssetMenu(fileName = "Level Data Collection", menuName = "Levels/Level Data Collection", order = 0)]
public class LevelCollection : ScriptableObject
{
    public LevelData[] Levels;
}
