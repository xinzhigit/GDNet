using Net.Share;
using Net.System;
using System;
using System.Collections;
using System.Reflection;

namespace Net.Helper
{
    public class SyncVarHelper
    {
        public static void InitSyncVar(MemberInfo info, object target, Action<SyncVarInfo> onSyncVarCollect)
        {
            var syncVar = info.GetCustomAttribute<SyncVar>();
            if (syncVar == null)
                return;
            var type = target.GetType();
            SyncVarInfo syncVarInfo = null;
            if (InvokeHelper.Cache.TryGetValue(type, out var dict))
                dict.TryGetValue(info.Name, out syncVarInfo);
            if (syncVarInfo == null)
                throw new Exception("请使用unity菜单GameDesigner/Network/InvokeHelper工具生成字段，属性同步辅助类!");
            if(!string.IsNullOrEmpty(syncVar.hook))
                syncVarInfo.onValueChanged = type.GetMethod(syncVar.hook, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            syncVarInfo.id = syncVar.id;
            syncVarInfo.authorize = syncVar.authorize;
            onSyncVarCollect(syncVarInfo);
        }

        private static bool CheckIsClass(Type type, ref int layer, bool root = true)
        {
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var field in fields)
            {
                var code = Type.GetTypeCode(field.FieldType);
                if (code == TypeCode.Object)
                {
                    if (field.FieldType.IsClass)
                        return true;
                    if (root)
                        layer = 0;
                    if (layer++ < 5)
                    {
                        var isClass = CheckIsClass(field.FieldType, ref layer, false);
                        if (isClass)
                            return true;
                    }
                }
            }
            return false;
        }

        public static bool ALEquals(IList a, IList b)
        {
            if ((a != null & b == null) | (a == null & b != null))
                return false;
            if (a == null & b == null)
                return true;
            if (a.Count != b.Count)
                return false;
            for (int i = 0; i < a.Count; i++)
                if (!a[i].Equals(b[i]))
                    return false;
            return true;
        }

        public static byte[] CheckSyncVar(bool isLocal, MyDictionary<ushort, SyncVarInfo> syncVarInfos)
        {
            Segment segment = null;
            var tick = (uint)Environment.TickCount;
            for (int i = 0; i < syncVarInfos.count; i++)
            {
                if (syncVarInfos.entries[i].hashCode == -1)
                    continue;
                var syncVar = syncVarInfos.entries[i].value;
                if ((!isLocal & !syncVar.authorize) | syncVar.isDispose | tick < syncVar.tick)
                    continue;
                syncVar.CheckHandlerValue(ref segment, true);
            }
            byte[] buffer = null;
            if (segment != null)
                buffer = segment.ToArray(true);
            return buffer;
        }

        public static void SyncVarHandler(MyDictionary<ushort, SyncVarInfo> syncVarDic, byte[] buffer)
        {
            var segment1 = new Segment(buffer, false);
            while (segment1.Position < segment1.Offset + segment1.Count)
            {
                var index = segment1.ReadUInt16();
                if (!syncVarDic.TryGetValue(index, out var syncVar))
                    break;
                if (syncVar == null)
                    break;
                syncVar.tick = (uint)Environment.TickCount + 500;
                syncVar.CheckHandlerValue(ref segment1, false);
            }
        }

        public static void RemoveSyncVar(MyDictionary<ushort, SyncVarInfo> syncVarList, object target)
        {
            foreach (var item in syncVarList)
            {
                var syncVar = item.Value;
                if(syncVar.EqualsTarget(target))
                    syncVar.isDispose = true;
            }
        }
    }
}
