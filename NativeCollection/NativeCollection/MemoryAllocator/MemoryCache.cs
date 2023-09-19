using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace NativeCollection
{
    public unsafe partial struct MemoryCache: IDisposable
    {
        // 最大维护的空slab 多出的空slab直接释放
        public int MaxUnUsedSlabs;
        
        public int ItemSize;

        public int BlockSize;

        public SlabLinkedList InUsedSlabs;

        public SlabLinkedList UnUsedSlabs;
        public MemoryCache* Self
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return (MemoryCache*)Unsafe.AsPointer(ref this); }
        }
        
        internal static MemoryCache* CreateInternal(int blockSize, int itemSize , int maxUnUsedSlabs)
        {
            MemoryCache* memoryCache = (MemoryCache*)NativeMemoryHelper.Alloc((UIntPtr)Unsafe.SizeOf<MemoryCache>());
            memoryCache->ItemSize = itemSize;
            memoryCache->BlockSize = blockSize;
            memoryCache->MaxUnUsedSlabs = maxUnUsedSlabs;
            Slab* initSlab = Slab.Create(blockSize, itemSize,null,null);
            memoryCache->InUsedSlabs = new SlabLinkedList(initSlab);
            memoryCache->UnUsedSlabs = new SlabLinkedList(null);
            return memoryCache;
        }

        public static MemoryCache* Create(int blockSize, int itemSize , int maxUnUsedSlabs = 3)
        {
            MemoryCache* memoryPool = (MemoryCache*)NativeMemoryHelper.Alloc((UIntPtr)Unsafe.SizeOf<MemoryCache>());
            memoryPool->ItemSize = itemSize;
            memoryPool->BlockSize = blockSize;
            memoryPool->MaxUnUsedSlabs = maxUnUsedSlabs;
            Slab* initSlab = Slab.Create(blockSize, itemSize,null,null);
            memoryPool->InUsedSlabs = new SlabLinkedList(initSlab);
            memoryPool->UnUsedSlabs = new SlabLinkedList(null);
            return memoryPool;
        }

        public void* Alloc()
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

        internal void FreeListNode(Slab* slab, ListNode* listNode)
        {
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
                
                // 释放多于的空slab
                if (UnUsedSlabs.SlabCount>MaxUnUsedSlabs)
                {
                    var bottomSlab = UnUsedSlabs.Bottom;
                    UnUsedSlabs.SplitOut(bottomSlab);
                    bottomSlab->Dispose();
                }
            }
                
            // 对应slab移至链表头部
            if (slab!=InUsedSlabs.Top)
            {
                InUsedSlabs.SplitOut(slab);
                InUsedSlabs.AddToTop(slab);
            }
        }

        public void Free(void* ptr)
        {
            Debug.Assert(ptr!=null);
            ListNode* listNode = (ListNode*)((byte*)ptr - Unsafe.SizeOf<ListNode>());
            Debug.Assert(listNode!=null);
            Slab* slab = listNode->ParentSlab;
            FreeListNode(slab, listNode);
        }

        public void ReleaseUnUsedSlabs()
        {
            Slab* unUsedSlab = UnUsedSlabs.Top;
            while (unUsedSlab!=null)
            {
                Slab* currentSlab = unUsedSlab;
                unUsedSlab = unUsedSlab->Next;
                currentSlab->Dispose();
            }
            UnUsedSlabs = new SlabLinkedList(null);
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

            ReleaseUnUsedSlabs();

            if (Self!=null)
            {
                NativeMemoryHelper.Free(Self);
                NativeMemoryHelper.RemoveNativeMemoryByte(Unsafe.SizeOf<MemoryCache>());
            }
        }
    }
}

