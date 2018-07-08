using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;



public class TileManager : MonoBehaviour {

    public List<GameObject> tiles;

    public delegate void TileSetComplete();
    public static event TileSetComplete onTileSetComplete;

    private bool tileAnimationStarted = false;

	// Use this for initialization
	private void Awake() {
        this.tiles = new List<GameObject>();
        this.tiles = this.gameObject.GetComponentsInChildren<EventTrigger>().Select(data => data.gameObject).ToList();

        this.transform.parent.Find("WelcomePanel").gameObject.SetActive(true);
        StartCoroutine(this.FadeWelcomePanel(2));

        this.ReadTileEvents();
	}

    /// <summary>
    /// Short Linear Tween to fade the Canvas Group
    /// </summary>
    /// <param name="initialWaitTime"></param>
    /// <returns></returns>
    private IEnumerator FadeWelcomePanel(float initialWaitTime)
    {
        CanvasGroup canvasGroup = this.transform.parent.Find("WelcomePanel").GetComponent<CanvasGroup>();
        yield return new WaitForSeconds(initialWaitTime);
        float value = 1;
        float rate = 3;
        while( value > 0 )
        {
            value -= rate * Time.deltaTime;
            rate = value / 1.2f;
            if (value < 0) value = 0;
            canvasGroup.alpha = value;
            yield return new WaitForEndOfFrame();
        }
        canvasGroup.gameObject.SetActive(false);
    }

    private void ReadTileEvents()
    {   
        for (int i = 0; i < this.tiles.Count; i++)
            this.CreateEventEntries(this.tiles[i].GetComponent<EventTrigger>() , this.tiles[i].GetComponent<Image>() , i);
    }

    private void CreateEventEntries(EventTrigger eTrigger, Image tileDetail, int tileIndex)
    {
        EventTrigger.Entry pDownEntry = new EventTrigger.Entry();
        pDownEntry.eventID = EventTriggerType.PointerDown;
        pDownEntry.callback.AddListener(data => this.OnTileClicked(data as PointerEventData , tileDetail , tileIndex));
        eTrigger.triggers.Add(pDownEntry);
    }

    
    private void PlayTileAnimation(int tileIndex)
    {
        this.tileAnimationStarted = true;
        int lastIndex = tileIndex > 0 ? tileIndex - 1 : this.tiles.Count - 1;
        StartCoroutine(this.ChangeTileColor(tileIndex, tileIndex, lastIndex, 0));
    }

    private IEnumerator ChangeTileColor(int startIndex , int currentIndex , int lastIndex, int timeToWait)
    {
        yield return new WaitForSecondsRealtime(timeToWait);
        this.tiles[currentIndex].GetComponent<Image>().color = Random.ColorHSV(0.2f, 1, 0.2f, 1, 0.2f, 1);
        currentIndex = currentIndex == this.tiles.Count - 1 ? lastIndex < startIndex ? -1 : currentIndex : currentIndex;
        if (currentIndex != lastIndex)
        {
            StartCoroutine(this.ChangeTileColor(startIndex, currentIndex + 1, lastIndex, 2));
        }
        else if(currentIndex == lastIndex)
        {
            this.tileAnimationStarted = false;
            if (onTileSetComplete != null) onTileSetComplete();
        }   
    }

    #region Event_Handlers
    public void OnTileClicked(PointerEventData data, Image tileDetail, int tileIndex)
    {
        if(!this.tileAnimationStarted)
            this.PlayTileAnimation(tileIndex);
    }       
#endregion
}
