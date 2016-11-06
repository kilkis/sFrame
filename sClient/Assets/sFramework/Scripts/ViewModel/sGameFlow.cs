using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace sFramework
{
    public enum sGameState
    {
        None,
        Lua,//Lua初始化
        Init,//游戏初始化
        CheckUpdate,//检测更新
        Update,//更新中
        LoadAllFinish,//必要资源全部加载完成
        Login,//登录界面
        Create,//创建角色界面
        Game,//游戏内
        End,
    }

    public class sGameFlow : MonoBehaviour
    {

        public static sGameFlow instance;

        private sGameState _state = sGameState.None;
        private Dictionary<sGameState, sBaseFlow> _flows = new Dictionary<sGameState, sBaseFlow>();
        private sBaseFlow _curFlow = null;

        void Awake()
        {
            instance = this;
        }

     
        void Start()
        {
            _flows.Add(sGameState.Login, new sFlow_Login());
            _flows.Add(sGameState.LoadAllFinish, new sFlow_LoadAllFinish());
            _flows.Add(sGameState.Create, new sFlow_Create());
            _flows.Add(sGameState.Init, new sFlow_Init());
            _flows.Add(sGameState.Lua, new sFlow_Lua());
            _flows.Add(sGameState.Game, new sFlow_Game());
            changeNextState();
        }

        void Init()
        {
            Debug.Log("game flow init");
            sEvent.GetInstance().register(UpdateEvent.NeedUpdateState.ToString(), this, "onNeedUpdateState");
            sEvent.GetInstance().register(UpdateEvent.UpdatingState.ToString(), this, "onUpdateState");
            sEvent.GetInstance().register(UpdateEvent.UpdateFinish.ToString(), this, "onUpdateFinish");
            sEvent.GetInstance().register(UpdateEvent.LoadAllFinish.ToString(), this, "onLoadAllFinish");
        }

        public void startGame()
        {
            Init();
            
        }

        public sGameState getCurState()
        {
            return _state;
        }
        /// <summary>
        /// 改变状态的时候执行
        /// 只执行一次
        /// </summary>
        public void changeNextState()
        {
            if (_state == sGameState.Game)
                return;
            Debug.Log("######## changeNextState:" + _state + " -> " + (_state+1));
           
            if(_curFlow != null )
            {
                _curFlow.flowOut();
                _curFlow = null;
            }
            ++_state;
            if (_flows.ContainsKey(_state))
            {
                _curFlow = _flows[_state];
                _curFlow.flowIn();
            }
        }
        

        /// <summary>
        /// 每个逻辑执行
        /// </summary>
        public void logicUpdate()
        {
            if(_curFlow != null )
            {
                _curFlow.flowing();
            }
        }

        public void onNeedUpdateState(int state, string vid)
        {
            //有版本需要更新
            //action:跳弹框，检测wifi状态，由玩家选择是否更新
            Debug.Log("onNeedUpdateState:" + state + " -- " + vid);
        }

        public void onUpdateState(int state, string vid, int per)
        {
            Debug.Log("onUpdateState: " + state + " -- " + vid + " -- " + per);
        }

        public void onUpdateFinish()
        {
            //没有需要更新的，跳过更新状态，直接进入login状态
            Debug.Log("onUpdateFinish");
            this.changeNextState();
            this.changeNextState();
        }

        public void onLoadAllFinish()
        {
            Debug.Log("############### onLoadAllFinish");
            this.changeNextState();
            this.changeNextState();
        }
    }

}