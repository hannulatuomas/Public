using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public int posX;
    public int posY;

    public Ground ground;
    public Floor floor;
    public Structure structure;
    public Item item;
    public Roof roof;
    public Resource resource;

    public GameObject tileGameObject;

    private Controller controller;

    public Tile(int x, int y)
    {
        this.posX = x;
        this.posY = y;

    }

    public void CreateTileGameObject()
    {
        controller = GameObject.Find("Controller").gameObject.GetComponent<Controller>();
        tileGameObject = new GameObject();
        tileGameObject.name = "Tile_" + posX + "_" + posY;
        tileGameObject.transform.position = new Vector3(posX, posY, 0);
        tileGameObject.transform.SetParent(GameObject.Find("Controller").gameObject.transform);
        SpriteRenderer tileSpriteRenderer = tileGameObject.AddComponent<SpriteRenderer>();
        tileSpriteRenderer.sprite = controller.sprites[Random.Range((int)0, (int)28)];

    }

}
