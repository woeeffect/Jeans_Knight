using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OpalStudio.CustomToolbar.Editor.ToolbarElements.Favorites.Data
{
      public sealed class FavoritesManager : ScriptableObject
      {
            public List<FavoriteList> allLists = new();
            public int lastUsedListIndex;

            private static FavoritesManager _instance;
            public static FavoritesManager Instance
            {
                  get
                  {
                        if (_instance)
                        {
                              return _instance;
                        }

                        string[] guids = AssetDatabase.FindAssets("t:FavoritesManager");

                        if (guids.Length > 0)
                        {
                              string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                              _instance = AssetDatabase.LoadAssetAtPath<FavoritesManager>(path);
                        }
                        else
                        {
                              _instance = CreateInstance<FavoritesManager>();
                              const string directoryPath = "Assets/Settings/CustomToolbar";

                              if (!System.IO.Directory.Exists(directoryPath))
                              {
                                    System.IO.Directory.CreateDirectory(directoryPath);
                              }

                              AssetDatabase.CreateAsset(_instance, $"{directoryPath}/FavoritesManager.asset");
                              AssetDatabase.SaveAssets();
                        }

                        return _instance;
                  }
            }
      }
}
