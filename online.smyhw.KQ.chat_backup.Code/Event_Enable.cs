using Native.Sdk.Cqp.EventArgs;
using Native.Sdk.Cqp.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace online.smyhw.KQ.chat_backup.Code
{
    public class Event_Enable : IAppEnable
    {
        public void AppEnable(object sender, CQAppEnableEventArgs e)
        {
            Event_GroupMessage.GroupList = new Hashtable();
        }
    }
}
