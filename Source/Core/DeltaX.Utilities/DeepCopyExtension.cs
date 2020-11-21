using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace DeltaX.Utilities
{
    public static class DeepCopyExtension
    {
        public static object DeepCloneObject(this object original)
        {
            if (original == null)
                return null;

            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Context = new StreamingContext(StreamingContextStates.Clone);
                formatter.Serialize(stream, original);
                stream.Position = 0;
                return formatter.Deserialize(stream);
            }
        }


        public static TObject DeepClone<TObject>(this TObject original) where TObject : class,ISerializable
        {
            if (original == null)
                return default;

            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Context = new StreamingContext(StreamingContextStates.Clone);
                formatter.Serialize(stream, original);
                stream.Position = 0;
                return formatter.Deserialize(stream) as TObject;
            }
        }
    }
}
