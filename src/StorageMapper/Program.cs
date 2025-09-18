using StorageMapperLib;

public class Program
{
    public static void Main(string[] args)
    {
        var data = DiskInfoProvider.GetDataAsync();
    }
}