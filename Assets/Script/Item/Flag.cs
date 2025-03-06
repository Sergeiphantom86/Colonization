public class Flag : Item
{
    private string _nameParentBase;
    public string NameParentBase {  get { return _nameParentBase; } }

    public void Initialize(string nameParentBase)
    {
        _nameParentBase = nameParentBase;
    }
}