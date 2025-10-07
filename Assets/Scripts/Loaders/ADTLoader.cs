using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ADTLoader : AsyncObject
{
    public ADT ADT { get; private set; }

    public ADTLoader(MPQManager mpqManager, string filePath) : base(mpqManager, filePath)
    {
        
    }

    public override void OnFileLoaded(MPQFile file)
    {
        ADT = new ADT(file);



        IsReady = true;
    }
}
