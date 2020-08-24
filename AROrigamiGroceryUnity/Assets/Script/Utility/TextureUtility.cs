using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureUtility
{
    TextureStructure _textureStructure = new TextureStructure();

    public TextureStructure GrabTextureRadius(int p_width, int p_height, float ratio)
    {
        int criteria = (p_width > p_height) ? p_height : p_width;
        int radius = (int)((criteria * ratio) / 2f);
        int size = radius * 2;
        Vector2Int center = new Vector2Int((int)(p_width / 2f), (int)(p_height / 2f));

        _textureStructure.width = size;
        _textureStructure.height = size;
        _textureStructure.x = center.x - radius;
        _textureStructure.y = center.y - radius;
            
        return _textureStructure;
    }

    public struct TextureStructure
    {
        public int width;
        public int height;
        public int x;
        public int y;
    }



}
