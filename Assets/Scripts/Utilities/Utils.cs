using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using URandom = UnityEngine.Random;

public class Utils
{
    public static NormalItem.eNormalType GetRandomNormalType()
    {
        Array values = Enum.GetValues(typeof(NormalItem.eNormalType));
        NormalItem.eNormalType result = (NormalItem.eNormalType)values.GetValue(URandom.Range(0, values.Length));

        return result;
    }

    public static NormalItem.eNormalType GetRandomNormalTypeExcept(NormalItem.eNormalType[] types)
    {
        List<NormalItem.eNormalType> list = Enum.GetValues(typeof(NormalItem.eNormalType)).Cast<NormalItem.eNormalType>().Except(types).ToList();

        int rnd = URandom.Range(0, list.Count);
        NormalItem.eNormalType result = list[rnd];

        return result;
    }

    public static List<NormalItem.eNormalType> GetRandomNormalTypeDivisibleby3(int sizeX, int sizeY)
    {
        List<NormalItem.eNormalType> list = Enum.GetValues(typeof(NormalItem.eNormalType)).Cast<NormalItem.eNormalType>().ToList();
        List<NormalItem.eNormalType> listResual = new List<NormalItem.eNormalType>();
        shuffe(list);
        for(int i =0;i< sizeX* sizeY; i++)
        {
            listResual.Add(list[0]);
            if (list.Count <= 1)
            {
                continue;
            }
            if ((i+1) % 3 == 0)
            {
                list.RemoveAt(0);
            }
        }
        shuffe(listResual);
        return listResual;
    }

    private static void shuffe(List<NormalItem.eNormalType> list)
    {
        int n = list.Count;
        System.Random ran = new System.Random();
        while (n > 1)
        {
            n--;
            int i = ran.Next(n+1);
            (list[n], list[i]) = (list[i], list[n]);

        }
    }

}
