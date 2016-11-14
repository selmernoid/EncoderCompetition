using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Encoder {

    public class RandomEmulator {
        static byte[] States = new byte[] {0,2,3,1};
        public int Position { get; private set; }
        bool rightWay = true;
        readonly int _percentToSwapWay;
        Random random = new Random();

        public RandomEmulator() {
            Position = 0;
            _percentToSwapWay = 10;
        }
        public RandomEmulator(int percentToSwapWay) {
            _percentToSwapWay = percentToSwapWay;
        }

        public static byte GetState(int position) {
            return States[(position + 4000)%4];
        }

        public byte GetNextOutput() {
            if (random.Next(100) < _percentToSwapWay)
                rightWay = !rightWay;
            return rightWay
                ? GetState(++Position)
                : GetState(--Position);
        }
    }
    public class RandomMultiEmulator {
        public RandomEmulator[] Emulators { get; private set; }
        public int Size = 4;
        public int LastNumber { get; private set; }
        
        readonly int _percentToSwapWay;
        Random random = new Random();

        public RandomMultiEmulator() {
            Emulators = new RandomEmulator[4];
            for (int i = 0; i < Size; i++) {
                Emulators[i] = new RandomEmulator(10);
            }
        }
        public RandomMultiEmulator(int percentToSwapWay) {
            Emulators = new RandomEmulator[4];
            for (int i = 0; i < Size; i++) {
                Emulators[i] = new RandomEmulator(percentToSwapWay);
            }
        }

        private void NextStep() {
            LastNumber = random.Next(Size);
            Emulators[LastNumber].GetNextOutput();
        }

        public byte GetNextOutput() {
            byte res = 0;
            NextStep();
            for (int i = 0; i < Size; i++) {
                var state = RandomEmulator.GetState(Emulators[i].Position);
                res |= unchecked ((byte)(((state & 1) << i) | ((state & 2) << (i+3))));
            }
            
            return res;
        }
        public byte GetNextOutput_v2() {
            byte res = 0;
            NextStep();
            for (int i = 0; i < Size; i++) {
                var state = RandomEmulator.GetState(Emulators[i].Position);
                res |= unchecked ((byte) ((state) << (i << 1)));
            }
            
            return res;
        }
    }
}
