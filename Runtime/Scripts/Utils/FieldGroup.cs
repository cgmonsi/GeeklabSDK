[System.AttributeUsage(System.AttributeTargets.Property)]
public class FieldGroup : System.Attribute
{
    public string GroupName { get; private set; }

    public FieldGroup(string groupName)
    {
        this.GroupName = groupName;
    }
}