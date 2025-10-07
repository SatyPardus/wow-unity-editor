using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class AsyncObject : IAsyncObject
{
    public int Refs { get; private set; }
    public bool IsLoading { get; private set; }
    public bool IsLoaded { get; private set; }
    public bool IsReady { get; protected set; }

    public Action<AsyncObject> OnFinished;

    private MPQManager mpqManager;
    private string filePath;

    public AsyncObject(MPQManager mpqManager, string filePath)
    {
        this.mpqManager = mpqManager;
        this.filePath = filePath;

        Refs = 1;

        IsLoaded = false;
        IsLoading = false;
        IsReady = false;
    }

    public void StartLoading()
    {
        IsLoading = true;

        var file = mpqManager.LoadFile(filePath);
        if (file == null)
        {
            return;
        }

        OnFileLoaded(file);
    }

    public virtual void OnFileLoaded(MPQFile file)
    {

    }

    public void SetLoaded()
    {
        IsLoading = false;
        IsLoaded = true;
        IsReady = false;
        OnFinished?.Invoke(this);
    }

    public void AddRef()
    {
        Refs++;
    }

    public void RemoveRef()
    {
        Refs--;
    }
}
