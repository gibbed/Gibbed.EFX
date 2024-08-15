// https://gist.github.com/ladeak/71b0b4c59bdd4eb548535dc641729682#file-chainedarraybufferwriter-cs

using System;
using System.Buffers;

namespace Gibbed.Buffers
{
    public sealed class ChainedArrayBufferWriter<T> : IBufferWriter<T>, IDisposable
    {
        private const int DefaultInitialBufferSize = 4096 * 2;
        private static MemorySegment<T> _Empty = new(Array.Empty<T>());
        private MemorySegment<T> _Head;
        private MemorySegment<T> _Tail;

        public ChainedArrayBufferWriter()
        {
            this._Head = this._Tail = _Empty;
        }

        public int Length => this._Tail.RunningIndex + this._Tail.Index;

        public void CopyTo(Span<T> destination)
        {
            if (this.Length > destination.Length)
            {
                throw new AggregateException(nameof(destination));
            }
            var current = _Head.NextSegment;
            while (current != null)
            {
                current.Span.CopyTo(destination);
                destination = destination.Slice(current.Index);
                current = current.NextSegment;
            }
        }

        public T[] ToArray()
        {
            var destination = new T[Length];
            this.CopyTo(destination.AsSpan());
            return destination;
        }

        public void Clear()
        {
            var current = this._Head.NextSegment;
            while (current != null)
            {
                ArrayPool<T>.Shared.Return(current.Buffer);
                current = current.NextSegment;
            }
            this._Head = this._Tail = _Empty;
        }

        public void Advance(int count) => this._Tail.Advance(count);

        public Memory<T> GetMemory(int sizeHint = 0)
        {
            this.CheckAndExapandBuffer(sizeHint);
            return this._Tail.WritableMemory;
        }

        public Span<T> GetSpan(int sizeHint = 0)
        {
            this.CheckAndExapandBuffer(sizeHint);
            return this._Tail.WritableSpan;
        }

        private void CheckAndExapandBuffer(int sizeHint)
        {
            if (sizeHint < 0)
            {
                throw new ArgumentException(nameof(sizeHint));
            }
            if (sizeHint > this._Tail.FreeCapacity)
            {
                sizeHint = Math.Max(sizeHint, DefaultInitialBufferSize);
                var temp = ArrayPool<T>.Shared.Rent(sizeHint);
                this._Tail = this._Tail.Append(temp);
            }
        }

        public void Dispose() => Clear();
    }
}
