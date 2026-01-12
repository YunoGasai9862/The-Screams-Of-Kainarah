using Assets.Annotations;

namespace Assets.Scripts.ObserverPattern.models
{
    public class SubjectBundle
    {
        public SubjectContext SubjectContext { get; set; }

        public SubjectAttribute SubjectAttribute { get; set; }

        public override string ToString()
        {
            return $"SubjectAttribute: {SubjectAttribute}, SubjectInstance : {SubjectContext.Instance}";
        }
    }
}
