using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class SubjectAttribute: Attribute
{
    private Type SubjectType { get; set; }   

    public SubjectAttribute(Type subjectType)
    {
        SubjectType = subjectType;
    }

    public Type GetSubjectType()
    {
        return SubjectType;
    }
}
