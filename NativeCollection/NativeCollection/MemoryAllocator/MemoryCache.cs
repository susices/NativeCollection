using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace NativeCollection
{
    
    public unsafe partial struct MemoryCache: IDisposable
    {
        // 最大维护的空slab 多出的空slab直接释放
        public uint MaxUnUsedSlabs;
        
        public uint ItemSize;

        public uint BlockSize;

        public SlabLinkedList InUsedSlabs;

        public SlabLinkedList UnUsedSlabs;
        public MemoryCache* Self
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return (MemoryCache*)Unsafe.AsPointer(ref this);}
        }

        internal IntPtr LockObjPtr;

        private object LockObj => GCHandle.FromIntPtr(LockObjPtr).Target;
        
        internal static MemoryCache* CreateInternal(uint blockSize, uint itemSize , uint maxUnUsedSlabs)
        {
            MemoryCache* memoryCache = (MemoryCache*)NativeMemoryHelper.Alloc((UIntPtr)Unsafe.SizeOf<MemoryCache>());
            memoryCache->ItemSize = itemSize;
            memoryCache->BlockSize = blockSize;
            memoryCache->MaxUnUsedSlabs = maxUnUsedSlabs;
            Slab* initSlab = Slab.Create(blockSize, itemSize,null,null);
            memoryCache->InUsedSlabs = new SlabLinkedList(initSlab);
            memoryCache->UnUsedSlabs = new SlabLinkedList(null);
            memoryCache->LockObjPtr = MemoryAllocator.GetCacheLockObjPtr();
            return memoryCache;
        }

        public static MemoryCache* CreateForMemoryPool(uint blockSize, uint itemSize , uint maxUnUsedSlabs = 3)
        {
            MemoryCache* memoryPool = (MemoryCache*)NativeMemoryHelper.Alloc((UIntPtr)Unsafe.SizeOf<MemoryCache>());
            memoryPool->ItemSize = itemSize;
            memoryPool->BlockSize = blockSize;
            memoryPool->MaxUnUsedSlabs = maxUnUsedSlabs;
            Slab* initSlab = Slab.Create(blockSize, itemSize,null,null);
            memoryPool->InUsedSlabs = new SlabLinkedList(initSlab);
            memoryPool->UnUsedSlabs = new SlabLinkedList(null);
            memoryPool->LockObjPtr = IntPtr.Zero;
            return memoryPool;
        }

        public void* Alloc()
        {
            if (!MemoryAllocator.UsedInMultiThread || LockObjPtr== IntPtr.Zero)
            {
                return AllocInternal();
            }
            lock (LockObj)
            {
                return AllocInternal();
            }
        }
        
        public void Free(void* ptr)
        {
            Debug.Assert(ptr!=null);
            ListNode* listNode = (ListNode*)((byte*)ptr - Unsafe.SizeOf<ListNode>());
            Debug.Assert(listNode!=null);
            Slab* slab = listNode->ParentSlab;
            if (!MemoryAllocator.UsedInMultiThread || LockObjPtr == IntPtr.Zero)
            {
                FreeInternal(slab, listNode);
                return;
            }
            
            lock (LockObj)
            {
                //Console.WriteLine($"lockObj: {(nuint)LockObjPtr.ToPointer()} slab:{slab->ItemSize}");
                FreeInternal(slab, listNode);
            }
        }

        public void ReleaseUnUsedSlabs()
        {
            if (!MemoryAllocator.UsedInMultiThread || LockObjPtr == IntPtr.Zero)
            {
                ReleaseUnUsedSlabsInternal();
                return;
            }
            
            lock (LockObj)
            {
                ReleaseUnUsedSlabsInternal();
            }
        }

        private void ReleaseUnUsedSlabsInternal()
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
        


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void* AllocInternal()
        {
            Debug.Assert(InUsedSlabs.Top!=null && InUsedSlabs.Top->FreeSize>0, $"InUsedSlabs.Top!=null:{InUsedSlabs.Top!=null} InUsedSlabs.Top->FreeSize>0:{InUsedSlabs.Top->FreeSize}");

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
        
        private void FreeInternal(Slab* slab, ListNode* listNode)
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

