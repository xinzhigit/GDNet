#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA || UNITY_WEBGL
using Net.Component;
using Net.Share;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LockStep.Client
{
    [Serializable]
    public class GameSystem : ECS.GSystem
    {
        public List<Player> playersView = new List<Player>();//unity视图观察
        public Dictionary<int, Player> players = new Dictionary<int, Player>();
        public Func<Operation, Player> OnCreate;
        public Action OnExitBattle;

        public void Run(OperationList list)
        {
            LSTime.time += LSTime.deltaTime;//最先执行的时间,逻辑时间
            for (int i = 0; i < list.operations.Length; i++)
            {
                Operation opt = list.operations[i];
                switch (opt.cmd)
                {
                    case Command.Input:
                        if (!players.TryGetValue(opt.identity, out Player actor))
                        {
                            actor = OnCreate(opt);
                            actor.name = opt.identity.ToString();
                            players.Add(opt.identity, actor);
                            playersView.Add(actor);
                        }
                        actor.opt = opt;
                        break;
                    case NetCmd.QuitGame:
                        if (players.TryGetValue(opt.identity, out Player actor1))
                        {
                            playersView.Remove(actor1);
                            ECS.GObject.Destroy(actor1.entity);
                            players.Remove(opt.identity);
                        }
                        break;
                }
            }
            Update();
            Physics.Simulate(0.1f);
            EventSystem.UpdateEvent();//事件帧同步更新
        }

        [Rpc]
        void ExitBattle(int uid) 
        {
            if (players.TryGetValue(uid, out Player actor1))
            {
                playersView.Remove(actor1);
                ECS.GObject.Destroy(actor1.entity);
                players.Remove(uid);
            }
            OnExitBattle?.Invoke();
            Debug.Log("退出战斗");
        }

        ~GameSystem() 
        {
            ClientManager.Instance.client.RemoveRpc(this);
        }
    }
}
#endif