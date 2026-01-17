using Assets.Annotations;
using Assets.Scripts.Interfaces;
using Assets.Scripts.ObserverPattern.interfaces;

namespace Assets.Scripts.ObserverPattern.models
{
    public class SubjectBundle<T>: ISubjectBundle<T>
    {
        public IRequest<T> Subject { get; set; }

        public SubjectAttribute SubjectAttribute { get; set; }

        public override string ToString()
        {
            return $"SubjectAttribute: {SubjectAttribute}, Subject : {Subject}";
        }
    }

    public class SubjectBundle : ISubjectBundle
    {
        public IRequest Subject { get; set; }

        public SubjectAttribute SubjectAttribute { get; set; }

        public override string ToString()
        {
            return $"SubjectAttribute: {SubjectAttribute}, Subject : {Subject}";
        }
    }
}
