using Net.Helper;
using Net.Serialize;
using Net.Share;
using Net.System;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

internal static class InvokeHelperGenerate
{
#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA || UNITY_WEBGL
    [UnityEngine.RuntimeInitializeOnLoadMethod]
#else
    [RuntimeInitializeOnLoadMethod]
#endif
    internal static void Init()
    {
        InvokeHelper.Cache.Clear();

        InvokeHelper.Cache.Add(typeof(Example1.Client), new Dictionary<string, SyncVarInfo>() {

            { "testint", new SyncVarInfoPtr<Example1.Client, System.Int32>(testint) },

            { "teststring", new SyncVarInfoPtr<Example1.Client, System.String>(teststring) },

        });

    }

    internal static void testint(this Example1.Client self, ref System.Int32 testint, ushort id, ref Segment segment, bool isWrite, Action<System.Int32, System.Int32> onValueChanged) 
    {
        if (isWrite)
        {
            if (Equals(self.testint, null))
                return;
            if (self.testint == testint)
                return;
            if (segment == null)
                segment = BufferPool.Take();
            segment.Write(id);
            var pos = segment.Position;
            segment.Write(self.testint);
            var end = segment.Position;
            segment.Position = pos;
            testint = segment.ReadInt32();
            segment.Position = end;
        }
        else 
        {
            var pos = segment.Position;
            var testint1 = segment.ReadInt32();
            var end = segment.Position;
            segment.Position = pos;
            testint = segment.ReadInt32();
            segment.Position = end;
            if (onValueChanged != null)
                onValueChanged(self.testint, testint1);
            self.testint = testint1;
        }
    }

    internal static void teststring(this Example1.Client self, ref System.String teststring, ushort id, ref Segment segment, bool isWrite, Action<System.String, System.String> onValueChanged) 
    {
        if (isWrite)
        {
            if (Equals(self.teststring, null))
                return;
            if (self.teststring == teststring)
                return;
            if (segment == null)
                segment = BufferPool.Take();
            segment.Write(id);
            var pos = segment.Position;
            segment.Write(self.teststring);
            var end = segment.Position;
            segment.Position = pos;
            teststring = segment.ReadString();
            segment.Position = end;
        }
        else 
        {
            var pos = segment.Position;
            var teststring1 = segment.ReadString();
            var end = segment.Position;
            segment.Position = pos;
            teststring = segment.ReadString();
            segment.Position = end;
            if (onValueChanged != null)
                onValueChanged(self.teststring, teststring1);
            self.teststring = teststring1;
        }
    }

}

internal static class RpcInvokeHelper1
{

    internal static void CreateRoomCallback(this LockStep.Server.Player client , System.String str) 
    {
        
        LockStep.Server.Service.Instance.SendRT(client, "CreateRoomCallback", str);
    }

    internal static void JoinRoomCallback(this LockStep.Server.Player client , System.String str) 
    {
        
        LockStep.Server.Service.Instance.SendRT(client, "JoinRoomCallback", str);
    }

    internal static void ExitRoomCallback(this LockStep.Server.Player client , System.String str) 
    {
        
        LockStep.Server.Service.Instance.SendRT(client, "ExitRoomCallback", str);
    }

    internal static void StartGameSync(this LockStep.Server.Player client ) 
    {
        
        LockStep.Server.Service.Instance.SendRT(client, "StartGameSync");
    }

    internal static void ExitBattle(this LockStep.Server.Player client , System.Int32 uid) 
    {
        
        LockStep.Server.Service.Instance.SendRT(client, "ExitBattle", uid);
    }

    internal static void LoginCallback(this LockStep.Server.Player client , System.Boolean result, System.String info) 
    {
        
        LockStep.Server.Service.Instance.SendRT(client, "LoginCallback", result, info);
    }

    internal static void BackLogin(this LockStep.Server.Player client , System.String info) 
    {
        
        LockStep.Server.Service.Instance.SendRT(client, "BackLogin", info);
    }

    internal static void LogOut(this LockStep.Server.Player client ) 
    {
        
        LockStep.Server.Service.Instance.SendRT(client, "LogOut");
    }

    internal static void RegisterCallback(this LockStep.Server.Player client , System.String info) 
    {
        
        LockStep.Server.Service.Instance.SendRT(client, "RegisterCallback", info);
    }

    internal static void Test(this LockStep.Server.Player client , System.String info) 
    {
        
        LockStep.Server.Service.Instance.SendRT(client, "Test", info);
    }

}

internal static class RpcInvokeHelper2
{

    internal static void CreateRoomCallback(this Example2.Player client , System.String str) 
    {
        
        Example2.Service.Instance.SendRT(client, "CreateRoomCallback", str);
    }

    internal static void JoinRoomCallback(this Example2.Player client , System.String str) 
    {
        
        Example2.Service.Instance.SendRT(client, "JoinRoomCallback", str);
    }

    internal static void ExitRoomCallback(this Example2.Player client , System.String str) 
    {
        
        Example2.Service.Instance.SendRT(client, "ExitRoomCallback", str);
    }

    internal static void StartGameSync(this Example2.Player client ) 
    {
        
        Example2.Service.Instance.SendRT(client, "StartGameSync");
    }

    internal static void ExitBattle(this Example2.Player client , System.Int32 uid) 
    {
        
        Example2.Service.Instance.SendRT(client, "ExitBattle", uid);
    }

    internal static void LoginCallback(this Example2.Player client , System.Boolean result, System.String info) 
    {
        
        Example2.Service.Instance.SendRT(client, "LoginCallback", result, info);
    }

    internal static void BackLogin(this Example2.Player client , System.String info) 
    {
        
        Example2.Service.Instance.SendRT(client, "BackLogin", info);
    }

    internal static void LogOut(this Example2.Player client ) 
    {
        
        Example2.Service.Instance.SendRT(client, "LogOut");
    }

    internal static void RegisterCallback(this Example2.Player client , System.String info) 
    {
        
        Example2.Service.Instance.SendRT(client, "RegisterCallback", info);
    }

    internal static void Test(this Example2.Player client , System.String info) 
    {
        
        Example2.Service.Instance.SendRT(client, "Test", info);
    }

}

internal static class RpcInvokeHelper3
{

    internal static void CreateRoomCallback(this Example1.Client client , System.String str) 
    {
        
        Example1.Service.Instance.SendRT(client, "CreateRoomCallback", str);
    }

    internal static void JoinRoomCallback(this Example1.Client client , System.String str) 
    {
        
        Example1.Service.Instance.SendRT(client, "JoinRoomCallback", str);
    }

    internal static void ExitRoomCallback(this Example1.Client client , System.String str) 
    {
        
        Example1.Service.Instance.SendRT(client, "ExitRoomCallback", str);
    }

    internal static void StartGameSync(this Example1.Client client ) 
    {
        
        Example1.Service.Instance.SendRT(client, "StartGameSync");
    }

    internal static void ExitBattle(this Example1.Client client , System.Int32 uid) 
    {
        
        Example1.Service.Instance.SendRT(client, "ExitBattle", uid);
    }

    internal static void LoginCallback(this Example1.Client client , System.Boolean result, System.String info) 
    {
        
        Example1.Service.Instance.SendRT(client, "LoginCallback", result, info);
    }

    internal static void BackLogin(this Example1.Client client , System.String info) 
    {
        
        Example1.Service.Instance.SendRT(client, "BackLogin", info);
    }

    internal static void LogOut(this Example1.Client client ) 
    {
        
        Example1.Service.Instance.SendRT(client, "LogOut");
    }

    internal static void RegisterCallback(this Example1.Client client , System.String info) 
    {
        
        Example1.Service.Instance.SendRT(client, "RegisterCallback", info);
    }

    internal static void Test(this Example1.Client client , System.String info) 
    {
        
        Example1.Service.Instance.SendRT(client, "Test", info);
    }

}

internal static class RpcInvokeHelper4
{

    internal static void CreateRoomCallback(this AOIExample.Client client , System.String str) 
    {
        
        AOIExample.Service.Instance.SendRT(client, "CreateRoomCallback", str);
    }

    internal static void JoinRoomCallback(this AOIExample.Client client , System.String str) 
    {
        
        AOIExample.Service.Instance.SendRT(client, "JoinRoomCallback", str);
    }

    internal static void ExitRoomCallback(this AOIExample.Client client , System.String str) 
    {
        
        AOIExample.Service.Instance.SendRT(client, "ExitRoomCallback", str);
    }

    internal static void StartGameSync(this AOIExample.Client client ) 
    {
        
        AOIExample.Service.Instance.SendRT(client, "StartGameSync");
    }

    internal static void ExitBattle(this AOIExample.Client client , System.Int32 uid) 
    {
        
        AOIExample.Service.Instance.SendRT(client, "ExitBattle", uid);
    }

    internal static void LoginCallback(this AOIExample.Client client , System.Boolean result, System.String info) 
    {
        
        AOIExample.Service.Instance.SendRT(client, "LoginCallback", result, info);
    }

    internal static void BackLogin(this AOIExample.Client client , System.String info) 
    {
        
        AOIExample.Service.Instance.SendRT(client, "BackLogin", info);
    }

    internal static void LogOut(this AOIExample.Client client ) 
    {
        
        AOIExample.Service.Instance.SendRT(client, "LogOut");
    }

    internal static void RegisterCallback(this AOIExample.Client client , System.String info) 
    {
        
        AOIExample.Service.Instance.SendRT(client, "RegisterCallback", info);
    }

    internal static void Test(this AOIExample.Client client , System.String info) 
    {
        
        AOIExample.Service.Instance.SendRT(client, "Test", info);
    }

}

internal static class HelperFileInfo 
{
    internal static string GetPath()
    {
        return GetClassFileInfo();
    }

    internal static string GetClassFileInfo([CallerFilePath] string sourceFilePath = "")
    {
        return sourceFilePath;
    }
}