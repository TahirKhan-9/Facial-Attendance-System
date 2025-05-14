using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FaceAttendance.Classes;

namespace FaceAttendance.Classes
{
    class Dev_Validation
    {
        //Random Valid Serial
        //AIPE07000504

        public static bool serial_validation(string serial)
        {
            if (serial.Length == 12)
                return true;
            else
                return false;
        }

        public static string tranform_via_Cipher(string Serial)
        {
            //get serial value 
            string serialNo = Serial.ToString();
            //Encrypt serial number
            string serialEnc = StringCipher.Encrypt(serialNo, "123");
            //slicing encrypted number
            string subEnc = serialEnc.Substring(3, 9);
            //retuen value
            return ("HW" + subEnc + serialNo.Substring(serialNo.Length - 1));
        }

        public static char Random_character()
        {
            //Random Uppercase letter:
            Random rnd = new Random();
            int ascii_index = rnd.Next(65, 91); //ASCII character codes 65-90
            char myRandomUpperCase = Convert.ToChar(ascii_index); //produces any char A-Z

            //random lowercase letter:
            Random rnd2 = new Random();
            int ascii_index2 = rnd2.Next(97, 123); //ASCII character codes 97-123
            char myRandomLowerCase = Convert.ToChar(ascii_index2); //produces any char a-z

            //Random letter irrespective of case: (search up "random number from 2 different ranges" for explanation)
            int i = rnd.Next(1, 3) == 1 ? rnd.Next(65, 91) : rnd.Next(97, 123);
            char anyLetter = Convert.ToChar(i);
            return myRandomUpperCase;
        }

        public static string tranform_via_hex(string serial)
        {
            //get serial value 
            string serialNo = serial.ToString();


            //convert serial to hex;
            int decValue = int.Parse(serial.Substring(4));

            string hexValue = decValue.ToString("X");
            decValue = Convert.ToInt32(hexValue, 16);

            return ("HW" + hexValue.Substring(0, (hexValue.Length / 2)) + Random_character() + hexValue.Substring(hexValue.Length / 2) );


        }

        public static bool match_serial(string serial, string hw_pin)
        {
            string slicedpt1 = hw_pin.Substring(2, 3);
            string slicedpt2 = hw_pin.Substring(6, (hw_pin.Length - 6));

            string slicedPin = slicedpt1 + slicedpt2;
            //string slicedPin = hw_pin.Substring(2, 4)+hw_pin.Substring(6,(hw_pin.Length-1));

            //convert back to dec
            int decValue = Convert.ToInt32(slicedPin, 16);


            if (int.Parse(serial.Substring(4)) == decValue && hw_pin.Substring(0,2)=="HW")
            {
                //HWPin.Text += " " + decValue.ToString();
                return true;

            }
            else
            {
                //HWPin.Text += " " + decValue.ToString();
                return false;
            }
        }

        
        


    }
}
