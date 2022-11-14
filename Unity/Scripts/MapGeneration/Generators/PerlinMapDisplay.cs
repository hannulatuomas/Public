using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinMapDisplay : MonoBehaviour
{
    public Renderer textureRenderer;
    public void DrawMapTexture(Texture2D texture)
    {
        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(texture.width, texture.height, 1);
    }
}
