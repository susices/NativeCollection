using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NativeCollection.UnsafeType
{
    public unsafe partial struct MemoryPool
    {
        public struct Slab : IDisposable
        {
            public int BlockSize;
            
            public int FreeSize;

            public int ItemSize;

            public ListNode* FreeList;

            public Slab* Prev;

            public Slab* Next;

            public Slab* Self => (Slab*)Unsafe.AsPointer(ref this);

            public static Slab* Create(int blockSize,int itemSize,Slab* prevSlab , Slab* nextSlab )
            {
                int slabSize =Unsafe.SizeOf<Slab>() + (itemSize + Unsafe.SizeOf<IntPtr>()) * blockSize;
                byte* slabBuffer  = (byte*)NativeMemoryHelper.Alloc((UIntPtr)slabSize);
                Slab* slab = (Slab*)slabBuffer;
                slab->BlockSize = blockSize;
                slab->FreeSize = blockSize;
                slab->ItemSize = itemSize;
                slab->Prev = prevSlab;
                slab->Next = nextSlab;
                slabBuffer+=Unsafe.SizeOf<Slab>();

                ListNode* next = null;
                for (int i = blockSize-1; i >= 0; i--)
                {
                    ListNode* listNode = (ListNode*)slabBuffer[i];
                    listNode->Next = next;
                    next = listNode;
                }

                slab->FreeList = next;
                return slab;
            }

            public byte* Alloc()
            {
                Debug.Assert(FreeList!=null && FreeSize>0);
                FreeSize--;
                ListNode* node = FreeList;
                FreeList = FreeList->Next;
                node->ParentSlab = Self;
                node += 1;
                return (byte*)node;
            }

            public void Free(ListNode* node)
            {
                Debug.Assert(FreeSize<BlockSize && node!=null);
                FreeSize++;
                node->Next = FreeList;
                FreeList = node;
            }

            public bool IsAllFree()
            {
                return FreeSize == ItemSize;
            }

            public bool IsAllAlloc()
            {
                return FreeSize == 0;
            }
            
            public void Dispose()
            {
                int slabSize =Unsafe.SizeOf<Slab>() + (ItemSize + Unsafe.SizeOf<IntPtr>()) * BlockSize;
                NativeMemoryHelper.Free(Self);
                NativeMemoryHelper.RemoveNativeMemoryByte(slabSize);
            }
        }
        
        [StructLayout(LayoutKind.Explicit)]
        public struct ListNode
        {
            [FieldOffset(0)]
            public ListNode* Next;
            [FieldOffset(0)]
            public Slab* ParentSlab;
        }
        
        public struct SlabLinkedList
        {
            public Slab* Top;
            public Slab* Bottom;

            public SlabLinkedList(Slab* initSlab)
            {
                Top = initSlab;
                Bottom = initSlab;
            }

            public void MoveTopToBottom()
            {
                Debug.Assert(Top!=null && Bottom!=null);
                if (Top==Bottom)
                {
                    return;
                }

                Slab* oldTop = Top;
                Top = Top->Next;
                Top->Prev = null;
                Bottom->Next = oldTop;
                oldTop->Prev = Bottom;
                oldTop->Next = null;
                Bottom = oldTop;
            }

            public void SplitOut(Slab* splitSlab)
            {
                Debug.Assert(splitSlab!=null && Top!=null && Bottom!=null);

                // 只有一个slab
                if (Top==Bottom)
                {
                    splitSlab->Prev = null;
                    splitSlab->Next = null;
                    Top = null;
                    Bottom = null;
                    return;
                }
                
                // 链表头部
                if (splitSlab == Top)
                {
                    Top = splitSlab->Next;
                    splitSlab->Next = null;
                    return;
                }

                if (splitSlab == Bottom)
                {
                    Bottom = splitSlab->Prev;
                    Bottom->Next = null;
                    splitSlab->Prev = null;
                    return;
                }

                splitSlab->Prev->Next = splitSlab->Next;
                splitSlab->Prev = null;
                splitSlab->Next = null;
            }

            public void AddToTop(Slab* slab)
            {
                if (Top == Bottom)
                {
                    if (Top==null)
                    {
                        Top = slab;
                        Bottom = slab;
                        slab->Prev = null;
                        slab->Next = null;
                    }
                    else
                    {
                        Slab* oldTop = Top;
                        Top = slab;
                        Top->Next = oldTop;
                        Top->Prev = null;
                        oldTop->Prev = Top;
                    }
                }
            }
        }
    }
}
