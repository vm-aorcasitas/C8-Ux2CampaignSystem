using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GanoExcel.Web.Base
{
    public class Cryptography
    {
        private const int ALP_KEY_LENGTH = 8; //used for hex encryption/decryption
        private const string HEX_MASK_A = "0,1,2,3,4,5,6,7,8,9,A,B,C,D,E,F"; //HEX POSSIBILITIES
        private const string HEX_MASK_B = "j,0,H,n,1,N,m,@,Y,i,$,S,o,G,A,y"; //HEX SUBS
        private const string HEX_MASK_C = "2,3,4,5,6,7,8,9,2,3,4,5,6,7,8,9,B,C,D,E,F,J,K,L,M,O,P,Q,R,T,U,V,W,X,Z"; //ZERO subs
        private const string PRIVATE_KEY = @"!$(@$@nT@m@riA1492";

        public static string Decrypt(string value, string publicKey)
        {
            string intermediate = "";
            string pKey;

            try
            {
                intermediate = DecryptWithALP(value);

                pKey = DecryptWithALP(intermediate.Substring(8, Convert.ToInt32(UnmaskHex(intermediate).Substring(0, 8), 16)));

                if (pKey == PRIVATE_KEY)
                {
                    intermediate = DecryptWithALP(intermediate.Substring(8 + Convert.ToInt32(UnmaskHex(intermediate).Substring(0, 8), 16)));
                    pKey = DecryptWithALP(intermediate.Substring(8, Convert.ToInt32(UnmaskHex(intermediate).Substring(0, 8), 16)));

                    if (pKey == publicKey)
                        return DecryptWithALP(intermediate.Substring(8 + Convert.ToInt32(UnmaskHex(intermediate).Substring(0, 8), 16)));

                }
                return null;
            }
            catch (Exception exc)
            {
                throw new CryptoInvalidKeyException(exc.Message, exc.InnerException);
            }

        }

        private static string UnmaskHex(string value)
        {

            bool addZero = false;
            string output = "";

            string[] maskA = HEX_MASK_A.Split(new char[] { ',' });
            string[] maskB = HEX_MASK_B.Split(new char[] { ',' });
            string[] maskC = HEX_MASK_C.Split(new char[] { ',' });

            for (int i = 0; i < value.Length; i++)
            {
                addZero = false;
                for (int j = 0; j < maskC.Length; j++)
                {
                    if (value[i].ToString() == maskC[j])
                    {
                        addZero = true;
                        break;
                    }
                }

                if (addZero)
                {
                    output += "0";
                }
                else
                {
                    for (int j = 0; j < maskB.Length; j++)
                    {
                        if (value[i].ToString() == maskB[j])
                        {
                            output += maskA[j];
                            break;
                        }
                    }
                }
            }
            
            return output;
        }

        public static string EncryptWithALP(string value)
        {


            return "";
        }

        public static string DecryptWithALP(string value)
        {
            string mask = "";
            string data = "";
            string key = "";
            int index = 0;
            bool osc = true;
            string text = "";
            int val = 0;

            if (value.Length < ALP_KEY_LENGTH + 2)
                return "";

            mask = Convert.ToString(Convert.ToUInt16(value.Substring(0, 2), 16), 2);
            
            mask = mask.PadLeft(ALP_KEY_LENGTH, '0');

            data = value.Remove(0, 2);

            for (int i = ALP_KEY_LENGTH; i >= 1; i--)
            {
                if (mask[i - 1] == '1')
                {
                    key = data[0] + key;
                    data = data.Remove(0, 1);
                }
                else
                {
                    key = data[data.Length - 1] + key;
                    data = data.Remove(data.Length - 1);
                }

            }

            while (data.Length > 0)
            {


                if (index >= ALP_KEY_LENGTH)
                    index = 0;

                val = osc ? Convert.ToInt32(data.Substring(0, 2), 16) - Convert.ToInt32(key[index]) : 
                    Convert.ToInt32(data.Substring(0, 2), 16) + Convert.ToInt32(key[index]);
                
                if (val > 255)
                    val -= 255;

                if (val < 0)
                    val += 255;

                text += Convert.ToChar(val);

                osc = !osc;
                index++;
                data = data.Remove(0, 2);
            }

            return text;
        
        }

        public static string DecryptEx(string value, string publicKey)
        {
            string buffer = "";
            byte[] bytes = System.Text.Encoding.Default.GetBytes(value);   
            byte[] key  = System.Text.Encoding.Default.GetBytes(publicKey);               
            
            if (publicKey.Length > 0)
            {
                for (int i = 0; i < value.Length; i++)
                {
                    buffer += Convert.ToChar((bytes[i] - key[((i + 1) % publicKey.Length)]) & 0xff);
                }
                return buffer;
            }
            else
                return value;
        }

//        Public Function DecryptEx(ByVal strText As String, ByVal strPwd As String) As String
//    Dim i As Integer, c As Integer
//    Dim strBuff As String

//    'Decrypt string
//    If Len(strPwd) Then
//        For i = 1 To Len(strText)
//            c = Asc(Mid$(strText, i, 1))
//            c = c - Asc(Mid$(strPwd, (i Mod Len(strPwd)) + 1, 1))
//            strBuff = strBuff & Chr$(c And &HFF)
//        Next i
//    Else
//        strBuff = strText
//    End If
//    DecryptEx = strBuff
//End Function
    }

    [Serializable]
    public class CryptoInvalidKeyException : BaseException
    {
        public CryptoInvalidKeyException() :
            base()
        {
        }

        public CryptoInvalidKeyException(string message) :
            base(message)
        {
        }

        public CryptoInvalidKeyException(string message, Exception innerException) :
            base(message, innerException)
        {
        }
    }
}

