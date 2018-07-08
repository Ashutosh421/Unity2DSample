using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum TileType
{
    KID,
    TEEN
}

public enum CharType
{
    SOURCE,
    TARGET
}

public class CharTile : MonoBehaviour {

    public GameObject objectPrefab;
    public TileType tileType = TileType.TEEN;
    public CharType charType = CharType.SOURCE;

    private Draggable eventManager;
    [HideInInspector] public GameObject targetObject;

    private void Start()
    {
        this.eventManager = this.GetComponent<Draggable>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<CharTile>() && collision.gameObject.GetComponent<CharTile>().charType == CharType.TARGET)
        {
            collision.gameObject.GetComponent<Image>().color = Color.green;
            this.targetObject = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<CharTile>() && collision.gameObject.GetComponent<CharTile>().charType == CharType.TARGET)
        {
            collision.gameObject.GetComponent<Image>().color = Color.white;
            if(this.targetObject == collision.gameObject)
            {
                 this.targetObject = null;
            }
        }
    }
}
