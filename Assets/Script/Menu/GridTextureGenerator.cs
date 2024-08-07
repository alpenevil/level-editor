using UnityEngine;
using UnityEngine.UI;

public class GridTextureGenerator : MonoBehaviour
{
    public int textureSize = 256; 
    public int cellSize = 16; 
    public Color backgroundColor = Color.blue;
    public Color lineColor = Color.white;
    public Color majorLineColor = Color.gray;
    public int majorLineWidth = 2; 
    public int minorLineWidth = 1;

    public Texture2D GenerateGridTexture()
    {
        Texture2D texture = new Texture2D(textureSize, textureSize);
        Color[] pixels = new Color[textureSize * textureSize];

        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                bool isMajorLine = (x % (cellSize * 5) < majorLineWidth) || (y % (cellSize * 5) < majorLineWidth);
                bool isMinorLine = (x % cellSize < minorLineWidth) || (y % cellSize < minorLineWidth);

                if (isMajorLine)
                {
                    pixels[y * textureSize + x] = majorLineColor;
                }
                else if (isMinorLine)
                {
                    pixels[y * textureSize + x] = lineColor;
                }
                else
                {
                    pixels[y * textureSize + x] = backgroundColor;
                }
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();

        return texture;
    }

    void Start()
    {
        Texture2D gridTexture = GenerateGridTexture();
        RawImage rawImage = GetComponent<RawImage>();
        rawImage.texture = gridTexture;
        float uvRectW = (float)(Screen.width) / textureSize;
        float uvRectH = uvRectW / (Screen.width / (float)Screen.height);
        rawImage.uvRect = new Rect(0, 0, uvRectW, uvRectH);
    }
}
