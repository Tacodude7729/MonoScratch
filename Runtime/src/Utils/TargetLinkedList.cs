using System.Collections.Generic;

namespace MonoScratch.Runtime {
    // Over engineered? Maybe.
    // Fast? Probably not.
    // Stylish? Yes
    public class TargetLinkedList {

        public Node? First, Last;
        public int Count { get; private set; }

        public TargetLinkedList() { }

        public IEnumerable<IMonoScratchTarget> Forward() {
            Node? node = First;
            while (node != null) {
                yield return node.Sprite;
                node = node.After;
            }
        }

        public IEnumerable<IMonoScratchTarget> Backward() {
            Node? node = Last;
            while (node != null) {
                yield return node.Sprite;
                node = node.Before;
            }
        }

        public Node InsertLast(IMonoScratchTarget other) {
            Node node = new Node(this, other);
            if (Last == null) {
                Last = node;
                First = node;
            } else {
                Last.After = node;
                node.Before = Last;
                Last = node;
            }
            ++Count;
            return node;
        }

        public Node InsertFirst(IMonoScratchTarget other) {
            Node node = new Node(this, other);
            if (First == null) {
                Last = node;
                First = node;
            } else {
                First.Before = node;
                node.After = First;
                First = node;
            }
            ++Count;
            return node;
        }

        public class Node {
            public Node? Before, After;
            public IMonoScratchTarget Sprite;
            public readonly TargetLinkedList List;

            public Node(TargetLinkedList list, IMonoScratchTarget sprite) {
                Sprite = sprite;
                List = list;
            }

            public void Remove() {
                if (Before != null)
                    Before.After = After;
                if (After != null)
                    After.Before = Before;
                Before = null;
                After = null;
                --List.Count;
            }

            public Node InsertBefore(IMonoScratchTarget other) {
                Node node = new Node(List, other);
                if (Before != null) {
                    Before.After = node;
                    node.Before = Before;
                }
                Before = node;
                node.After = this;
                ++List.Count;
                return node;
            }

            public Node InsertAfter(IMonoScratchTarget other) {
                Node node = new Node(List, other);
                if (After != null) {
                    After.Before = node;
                    node.After = After;
                }
                After = node;
                node.Before = this;
                ++List.Count;
                return node;
            }
        }
    }
}