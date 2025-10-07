using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IAsyncObject
{
    bool IsLoading { get; }
    bool IsLoaded { get; }
    bool IsReady { get; }

    void AddRef();
    void RemoveRef();
    void StartLoading();
    void SetLoaded();
}
