using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using RandomNumbers.Utils;

namespace RandomNumbers {
    public class Model {
        public readonly List<int> epsilon;
        private int block_size { get; set; }
        private FileInfo fileinfo;
        private int block_size_in_bytes { get; set; }
        private BinaryReader reader;

        public Model(String filename, int block_size = 1024) {
            this.fileinfo = new FileInfo(filename);
            this.reader = new BinaryReader(new FileStream(filename, FileMode.Open));
            this.epsilon = new List<int>(block_size);
            this.block_size = block_size;
            this.block_size_in_bytes = block_size << 3;
        }

        private void NextBlock() {
            epsilon.Clear();
            byte[] bytes = reader.ReadBytes(block_size_in_bytes);
            AddBytesToEpsilon(ref bytes);
        }

        private void AddBytesToEpsilon(ref byte[] bytes) {
            BitArray bits = new BitArray(bytes);
                
            for(int i = 0; i < bits.Length; ++i) {
                epsilon.Add(Convert.ToInt32(bits[i]));
            }
        }

        void Dispose() {
            reader.Close();
            epsilon.Clear();
        }
    }
}
