using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

 public static class AbbrevationUtility
 {
     private static readonly SortedDictionary<int, string> abbrevations = new SortedDictionary<int, string>
     {
         {1000,"K"},
         {1000000, "M" },
         {1000000000, "B" }
     };
 
     public static string AbbreviateNumber(float number)
     {
         for (int i = abbrevations.Count - 1; i >= 0; i--)
         {
             KeyValuePair<int, string> pair = abbrevations.ElementAt(i);
             if (Mathf.Abs(number) >= pair.Key)
             {
                 int roundedNumber = Mathf.FloorToInt(number / pair.Key);
                 return roundedNumber.ToString() + pair.Value;
             }
         }
         return number.ToString();
     }
     
    public static string intToSimple(int val) {
        
        if(val > 1000000000)
            return (val / 1000000000).ToString("0.00") + "b";
        else if(val > 1000000)
            return (val / 1000000).ToString("0.00") + "m";
        else if(val > 1000)
            return (val / 1000).ToString("0.00") + "k";
        else
            return val.ToString("0.00");
    }
 }
 