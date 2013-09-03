using System;
using System.Collections.Generic;
using SharpDX.DirectInput;


namespace Fusee.Engine
{
    internal class Keymapper : Dictionary<SharpDX.DirectInput.Key, Fusee.Engine.KeyCodes>
    {
        /// <summary>
        /// Initializes the map between KeyCodes and OpenTK.Key
        /// </summary>
        internal Keymapper()
        {
            this.Add(Key.Escape, KeyCodes.Escape);

            //Function Keys
            for (int i = 0; i < 24; i++)
            {
                this.Add(Key.F1+i,(KeyCodes)((int)KeyCodes.F1+i));
            }

            // Number keys (0-9)
            for (int i = 0; i <= 9; i++)
            {
                this.Add(Key.D0 + i, (KeyCodes)(0x30 + i));
            }

            // Letters (A-Z)
            for (int i = 0; i < 26; i++)
            {
                this.Add(Key.A + i, (KeyCodes)(0x41 + i));
            }

            //this.Add(Key.Tab, KeyCodes.Tab);
            this.Add(Key.Capital, KeyCodes.Capital);
            this.Add(Key.LeftControl, KeyCodes.LControl);
            //this.Add(Key.LeftShift, KeyCodes.LShift);
            this.Add(Key.LeftWindowsKey, KeyCodes.LWin);
            this.Add(Key.LeftAlt, KeyCodes.LMenu);
            this.Add(Key.Space, KeyCodes.Space);
            this.Add(Key.RightAlt, KeyCodes.RMenu);
            this.Add(Key.RightWindowsKey, KeyCodes.RWin);
            this.Add(Key.Applications, KeyCodes.Apps);
            this.Add(Key.RightControl, KeyCodes.RControl);
            //this.Add(Key.RightShift, KeyCodes.RShift);
            this.Add(Key.Return, KeyCodes.Return);
            //this.Add(Key.Back, KeyCodes.Back);

            //this.Add(Key.Semicolon, KeyCodes.Oem1);
            //this.Add(Key.Slash, KeyCodes.Oem2);
            //this.Add(Key.Grave, KeyCodes.Oem3);
            //this.Add(Key.LeftBracket, KeyCodes.Oem4);
            //this.Add(Key.Backslash, KeyCodes.Oem5);
            //this.Add(Key.RightBracket, KeyCodes.Oem6);
            //this.Add(Key.Apostrophe, KeyCodes.Oem7);
            //this.Add(Key.Add, KeyCodes.OemPlus);
            //this.Add(Key.Comma, KeyCodes.OemComma);
            //this.Add(Key.Minus, KeyCodes.OemMinus);
            //this.Add(Key.Period, KeyCodes.OemPeriod);

            this.Add(Key.Home, KeyCodes.Home);
            this.Add(Key.End, KeyCodes.End);
            this.Add(Key.Delete, KeyCodes.Delete);
            this.Add(Key.PageUp, KeyCodes.Prior);
            this.Add(Key.PageDown, KeyCodes.Next);
            this.Add(Key.PrintScreen, KeyCodes.Print);
            this.Add(Key.Pause, KeyCodes.Pause);
            //this.Add(Key.NumberLock, KeyCodes.NumLock);

            //this.Add(Key.ScrollLock, KeyCodes.Scroll);
            // Do we need to do something here?? this.Add(Key.PrintScreen, KeyCodes.Snapshot);
            //this.Add(Key.Delete, KeyCodes.Clear);
            this.Add(Key.Insert, KeyCodes.Insert);

            this.Add(Key.Sleep, KeyCodes.Sleep);

            // Keypad
            //for (int i = 0; i <= 9; i++)
            //{
            //    this.Add(Key.NumberPad0 + i, (KeyCodes)((int)KeyCodes.NumPad0 + i));
            //}
            this.Add(Key.Decimal, KeyCodes.Decimal);
            //this.Add(Key.Add, KeyCodes.Add);
            //this.Add(Key.Subtract, KeyCodes.Subtract);
            //this.Add(Key.Divide, KeyCodes.Divide);
            //this.Add(Key.Multiply, KeyCodes.Multiply);

            // Navigation
            this.Add(Key.Up, KeyCodes.Up);
            this.Add(Key.Down, KeyCodes.Down);
            this.Add(Key.Left, KeyCodes.Left);
            this.Add(Key.Right, KeyCodes.Right);
            /*
            catch (ArgumentException e)
            {
                //Debug.Print("Exception while creating keymap: '{0}'.", e.ToString());
                System.Windows.Forms.MessageBox.Show(
                    String.Format("Exception while creating keymap: '{0}'.", e.ToString()));
            }
           */
        }
    }
}
