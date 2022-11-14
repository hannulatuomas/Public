using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoissonDiscManager : MonoBehaviour
{
    public Sprite sprite;
    public float radius  = 1;
    Vector2 regionSize = new Vector2(100,100);
    int sampelsBeforeRejection = 20;
    GameObject testGameObject;
    int index = 0;
    private void Start()
    {
        List<Vector2> nodeList = PoissonDiscSampling.GeneratePoints(radius, regionSize, sampelsBeforeRejection);
        AddSprites(nodeList);
    }
    void AddSprites(List<Vector2> pointsToAdd)
    {
        foreach (Vector2 point in pointsToAdd)
        {
            testGameObject = new GameObject();
            testGameObject.name = "Test" + index;
            testGameObject.transform.position = new Vector3(point.x, point.y, 0);
            testGameObject.transform.SetParent(GameObject.Find("PoissonDiscTest").gameObject.transform);
            SpriteRenderer tileSpriteRenderer = testGameObject.AddComponent<SpriteRenderer>();
            tileSpriteRenderer.sprite = sprite;
            index++;
        }
    }
}
