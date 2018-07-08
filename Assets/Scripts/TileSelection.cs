using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

using AR;


public class TileSelection : MonoBehaviour
{
    public GameObject burstPrefab;
    public List<CharTile> selectableTiles;

    private CharTile currentTarget;

    public List<GameObject> rayCastHits;

    private Ray2D ray;
    private Camera uiCamera;

    private bool dragCheckStart = false;
    private Transform ghostElement;
    [SerializeField] private float targetTileCount = 0;
    private Ray2D ray2D;

    private Text loadingText;

    // Use this for initialization
    void Awake()
    {
        this.selectableTiles = this.transform.Find("Char_Grid").gameObject.GetComponentsInChildren<CharTile>().ToList();
        this.targetTileCount = this.transform.Find("Targets").gameObject.GetComponentsInChildren<CharTile>().ToList().Count;
        this.uiCamera = this.transform.root.GetComponent<Camera>();

        this.loadingText = this.transform.parent.Find("InfoPanel").Find("Text").GetComponent<Text>();
        this.loadingText.transform.parent.gameObject.SetActive(false);
        Draggable.onDragOver += Draggable_onDragOver;  //Listening to drag over of a CharTile
    }

    private void GameOver()
    {
        this.loadingText.transform.parent.gameObject.SetActive(true);
        this.loadingText.transform.parent.GetComponent<Image>().CrossFadeAlpha(0, 0, true);
        this.loadingText.transform.parent.GetComponent<Image>().CrossFadeAlpha(1, 1, true);
        BundleManager.Instance.LoadAssetBundle("bundle1", (bProgress) =>
        {
            //Debug.Log("Loading bundle "+bProgress.bundleProgress);
            this.loadingText.text = "Loading bundle.. " + bProgress.bundleProgress;
            if (bProgress.error != null)
            {
                //Debug.Log("Error occured while loading bundle "+bProgress.error);
                this.loadingText.text = "Error occured while loading bundle " + bProgress.error;
            }
            if (bProgress.bundle) {
                //Debug.Log("Loading bundle " + bProgress.bundle.name);
                this.loadingText.transform.parent.GetComponent<Image>().CrossFadeAlpha(0, 1, true);
                Sprite providedSketch = bProgress.bundle.LoadAsset<Sprite>("assets/images/image1.jpg");
                GameObject obj = new GameObject("LoadedSprite");
                obj.transform.position = Vector3.up;
                obj.AddComponent<SpriteRenderer>();
                obj.GetComponent<SpriteRenderer>().sprite = providedSketch;
            }
        });
    }

    private void Draggable_onDragOver(Transform sourceTransform)
    {
        if(sourceTransform.GetComponent<CharTile>())
        {
            if (sourceTransform.GetComponent<CharTile>().targetObject && sourceTransform.GetComponent<CharTile>().targetObject.GetComponent<CharTile>())
            {
                this.CheckIfMatch(sourceTransform.GetComponent<CharTile>(), sourceTransform.GetComponent<CharTile>().targetObject.GetComponent<CharTile>());
            }
            else
            {
                ARPoolManager.Instance.DestroyObject(sourceTransform);
            }
        }
    }

    /// <summary>
    /// Checking of the Source Tile meets the target tile
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    private void CheckIfMatch(CharTile source , CharTile target)
    {
        if(source.tileType == target.tileType)
        {
            Debug.Log("Correct Match");
            Instantiate(this.burstPrefab, source.targetObject.transform.position, Quaternion.identity);
            ARPoolManager.Instance.DestroyObject(source.transform);  //Remove the element from the Pool
            Destroy(target.gameObject);
            this.targetTileCount--;
            if (this.targetTileCount == 0) this.GameOver();
        }
        else
        {
            Debug.Log("Incorrect Match");
            ARPoolManager.Instance.DestroyObject(source.transform);
        }
    }

    public void OnSpawnGhost(GameObject target)
    {
        this.ghostElement = ARPoolManager.Instance.GetObjectFromPool(target.GetComponent<CharTile>().objectPrefab);
        this.ghostElement.transform.position = target.transform.position;
        this.ghostElement.transform.rotation = target.transform.rotation;
    }
}