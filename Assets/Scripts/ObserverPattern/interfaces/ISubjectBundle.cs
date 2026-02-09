using Assets.Annotations;
using Assets.Scripts.Interfaces.Mediator;

namespace Assets.Scripts.ObserverPattern.interfaces
{
    public interface ISubjectBundle
    {
        public IRequest Subject { get; set; }
        public SubjectAttribute SubjectAttribute { get; }
    }

    public interface ISubjectBundle<T>
    {
        public IRequest<T> Subject { get; set; }
        public SubjectAttribute SubjectAttribute { get; }
    }
}
