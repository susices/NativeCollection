using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NativeCollection;

internal enum NodeColor : byte
{
    Black,
    Red
}

internal enum TreeRotation : byte
{
    Left,
    LeftRight,
    Right,
    RightLeft
}

public unsafe partial class SortedSet<T>
{
    internal struct Node : IEquatable<Node>
    {
        public T Item;

        public Node* Self;

        public NodeColor Color;

        public Node* Left;

        public Node* Right;

        public bool IsBlack => Color == NodeColor.Black;

        public bool IsRed => Color == NodeColor.Red;

        public bool Is2Node => IsBlack && IsNullOrBlack(Left) && IsNullOrBlack(Right);

        public bool Is4Node => IsNonNullRed(Left) && IsNonNullRed(Right);

        public void ColorBlack()
        {
            Color = NodeColor.Black;
        }

        public void ColorRed()
        {
            Color = NodeColor.Red;
        }

        public static bool IsNonNullBlack(Node* node)
        {
            return node != null && node->IsBlack;
        }

        public static bool IsNonNullRed(Node* node)
        {
            return node != null && node->IsRed;
        }

        public static bool IsNullOrBlack(Node* node)
        {
            return node == null || node->IsBlack;
        }

        public static Node* Create(in T item, NodeColor nodeColor)
        {
            var node = (Node*)NativeMemory.Alloc((uint)Unsafe.SizeOf<Node>());
            node->Self = node;
            node->Item = item;
            node->Color = nodeColor;
            node->Left = null;
            node->Right = null;
            return node;
        }

        public struct NodeSourceTarget : IEquatable<NodeSourceTarget>
        {
            public Node* Source;
            public Node* Target;

            public NodeSourceTarget(Node* source, Node* target)
            {
                Source = source;
                Target = target;
            }

            public bool Equals(NodeSourceTarget other)
            {
                return Source == other.Source && Target == other.Target;
            }

            public override bool Equals(object? obj)
            {
                return obj is NodeSourceTarget other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(unchecked((int)(long)Source), unchecked((int)(long)Target));
            }
        }

        public Node* DeepClone(int count)
        {
#if DEBUG
            Debug.Assert(count == GetCount());
#endif
            var newRoot = ShallowClone();

            var pendingNodes = Internal.Stack<NodeSourceTarget>.Create(2 * Log2(count) + 2);
            pendingNodes->Push(new NodeSourceTarget(Self, newRoot));

            while (pendingNodes->TryPop(out var next))
            {
                Node* clonedNode;

                var left = next.Source->Left;
                var right = next.Source->Right;
                if (left != null)
                {
                    clonedNode = left->ShallowClone();
                    next.Target->Left = clonedNode;
                    pendingNodes->Push(new NodeSourceTarget(left, clonedNode));
                }

                if (right != null)
                {
                    clonedNode = right->ShallowClone();
                    next.Target->Right = clonedNode;
                    pendingNodes->Push(new NodeSourceTarget(right, clonedNode));
                }
            }

            pendingNodes->Dispose();
            NativeMemoryHelper.Free(pendingNodes);
            GC.RemoveMemoryPressure(Unsafe.SizeOf<Internal.Stack<NodeSourceTarget>>());
            return newRoot;
        }

        public TreeRotation GetRotation(Node* current, Node* sibling)
        {
            Debug.Assert(IsNonNullRed(sibling->Left) || IsNonNullRed(sibling->Right));
#if DEBUG
            Debug.Assert(HasChildren(current, sibling));
#endif
            var currentIsLeftChild = Left == current;
            return IsNonNullRed(sibling->Left) ? currentIsLeftChild ? TreeRotation.RightLeft : TreeRotation.Right :
                currentIsLeftChild ? TreeRotation.Left : TreeRotation.LeftRight;
        }

        public Node* GetSibling(Node* node)
        {
            Debug.Assert(node != null);
            Debug.Assert((node == Left) ^ (node == Right));

            return node == Left ? Right : Left;
        }

        public Node* ShallowClone()
        {
            return Create(Item, Color);
        }

        public void Split4Node()
        {
            Debug.Assert(Left != null);
            Debug.Assert(Right != null);

            ColorRed();
            Left->ColorBlack();
            Right->ColorBlack();
        }

        public Node* Rotate(TreeRotation rotation)
        {
            Node* removeRed;
            switch (rotation)
            {
                case TreeRotation.Right:
                    removeRed = Left == null ? Left : Left->Left;
                    Debug.Assert(removeRed->IsRed);
                    removeRed->ColorBlack();
                    return RotateRight();
                case TreeRotation.Left:
                    removeRed = Right == null ? Right : Right->Right!;
                    Debug.Assert(removeRed->IsRed);
                    removeRed->ColorBlack();
                    return RotateLeft();
                case TreeRotation.RightLeft:
                    Debug.Assert(Right->Left->IsRed);
                    return RotateRightLeft();
                case TreeRotation.LeftRight:
                    Debug.Assert(Left->Right->IsRed);
                    return RotateLeftRight();
                default:
                    Debug.Fail($"{nameof(rotation)}: {rotation} is not a defined {nameof(TreeRotation)} value.");
                    return null;
            }
        }

        public Node* RotateLeft()
        {
            var child = Right;
            Right = child->Left;
            child->Left = Self;
            return child;
        }

        public Node* RotateLeftRight()
        {
            var child = Left;
            var grandChild = child->Right!;

            Left = grandChild->Right;
            grandChild->Right = Self;
            child->Right = grandChild->Left;
            grandChild->Left = child;
            return grandChild;
        }

        public Node* RotateRight()
        {
            var child = Left;
            Left = child->Right;
            child->Right = Self;
            return child;
        }

        public Node* RotateRightLeft()
        {
            var child = Right;
            var grandChild = child->Left;

            Right = grandChild->Left;
            grandChild->Left = Self;
            child->Left = grandChild->Right;
            grandChild->Right = child;
            return grandChild;
        }

        public void Merge2Nodes()
        {
            Debug.Assert(IsRed);
            Debug.Assert(Left->Is2Node);
            Debug.Assert(Right->Is2Node);

            // Combine two 2-nodes into a 4-node.
            ColorBlack();
            Left->ColorRed();
            Right->ColorRed();
        }

        public void ReplaceChild(Node* child, Node* newChild)
        {
#if DEBUG
            Debug.Assert(HasChild(child));
#endif

            if (Left == child)
                Left = newChild;
            else
                Right = newChild;
        }


#if DEBUG
        private int GetCount()
        {
            var value = 1;
            if (Left != null) value += Left->GetCount();

            if (Right != null) value += Right->GetCount();
            return value;
        }

        private bool HasChild(Node* child)
        {
            return child == Left || child == Right;
        }

        private bool HasChildren(Node* child1, Node* child2)
        {
            Debug.Assert(child1 != child2);

            return (Left == child1 && Right == child2)
                   || (Left == child2 && Right == child1);
        }
#endif

        public bool Equals(Node other)
        {
            return Item.Equals(other.Item) && Self == other.Self && Color == other.Color && Left == other.Left &&
                   Right == other.Right;
        }

        public override bool Equals(object? obj)
        {
            return obj is Node other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Item, unchecked((int)(long)Self), (int)Color, unchecked((int)(long)Left),
                unchecked((int)(long)Right));
        }
    }
}