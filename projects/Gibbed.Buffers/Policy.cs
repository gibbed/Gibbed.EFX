// https://gist.github.com/ladeak/71b0b4c59bdd4eb548535dc641729682#file-pooledarraybufferwriter-cs

using Microsoft.Extensions.ObjectPool;

namespace Gibbed.Buffers
{
    public class Policy : IPooledObjectPolicy<PooledArrayBufferWriter<byte>>
    {
        public PooledArrayBufferWriter<byte> Create()
        {
            return new();
        }

        public bool Return(PooledArrayBufferWriter<byte> obj)
        {
            obj.Clear();
            return true;
        }
    }
}
