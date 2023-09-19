# Hdf
[![latest version](https://img.shields.io/nuget/v/AndrejKrizan.DotNet)](https://www.nuget.org/packages/AndrejKrizan.DotNet)

## Example
```cs
string fileName = Path.GetTempFileName();
HdfFile file = new(fileName, FileAccessType.Create,
    ("neurodata_type", "NWBFile"),
    ("nwb_version", "2.4.0"),
    ("object_id", Guid.NewGuid().ToString())
);
using (file.Create())
{
    file.CreateDataset("file_create_date", DateTime.UtcNow);

    using (HdfGroup groupA = file.CreateGroup("A",
        ("string_attribute", "string"),
        ("DateTime_attribute", DateTime.Now),
        ("Long_attribute", 532L))
    )
    {
        groupA.CreateDataset("bool_value", true, ("description", "scalar bool dataset"));

        ImmutableArray<string> stringCollection = ImmutableArray.Create("one", "two", "three");
        groupA.CreateDataset("string_collection", collection: stringCollection);

        double[][] matrix = new double[][]
        {
            new double[] {1, 2, 3 },
            new double[] {4, 5, 6 }
        };
        groupA.CreateDataset<double, double[]>("double_matrix", matrix,
            ("conversion", 3.33f),
            ("resolution", 5.55d),
            ("unit", "volts")
        );

        groupA.CreateGroup("empty_group", dispose: true, ("description", "empty group"));
    }

    using (HdfGroup groupY = file.CreateGroup("B/X/Y"))
    {
        groupY.CreateAttribute("description", "Group Y");

        using (HdfDataset<int> datasetA = groupY.CreateDataset("Z/A", 123, dispose: false, ("abc", "def")))
        {
            datasetA.CreateAttribute("123", 456);
        }
    }
}
```
Creates
![Generated example HDF file](https://i.imgur.com/2zWMGet.png)