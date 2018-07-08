using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager: MonoBehaviour {
   
    private void Awake()
    {
        TileManager.onTileSetComplete += TileManager_onTileSetComplete;
    }

    //Listening to Event When tile set is complete
    private void TileManager_onTileSetComplete()
    {
        SceneManager.LoadSceneAsync("Scene2");
    }
}
