using System.Collections.Immutable;
using System.Text;

using AndrejKrizan.Common.Extensions;
using AndrejKrizan.Common.ValueObjects.Pointables;
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
            ImmutableArray<Pointable> pointables = ImmutableArray.Create(pointable);
            PointableArray pointableArray = new(pointables);
            return pointableArray;
        }

        public Pointable CreatePointable(IReadOnlyCollection<string> collection)
        {
            ImmutableArray<Pointable> pointables = collection.Convert(CreatePointableInternal);
            PointableArray pointableArray = new(pointables);
            return pointableArray;
        }

        public Pointable CreatePointable(IReadOnlyCollection<IReadOnlyCollection<string>> matrix)
        {
            ImmutableArray<Pointable> pointableArrays = matrix.Convert(CreatePointable);
            PointableArray pointableMatrix = new(pointableArrays);
            return pointableMatrix;
        }

        // Protected methods
        protected override long CreateInternal()
        {
            long id = H5T.create(H5T.class_t.STRING, H5T.VARIABLE)
                .ValidateHDFId(() => $"create a variable-length string type for {Describe()}");
            H5T.set_cset(id, H5T.cset_t.UTF8)
                .ValidateHDFResponse(() => $"set the character set to UTF8 for {Describe()}");
            H5T.set_strpad(id, H5T.str_t.NULLTERM)
                .ValidateHDFResponse(() => $"set the padding to null term for {Describe()}");
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
