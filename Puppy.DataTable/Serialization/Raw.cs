namespace Puppy.DataTable.Serialization
{
    public class Raw
    {
        private string _value;

        public Raw(string value)
        {
            _value = value;
        }

        public override string ToString()
        {
            return _value;
        }
    }
}