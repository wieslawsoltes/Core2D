using System.Runtime.Serialization;

namespace Core2D.Renderer
{
    [DataContract(IsReference = true)]
    public class ImageKey : IImageKey
    {
        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public string Key { get; set; }
    }
}
