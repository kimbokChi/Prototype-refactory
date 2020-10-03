using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kimbokchi
{
    public static class Utility
    {
        public static void Swap<T>(ref T a, ref T b)
        {
            T temp = a; a = b; b = temp;
        }

        public static void Swap<T>(this T[] array, int indexA, int indexB)
        {
            T temp = array[indexA]; array[indexA] = array[indexB]; array[indexB] = temp;
        }

        public static void Swap<T>(this List<T> list, int indexA, int indexB)
        {
            T temp = list[indexA]; list[indexA] = list[indexB]; list[indexB] = temp;
        }

        public static int LuckyNumber(params float[] probablities)
        {
            float sum = 0f;

            float lucky = (float)new Random().NextDouble();

            for (int i = 0; i < probablities.Length; ++i)
            {
                sum += probablities[i];

                if (lucky <= sum) return i;
            }
            return -1;
        }
    }

    public class LuckyBox<T>
    {
        private Action<LuckyBox<T>> mEmptyItemList;

        private Dictionary<float, List<T>> mItemTableOrigin;
        private Dictionary<float, List<T>> mItemTable;

        private float mComplement;

        private Random mRandom;

        public LuckyBox(Action<LuckyBox<T>> emptyItemListAction = null)
        {
            mComplement = 0f;

            mEmptyItemList = emptyItemListAction ?? new Action<LuckyBox<T>>(o => o.Refill());

            mItemTableOrigin = new Dictionary<float, List<T>>();
            mItemTable       = new Dictionary<float, List<T>>();

            mRandom = new Random();
        }
        public void AddItem(float probablity, params T[] itemList)
        {
            if (!mItemTableOrigin.ContainsKey(probablity))
            {
                mItemTableOrigin.Add(probablity, new List<T>());
                mItemTable      .Add(probablity, new List<T>());
            }
            for (int i = 0; i < itemList.Length; ++i)
            {
                mItemTableOrigin[probablity].Add(itemList[i]);
                mItemTable      [probablity].Add(itemList[i]);
            }
        }
        public void Refill()
        {
            mComplement = 0f;

            foreach (var item in mItemTableOrigin)
            {                
                if (item.Value.Count > mItemTable[item.Key].Count)
                {
                    item.Value.ForEach(o => mItemTable[item.Key].Add(o));
                }
            }
        }
        public T RandomItem()
        {
            float sum = 0f;
            float probablity = (float)mRandom.NextDouble();

            if (mItemTable.All(o => o.Value.Count == 0))
            {
                mEmptyItemList?.Invoke(this);
            }
            foreach (var item in mItemTable)
            {
                if (item.Value.Count == 0) continue;

                sum += item.Key + mComplement;

                if (probablity <= sum)
                {
                    int index = mRandom.Next(0, item.Value.Count);

                    T value = item.Value         [index];
                              item.Value.RemoveAt(index);

                    if (item.Value.Count == 0)
                    {
                        mComplement += (item.Key + mComplement) / mItemTable.Count(o => o.Value.Count > 0);
                    }
                    return value;
                }
            }
            return default;
        }
    }
}
