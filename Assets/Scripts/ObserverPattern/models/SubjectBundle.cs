using Assets.Annotations;
using Assets.Scripts.Interfaces;

namespace Assets.Scripts.ObserverPattern.models
{
    public class SubjectBundle<T>: ISub
    {
        public IRequest<T> Subject { get; set; }

        public SubjectAttribute SubjectAttribute { get; set; }

        public override string ToString()
        {
            return $"SubjectAttribute: {SubjectAttribute}, Subject : {Subject}";
        }
    }
}
