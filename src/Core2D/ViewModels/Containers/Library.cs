using System.Runtime.Serialization;

namespace Core2D.Containers
{
    [DataContract(IsReference = true)]
    public abstract class Library : ObservableObject
    {
    }
}
