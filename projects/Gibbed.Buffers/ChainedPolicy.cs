// https://gist.github.com/ladeak/71b0b4c59bdd4eb548535dc641729682#file-chainedarraybufferwriter-cs

using Microsoft.Extensions.ObjectPool;

namespace Gibbed.Buffers
{
    public class ChainedPolicy : IPooledObjectPolicy<ChainedArrayBufferWriter<byte>>
    {
        public ChainedArrayBufferWriter<byte> Create()
        {
            return new();
        }

        public bool Return(ChainedArrayBufferWriter<byte> obj)
        {
            obj.Clear();
            return true;
        }
    }
}
