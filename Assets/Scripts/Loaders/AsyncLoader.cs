using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class AsyncLoader
{
    private MPQManager mpqManager;
    private Dictionary<string, IAsyncObject> objectCache = new Dictionary<string, IAsyncObject>();
    private List<IAsyncObject> loadingObjects = new List<IAsyncObject>();

    public AsyncLoader(MPQManager mpqManager)
    {
        this.mpqManager = mpqManager;
    }

    public void Update()
    {
        foreach (var item in loadingObjects.ToArray())
        {
            if(item.IsReady)
            {
                item.SetLoaded();
                loadingObjects.Remove(item);
            }
        }
    }

    public T LoadFile<T>(string filePath) where T : IAsyncObject
    {
        if(objectCache.TryGetValue(filePath, out var asyncObject))
        {
            if(typeof(T) != asyncObject.GetType())
            {
                throw new NotSupportedException($"Wrong type {asyncObject.GetType()} -> Expected {typeof(T)}");
            }
            asyncObject.AddRef();
            return (T)asyncObject;
        }

        var ext = Path.GetExtension(filePath);

        IAsyncObject obj = default(T);
        if (ext == ".adt" && typeof(T) == typeof(ADTLoader))
        {
            obj = new ADTLoader(mpqManager, filePath);
        }
        else
        {
            throw new NotSupportedException($"Extension {ext} not supported for type {typeof(T)}");
        }

        objectCache.Add(filePath, obj);

        return (T)obj;
    }

    public void LoadAsyncObject(IAsyncObject asyncObject)
    {
        if(asyncObject == null || asyncObject.IsLoading || asyncObject.IsLoaded)
        {
            return;
        }

        Task.Run(() =>
        {
            asyncObject.StartLoading();
        });
    }
}
