using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Encoder {
    class Program {
        static void Main(string[] args) {
            var emulator = new RandomEmulator(25);
            var emulatorMulti = new RandomMultiEmulator(25);
            var emulatorMulti_v2 = new RandomMultiEmulator(25);
            Response pos = new Response();
            IEncoder encoder = new SingleEncoder();
            IEncoder multiEncoder = new Multi_4a4b_Encoder();;
            IEncoder multiEncoder_v2 = new Multi_4ab_Encoder();;


            Console.WriteLine("Started emulator ab");
            for (int i = 0; i < 100; i++) {
                pos = encoder.GetPosition(emulator.GetNextOutput());
                //Console.WriteLine($"{pos.State == emulator.Position} {pos.State} {emulator.Position}");
            }
            Console.WriteLine($"{pos.State == emulator.Position} {pos.State} {emulator.Position}");




            Console.WriteLine();
            Console.WriteLine("Started emulator 4a4b");
            for (int i = 0; i < 100; i++) {
                pos = multiEncoder.GetPosition(emulatorMulti.GetNextOutput());
                //Console.WriteLine(
                //    $"{pos.Number == emulatorMulti.LastNumber} {pos.Number} {emulatorMulti.LastNumber} "+
                //    $"{pos.State == emulatorMulti.Emulators[pos.Number].Position} {pos.State} {emulatorMulti.Emulators[pos.Number].Position}");
            }

            Console.WriteLine(
                $"{pos.Number == emulatorMulti.LastNumber} {pos.Number} {emulatorMulti.LastNumber} " +
                $"{pos.State == emulatorMulti.Emulators[pos.Number].Position} {pos.State} {emulatorMulti.Emulators[pos.Number].Position}");


            Console.WriteLine();
            Console.WriteLine("Started emulator 4(ab) pairs");

            for (int i = 0; i < 100; i++) {
                pos = multiEncoder_v2.GetPosition(emulatorMulti_v2.GetNextOutput_v2());
                //Console.WriteLine(
                //    $"{pos.Number == emulatorMulti_v2.LastNumber} {pos.Number} {emulatorMulti_v2.LastNumber} "+
                //    $"{pos.State == emulatorMulti_v2.Emulators[pos.Number].Position} {pos.State} {emulatorMulti_v2.Emulators[pos.Number].Position}");
            }

            Console.WriteLine(
                $"{pos.Number == emulatorMulti_v2.LastNumber} {pos.Number} {emulatorMulti_v2.LastNumber} " +
                $"{pos.State == emulatorMulti_v2.Emulators[pos.Number].Position} {pos.State} {emulatorMulti_v2.Emulators[pos.Number].Position}");



            Console.ReadKey();
        }
    }

    public interface IEncoder {
        Response GetPosition();
        Response GetPosition(byte newSignal);
    }

    public class SingleEncoder : IEncoder {
        private int position;
        private byte prevState;

        public SingleEncoder() {
            position = 0;
            prevState = 0;
        }

        public Response GetPosition() => new Response(position);

        public Response GetPosition(byte newSignal) {
            //maybe better because of branch prediction
//            if ((newSignal & prevState & 1) == 1)
//                --position;
//            else
//                ++position;

            position = --position + ((newSignal ^ prevState) & 2);
            prevState = unchecked((byte) (((newSignal & 2) >> 1) | ((newSignal & 1) << 1))); //swap 2 first bits
            return GetPosition();
        }
    }

    public class Multi_4a4b_Encoder : IEncoder {
        private int[] position = new int[4] {0,0,0,0};
        private byte prevSwapState, prevSignal;

        public Multi_4a4b_Encoder() {
            prevSwapState = prevSignal = 0;
        }

        public Response GetPosition() => new Response(position[0]);

        public Response GetPosition(byte newSignal) {
            var temp = unchecked(((byte)(((newSignal >> 4) | (newSignal << 4)) & 0xFF))); //swap a & b bits

            //Get position of changed bit
            byte pos = unchecked((byte) (BitHelper.GetFirstBitPos(
                ((prevSignal ^ newSignal) | (prevSwapState ^ temp)) // 2 changed bits
                & ((newSignal ^ prevSwapState))
                ) ));
            byte arrayIdx = unchecked((byte)(pos & 3));
            
            position[arrayIdx] = --position[arrayIdx] + ((pos >> 2) << 1);
            
            prevSignal = newSignal;
            prevSwapState = temp;
            return new Response(arrayIdx, position[arrayIdx]);
        }
    }

    public class Multi_4ab_Encoder : IEncoder {
        private int[] position = new int[4] {0,0,0,0};
        private byte prevSwapState, prevSignal;

        public Multi_4ab_Encoder() {
            prevSwapState = prevSignal = 0;
        }

        public Response GetPosition() => new Response(position[0]);

        public Response GetPosition(byte newSignal) {
            var temp = unchecked((byte)(((newSignal & 170) >> 1) | ((newSignal & 85) << 1))); //swap a & b bits

            //Get position of changed bit
            byte pos = unchecked((byte)(BitHelper.GetFirstBitPos(
                            ((prevSignal ^ newSignal) | (prevSwapState ^ temp)) // 2 changed bits
                            & ((newSignal ^ prevSwapState))
                            )));

            var arrayIdx = unchecked((byte)(pos >> 1));

            
            position[arrayIdx] = --position[arrayIdx] + ((pos & 1) << 1);

            prevSignal = newSignal;
            prevSwapState = temp;
            return new Response(arrayIdx, position[arrayIdx]);
        }
    }


    public static class BitHelper {
        private static readonly int[] MultiplyDeBruijnBitPosition = new[] {
            0, 1, 28, 2, 29, 14, 24, 3, 30, 22, 20, 15, 25, 17, 4, 8,
            31, 27, 13, 23, 21, 19, 16, 7, 26, 12, 18, 6, 11, 5, 10, 9
        };

        public static int GetFirstBitPos(int number) {
            return MultiplyDeBruijnBitPosition[((uint)((number & -number) * 0x077CB531U)) >> 27];
        }
    }

    public struct Response {
        public byte Number;
        public int State;
        public Response(byte n, int s) {
            Number = n;
            State = s;
        }
        public Response(int s) {
            Number = 0;
            State = s;
        }
    }


}
