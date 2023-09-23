using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace BloomFilter
{
    public class BloomFilter<T>
    {
        Byte[] bitArray;
        Byte[] tempHashArray;
        public List<Func<T, int>> hashFuncArray;

        public BloomFilter(int cap)
        {
            bitArray = new Byte[cap];
            tempHashArray = new Byte[cap];
            hashFuncArray = new List<Func<T, int>>() { HashFuncOne, HashFuncTwo, HashFuncThree };
        }

        public void LoadHashFunc(Func<T, int> hashFunc)
        {
            hashFuncArray.Add(hashFunc);
        }

        public void Insert(T item)
        {
            if (hashFuncArray.Count <= 0) throw new ArgumentNullException("doesn't exist");
            if (item == null) throw new ArgumentNullException("null");

            foreach (var hashFunctions in hashFuncArray)
            {
                int indexAmt = hashFunctions(item) % bitArray.Length;
                int byteChunk = Math.Abs(indexAmt / 8);
                int chunkIndex = indexAmt - (byteChunk * 8);
                tempHashArray[tempHashArray.Length - indexAmt] = 1;
                tempHashArray[byteChunk] >>= chunkIndex;
                for (int i = 0; i < bitArray.Length; i++)
                {
                    bitArray[i] = (byte)(bitArray[i] | tempHashArray[i]);
                }
            }
        }

        public bool ProbablyContains(T item)
        {
            if (hashFuncArray.Count <= 0) return false;
            if (item == null) return false; 

            foreach (var hashFunctions in hashFuncArray)
            {
                int indexAmt = hashFunctions(item) % bitArray.Length;
                int byteChunk = Math.Abs(indexAmt / 8);
                int chunkIndex = indexAmt - (byteChunk * 8);
                if (bitArray[tempHashArray.Length - indexAmt] != 1) return false;
            }
            return true;
        }

        private int HashFuncOne(T item)
        {
            return (int)(item.GetHashCode()) % bitArray.Length;
        }

        protected int HashFuncTwo(T item)
        {
            return (int)(item.GetHashCode() / 2 * 9.2320310911) % bitArray.Length;
        }

        protected int HashFuncThree(T item)
        {
            return (int)(Math.PI*(item.GetHashCode())) % bitArray.Length;
        }
    }
}
