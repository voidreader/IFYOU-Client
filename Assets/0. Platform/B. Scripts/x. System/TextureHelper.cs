using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureHelper
{
    public static Texture2D ChangeFormat(this Texture2D oldTexture, TextureFormat newFormat) {
        //Create new empty Texture
        Texture2D newTex = new Texture2D(2, 2, newFormat, false);
        //Copy old texture pixels into new one
        newTex.SetPixels(oldTexture.GetPixels());
        //Apply
        newTex.Apply();


        return newTex;
    }

}
