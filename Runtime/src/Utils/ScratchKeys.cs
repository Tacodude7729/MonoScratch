using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace MonoScratch.Runtime {

    public static class SrcatchInput {

        private static readonly Dictionary<string, Keys> _keyMap;

        static SrcatchInput() {
            _keyMap = new Dictionary<string, Keys>();

            _keyMap.Add("a", Keys.A);
            _keyMap.Add("b", Keys.B);
            _keyMap.Add("c", Keys.C);
            _keyMap.Add("d", Keys.D);
            _keyMap.Add("e", Keys.E);
            _keyMap.Add("f", Keys.F);
            _keyMap.Add("g", Keys.G);
            _keyMap.Add("h", Keys.H);
            _keyMap.Add("i", Keys.I);
            _keyMap.Add("j", Keys.J);
            _keyMap.Add("k", Keys.K);
            _keyMap.Add("l", Keys.L);
            _keyMap.Add("m", Keys.M);
            _keyMap.Add("n", Keys.N);
            _keyMap.Add("o", Keys.O);
            _keyMap.Add("p", Keys.P);
            _keyMap.Add("q", Keys.Q);
            _keyMap.Add("r", Keys.R);
            _keyMap.Add("s", Keys.S);
            _keyMap.Add("t", Keys.T);
            _keyMap.Add("u", Keys.U);
            _keyMap.Add("v", Keys.V);
            _keyMap.Add("w", Keys.W);
            _keyMap.Add("x", Keys.X);
            _keyMap.Add("y", Keys.Y);
            _keyMap.Add("z", Keys.Z);

            _keyMap.Add("1", Keys.D1);
            _keyMap.Add("2", Keys.D2);
            _keyMap.Add("3", Keys.D3);
            _keyMap.Add("4", Keys.D4);
            _keyMap.Add("5", Keys.D5);
            _keyMap.Add("6", Keys.D6);
            _keyMap.Add("7", Keys.D7);
            _keyMap.Add("8", Keys.D8);
            _keyMap.Add("9", Keys.D9);
            _keyMap.Add("0", Keys.D0);

            _keyMap.Add("down arrow", Keys.Down);
            _keyMap.Add("up arrow", Keys.Up);
            _keyMap.Add("left arrow", Keys.Left);
            _keyMap.Add("right arrow", Keys.Right);

            _keyMap.Add("space", Keys.Space);
            _keyMap.Add("enter", Keys.Enter);
        }

        public static int GetKey(string keyName) {
            if (keyName.Length == 0)
                return -1;

            Keys key;
            if (keyName.Length == 1) {
                if (_keyMap.TryGetValue(keyName.ToLower(), out key))
                    return (int)key;
                return -1;
            }

            if (_keyMap.TryGetValue(keyName, out key))
                return (int)key;

            if (keyName == "any")
                return -2;

            return GetKey(keyName[0].ToString());
        }

        public static bool IsAnyKeyDown() {
            return Program.Runtime.KeyboardState.GetPressedKeyCount() > 0;
        }

        public static bool IsKeyDown(string keyName) {
            int key = GetKey(keyName);
            if (key == -1) return false;
            if (key == -2) return IsAnyKeyDown();
            return Program.Runtime.KeyboardState.IsKeyDown((Keys) key);
        }

        public static bool IsMouseDown() {
            return Program.Runtime.MouseState.LeftButton == ButtonState.Pressed;
        }
    }
}