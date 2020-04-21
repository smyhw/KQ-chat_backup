using Native.Sdk.Cqp.EventArgs;
using Native.Sdk.Cqp.Interface;
using Native.Sdk.Cqp.Model;
using System;
using System.Collections;

namespace online.smyhw.KQ.chat_backup.Code
{
    public class Event_GroupMessage : IGroupMessage
    {
        public static Hashtable GroupList;


        /// <summary>
        /// 收到群消息
        /// </summary>
        /// <param name="sender">事件来源</param>
        /// <param name="e">事件参数</param>
        public void GroupMessage(object sender, CQGroupMessageEventArgs e)
        {
//            if (e.FromGroup != 932637215) { return; }
            // 获取 At 某人对象
            //            CQCode cqat = e.FromQQ.CQCode_At();
            // 往来源群发送一条群消息, 下列对象会合并成一个字符串发送
            //            e.FromGroup.SendGroupMessage(cqat, "test info", e.Message);

            if (GroupList == null) { e.CQLog.Warning("应用在未启用时被调用"); return; }
            if (e.Message.Text.StartsWith("//chat_backup") || e.Message.Text.StartsWith("//cbp")) 
            {
                string[] temp1 = e.Message.Text.Split(' ');
                if (temp1.Length != 2) 
                {
                    e.FromGroup.SendGroupMessage("格式错误，正确的格式为：//chat_backup <消息条数> 或 //cbp <消息条数>");
                    return;
                }
                int num;
                try
                {
                    num=int.Parse(temp1[1]);
                }
                catch (Exception ee)
                {
                    e.FromGroup.SendGroupMessage("<消息条数> 不是数字！");
                    return;
                }

                if (!GroupList.ContainsKey(e.FromGroup.Id))
                {
                    e.FromGroup.SendGroupMessage("没有相关消息缓存");
                    return;
                }

                ArrayList temp2 = (ArrayList)GroupList[e.FromGroup.Id];
                if (temp2.Count < num) 
                {
                    e.FromGroup.SendGroupMessage("没有足够相关消息缓存{"+ temp2.Count+" < "+num+"}");
                    return;
                }
                e.FromGroup.SendGroupMessage("消息回溯<"+num+">条");
                for (; num > 0; num--)
                {
                    e.FromGroup.SendGroupMessage(temp2[temp2.Count-num]);
                }

            }
            else
            {
                if (!GroupList.ContainsKey(e.FromGroup.Id))
                { GroupList.Add(e.FromGroup.Id, new ArrayList()); }
                ArrayList temp1 = (ArrayList)GroupList[e.FromGroup.Id];
                temp1.Add(e.FromQQ.Id + ":" + e.Message.Text);
                if (temp1.Count > 15) { temp1.RemoveAt(0); }
                GroupList[e.FromGroup.Id] = temp1;
            }
            // 设置该属性, 表示阻塞本条消息, 该属性会在方法结束后传递给酷Q
//            e.Handler = true;
        }
    }
}