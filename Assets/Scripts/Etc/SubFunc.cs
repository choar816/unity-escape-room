using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Sub
{
    [Serializable]
    public class AnimPlay
    {
        public string AnimName;
        public Animation Anim;
    }

    public class Func : MonoBehaviour
    {
        public static NGUIAtlas FindAtlas(string _name)
        {
            for (int i = 0; i < DataManager.Instance.atlasList.Length; i++)
            {
                if (_name == DataManager.Instance.atlasList[i].name)
                    return DataManager.Instance.atlasList[i];
            }

            return null;
        }

        public static void SetSprite(UISprite _sprite, string _atlas, string _spriteName)
        {
            _sprite.atlas = FindAtlas(_atlas);
            _sprite.spriteName = _spriteName;
        }
    }

    public class Image
    {
        /// <summary>
        /// 이미지 자동 자르기
        /// </summary>
        /// <param name="_source"></param>
        /// <returns></returns>
        public static Texture2D CropTexture(Texture2D _source)
        {
            int[] findDraw = new int[4]; // bot, top, left, right
            bool find = false;
            for (int h = 0; h < _source.height; h++)
            {
                for (int w = 0; w < _source.width; w++)
                {
                    if (_source.GetPixel(w, h) != Color.black)
                    {
                        findDraw[0] = h;
                        find = true;
                        break;
                    }

                    if (find)
                        break;
                }
            }

            find = false;
            for (int h = _source.height - 1; h >= 0; h--)
            {
                for (int w = _source.width - 1; w >= 0; w--)
                {
                    if (_source.GetPixel(w, h) != Color.black)
                    {
                        findDraw[1] = h;
                        find = true;
                        break;
                    }
                }

                if (find)
                    break;
            }

            find = false;
            for (int w = 0; w < _source.width; w++)
            {
                for (int h = 0; h < _source.height; h++)
                {
                    if (_source.GetPixel(w, h) != Color.black)
                    {
                        findDraw[2] = w;
                        find = true;
                        break;
                    }

                    if (find)
                        break;
                }
            }

            find = false;
            for (int w = _source.width - 1; w >= 0; w--)
            {
                for (int h = _source.height - 1; h >= 0; h--)
                {
                    if (_source.GetPixel(w, h) != Color.black)
                    {
                        findDraw[3] = w;
                        find = true;
                        break;
                    }
                }

                if (find)
                    break;
            }

            int rectSize = findDraw[3] - findDraw[2] >= findDraw[1] - findDraw[0] ? findDraw[3] - findDraw[2] + 30 : findDraw[1] - findDraw[0] + 30; // 10픽셀은 여유 공간
            Texture2D result = new Texture2D(rectSize, rectSize, TextureFormat.R8, false);

            // 초기화
            for (int h = 0; h < rectSize; h++)
            {
                for (int w = 0; w < rectSize; w++)
                {
                    result.SetPixel(w, h, Color.black);
                }
            }

            int widthAdd = 0;
            int heightAdd = 0;

            // 가운데 정렬
            heightAdd = (rectSize - (findDraw[1] - findDraw[0] + 1)) / 2;
            widthAdd = (rectSize - (findDraw[3] - findDraw[2] + 1)) / 2;

            int _width = widthAdd;
            int _height = heightAdd;
            int width = 0;
            int height = 0;
            for (int h = findDraw[0]; h <= findDraw[1]; h++)
            {
                for (int w = findDraw[2]; w <= findDraw[3]; w++)
                {
                    result.SetPixel(width + _width, height + _height, _source.GetPixel(w, h));
                    width++;
                }

                width = 0;
                height++;
            }

            result.Apply();
            return result;
        }

        /// <summary>
        /// 사이즈 키우기
        /// </summary>
        /// <param name="_source"></param>
        /// <param name="_size"></param>
        /// <param name="_basicColor"></param>
        /// <returns></returns>
        public static Texture2D ResizeTextureToBig(Texture2D _source, Vector2 _size, Color _basicColor)
        {
            Texture2D result = new Texture2D((int)_size.x, (int)_size.y, TextureFormat.R8, false);

            for (int width = 0; width < result.width; width++)
            {
                for (int height = 0; height < result.height; height++)
                {
                    if (((result.height - _source.height) / 2 <= height && (result.height - _source.height) / 2 + _source.height > height) && ((result.width - _source.width) / 2 <= width && (result.width - _source.width) / 2 + _source.width > width))
                    {
                        Color color = _source.GetPixel(width - (result.width - _source.width) / 2, height - (result.height - _source.height) / 2);
                        result.SetPixel(width, height, color == Color.white ? Color.white : Color.black);
                    }
                    else
                        result.SetPixel(width, height, _basicColor);
                }
            }

            result.Apply();
            return result;
        }


        /// <summary>
        /// 작은 사이즈 보간
        /// </summary>
        /// <param name="_source"></param>
        /// <param name="_size"></param>
        /// <returns></returns>
        public static Texture2D ResizeTextureSoft(Texture2D _source, Vector2 _size)
        {
            //*** Get All the source pixels
            Color[] sourceColor = _source.GetPixels(0);
            Vector2 sourceSize = new Vector2(_source.width, _source.height);

            //*** Calculate New Size
            float width = _size.x;
            float height = _size.y;

            //*** Make New
            Texture2D newTexture = new Texture2D((int)width, (int)height, TextureFormat.R8, false);

            //*** Make destination array
            int length = (int)width * (int)height;
            Color[] color = new Color[length];

            Vector2 pixelSize = new Vector2(sourceSize.x / width, sourceSize.y / height);

            //*** Loop through destination pixels and process
            Vector2 center = new Vector2();
            float fit_Size = 0.5f;
            for (int imageIdx = 0; imageIdx < length; imageIdx++)
            {
                //*** Figure out x&y
                float x = (float)imageIdx % width;
                float y = Mathf.Floor((float)imageIdx / width);

                //*** Calculate Center
                center.x = (x / width) * sourceSize.x;
                center.y = (y / height) * sourceSize.y;

                //*** Average
                //*** Calculate grid around point
                int xXFrom = (int)Mathf.Max(Mathf.Floor(center.x - (pixelSize.x * fit_Size)), 0);
                int xXTo = (int)Mathf.Min(Mathf.Ceil(center.x + (pixelSize.x * fit_Size)), sourceSize.x);
                int xYFrom = (int)Mathf.Max(Mathf.Floor(center.y - (pixelSize.y * fit_Size)), 0);
                int xYTo = (int)Mathf.Min(Mathf.Ceil(center.y + (pixelSize.y * fit_Size)), sourceSize.y);

                //*** Loop and accumulate
                Color colorTemp = new Color();
                float gridCount = 0;
                for (int iy = xYFrom; iy < xYTo; iy++)
                {
                    for (int ix = xXFrom; ix < xXTo; ix++)
                    {
                        //*** Get Color
                        colorTemp += sourceColor[(int)(((float)iy * sourceSize.x) + ix)];

                        //*** Sum
                        gridCount++;
                    }
                }

                //*** Average Color
                color[imageIdx] = colorTemp / (float)gridCount;
            }

            //*** Set Pixels
            newTexture.SetPixels(color);
            newTexture.Apply();

            /// custom
            for (int w = 0; w < newTexture.width; w++)
            {
                for (int h = 0; h < newTexture.height; h++)
                {
                    if (newTexture.GetPixel(w, h).r <= 0.15f)
                        newTexture.SetPixel(w, h, Color.black);
                    else
                        newTexture.SetPixel(w, h, Color.white);
                }
            }

            newTexture.Apply();
            //*** Return
            return newTexture;
            
        }

        /// <summary>
        ///  텍스쳐 보간 하드
        /// </summary>
        /// <param name="_source"></param>
        /// <param name="_size"></param>
        /// <returns></returns>
        public static Texture2D ResizeTextureHard(Texture2D _source, Vector2 _size)
        {
            Texture2D result = new Texture2D((int)_size.x, (int)_size.y, TextureFormat.R8, true);
            Color[] resultPixels = result.GetPixels(0);
            float incX = (1.0f / (float)_size.x);
            float incY = (1.0f / (float)_size.y);

            for (int pixelIdx = 0; pixelIdx < resultPixels.Length; pixelIdx++)
            {
                resultPixels[pixelIdx] = _source.GetPixelBilinear(incX * ((float)pixelIdx % _size.x), incY * ((float)Mathf.Floor(pixelIdx / _size.y)));
            }

            result.SetPixels(resultPixels, 0);
            result.Apply();
            return result;
        }


        /// <summary>
        /// 텍스쳐 복사
        /// </summary>
        /// <param name="_origin"></param>
        /// <param name="_copy"></param>
        public static void CopyTexture(Texture2D _origin, Texture2D _copy)
        {
            for (int w = 0; w < _copy.width; w++)
            {
                for (int h = 0; h < _copy.height; h++)
                {
                    _copy.SetPixel(w, h, Color.black);
                }
            }
            _copy.Apply();

            for (int w = 0; w < _origin.width; w++)
            {
                for (int h = 0; h < _origin.height; h++)
                {
                    _copy.SetPixel(w, h, _origin.GetPixel(w, h));
                }
            }

            for (int w = 0; w < _origin.width; w++)
            {
                _copy.SetPixel(w, _origin.height, Color.white);
            }

            for (int h = 0; h < _origin.height; h++)
            {
                _copy.SetPixel(_origin.width, h, Color.white);
            }
            _copy.Apply();
        }
    }
}
