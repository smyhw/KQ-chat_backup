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
                if (temp1.Length < 2) 
                {
                    e.FromGroup.SendGroupMessage("格式错误!");
                    sendHelp(e.FromGroup);
                    return;
                }
                int num;
                Boolean ListMode = false;
                Boolean NameMode = true;
                for (int i = 2; i < temp1.Length; i++)
                {
                    switch (temp1[i])
                    {
                        case "-l":
                            ListMode = true;
                            break;
                        case "-n":
                            NameMode = false;
                            break;
                        default:
                            e.FromGroup.SendGroupMessage("未知参数<"+temp1[i]+">");
                            sendHelp(e.FromGroup);
                            return;
                    }
                }
                try
                {
                    num=int.Parse(temp1[1]);
                }
                catch (Exception ee)
                {
                    e.FromGroup.SendGroupMessage("<"+temp1[1]+">无法被识别为数字！");
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
                if (ListMode)
                {//输出完整的消息列表 
                    e.FromGroup.SendGroupMessage("消息回溯<" + num + ">条");
                    for (; num > 0; num--)
                    {
                        String fin = "";
                        if (NameMode)
                        {
                            cbp_message tmp1 = (cbp_message)temp2[temp2.Count - num];
                            fin = tmp1.getFromName() + ":" + tmp1.getMsg();
                        }
                        else
                        {
                            cbp_message tmp1 = (cbp_message)temp2[temp2.Count - num];
                            fin = tmp1.getFromQQ() + ":" + tmp1.getMsg();
                        }
                        e.FromGroup.SendGroupMessage(fin);
                    }
                }
                else
                {
                    String fin = "";
                    if (NameMode)
                    {
                        cbp_message tmp1 = (cbp_message)temp2[temp2.Count - num];
                        fin = tmp1.getFromName() + ":" + tmp1.getMsg();
                    }
                    else
                    {
                        cbp_message tmp1 = (cbp_message)temp2[temp2.Count - num];
                        fin = tmp1.getFromQQ() + ":" + tmp1.getMsg();
                    }
                    e.FromGroup.SendGroupMessage("消息回溯第<" + num + ">条");
                    e.FromGroup.SendGroupMessage(fin);
                }


            }
            else
            {//缓存消息
                if (!GroupList.ContainsKey(e.FromGroup.Id))
                { GroupList.Add(e.FromGroup.Id, new ArrayList()); }
                ArrayList temp1 = (ArrayList)GroupList[e.FromGroup.Id];
                temp1.Add(new cbp_message(e.FromQQ.Id, e.FromGroup, e.Message.Text));
                if (temp1.Count > 15) { temp1.RemoveAt(0); }
                GroupList[e.FromGroup.Id] = temp1;
            }
            // 设置该属性, 表示阻塞本条消息, 该属性会在方法结束后传递给酷Q
//            e.Handler = true;
        }

        static void sendHelp(Group tar)
        {
            tar.SendGroupMessage("用法://chat_backup <消息条数> [参数]\n" +
                "或 //cbp <消息条数> [参数]\n" +
                "-l   #查询到目标条数的完整列表\n" +
                "-n   #使用QQ号代替群昵称\n");
        }
    }

    /**
     * 被设计来存储一条消息
     */
    class cbp_message 
    {
        long FromQQ;
        Group FromGroup;
        String FromName;
        String msg;
        public cbp_message(long FromQQ, Group FromGroup,String msg)
        {
            this.FromQQ = FromQQ;
            this.msg = msg;
            GroupMemberInfo tmp1 = FromGroup.GetGroupMemberInfo(FromQQ, true);
            FromName = tmp1.Card;
        }

        public long getFromQQ()
        {
            return this.FromQQ;
        }
        public Group getFromGroup()
        {
            return this.FromGroup;
        }
        public String getFromName()
        {
            return this.FromName;
        }
        public String getMsg()
        {
            return this.msg;
        }
    }
}
