using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace NativeCollection.UnsafeType
{
    public unsafe partial struct MemoryPool: IDisposable
    {
        public int ItemSize;

        public int BlockSize;

        public SlabLinkedList InUsedSlabs;

        public SlabLinkedList UnUsedSlabs;
        public MemoryPool* Self => (MemoryPool*)Unsafe.AsPointer(ref this);

        public static MemoryPool* Create(int blockSize, int itemSize)
        {
            MemoryPool* memoryPool = (MemoryPool*)NativeMemoryHelper.Alloc((UIntPtr)Unsafe.SizeOf<MemoryPool>());
            memoryPool->ItemSize = itemSize;
            memoryPool->BlockSize = blockSize;
            Slab* initSlab = Slab.Create(blockSize, itemSize,null,null);
            memoryPool->InUsedSlabs = new SlabLinkedList(initSlab);
            memoryPool->UnUsedSlabs = new SlabLinkedList(null);
            return memoryPool;
        }

        public byte* Alloc()
        {
            Debug.Assert(InUsedSlabs.Top!=null && InUsedSlabs.Top->FreeSize>0);
            byte* allocPtr = InUsedSlabs.Top->Alloc();
            
            if (InUsedSlabs.Top->IsAllAlloc())
            {
                InUsedSlabs.MoveTopToBottom();
                
                if (InUsedSlabs.Top->IsAllAlloc())
                {
                    Slab* newSlab = Slab.Create(BlockSize, ItemSize,null,null);
                    InUsedSlabs.AddToTop(newSlab);
                }
            }
            return allocPtr;
        }

        public void Free(byte* ptr)
        {
            Debug.Assert(ptr!=null);
            ListNode* listNode = (ListNode*)(ptr - Unsafe.SizeOf<ListNode>());
            Debug.Assert(listNode!=null);

            Slab* slab = listNode->ParentSlab;
            slab->Free(listNode);
            
            if (slab==InUsedSlabs.Top)
            {
                return;
            }
            
            // 当前链表头为空时 移至空闲链表
            if (InUsedSlabs.Top->IsAllFree())
            {
                Slab* oldTopSlab = InUsedSlabs.Top;
                InUsedSlabs.SplitOut(oldTopSlab);
                UnUsedSlabs.AddToTop(oldTopSlab);
            }
                
            // 对应slab移至链表头部
            if (slab!=InUsedSlabs.Top)
            {
                InUsedSlabs.SplitOut(slab);
                InUsedSlabs.AddToTop(slab);
            }
        }
        
        
        public void Dispose()
        {
            Slab* inUsedSlab = InUsedSlabs.Top;
            while (inUsedSlab!=null)
            {
                Slab* currentSlab = inUsedSlab;
                inUsedSlab = inUsedSlab->Next;
                currentSlab->Dispose();
            }
            
            Slab* unUsedSlab = UnUsedSlabs.Top;
            while (unUsedSlab!=null)
            {
                Slab* currentSlab = unUsedSlab;
                unUsedSlab = unUsedSlab->Next;
                currentSlab->Dispose();
            }

            if (Self!=null)
            {
                NativeMemoryHelper.Free(Self);
                NativeMemoryHelper.RemoveNativeMemoryByte(Unsafe.SizeOf<MemoryPool>());
            }
        }
    }
}

