using System;
using System.Collections.Generic;

namespace MonoScratch.Runtime {

    public class MonoScratchList {

        public static MonoScratchValue INVALID_RETURN = new MonoScratchValue("");

        public List<MonoScratchValue> Contents;

        public int Length => Contents.Count;

        public MonoScratchList() {
            Contents = new List<MonoScratchValue>();
        }

        private int ToRealIndex(double index) {
            return (int) Math.Floor(index - 1);
        }

        public void Add(MonoScratchValue value) {
            Contents.Add(value);
        }

        public void Delete(double index) {
            if (index <= 0 || index > Length)
                return;
            Contents.RemoveAt(ToRealIndex(index));
        }

        public void Insert(double index, MonoScratchValue value) {
            if (index <= 0 || index > Length + 1)
                return;
            Contents.Insert(ToRealIndex(index), value);
        }

        public void Replace(double index, MonoScratchValue value) {
            if (index <= 0 || index > Length)
                return;
            Contents[ToRealIndex(index)] = value;
        }

        public void DeleteAll() {
            Contents.Clear();
        }

        public MonoScratchValue Get(double index) {
            if (index <= 0 || index > Length)
                return INVALID_RETURN;
            return Contents[ToRealIndex(index)];
        }

        public int IndexOf(MonoScratchValue value) {
            for (int i = 0; i < Length; i++) {
                if (Contents[i].Compare(value) == 0)
                    return i + 1;
            }
            return 0;
        }

        public bool ContainsItem(MonoScratchValue value) {
            return IndexOf(value) != 0;
        }

    }
}