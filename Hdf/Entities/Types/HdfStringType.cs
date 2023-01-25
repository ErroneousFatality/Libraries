using System.Text;

using AndrejKrizan.DotNet.ValueObjects.Pointables;
using AndrejKrizan.Hdf.Entities.Objects;
using AndrejKrizan.Hdf.Extensions;

using HDF.PInvoke;

namespace AndrejKrizan.Hdf.Entities.Types
{
    public class HdfStringType : HdfObject, IHdfType<string>
    {
        // Constructors
        public HdfStringType() { }

        // Methods
        public override string Describe()
            => $"string";

        public Pointable CreatePointable(string value)
        {
            Pointable pointable = CreatePointableInternal(value);
            PointableArray pointableArray = new(pointable);
            return pointableArray;
        }

        public Pointable CreatePointable(IEnumerable<string> collection)
        {
            IEnumerable<Pointable> pointables = collection.Select(CreatePointableInternal);
            PointableArray pointableArray = new(pointables);
            return pointableArray;
        }

        public Pointable CreatePointable<TRow>(IEnumerable<TRow> matrix)
            where TRow: IEnumerable<string>
        {
            IEnumerable<Pointable> pointableArrays = matrix.Select(row => CreatePointable(row));
            PointableArray pointableMatrix = new(pointableArrays);
            return pointableMatrix;
        }

        // Protected methods
        protected override long CreateInternal()
        {
            long id = H5T.create(H5T.class_t.STRING, H5T.VARIABLE)
                .ValidateHdfId(() => $"create a variable-length string type for {Describe()}");
            H5T.set_cset(id, H5T.cset_t.UTF8)
                .ValidateHdfResponse(() => $"set the character set to UTF8 for {Describe()}");
            H5T.set_strpad(id, H5T.str_t.NULLTERM)
                .ValidateHdfResponse(() => $"set the padding to null term for {Describe()}");
            return id;
        }

        protected override long OpenInternal()
            => CreateInternal();

        protected override int CloseInternal()
            => H5T.close(Id);

        // Private methods
        private static Pointable CreatePointableInternal(string value)
        {
            byte[] bytes = ConvertToBytes(value);
            Pointable pointable = new(bytes);
            return pointable;
        }

        private static byte[] ConvertToBytes(string value)
            => Encoding.UTF8.GetBytes(value + char.MinValue);
    }
}
