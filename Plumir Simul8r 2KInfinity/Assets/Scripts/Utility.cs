using UnityEngine;

public class Utility : MonoBehaviour
{

    static string key;

    public static string KeyGen()
    {
        do
        {
            int item = Random.Range(0, 36);
            switch (item)
            {
                case 0:
                    key = "0";
                    break;
                case 1:
                    key = "1";
                    break;
                case 2:
                    key = "2";
                    break;
                case 3:
                    key = "3";
                    break;
                case 4:
                    key = "4";
                    break;
                case 5:
                    key = "5";
                    break;
                case 6:
                    key = "6";
                    break;
                case 7:
                    key = "7";
                    break;
                case 8:
                    key = "8";
                    break;
                case 9:
                    key = "9";
                    break;
                case 10:
                    key = "a";
                    break;
                case 11:
                    key = "b";
                    break;
                case 12:
                    key = "c";
                    break;
                case 13:
                    key = "d";
                    break;
                case 14:
                    key = "e";
                    break;
                case 15:
                    key = "f";
                    break;
                case 16:
                    key = "g";
                    break;
                case 17:
                    key = "h";
                    break;
                case 18:
                    key = "i";
                    break;
                case 19:
                    key = "j";
                    break;
                case 21:
                    key = "k";
                    break;
                case 22:
                    key = "l";
                    break;
                case 23:
                    key = "m";
                    break;
                case 24:
                    key = "n";
                    break;
                case 25:
                    key = "o";
                    break;
                case 26:
                    key = "p";
                    break;
                case 27:
                    key = "q";
                    break;
                case 28:
                    key = "r";
                    break;
                case 29:
                    key = "s";
                    break;
                case 30:
                    key = "t";
                    break;
                case 31:
                    key = "u";
                    break;
                case 32:
                    key = "v";
                    break;
                case 33:
                    key = "w";
                    break;
                case 34:
                    key = "x";
                    break;
                case 35:
                    key = "y";
                    break;
                case 36:
                    key = "z";
                    break;
            }
        } while (key == null);
        return key;
    }
}
