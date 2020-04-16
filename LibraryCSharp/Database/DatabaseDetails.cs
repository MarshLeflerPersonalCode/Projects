using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Database
{
    /// <summary>
    /// A database is a directory of multiple single files representing a class.
    /// The master database keeps all those databases in check.
    /// THE UGLY
    /// ---------------------------------
    /// We need to have this in sync with the game so we need to define the databases in a way that 
    /// can be shared between the game and c# side of things.
    /// 
    /// I ended up doing this based on a config file. Each database will need to point to a folder
    /// that contains the config file.
    /// 
    /// The master database can do this automatically for you by specifying the top folder.
    /// 
    /// c:\databases\ (master points here)
    /// c:\databases\monsters\database.xml (defines monsters database)
    /// c:\databases\items\database.xml (defines items)
    /// </summary>

}
