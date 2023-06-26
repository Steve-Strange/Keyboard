using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Valve.VR;

/* 倾斜键盘，继承自ClickKeyboard，应当只用实现自己的Axis2Letter方法. */
public class InclinedKeyboard : ClickKeyboard
{
    private float radius = 1;
    public float thumbTheta;
    public float thumbLength;
    private Vector2[] thumbCenter = new Vector2[7];
    private float[] d = new float[7];

    private int[] keyColumn = new int[6] { 1, 2, 3, 5, 4, 3};

    private int[,,] keys = new int[6, 6, 5]     { { { 0, 0, 0, 0, 0 }, {'q', 0x20,  0, 0, 0 }, {'w', 's','a' , 0, 0, }, { 'e', 'd', 'x', 'z', '.'}, {'r', 'f', 'c', ',', 0, }, { 't', 'g', 'v' , 0, 0 } },
                                                  { { 0, 0, 0, 0, 0 }, {'Q', 0x20,  0, 0, 0 }, {'W', 'S','A' , 0, 0, }, { 'E', 'D', 'X', 'Z', '.'}, {'R', 'F', 'C', ',', 0, }, { 'T', 'G', 'V' , 0, 0 } },
                                                  { { 0, 0, 0, 0, 0 }, {'1', 0x20,  0, 0, 0 }, {'2', '!','~' , 0, 0, }, { '3', '@', ')', '(', '.'}, {'4', '#', '-', ',', 0, }, { '5', '%', '_' , 0, 0 } },
                                                  { { 0x10, 0, 0, 0, 0 }, {',', 'p', 0, 0, 0 }, {'l' , 'k','o',  0, 0, }, { '?', 'm', 'n', 'j', 'i'}, {'!', 'b', 'h', 'u', 0, }, { 'v', 'g', 'y' , 0, 0 } },
                                                  { { 0x10, 0, 0, 0, 0 }, {',', 'P', 0, 0, 0 }, {'L' , 'K','O',  0, 0, }, { '?', 'M', 'N', 'J', 'I'}, {'!', 'B', 'H', 'U', 0, }, { 'V', 'G', 'Y' , 0, 0 } },
                                                  { { 0x10, 0, 0, 0, 0 }, {',', '0', 0, 0, 0 }, {'?' , '*','9',  0, 0, }, { '?', '/', ';', '&', '8'}, {'!', ':', '\'', '7', 0, }, { '_', '%', '6' , 0, 0 } }};

    //private void Start()
    //{
    //}

    //作为测试，在Update里面轮询.
    //private void Update()
    //{
    //    //GameObject key;
    //    //if (touched)
    //    //{
    //    //    if (PadSlide[SteamVR_Input_Sources.LeftHand].axis != new Vector2(0, 0))
    //    //    {
    //    //        Axis2Letter(PadSlide[SteamVR_Input_Sources.LeftHand].axis, SteamVR_Input_Sources.LeftHand, 0, out key);
    //    //        //Debug.Log("Key: " + ascii);
    //    //    }
    //    //    if (PadSlide[SteamVR_Input_Sources.RightHand].axis != new Vector2(0, 0))
    //    //    {
    //    //        Axis2Letter(PadSlide[SteamVR_Input_Sources.RightHand].axis, SteamVR_Input_Sources.RightHand, 0, out key);
    //    //        //Debug.Log("Key: " + ascii);
    //    //    }
    //    //}
    //}

    public override int Axis2Letter(Vector2 axis, SteamVR_Input_Sources hand, int mode, out GameObject key)
    {
        if(hand == SteamVR_Input_Sources.LeftHand)
        {
            for (int i = 0; i < 7; i++)
            {
                d[i] = thumbLength + (float)(i - 3) * 1 / 3f;
                thumbCenter[i] = new Vector2((d[i] * Mathf.Sin(thumbTheta)), (d[i] * Mathf.Cos(thumbTheta)));
            }
        }
        else
        {
            for (int i = 0; i < 7; i++)
            {
                d[i] = thumbLength + (float)(i - 3) * 1 / 3f;
                thumbCenter[i] = new Vector2(-(d[i] * Mathf.Sin(thumbTheta)), (d[i] * Mathf.Cos(thumbTheta)));
            }
        }

        // TODO: 获取相应位置的按件对象并赋值给key
        int column;
        int row = 0;
        for (int i = 0; i < 6; i++)
        {
            float distanceA = Mathf.Sqrt(Mathf.Pow((axis.x - thumbCenter[i].x), 2) + Mathf.Pow((axis.y - thumbCenter[i].y), 2));
            float distanceB = Mathf.Sqrt(Mathf.Pow((axis.x - thumbCenter[i+1].x), 2) + Mathf.Pow((axis.y - thumbCenter[i+1].y), 2));

            if (distanceA < thumbLength && distanceB > thumbLength)
            {
                row = i;
                break;
            }
        }

        //print("d: " + axis.y);
        //print("d[row] " + d[row]);
        //print("thetaMax cos num " + Mathf.Min((Mathf.Pow(d[row], 2) + Mathf.Pow(thumbLength, 2) - Mathf.Pow(radius, 2)) / (2 * d[row] * thumbLength),
        //                                      (Mathf.Pow(d[row + 1], 2) + Mathf.Pow(thumbLength, 2) - Mathf.Pow(radius, 2)) / (2 * d[row + 1] * thumbLength)));

        int maxThetaRow = (Mathf.Pow(d[row], 2) + Mathf.Pow(thumbLength, 2) - Mathf.Pow(radius, 2)) / (2 * d[row] * thumbLength)
                            < (Mathf.Pow(d[row + 1], 2) + Mathf.Pow(thumbLength, 2) - Mathf.Pow(radius, 2)) / (2 * d[row + 1] * thumbLength) ? row : row + 1;

        float thetaMax = Mathf.Acos((Mathf.Pow(d[maxThetaRow], 2) + Mathf.Pow(thumbLength, 2) - Mathf.Pow(radius, 2)) / (2 * d[maxThetaRow] * thumbLength));

        float currentTheta = (thumbTheta == 0) ? Mathf.Atan((axis.x - thumbCenter[maxThetaRow].x) / (axis.y - thumbCenter[maxThetaRow].y)) : 
                                                 Mathf.Abs(Mathf.Atan((axis.x - thumbCenter[maxThetaRow].x) / (axis.y - thumbCenter[maxThetaRow].y))) - thumbTheta;

        float frow = (currentTheta + thetaMax) / (2 * thetaMax / keyColumn[maxThetaRow]);

        //print("thetaMax" + thetaMax);
        //print("currentTheta" + currentTheta);
        //print("frow" + frow);

        column = (int)frow;
        if (column < 0) column = 0;
        if (column > keyColumn[row] - 1) column = keyColumn[row] - 1;

        Debug.Log("( " + column + ' ' + row + " )");

        key = this.gameObject;

        int handmode = (hand == SteamVR_Input_Sources.LeftHand) ? mode : mode + 3;

        char output = (char)keys[handmode, row, column];
        print(handmode);
        print(output);
        return keys[handmode, row, column];
    }

    protected override TextMeshProUGUI[,] fetchKeyStrings()
    {
        TextMeshProUGUI[,] ret = new TextMeshProUGUI[2, 28];
        int i = 0;
        Transform[] children = new Transform[2] { keyboardRoot.GetChild(0), keyboardRoot.GetChild(1) };
        foreach(Transform LR in children)
        {
            foreach(var keys in LR.GetComponentsInChildren<MeshRenderer>())
            {
                Transform canvas = keys.transform.GetChild(0);
                if(canvas.childCount == 2)
                {
                    // 有两个儿子，有用.  有字母有符号.
                    foreach(var text in canvas.GetComponentsInChildren<TextMeshProUGUI>())
                    {
                        if (text.text[0] >= 'a' && text.text[0] <= 'z')  //初始必定小写.
                            ret[0, i] = text;
                        else
                            ret[1, i] = text;
                    }
                    ++i;
                }
            }
        }
        return ret;
    }
}
